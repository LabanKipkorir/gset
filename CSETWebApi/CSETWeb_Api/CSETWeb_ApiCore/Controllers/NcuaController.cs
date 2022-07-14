using Microsoft.AspNetCore.Mvc;
using System.IO;
using CSETWebCore.DataLayer.Model;
using System.Linq;
using System.Collections.Generic;
using CSETWebCore.Interfaces.AdminTab;
using CSETWebCore.Interfaces.Helpers;
using Snickler.EFCore;


namespace CSETWebCore.Api.Controllers
{
    [ApiController]
    public class NcuaController : ControllerBase
    {
        public CSETContext _context;
        
        public NcuaController(CSETContext context)
        {
            _context = context;
        }


        [HttpGet]
        [Route("api/isExaminersModule")]
        public IActionResult isExaminersModule()
        {
            string currentDir = Directory.GetCurrentDirectory();
            string fileLocation = Path.Combine(currentDir, @"NCUA-EXAMINER-TOOL");
            if (System.IO.File.Exists(fileLocation)) {

            }

            return Ok(System.IO.File.Exists(fileLocation));
        }


        [HttpGet]
        [Route("api/getQuestionText")]
        public IActionResult GetQuestionText([FromQuery] int modelId) {
            // Query DB for question text from modelId
            // Return list of question text
            return Ok();
        }


        [HttpGet]
        [Route("api/getMergeData")]
        public IList<Get_Merge_ConflictsResult> GetMergeAnswers([FromQuery] int id1, [FromQuery] int id2, [FromQuery] int id3)
        {
            // Todo: Add security check for authorization
            // Call assessments for user on each assessment ID

            // if true
            return _context.Get_Merge_Conflicts(id1, id2, id3); 
        }
    }
}