using CZDMS.Db;
using CZDMS.Db.Entities;
using CZDMS.Models;
using DevExtreme.AspNet.Mvc.FileManagement;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;

namespace CZDMS.Services
{
    public class DbFileProvider 
    {
        const int DbRootItemId = -1;
        static readonly char[] PossibleDirectorySeparators = { '\\', '/' };
        FileDbContext DataContext { get; }
        public DbFileProvider(FileDbContext _context)
        {
            DataContext = _context;
        }

        public void Copy(FileItemIdentifier<string> sourceKey, FileItemIdentifier<string> destinationKey)
        {
            FileItem sourceItem = GetDbItemByFileKey(sourceKey.Key);
            FileItem targetItem = GetDbItemByFileKey(destinationKey.Key);
            if (targetItem.Id == sourceItem.ParentId)
                throw new SecurityException("You can't copy to the same folder.");
            List<FileItem> childItems = DataContext.FileItems.Where(p => p.ParentId == targetItem.Id).ToList();
            if (childItems.Select(i => i.Name).Contains(sourceItem.Name))
                throw new SecurityException("The folder contains an item with the same name.");
            CopyFolderInternal(sourceItem, targetItem);
        }

        public void CreateDirectory(FileItemIdentifier<string> rootKey, string name)
        {
            FileItem parentItem = GetDbItemByFileKey(rootKey.Key);
            FileItem newFolderItem = new FileItem
            {
                Name = name,
                ParentId = parentItem.Id,
                IsFolder = true,
                LastWriteTime = DateTime.Now
            };
            DataContext.FileItems.Add(newFolderItem);
            DataContext.SaveChanges();
        }

        public IList<IClientFileSystemItem> GetDirectoryContents(FileItemIdentifier<string> dirKey)
        {
            FileItem parent = GetDbItemByFileKey(dirKey.Key);
            if (parent != null)
            {
                return DataContext.FileItems
                    .Where(p => p.ParentId == parent.Id)
                    .Select(CreateDbFileSystemItem)
                    .ToList<IClientFileSystemItem>();
            }
            else
            {
                throw new SecurityException("No Parent folder.");
            }
        }

        public void Move(FileItemIdentifier<string> sourceKey, FileItemIdentifier<string> destinationKey)
        {
            FileItem sourceItem = GetDbItemByFileKey(sourceKey.Key);
            FileItem targetItem = GetDbItemByFileKey(destinationKey.Key);
            if (targetItem.Id == sourceItem.ParentId)
                throw new SecurityException("You can't copy to the same folder.");
            List<FileItem> childItems = DataContext.FileItems.Where(p => p.ParentId == targetItem.Id).ToList();
            if (childItems.Select(i => i.Name).Contains(sourceItem.Name))
                throw new SecurityException("The folder contains an item with the same name.");
            sourceItem.ParentId = targetItem.Id;
            DataContext.SaveChanges();
        }

        public Stream GetFileContent(FileItemPathInfo pathInfo)
        {
            FileItem sourceItem = GetDbItemByFileKey(pathInfo.GetFileItemKey<string>());

            //MemoryStream memStream = new MemoryStream();
            //BinaryFormatter binForm = new BinaryFormatter();
            //memStream.Write(sourceItem.Data, 0, sourceItem.Data.Length);
            //memStream.Seek(0, SeekOrigin.Begin);
            //Object obj = (Object)binForm.Deserialize(memStream);

            //var fs = new FileStream(sourceItem.Name, FileMode.Create, FileAccess.Write);
            //fs.Write(sourceItem.Data, 0, sourceItem.Data.Length);

            //FileStream stream = new FileStream(sourceItem.Name, FileMode.);


            //MemoryStream memoryStream = new MemoryStream(sourceItem.Data);
            //var br = new BinaryWriter();

            var ms = new MemoryStream(sourceItem.Data);
            ms.Flush();
            ms.Position = 0;

            StreamReader sr = new StreamReader(ms);
            var  content = sr.ReadToEnd();

            return sr.BaseStream;
        }

        public void MoveUploadedFile(IFormFile file, string destinationKey)
        {
            byte[] data = new byte[file.Length];
            using (Stream fs = file.OpenReadStream())
            {
                fs.Read(data, 0, data.Length);
            }

            FileItem parentItem = GetDbItemByFileKey(destinationKey);
            FileItem item = new FileItem
            {
                Name = file.FileName,
                ParentId = parentItem.Id,
                Data = data,
                IsFolder = false,
                LastWriteTime = DateTime.Now
            };
            DataContext.FileItems.Add(item);
            DataContext.SaveChanges();
        }

        public void Remove(FileItemIdentifier<string> key)
        {
            FileItem item = GetDbItemByFileKey(key.Key);
            if (item.Id == DbRootItemId)
                throw new SecurityException("You can't delete the root folder.");
            RemoveInternal(item);
        }

        public void RemoveUploadedFile(FileInfo file)
        {
            file.Delete();
        }

        public void Rename(FileItemPathInfo key, string newName)
        {
            FileItem item = GetDbItemByFileKey(key.GetFileItemKey<string>());
            if (item.ParentId == DbRootItemId)
                throw new SecurityException("You can't rename the root folder.");
            DataContext.FileItems.Find(item.Id).Name = newName;
            DataContext.SaveChanges();
        }

        #region privates
        FileItem copy(FileItem sourceItem, FileItem targetItem)
        {
            FileItem copyItem = new FileItem
            {
                Key = sourceItem.Key,       //?????
                Data = sourceItem.Data,
                LastWriteTime = DateTime.Now,
                IsFolder = sourceItem.IsFolder,
                Name = sourceItem.Name,
                ParentId = targetItem.Id
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

        FileItem GetDbItemByFileKey(string fileKey)
        {
            if (string.IsNullOrEmpty(fileKey) || fileKey == "\\")
                return DataContext.FileItems.Where(p => p.ParentId == DbRootItemId).FirstOrDefault();
            string[] pathPFileItem = fileKey.Split(PossibleDirectorySeparators);
            var query = DataContext.FileItems.Where(item => (bool)item.IsFolder && item.Name == pathPFileItem.First());
            var childItemsQuery = DataContext.FileItems.Where(item => item.ParentId != null);
            for (int i = 1; i < pathPFileItem.Length; i++)
            {
                string itemName = pathPFileItem[i];
                query = childItemsQuery.
                 Join(query,
                  childItem => childItem.ParentId,
                  parentItem => parentItem.Id,
                  (childItem, parentItem) => childItem).
                 Where(item => item.Name == itemName);
            }

            var result = query.FirstOrDefault();

            if(result == null)
            {
                result = DataContext.FileItems.Where(p => p.ParentId == DbRootItemId).FirstOrDefault();
            }

            return result;
        }

        DbFileSystemItem CreateDbFileSystemItem(FileItem dbItem)
        {
            return new DbFileSystemItem
            {
                Id = dbItem.Id.ToString(),
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
