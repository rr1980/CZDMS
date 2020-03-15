using CZDMS.Db;
using CZDMS.Db.Entities;
using CZDMS.Models;
using DevExtreme.AspNet.Mvc.FileManagement;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using Microsoft.Extensions.Hosting;
using System.IO.Compression;

namespace CZDMS.Services
{
    public class DbFileProvider
    {
        const int DbRootItemId = -1;
        FileDbContext DataContext { get; }
        string webRootPath;

        public DbFileProvider(FileDbContext _context, string webRootPath)
        {
            DataContext = _context;
            this.webRootPath = webRootPath;
        }

        public IList<IClientFileSystemItem> GetDirectoryContents(long uId, string dirKey)
        {
            FileItem parent = null;
            if (string.IsNullOrEmpty(dirKey))
            {
                var user = DataContext.Users.First(p => p.Id == uId);
                if (!DataContext.FileItems.Any(p => p.OwnerId == uId && p.IsFolder.Value && p.Name == user.Username))
                {
                    FileItem parentItem = GetDbItemByFileKey(uId, "root");
                    FileItem newFolderItem = new FileItem
                    {
                        Name = user.Username,
                        Key = Guid.NewGuid().ToString(),
                        ParentId = parentItem != null ? parentItem.Id : DbRootItemId,
                        IsFolder = true,
                        LastWriteTime = DateTime.Now,
                        OwnerId = uId
                    };
                    DataContext.FileItems.Add(newFolderItem);
                    DataContext.SaveChanges();
                }

                parent = GetDbItemByFileKey(uId, "root");
            }
            else
            {
                parent = GetDbItemByFileKey(uId, dirKey);
            }

            return DataContext.FileItems
                 .Where(p => p.ParentId == parent.Id && (p.OwnerId == uId || p.OwnerId == null))
                 .Select(CreateDbFileSystemItem)
                 .ToList<IClientFileSystemItem>();
        }

        public FileItem CreateDirectory(long uId, string rootKey, string name)
        {
            if (rootKey.StartsWith("__") || rootKey == "root")
            {
                throw new SecurityException("You can't create directory in the root folder.");
            }

            FileItem parentItem = GetDbItemByFileKey(uId, rootKey);
            FileItem newFolderItem = new FileItem
            {
                Name = name,
                Key = Guid.NewGuid().ToString(),
                ParentId = parentItem != null ? parentItem.Id : DbRootItemId,
                IsFolder = true,
                LastWriteTime = DateTime.Now,
                OwnerId = (parentItem.OwnerId == null && parentItem.Key != "root") ? null : (long?)uId
            };
            DataContext.FileItems.Add(newFolderItem);
            DataContext.SaveChanges();

            return newFolderItem;
        }

        public void Copy(long uId, string sourceName, string sourceKey, string destinationKey)
        {
            var user = DataContext.Users.First(p => p.Id == uId);

            if (sourceKey.StartsWith("__") || sourceKey == "root" || sourceKey == "public" || sourceName == user.Username)
            {
                throw new SecurityException("You can't copy this item.");
            }

            if (destinationKey.StartsWith("__") || destinationKey == "root")
            {
                throw new SecurityException("You can't copy to the root folder.");
            }

            FileItem sourceItem = GetDbItemByFileKey(uId, sourceKey);
            FileItem targetItem = GetDbItemByFileKey(uId, destinationKey);

            CopyFolderInternal(sourceItem, targetItem);
        }

        public void Move(long uId, string sourceName, string sourceKey, string destinationKey)
        {
            var user = DataContext.Users.First(p => p.Id == uId);

            if (sourceKey.StartsWith("__") || sourceKey == "root" || sourceKey == "public" || sourceName == user.Username)
            {
                throw new SecurityException("You can't move this item.");
            }

            if (destinationKey.StartsWith("__") || destinationKey == "root")
            {
                throw new SecurityException("You can't move to the root folder.");
            }

            FileItem sourceItem = GetDbItemByFileKey(uId, sourceKey);
            FileItem targetItem = GetDbItemByFileKey(uId, destinationKey);

            sourceItem.ParentId = targetItem.Id;
            sourceItem.OwnerId = targetItem.OwnerId;
            DataContext.SaveChanges();
        }

        public void MoveUploadedFile(long uId, IFormFile file, string destinationKey)
        {
            if (destinationKey.StartsWith("__") || destinationKey == "root")
            {
                throw new SecurityException("You can't upload to the root folder.");
            }

            byte[] data = new byte[file.Length];
            using (Stream fs = file.OpenReadStream())
            {
                fs.Read(data, 0, data.Length);
            }

            FileItem parentItem = GetDbItemByFileKey(uId, destinationKey);
            FileItem item = new FileItem
            {
                Name = file.FileName,
                Key = Guid.NewGuid().ToString(),
                ParentId = parentItem.Id,
                Data = data,
                IsFolder = false,
                LastWriteTime = DateTime.Now,
                OwnerId = (parentItem.OwnerId == null && parentItem.Key != "root") ? null : (long?)uId
            };
            DataContext.FileItems.Add(item);
            DataContext.SaveChanges();
        }

