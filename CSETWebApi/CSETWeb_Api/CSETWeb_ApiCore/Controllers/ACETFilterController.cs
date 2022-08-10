﻿//////////////////////////////// 
// 
//   Copyright 2022 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using CSETWebCore.DataLayer.Model;
using CSETWebCore.Interfaces.Helpers;
using Microsoft.EntityFrameworkCore;
using CSETWebCore.Business.Authorization;
using CSETWebCore.Model.Acet;
using System.Threading.Tasks;

namespace CSETWebCore.Api.Controllers
{
    [ApiController]
    public class ACETFilterController : ControllerBase
    {
        private readonly CSETContext _context;
        private readonly ITokenManager _tokenManager;
        private readonly IAssessmentUtil _assessmentUtil;

        public ACETFilterController(CSETContext context, IAssessmentUtil assessmentUtil, ITokenManager tokenManager)
        {
            _context = context;
            _assessmentUtil = assessmentUtil;
            _tokenManager = tokenManager;
        }

        [HttpGet]
        [Route("api/IsAcetOnly")]
        public async Task<IActionResult> GetAcetOnly()
        {
            //if the appcode is null throw an exception
            //if it is not null return the default for the app

            string app_code = _tokenManager.Payload(Constants.Constants.Token_Scope);
            if (app_code == null)
            {
                string ConnectionString = _context.Database.GetDbConnection().ConnectionString;
                return Ok(ConnectionString.Contains("NCUAWeb"));
            }
            try
            {
                int assessment_id = await _tokenManager.AssessmentForUser();
                var ar = await _context.INFORMATION.Where(x => x.Id == assessment_id).FirstOrDefaultAsync();
                bool defaultAcet = (app_code == "ACET");
                return Ok(ar.IsAcetOnly ?? defaultAcet);
            }
            catch (Exception)
            {
                return Ok(app_code == "ACET");
            }
        }


        [CsetAuthorize]
        [HttpPost]
        [Route("api/SaveIsAcetOnly")]
        public async Task<IActionResult> SaveACETFilters([FromBody] bool value)
        {
            int assessment_id = await _tokenManager.AssessmentForUser();
           
            var ar = await _context.INFORMATION.Where(x => x.Id == assessment_id).FirstOrDefaultAsync();
            if (ar != null)
            {
                ar.IsAcetOnly = value;
                await _context.SaveChangesAsync();
            }

            return Ok();
        }


        [HttpGet]
        [Route("api/ACETDomains")]
        public async Task<IActionResult> GetAcetDomains()
        {
            List<ACETDomain> domains = new List<ACETDomain>();
            var dbDomains = await _context.FINANCIAL_DOMAINS.ToListAsync();
            foreach (var domain in dbDomains)
            {
                domains.Add(new ACETDomain()
                {
                    DomainName = domain.Domain,
                    DomainId = domain.DomainId
                });
            }
            return Ok(domains);
        }


        /// <summary>
        /// Returns known filter settings.  The columns in the FINANCIAL_DOMAIN_FILTERS
        /// are "hard-coded" as B, E, Int, A and Inn.  This method genericizes them
        /// to their numeric equivalent.  If CSET implements a new maturity model
        /// in the future that wants domain-level maturity filtering, we will need
        /// the generic numbers, rather than the ACET names.
        /// </summary>
        /// <returns></returns>
        [CsetAuthorize]
        [HttpGet]
        [Route("api/acet/filters")]
        public async Task<IActionResult> GetACETFilters()
        {
            int assessmentId = await _tokenManager.AssessmentForUser();
            List<ACETFilter> filters = new List<ACETFilter>();

            filters = await (from a in _context.FINANCIAL_DOMAIN_FILTERS
                       join b in _context.FINANCIAL_DOMAINS on a.DomainId equals b.DomainId
                       where a.Assessment_Id == assessmentId
                       select new ACETFilter()
                       {
                           DomainId = a.DomainId,
                           DomainName = b.Domain,
                           B = a.B,
                           E = a.E,
                           Int = a.Int,
                           A = a.A,
                           Inn = a.Inn
                       }).ToListAsync();

            // create Settings according to the B, E, Int, A and Inn bits.
            filters.ForEach(f =>
            {
                f.Settings = new List<ACETFilterSetting>();
                f.Settings.Add(new ACETFilterSetting(1, f.B));
                f.Settings.Add(new ACETFilterSetting(2, f.E));
                f.Settings.Add(new ACETFilterSetting(3, f.Int));
                f.Settings.Add(new ACETFilterSetting(4, f.A));
                f.Settings.Add(new ACETFilterSetting(5, f.Inn));
            });

            return Ok(filters);
        }


