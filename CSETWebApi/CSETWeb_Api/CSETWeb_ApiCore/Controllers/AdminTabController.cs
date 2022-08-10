using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSETWebCore.Business.Authorization;
using CSETWebCore.Interfaces.AdminTab;
using CSETWebCore.Interfaces.Helpers;
using CSETWebCore.Model.AdminTab;

namespace CSETWebCore.Api.Controllers
{
    [CsetAuthorize]
    [ApiController]
    public class AdminTabController : ControllerBase
    {
        private readonly ITokenManager _tokenManager;
        private readonly IAdminTabBusiness _tabBusiness;

        public AdminTabController(ITokenManager tokenManager, IAdminTabBusiness tabBusiness)
        {
            _tokenManager = tokenManager;
            _tabBusiness = tabBusiness;
        }

        [HttpPost, HttpGet]
        [Route("api/admintab/Data")]
        public async Task<IActionResult> GetList()
        {
            int assessmentId = await _tokenManager.AssessmentForUser();
            return Ok(_tabBusiness.GetTabData(assessmentId));
        }

        [HttpPost]
        [Route("api/admintab/save")]
        public async Task<IActionResult> SaveData([FromBody] AdminSaveData save)
        {
            int assessmentId = await _tokenManager.AssessmentForUser();
            return Ok(_tabBusiness.SaveData(assessmentId, save));
        }


        [HttpPost]
        [Route("api/admintab/saveattribute")]
        public async Task<IActionResult> SaveDataAttribute([FromBody] AttributePair attribute)
        {
            int assessmentId = await _tokenManager.AssessmentForUser();
            await _tabBusiness.SaveDataAttribute(assessmentId, attribute);
            return Ok();
        }
    }
}
