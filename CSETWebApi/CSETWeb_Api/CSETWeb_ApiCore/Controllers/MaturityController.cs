﻿//////////////////////////////// 
// 
//   Copyright 2022 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 
using CSETWebCore.Business.Acet;
using CSETWebCore.Business.Maturity;
using CSETWebCore.Business.Reports;
using CSETWebCore.DataLayer.Model;
using CSETWebCore.Interfaces.AdminTab;
using CSETWebCore.Interfaces.Crr;
using CSETWebCore.Interfaces.Helpers;
using CSETWebCore.Interfaces.Reports;
using CSETWebCore.Model.Maturity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Newtonsoft.Json;


namespace CSETWebCore.Api.Controllers
{
    public class MaturityController : ControllerBase
    {
        private readonly IUserAuthentication _userAuthentication;
        private readonly ITokenManager _tokenManager;
        private readonly CSETContext _context;
        private readonly IAssessmentUtil _assessmentUtil;
        private readonly IAdminTabBusiness _adminTabBusiness;
        private readonly IReportsDataBusiness _reports;
        private readonly ICrrScoringHelper _crr;

        public MaturityController(IUserAuthentication userAuthentication, ITokenManager tokenManager, CSETContext context,
             IAssessmentUtil assessmentUtil, IAdminTabBusiness adminTabBusiness, IReportsDataBusiness reports,
             ICrrScoringHelper crr)
        {
            _userAuthentication = userAuthentication;
            _tokenManager = tokenManager;
            _context = context;
            _assessmentUtil = assessmentUtil;
            _adminTabBusiness = adminTabBusiness;
            _reports = reports;
            _crr = crr;
        }


        /// <summary>
        /// Get all maturity models for the assessment.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/MaturityModel")]
        public IActionResult GetMaturityModel()
        {
            int assessmentId = _tokenManager.AssessmentForUser();
            return Ok(new MaturityBusiness(_context, _assessmentUtil, _adminTabBusiness).GetMaturityModel(assessmentId));
        }


        /// <summary>
        /// Set selected maturity models for the assessment.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/MaturityModel")]
        public IActionResult SetMaturityModel(string modelName)
        {
            int assessmentId = _tokenManager.AssessmentForUser();
            new MaturityBusiness(_context, _assessmentUtil, _adminTabBusiness).PersistSelectedMaturityModel(assessmentId, modelName);
            return Ok(new MaturityBusiness(_context, _assessmentUtil, _adminTabBusiness).GetMaturityModel(assessmentId));
        }

        /// <summary>
        /// Set selected maturity models for the assessment.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/MaturityModel/DomainRemarks")]
        public IActionResult GetDomainRemarks()
        {
            int assessmentId = _tokenManager.AssessmentForUser();
            return Ok(new MaturityBusiness(_context, _assessmentUtil, _adminTabBusiness).GetDomainRemarks(assessmentId));
        }

        /// <summary>
        /// Set selected maturity models for the assessment.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/MaturityModel/DomainRemarks")]
        public IActionResult SetDomainRemarks([FromBody] MaturityDomainRemarks remarks)
        {
            int assessmentId = _tokenManager.AssessmentForUser();
            new MaturityBusiness(_context, _assessmentUtil, _adminTabBusiness).SetDomainRemarks(assessmentId, remarks);
            return Ok();
        }

        /// <summary>
        /// Return the current maturity level for an assessment.
        /// Currently returns an int, but could be expanded
        /// if more data needed.
        /// </summary>
        [HttpGet]
        [Route("api/MaturityLevel")]
        public IActionResult GetMaturityLevel()
        {
            int assessmentId = _tokenManager.AssessmentForUser();
            return Ok(new MaturityBusiness(_context, _assessmentUtil, _adminTabBusiness).GetMaturityLevel(assessmentId));
        }


        /// <summary>
        /// Set selected maturity models for the assessment.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/MaturityLevel")]
        public IActionResult SetMaturityLevel([FromBody] int level)
        {
            int assessmentId = _tokenManager.AssessmentForUser();
            new MaturityBusiness(_context, _assessmentUtil, _adminTabBusiness).PersistMaturityLevel(assessmentId, level);
            return Ok();
        }


        /// <summary>
        /// 
        /// </summary>
        [HttpGet]
        [Route("api/MaturityQuestions")]
        public IActionResult GetQuestions([FromQuery] string installationMode, bool fill)
        {
            int assessmentId = _tokenManager.AssessmentForUser();

            return Ok(new MaturityBusiness(_context, _assessmentUtil, _adminTabBusiness).GetMaturityQuestions(assessmentId, installationMode, fill));
        }

