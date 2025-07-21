using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wissance.WebApiToolkit.Core.Data.Files;
using Wissance.WebApiToolkit.Core.Managers;
using Wissance.WebApiToolkit.Dto;

namespace Wissance.WebApiToolkit.Core.Controllers
{
    [Route("api/[controller]")]
    public class BasicFileController : ControllerBase
    {
        // TODO:
        /*
         * 1. Add Method for reading list of sources (DTO ?)
         */
        public BasicFileController(IFileManager manager)
        {
            Manager = manager;
        }

        public async Task<OperationResultDto<IList<string>>> GetSourcesAsync()
        {
            OperationResultDto<IList<string>> result = Manager.GetSources();
            Response.StatusCode = result.Status;
            return result;
        }

        public async Task<OperationResultDto<IList<TinyFileInfo>>> GetFilesAsync([FromQuery]string source, [FromQuery]string path)
        {
            // Manager.GetFilesAsync()
            return null;
        }

        public async Task<IActionResult> ReadFileAsync()
        {
            return null;
        }

        private IFileManager Manager { get; set; }
    }
}