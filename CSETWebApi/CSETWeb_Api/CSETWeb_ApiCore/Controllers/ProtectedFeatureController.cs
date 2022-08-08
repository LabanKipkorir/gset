﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using CSETWebCore.Business.Authorization;
using CSETWebCore.CryptoBuffer;
using CSETWebCore.DataLayer.Model;
using CSETWebCore.Model.Module;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CSETWebCore.Api.Controllers
{
    [CsetAuthorize]
    [ApiController]
    public class ProtectedFeatureController : ControllerBase
    {
        

        private CSETContext _context;

        public ProtectedFeatureController(CSETContext context)
        {
            _context = context;
        }


        [HttpGet]
        [Route("api/EnableProtectedFeature/Features")]
        /// <summary>
        /// Returns the FAA module along with its 'locked' status.' 
        /// IsEncryptedModule is used to indicate a 'lockable' module; they are not actulaly encrypted.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> GetFeatures()
        {
            var openFaaSets = await _context.SETS.Where(s=> s.IsEncryptedModule).ToListAsync();

            var result = new List<EnabledModule>();

            foreach (var s in openFaaSets)
            {
                result.Add(new EnabledModule() { 
                    ShortName = s.Short_Name,
                    FullName = s.Full_Name,
                    Unlocked = s.IsEncryptedModuleOpen ?? false
                });
            }

            return Ok(result);
        }


        [HttpPost]
        [Route("api/EnableProtectedFeature/unlockFeature")]
        /// <summary>
        /// Marks the FAA set as 'unlocked.'
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> EnableFAA(string set_name)
        {
            await AddNewlyEnabledModules();

            var response = new { 
                Message = ""
            };
            return Ok(response);
        }


        /// <summary>
        /// Marks the FAA set as 'unlocked.'
        /// </summary>
        private async Task AddNewlyEnabledModules()
        {
            var sets2 = await _context.SETS.Where(s=> s.IsEncryptedModule).ToListAsync();

            foreach (SETS sts in sets2)
            {
                sts.IsEncryptedModuleOpen = true;
            }
            await _context.SaveChangesAsync();
        }
    }
}