        [HttpGet]
        [Route("api/maturity/targetlevel")]
        public IActionResult GetTargetLevel()
        {
            int assessmentId = _tokenManager.AssessmentForUser();

            return Ok(new MaturityBusiness(_context, _assessmentUtil, _adminTabBusiness).GetTargetLevel(assessmentId));
        }


        /// <summary>        
        /// </summary>
        [HttpGet]
        [Route("api/SPRSScore")]
        public IActionResult GetSPRSScore()
        {
            int assessmentId = _tokenManager.AssessmentForUser();

            return Ok(new MaturityBusiness(_context, _assessmentUtil, _adminTabBusiness).GetSPRSScore(assessmentId));
        }


        [HttpGet]
        [Route("api/results/compliancebylevel")]
        public IActionResult GetComplianceByLevel()
        {
            int assessmentId = _tokenManager.AssessmentForUser();

            return Ok(new MaturityBusiness(_context, _assessmentUtil, _adminTabBusiness).GetAnswerDistributionByLevel(assessmentId));
        }


        [HttpGet]
        [Route("api/results/compliancebydomain")]
        public IActionResult GetComplianceByDomain()
        {
            int assessmentId = _tokenManager.AssessmentForUser();

            return Ok(new MaturityBusiness(_context, _assessmentUtil, _adminTabBusiness).GetAnswerDistributionByDomain(assessmentId));
        }


        /// <summary>
        /// Returns the maturity grouping/question structure for an assessment.
        /// Specifying a query parameter of domainAbbreviation will limit the response
        /// to a single domain.
        /// </summary>
        [HttpGet]
        [Route("api/MaturityStructure")]
        public IActionResult GetQuestions([FromQuery] string domainAbbrev)
        {
            int assessmentId = _tokenManager.AssessmentForUser();

            var biz = new MaturityBusiness(_context, _assessmentUtil, _adminTabBusiness);
            var x = biz.GetMaturityStructure(assessmentId);


            var j = "";
            if (string.IsNullOrEmpty(domainAbbrev))
            {
                j = Helpers.CustomJsonWriter.Serialize(x.Root);
            }
            else
            {
                var domain = x.Root.XPathSelectElement($"//Domain[@abbreviation='{domainAbbrev}']");
                j = Helpers.CustomJsonWriter.Serialize(domain);
            }

            return Ok(j);
        }


        [HttpGet]
        [Route("api/maturity/structure")]
        public IActionResult GetGroupingAndQuestions([FromQuery] int modelId)
        {
            var biz = new MaturityBusiness(_context, _assessmentUtil, _adminTabBusiness);
            var x = biz.GetMaturityStructureForModel(modelId);
            return Ok(x.Model);
        }


        /// <summary>
        /// Returns the questions in a CIS section.
        /// </summary>
        /// <param name="sectionId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/maturity/cis/questions")]
        public IActionResult GetCisGroupingAndQuestions([FromQuery] int sectionId)
        {
            int assessmentId = _tokenManager.AssessmentForUser();

            var biz = new CisStructure(assessmentId, sectionId, _context);
            return Ok(biz.MyModel);
        }


        /// <summary>
        /// Returns the CIS subnode list.  This is used to 
        /// render the sidenav tree in the UI.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [Route("api/maturity/cis/navstruct")]
        public IActionResult GetCisNavStructure()
        {
            var nav = new CisNavStructure(_context);
            var s = nav.GetNavStructure();
            return Ok(s);
        }


        /// <summary>
        /// Returns list of CIS assessments accessible to the current user.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/maturity/cis/mycisassessments")]
        public IActionResult GetCisAssessments()
        {
            var assessmentId = _tokenManager.AssessmentForUser();
            var userId = _tokenManager.PayloadInt(Constants.Constants.Token_UserId);

            var biz = new CisQuestionsBusiness(_context, _assessmentUtil, assessmentId);
            var x = biz.GetMyCisAssessments(assessmentId, userId);
            return Ok(x);
        }


        /// <summary>
        /// Persists the selected baseline assessment
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/maturity/cis/baseline")]
        public IActionResult SaveBaseline([FromBody] int? baselineId)
        {
            var assessmentId = _tokenManager.AssessmentForUser();

            var biz = new CisQuestionsBusiness(_context, _assessmentUtil, assessmentId);
            biz.SaveBaseline(assessmentId, baselineId);

            return Ok();
        }
        
