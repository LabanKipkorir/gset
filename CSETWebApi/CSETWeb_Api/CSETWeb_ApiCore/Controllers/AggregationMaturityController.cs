﻿using Microsoft.AspNetCore.Mvc;
using CSETWebCore.Business.Aggregation;
using CSETWebCore.DataLayer.Model;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSETWebCore.Api.Controllers
{
    public class AggregationMaturityController : Controller
    {
        private CSETContext _context;

        public AggregationMaturityController(CSETContext context)
        {
            _context = context;

        }


        /// <summary>
        /// Returns domain scores for the assessments in
        /// the aggregation.  Each maturity model
        /// is compared against other assessments that have 
        /// that model.  No cross-model comparisons are done.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/aggregation/analysis/maturity/compliance")]
        public async Task<IActionResult> GetComplianceByModelAndDomain([FromQuery] int aggregationId)
        {
            var amb = new AggregationMaturityBusiness(_context);
            var resp = amb.GetMaturityModels(aggregationId);

            return Ok(resp);
        }
    }
}
