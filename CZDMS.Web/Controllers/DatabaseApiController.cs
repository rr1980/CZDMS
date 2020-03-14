using CZDMS.Db;
using CZDMS.Models;
using CZDMS.Services;
using DevExtreme.AspNet.Mvc.FileManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace CZDMS.Web.Controllers
{
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
            var result = dbFileProvider.GetDirectoryContents(UserId, pathInfos.LastOrDefault()?.Key);

            return result;
        }

        [HttpPost]
        [Route("CreateDirectory")]
        public void CreateDirectory([FromBody]CreateDirectoryRequest createDirectoryRequest)
        {
            //try
            //{
                dbFileProvider.CreateDirectory(UserId, createDirectoryRequest.ParentDir?.Key, createDirectoryRequest.Name);
            //}
            //catch(Exception ex)
            //{
            //    return BadRequest(ex);
            //}
        }

        [HttpPost]
        [Route("DeleteItem")]
        public void DeleteItem([FromBody]FileItemIdentifier<string> pathInfo)
        {
            dbFileProvider.Remove(UserId, pathInfo);
        }

        [HttpPost]
        [Route("MoveItem")]
        public void MoveItem([FromBody]MoveItemRequest moveItemRequest)
        {
            dbFileProvider.Move(UserId, moveItemRequest.Item?.Key, moveItemRequest.DestinationDir?.Key);
        }

        [HttpPost]
        [Route("CopyItem")]
        public void CopyItem([FromBody]MoveItemRequest moveItemRequest)
        {
            dbFileProvider.Copy(UserId, moveItemRequest.Item?.Key, moveItemRequest.DestinationDir?.Key);
        }

        [HttpPost]
        [Route("UploadFileChunk")]
        public void UploadFileChunk()
        {
            var _files = Request.Form.Files;
            var destinationDir = Request.Form["destinationDir"][0];

            dbFileProvider.MoveUploadedFile(UserId, _files[0], destinationDir);
        }

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