        /// <summary>
        /// Get deficiency chart data for comparative between current assessment and baseline
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/maturity/cis/getDeficiency")]
        public IActionResult GetDeficiency()
        {
            var assessmentId = _tokenManager.AssessmentForUser();
            var cisBiz = new CisQuestionsBusiness(_context, _assessmentUtil, assessmentId);
            var chartData = cisBiz.GetDeficiencyChartData();
            return Ok(chartData);
        }


        [HttpGet]
        [Route("api/maturity/cis/sectionscoring")]
        public IActionResult GetSectionScoring()
        {
            var assessmentId = _tokenManager.AssessmentForUser();
            var cisBiz = new CisQuestionsBusiness(_context, _assessmentUtil, assessmentId);
            var chartData = cisBiz.GetSectionScoringCharts();
            return Ok(chartData);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/maturity/cis/importsurvey")]
        public IActionResult ImportSurvey([FromBody] Model.Cis.CisImportRequest request)
        {
            var assessmentId = _tokenManager.AssessmentForUser();


            // TODO: verify that the user has permission to both assessments
            



            var biz = new CisQuestionsBusiness(_context, _assessmentUtil, assessmentId);
            biz.ImportCisAnswers(request.Dest, request.Source);
            return Ok();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("api/MaturityModels")]
        public IActionResult GetAllModels()
        {
            return Ok(new MaturityBusiness(_context, _assessmentUtil, _adminTabBusiness).GetAllModels());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/MaturityAnswerCompletionRate")]
        public IActionResult GetAnswerCompletionRate()
        {
            int assessmentId = _tokenManager.AssessmentForUser();

            return Ok(new MaturityBusiness(_context, _assessmentUtil, _adminTabBusiness).GetAnswerCompletionRate(assessmentId));
        }


        /// <summary>
        /// Get all EDM glossary entries in alphabetical order.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("api/GetGlossary")]
        public IActionResult GetGlossaryEntries(string model)
        {
            MaturityBusiness MaturityBusiness = new MaturityBusiness(_context, _assessmentUtil, _adminTabBusiness);
            return Ok(MaturityBusiness.GetGlossaryEntries(model));
        }


        // --------------------------------------
        // The controller methods that follow were originally built for NCUA/ACET.
        // It is hoped that they will eventually be refactored to fit a more
        // 'generic' approach to maturity models.
        // --------------------------------------


        /// <summary>
        /// Get maturity calculations
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/getMaturityResults")]
        public IActionResult GetMaturityResults()
        {
            int assessmentId = _tokenManager.AssessmentForUser();
            MaturityBusiness manager = new MaturityBusiness(_context, _assessmentUtil, _adminTabBusiness);
            var maturity = manager.GetMaturityAnswers(assessmentId);

            return Ok(maturity);
        }


        /// <summary>
        /// Get maturity range based on IRP rating
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/getMaturityRange")]
        public IActionResult GetMaturityRange()
        {
            int assessmentId = _tokenManager.AssessmentForUser();
            MaturityBusiness manager = new MaturityBusiness(_context, _assessmentUtil, _adminTabBusiness);
            var maturityRange = manager.GetMaturityRange(assessmentId);
            return Ok(maturityRange);
        }


        /// <summary>
        /// Get IRP total for maturity
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/getOverallIrpForMaturity")]
        public IActionResult GetOverallIrp()
        {
            int assessmentId = _tokenManager.AssessmentForUser();
            return Ok(new AcetBusiness(_context, _assessmentUtil, _adminTabBusiness).GetOverallIrp(assessmentId));
        }


        /// <summary>
        /// Get target band for maturity
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/getTargetBand")]
        public IActionResult GetTargetBand()
        {
            int assessmentId = _tokenManager.AssessmentForUser();
            return Ok(new MaturityBusiness(_context, _assessmentUtil, _adminTabBusiness).GetTargetBandOnly(assessmentId));
        }


        /// <summary>
        /// Set target band for maturity rating
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/setTargetBand")]
        public IActionResult SetTargetBand([FromBody] bool value)
        {
            int assessmentId = _tokenManager.AssessmentForUser();
            new MaturityBusiness(_context, _assessmentUtil, _adminTabBusiness).SetTargetBandOnly(assessmentId, value);
            return Ok();
        }


        /// <summary>
        /// get maturity definiciency list
        /// </summary>
        /// <param name="maturity"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/getMaturityDeficiencyList")]
        public IActionResult GetDeficiencyList()
        {
            try
            {
                int assessmentId = _tokenManager.AssessmentForUser();
                _reports.SetReportsAssessmentId(assessmentId);

                var data = new MaturityBasicReportData
                {
                    DeficienciesList = _reports.GetMaturityDeficiencies(),
                    Information = _reports.GetInformation()
                };


                // null out a few navigation properties to avoid circular references that blow up the JSON stringifier
                data.DeficienciesList.ForEach(d =>
                {
                    d.ANSWER.Assessment = null;
                    d.Mat.Maturity_Model = null;
                    d.Mat.Maturity_LevelNavigation = null;
                    d.Mat.InverseParent_Question = null;

                    if (d.Mat.Parent_Question != null)
                    {
                        d.Mat.Parent_Question.Maturity_Model = null;
                        d.Mat.Parent_Question.Maturity_LevelNavigation = null;
                        d.Mat.Parent_Question.InverseParent_Question = null;
                    }
                });


                return Ok(data);
            }
            catch (Exception exc)
            {
                log4net.LogManager.GetLogger(this.GetType()).Error($"... {exc}");

                return Ok();
            }
        }


        /// <summary>
        /// get all comments and marked for review
        /// </summary>
        /// <param name="maturity"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/getCommentsMarked")]
        public IActionResult GetCommentsMarked()
        {
            int assessmentId = _tokenManager.AssessmentForUser();
            _reports.SetReportsAssessmentId(assessmentId);

            MaturityBasicReportData data = new MaturityBasicReportData
            {
                Comments = _reports.GetCommentsList(),
                MarkedForReviewList = _reports.GetMarkedForReviewList(),
                Information = _reports.GetInformation()
            };


            // null out a few navigation properties to avoid circular references that blow up the JSON stringifier
            data.Comments.ForEach(d =>
            {
                d.ANSWER.Assessment = null;
                d.Mat.Maturity_Model = null;
                d.Mat.Maturity_LevelNavigation = null;
                d.Mat.InverseParent_Question = null;
            });

            data.MarkedForReviewList.ForEach(d =>
            {
                d.ANSWER.Assessment = null;
                d.Mat.Maturity_Model = null;
                d.Mat.Maturity_LevelNavigation = null;
                d.Mat.InverseParent_Question = null;
            });

            return Ok(data);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/getEdmScores")]
        public IActionResult GetEdmScores(string section)
        {
            try
            {
                int assessmentId = _tokenManager.AssessmentForUser();
                MaturityBusiness MaturityBusiness = new MaturityBusiness(_context, _assessmentUtil, _adminTabBusiness);
                var scores = MaturityBusiness.GetEdmScores(assessmentId, section);

                return Ok(scores);
            }
            catch (Exception exc)
            {
                log4net.LogManager.GetLogger(this.GetType()).Error($"... {exc}");

                return BadRequest();
            }
        }


        /// <summary>
        /// 
        /// </summary>        
        /// <returns>Root node</returns>
        [HttpGet]
        [Route("api/getEdmPercentScores")]
        public IActionResult GetEdmPercentScores()
        {
            try
            {
                int assessmentId = _tokenManager.AssessmentForUser();
                MaturityBusiness MaturityBusiness = new MaturityBusiness(_context, _assessmentUtil, _adminTabBusiness);
                var scores = MaturityBusiness.GetEdmPercentScores(assessmentId);

                return Ok(scores);
            }
            catch (Exception exc)
            {
                log4net.LogManager.GetLogger(this.GetType()).Error($"... {exc}");

                return BadRequest();
            }
        }


        /// <summary>
        /// Get EDM answers cross-mapped to NIST CSF.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/getEdmNistCsfResults")]
        public IActionResult GetEdmNistCsfResults()
        {
            int assessmentId = _tokenManager.AssessmentForUser();
            var manager = new EdmNistCsfMapping(_context);
            var maturity = manager.GetEdmNistCsfResults(assessmentId);

            return Ok(maturity);
        }


        /// <summary>
        /// Returns all reference text for the specified maturity model.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/referencetext")]
        public IActionResult GetReferenceText(string model)
        {
            try
            {
                var MaturityBusiness = new MaturityBusiness(_context, _assessmentUtil, _adminTabBusiness);
                var refText = MaturityBusiness.GetReferenceText(model);

                return Ok(refText);
            }
            catch (Exception exc)
            {
                log4net.LogManager.GetLogger(this.GetType()).Error($"... {exc}");

                return BadRequest();
            }
        }
    }
}
