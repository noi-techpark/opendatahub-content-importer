// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

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
        public async Task<IActionResult> Update(
            LTSDatatype datatype,
            Operation operation,
            string? id,
            string? lastchange)
        {
            var response = new UpdateResponse() { success = null, message = null, operation = String.Join(".", "lts", datatype, operation) };

            if (datatype == LTSDatatype.accommodations)
            {
                if (operation == Operation.list)
                {
                    DateTime updatefrom = DateTime.Now.AddDays(-1);

                    if (!String.IsNullOrEmpty(lastchange))
                        DateTime.TryParse(lastchange, out updatefrom);

                    await dataimport.ImportLTSAccommodationChanged(updatefrom);

                    response.success = true;
                    response.message = String.Format("Import Accommodation List succeeded {0}", updatefrom.ToShortDateString());
                }
                if (operation == Operation.single)
                {                    
                    await dataimport.ImportLTSAccommodationSingle(id);

                    response.success = true;
                    response.message = String.Format("Import Accommodation Single {0} succeeded", id);
                }
            }


            return new JsonResult(response);
        }
    }
    


    public enum LTSDatatype
    {
        accommodations,
        gastronomies,
        events,
        activities,
        pointofinterests,
        venues,
        webcams,
        weathersnows,
        beacons,
        tags
    }

    public enum Operation
    {
        list,
        single,
        changed,
        deleted,
        search,
        categories,
        types,
        roomgroups,
        mealplans
    }

    public class UpdateResponse
    {
        public bool? success { get; set; }
        public string? message { get; set; }
        public string? error { get; set; }
        public string? operation { get; set; }
    }
}
