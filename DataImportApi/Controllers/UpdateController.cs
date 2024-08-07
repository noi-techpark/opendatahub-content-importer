using DataImportHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.ComponentModel.DataAnnotations;

namespace DataImportApi.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class UpdateController : ControllerBase
    {
        private IDataImport dataimport;

        public UpdateController(IDataImport dataimport) 
        {
            this.dataimport = dataimport;
        }

        [HttpGet, Route("LTS/{datatype}/{operation}")]
        public ContentResult Update(
            string datatype, 
            string operation,
            string? id,
            string? lastchange)
        {            
            return new ContentResult
            {
                Content = datatype,
                ContentType = "text/html"
            };
        }
    }
}
