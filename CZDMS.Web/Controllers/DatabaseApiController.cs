using CZDMS.Db;
using CZDMS.Services;
using DevExtreme.AspNet.Mvc.FileManagement;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace CZDMS.Web.Controllers
{
    [Route("api/[controller]")]
    public class DatabaseApiController : Controller
    {
        IWebHostEnvironment HostingEnvironment;
        FileDbContext FileDbContext;
        public DatabaseApiController(FileDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            FileDbContext = context;
            HostingEnvironment = hostingEnvironment;
        }
        public IActionResult FileSystem(FileSystemCommand command, string arguments)
        {
            var config = new FileSystemConfiguration
            {
                Request = Request,
                FileSystemProvider = new DbFileProvider(FileDbContext),
                AllowCopy = true,
                AllowCreate = true,
                AllowMove = true,
                AllowRemove = true,
                AllowRename = true,
                AllowUpload = true,
                AllowDownload = true,
                UploadTempPath = HostingEnvironment.ContentRootPath + "/wwwroot/UploadTemp"
            };
            var processor = new FileSystemCommandProcessor(config);
            var result = processor.Execute(command, arguments);
            return Ok(result.GetClientCommandResult());
        }
    }
}