        public void Remove(long uId, string name, string key)
        {
            var user = DataContext.Users.First(p => p.Id == uId);

            if (key.StartsWith("__") || key == "root" || key == "public" || name == user.Username)
            {
                throw new SecurityException("You can't delete this item.");
            }

            FileItem item = GetDbItemByFileKey(uId, key);
            if (item.Id == DbRootItemId)
            {
                throw new SecurityException("You can't delete the root folder.");
            }
            RemoveInternal(item);
        }

        public void RemoveUploadedFile(FileInfo file)
        {
            file.Delete();
        }

        public void Rename(long uId, string oldName, string key, string newName)
        {
            var user = DataContext.Users.First(p => p.Id == uId);

            if (key.StartsWith("__") || key == "root" || key == "public" || oldName == user.Username)
            {
                throw new SecurityException("You can't rename this item.");
            }

            FileItem item = GetDbItemByFileKey(uId, key);
            if (item.ParentId == DbRootItemId)
            {
                throw new SecurityException("You can't rename the root folder.");
            }
            DataContext.FileItems.Find(item.Id).Name = newName;
            DataContext.SaveChanges();
        }


        public DownloadItemRequest GetForDownload(long uId, DbFileSystemItem[] items)
        {
            FileItem firstItem = GetDbItemByFileKey(uId, items[0].Key.ToString());

            if (items.Length == 1 && !firstItem.IsFolder.Value)
            {
                return new DownloadItemRequest
                {
                    Data = firstItem.Data,
                    FileName = firstItem.Name
                };
            }
            else
            {
                var tmpPath = Path.Combine(webRootPath, "tmp", Guid.NewGuid().ToString());

                foreach (var item in items)
                {
                    FileItem _item = GetDbItemByFileKey(uId, item.Key.ToString());

                    MakeFiles(uId, Path.Combine(tmpPath, _item.Name), _item.Id);
                }


                var zipName = Guid.NewGuid().ToString() + ".zip";

                ZipFile.CreateFromDirectory(tmpPath, Path.Combine(webRootPath, "tmp", zipName));

                var result = new DownloadItemRequest
                {
                    Data = File.ReadAllBytes(Path.Combine(webRootPath, "tmp", zipName)),
                    FileName = firstItem.Name + ".zip"
                };

                File.Delete(Path.Combine(webRootPath, "tmp", zipName));
                Directory.Delete(tmpPath, true);
                return result;
            }
        }

        private void MakeFiles(long uId, string path, long parentId)
        {
            Directory.CreateDirectory(path);

            var all = DataContext.FileItems.Where(item => (item.OwnerId == uId || item.OwnerId == null) && item.ParentId == parentId);


            foreach (var file in all.Where(x => !x.IsFolder.Value))
            {
                File.WriteAllBytes(Path.Combine(path, file.Name), file.Data);
            }

            foreach (var dir in all.Where(x => x.IsFolder.Value))
            {
                MakeFiles(uId, Path.Combine(path, dir.Name), dir.Id);
            }
        }

        #region privates
        FileItem copy(FileItem sourceItem, FileItem targetItem)
        {
            FileItem copyItem = new FileItem
            {
                Key = Guid.NewGuid().ToString(),
                Data = sourceItem.Data,
                LastWriteTime = DateTime.Now,
                IsFolder = sourceItem.IsFolder,
                Name = sourceItem.Name,
                ParentId = targetItem.Id,
                OwnerId = targetItem.OwnerId
            };
            DataContext.FileItems.Add(copyItem);
            DataContext.SaveChanges();
            return copyItem;
        }

        void RemoveInternal(FileItem sourceItem)
        {
            if (!(bool)sourceItem.IsFolder)
            {
                remove(sourceItem);
            }
            else
            {
                List<FileItem> childItems = DataContext.FileItems.Where(p => p.ParentId == sourceItem.Id).ToList();
                remove(sourceItem);
                foreach (FileItem item in childItems)
                    RemoveInternal(item);
            }
        }

        void remove(FileItem item)
        {
            DataContext.FileItems.Remove(item);
            DataContext.SaveChanges();
        }

        void CopyFolderInternal(FileItem sourceItem, FileItem targetFolder)
        {
            if (!(bool)sourceItem.IsFolder)
                copy(sourceItem, targetFolder);
            else
            {
                List<FileItem> childItems = DataContext.FileItems.Where(p => p.ParentId == sourceItem.Id).ToList();
                var newFolder = copy(sourceItem, targetFolder);
                foreach (FileItem item in childItems)
                    CopyFolderInternal(item, newFolder);
            }
        }

        FileItem GetDbItemByFileKey(long uId, string fileKey)
        {
            return DataContext.FileItems.FirstOrDefault(item => (item.OwnerId == uId && item.Key == fileKey) || (item.OwnerId == null && item.Key == fileKey));
        }

        DbFileSystemItem CreateDbFileSystemItem(FileItem dbItem)
        {
            return new DbFileSystemItem
            {
                Key = dbItem.Key,
                ParentId = dbItem.ParentId == null ? DbRootItemId : (int)dbItem.ParentId,
                Name = dbItem.Name,
                DateModified = (DateTime)dbItem.LastWriteTime,
                IsDirectory = (bool)dbItem.IsFolder,
                FileData = dbItem.Data,
                Size = dbItem.Data == null ? 0 : dbItem.Data.Length,
                HasSubDirectories = DataContext.FileItems.Where(i => i.ParentId == dbItem.Id && i.IsFolder == true).Count() > 0
            };
        }
        #endregion
    }
}
