using CZDMS.Db;
using CZDMS.Models;
using CZDMS.Services;
using DevExtreme.AspNet.Mvc.FileManagement;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    [Route("api/[controller]")]
    public class DatabaseApiController : Controller
    {
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
            var result = dbFileProvider.GetDirectoryContents(pathInfos.LastOrDefault());

            return result;
        }

        [HttpPost]
        [Route("CreateDirectory")]
        public void CreateDirectory([FromBody]CreateDirectoryRequest createDirectoryRequest)
        {
            dbFileProvider.CreateDirectory(createDirectoryRequest.ParentDir, createDirectoryRequest.Name);
        }

        [HttpPost]
        [Route("DeleteItem")]
        public void DeleteItem([FromBody]FileItemIdentifier<string> pathInfo)
        {
            dbFileProvider.Remove(pathInfo);
        }

        [HttpPost]
        [Route("MoveItem")]
        public void MoveItem([FromBody]MoveItemRequest moveItemRequest)
        {
            dbFileProvider.Move(moveItemRequest.Item, moveItemRequest.DestinationDir);
        }

        [HttpPost]
        [Route("CopyItem")]
        public void CopyItem([FromBody]MoveItemRequest moveItemRequest)
        {
            dbFileProvider.Copy(moveItemRequest.Item, moveItemRequest.DestinationDir);
        }

        [HttpPost]
        [Route("UploadFileChunk")]
        public void UploadFileChunk()
        {
            var _files = Request.Form.Files;
            var destinationDir = Request.Form["destinationDir"][0];

           dbFileProvider.MoveUploadedFile(_files[0], destinationDir);
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