        /// <summary>
        /// Persists a filter setting.
        /// </summary>
        /// <param name="filterValue"></param>
        [CsetAuthorize]
        [HttpPost]
        [Route("api/acet/savefilter")]
        public async Task<IActionResult> SaveACETFilters([FromBody] ACETFilterValue filterValue)
        {
            int assessmentId = await _tokenManager.AssessmentForUser();
            string domainname = filterValue.DomainName;
            int level = filterValue.Level;
            bool value = filterValue.Value;

            Dictionary<string, int> domainIds = await _context.FINANCIAL_DOMAINS.ToDictionaryAsync(x => x.Domain, x => x.DomainId);
            int domainId = domainIds[domainname];

            var filter = await _context.FINANCIAL_DOMAIN_FILTERS.Where(x => x.DomainId == domainId && x.Assessment_Id == assessmentId).FirstOrDefaultAsync();
            if (filter == null)
            {
                filter = new FINANCIAL_DOMAIN_FILTERS()
                {
                    Assessment_Id = assessmentId,
                    DomainId = domainId
                };
                await _context.FINANCIAL_DOMAIN_FILTERS.AddAsync(filter);
            }

            switch (level)
            {
                case 1:
                    filter.B = value;
                    break;
                case 2:
                    filter.E = value;
                    break;
                case 3:
                    filter.Int = value;
                    break;
                case 4:
                    filter.A = value;
                    break;
                case 5:
                    filter.Inn = value;
                    break;
            }

            await _context.SaveChangesAsync();

            await _assessmentUtil.TouchAssessment(assessmentId);

            return Ok();
        }


        /// <summary>
        /// Persists settings for a group of filters.  This is normally used
        /// upon visiting the questions page for the first time and the filters are set based on the bands.
        /// </summary>
        /// <param name="filters"></param>
        [CsetAuthorize]
        [HttpPost]
        [Route("api/acet/savefilters")]
        public async Task<IActionResult> SaveACETFilters([FromBody] List<ACETFilter> filters)
        {
            int assessmentId = await _tokenManager.AssessmentForUser();
            
            Dictionary<string, int> domainIds = await _context.FINANCIAL_DOMAINS.ToDictionaryAsync(x => x.Domain, x => x.DomainId);

            foreach (ACETFilter f in filters.Where(x => x.DomainName != null).ToList())
            {
                int domainId = domainIds[f.DomainName];
                var filter = await _context.FINANCIAL_DOMAIN_FILTERS.Where(x => x.DomainId == domainId && x.Assessment_Id == assessmentId).FirstOrDefaultAsync();
                if (filter == null)
                {
                    filter = new FINANCIAL_DOMAIN_FILTERS()
                    {
                        Assessment_Id = assessmentId,
                        DomainId = domainId
                    };
                    await _context.FINANCIAL_DOMAIN_FILTERS.AddAsync(filter);
                }

                foreach (var s in f.Settings)
                {
                    switch (s.Level)
                    {
                        case 1:
                            filter.B = s.Value;
                            break;
                        case 2:
                            filter.E = s.Value;
                            break;
                        case 3:
                            filter.Int = s.Value;
                            break;
                        case 4:
                            filter.A = s.Value;
                            break;
                        case 5:
                            filter.Inn = s.Value;
                            break;
                    }
                }
            }

            await _context.SaveChangesAsync();

            await _assessmentUtil.TouchAssessment(assessmentId);

            return Ok();
        }


        /// <summary>
        /// Removes all maturity filters for the current assessment.
        /// </summary>
        [CsetAuthorize]
        [HttpPost]
        [Route("api/acet/resetfilters")]
        public async Task<IActionResult> ResetAllAcetFilters()
        {
            int assessmentID = await _tokenManager.AssessmentForUser();

            var filters = await _context.FINANCIAL_DOMAIN_FILTERS.Where(f => f.Assessment_Id == assessmentID).ToListAsync();
            _context.FINANCIAL_DOMAIN_FILTERS.RemoveRange(filters);
            await _context.SaveChangesAsync();

            await _assessmentUtil.TouchAssessment(assessmentID);

            return Ok();
        }
    }
}
