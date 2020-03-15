using CZDMS.Db;
using CZDMS.Models;
using CZDMS.Services;
using DevExtreme.AspNet.Mvc.FileManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace CZDMS.Web.Controllers
{
    public class FileManagerItem
    {
        public string Name { get; set; }
        public string Key { get; set; }
    }

    public class MoveItemRequest
    {
        public FileItemIdentifier<string> Item { get; set; }
        public FileItemIdentifier<string> DestinationDir { get; set; }
    }

    public class CreateDirectoryRequest
    {
        public string Name { get; set; }
        public FileItemIdentifier<string> ParentDir { get; set; }
    }

    [Authorize]
    [Route("api/[controller]")]
    public class DatabaseApiController : Controller
    {
        public long UserId
        {
            get
            {
                var _uId = User.FindFirstValue("Id");
                var uId = long.Parse(_uId);
                if (uId == 0)
                {
                    throw new Exception("UserID not Found!");
                }

                return uId;
            }
        }

        public DbFileProvider dbFileProvider
        {
            get
            {
                return new DbFileProvider(FileDbContext);
            }
        }

        IWebHostEnvironment HostingEnvironment;
        FileDbContext FileDbContext;
        public DatabaseApiController(FileDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            FileDbContext = context;
            HostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        [Route("GetItems")]
        public IList<IClientFileSystemItem> GetItems([FromBody]FileItemIdentifier<string>[] pathInfos)
        {
            return dbFileProvider.GetDirectoryContents(UserId, pathInfos.LastOrDefault()?.Key);
        }

        [HttpPost]
        [Route("CreateDirectory")]
        public void CreateDirectory([FromBody]CreateDirectoryRequest createDirectoryRequest)
        {
            dbFileProvider.CreateDirectory(UserId, createDirectoryRequest.ParentDir?.Key, createDirectoryRequest.Name);
        }

        [HttpPost]
        [Route("RenameItem")]
        public void RenameItem([FromBody]CreateDirectoryRequest createDirectoryRequest)
        {
            dbFileProvider.Rename(UserId, createDirectoryRequest.ParentDir?.Name, createDirectoryRequest.ParentDir?.Key, createDirectoryRequest.Name);
        }

        [HttpPost]
        [Route("DeleteItem")]
        public void DeleteItem([FromBody]FileItemIdentifier<string> pathInfo)
        {
            dbFileProvider.Remove(UserId, pathInfo?.Name, pathInfo?.Key);
        }

        [HttpPost]
        [Route("MoveItem")]
        public void MoveItem([FromBody]MoveItemRequest moveItemRequest)
        {
            dbFileProvider.Move(UserId, moveItemRequest.Item?.Name, moveItemRequest.Item?.Key, moveItemRequest.DestinationDir?.Key);
        }

        [HttpPost]
        [Route("CopyItem")]
        public void CopyItem([FromBody]MoveItemRequest moveItemRequest)
        {
            dbFileProvider.Copy(UserId, moveItemRequest.Item?.Name, moveItemRequest.Item?.Key, moveItemRequest.DestinationDir?.Key);
        }

        [HttpPost]
        [Route("UploadFileChunk")]
        public void UploadFileChunk()
        {
            var _files = Request.Form.Files;
            var destinationDir = Request.Form["destinationDir"][0];

            dbFileProvider.MoveUploadedFile(UserId, _files[0], destinationDir);
        }

        [HttpPost]
        [Route("Download")]
        public FileResult Download([FromBody]DbFileSystemItem[] items)
        {
            var data = dbFileProvider.GetItemData(UserId, items);
            return File(data, System.Net.Mime.MediaTypeNames.Application.Octet, items[0].Name);
        }

        //IClientFileSystemItem

        //public IActionResult FileSystem(FileSystemCommand command, string arguments)
        //{
        //    var config = new FileSystemConfiguration
        //    {
        //        Request = Request,
        //        FileSystemProvider = new DbFileProvider(FileDbContext),
        //        AllowCopy = true,
        //        AllowCreate = true,
        //        AllowMove = true,
        //        AllowRemove = true,
        //        AllowRename = true,
        //        AllowUpload = true,
        //        AllowDownload = true,
        //        UploadTempPath = HostingEnvironment.ContentRootPath + "/wwwroot/UploadTemp"
        //    };
        //    var processor = new FileSystemCommandProcessor(config);
        //    var result = processor.Execute(command, arguments);
        //    return Ok(result.GetClientCommandResult());
        //}
    }
}

