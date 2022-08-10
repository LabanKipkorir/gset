﻿using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.XPath;
using CSETWebCore.Interfaces.Assessment;
using CSETWebCore.Interfaces.Crr;
using CSETWebCore.Interfaces.Helpers;
using CSETWebCore.Interfaces.Maturity;
using CSETWebCore.Reports.Helper;
using CSETWebCore.Business.Reports;
using CSETWebCore.Reports.Models;
using CSETWebCore.Interfaces.Reports;
using CSETWebCore.DataLayer.Model;
using IronPdf;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using CSETWebCore.Interfaces.Demographic;


namespace CSETWebCore.Reports.Controllers
{
    public class CrrController : Controller
    {
        private readonly IViewEngine _engine;
        private readonly ITokenManager _token;
        private readonly IAssessmentBusiness _assessment;
        private readonly IDemographicBusiness _demographic;
        private readonly IReportsDataBusiness _report;
        private readonly CSETContext _context;
        private readonly IMaturityBusiness _maturity;
        private readonly ICrrScoringHelper _crr;


        private readonly IDictionary<string, string> _viewToTitle = new Dictionary<string, string>
        {
            {"_crrcoversheet", "Top"},
            {"_crrcoversheet2", "Title"},
            {"_crrintroabout", "Introduction" },
            {"_crrmil1performancesummary", "CRR MIL-1 Performance Summary" },
            {"_crrperformancesummary", "CRR Performance Summary" },
            {"_crrnistcsfsummary", "NIST Cybersecurity Framework Summary" },
            {"_crrmil1performance", "CRR MIL-1 Performance" },
            {"_crrresultssummary", "CRR Results Summary" },
            {"_crrpercentageofpractices", "Percentage of Practices Completed by Domain" },
            {"_crrdd1_am", "Asset Management"},
            {"_crrdd2_cm", "Controls Management"},
            {"_crrdd3_ccm", "Configuration and Change Management"},
            {"_crrdd4_vm", "Vulnerability Management"},
            {"_crrdd5_im", "Incident Management"},
            {"_crrdd6_scm", "Service Continuity Management"},
            {"_crrdd7_rm", "Risk Management"},
            {"_crrdd8_edm", "External Dependencies Management"},
            {"_crrdd9_ta", "Training and Awareness"},
            {"_crrdd10_sa", "Situational Awareness"},
            {"_crrresources", "Resource List" },
            {"_crrcontactinformation", "Contact Information" },
            {"_crrappendixacover", "Appendix A" },
            {"_crrperformanceappendixa", "CRR Performance" },
            {"_crrnistcsfcatsummary", "NIST Category Summary" },
            {"_crrnistcsfcatperformance", "NIST Category Performance" },
            {"_crrmaintoc",  "Table of Contents"}
        };


        public CrrController(IViewEngine engine, ITokenManager token,
            IAssessmentBusiness assessment,
            IDemographicBusiness demographic,
            CSETContext context, IReportsDataBusiness report, IMaturityBusiness maturity, ICrrScoringHelper crr)
        {
            _engine = engine;
            _token = token;
            _assessment = assessment;
            _demographic = demographic;
            _report = report;
            _context = context;
            _maturity = maturity;
            _crr = crr;
        }

        public IActionResult Index()
        {
            TempData["links"] = UrlStringHelper.GetBaseUrl(Request);
            return View();
        }


        
        [HttpGet]
        [Route("getPdf")]
        public async Task<IActionResult> CreatePdf(string view)
        {
            try
            {
                byte[] assessmentIdBytes = null;
                byte[] securityTemp = null;
                int assessmentId = 0;
                string security = "None";
                if (HttpContext.Session.TryGetValue("assessmentId", out assessmentIdBytes))
                {
                   int.TryParse(Encoding.ASCII.GetString(assessmentIdBytes), out assessmentId);
                }

                if (HttpContext.Session.TryGetValue("security", out securityTemp))
                {
                    security = Encoding.ASCII.GetString(securityTemp);
                }

                _crr.InstantiateScoringHelper(assessmentId);
                var model = GetCrrModel(assessmentId);
                var views = ReportHelper.GetReportList(view);
                string baseUrl = UrlStringHelper.GetBaseUrl(Request);

                _crr.InstantiateScoringHelper(assessmentId);

                // PDFs
                List<PdfDocument> pdf = new List<PdfDocument>();
                PdfDocument tempPdf = null;


                
                int pageNumber = 0;
                // Report Pages
                string coverPage = ReportHelper.GetCoverSheet();
                List<string> marginPages = ReportHelper.GetMarginPages();
                List<string> javaScriptPages = ReportHelper.GetJSPages();
                List<string> assessmentPages = ReportHelper.GetAssessmentPages();
                List<string> nistPages = ReportHelper.GetNistPages();

                foreach (var depiction in views)
                {
                    var html = await ReportHelper.RenderRazorViewToString(this, depiction, model, baseUrl, _engine);

                    // Have to adjust for table of contents getting inserted
                    if (depiction == "_CrrIntroAbout") 
                    {
                        pageNumber++;
                    }

                    if (depiction == "_CrrMainToc") 
                    {
                        pageNumber = 3;
                    }

                    // Each page in the report has varying margins
                    if(depiction == coverPage)
                    {
                        // The cover page has unique margins
                        var margins = new Dictionary<string, int> { { "top", 15 }, { "bottom", 15 }, { "left", 0 }, { "right", 0 } };
                        tempPdf = ReportHelper.RenderPdf(html, security, pageNumber, margins, false, null);
                    }
                    else if(assessmentPages.Contains(depiction))
                    {
                        var title = this._viewToTitle[depiction.ToLower()];
                        var margins = new Dictionary<string, int> { { "top", 35 }, { "bottom", 15 }, { "left", 15 }, { "right", 15 } };
                        tempPdf = ReportHelper.RenderPdf(html, security, pageNumber, margins, false, title);
                    }
                    else if(javaScriptPages.Contains(depiction)) {
                        var margins = new Dictionary<string, int> { { "top", 15 }, { "bottom", 0 }, { "left", 15 }, { "right", 0 } };
                        tempPdf = ReportHelper.RenderPdf(html, security, pageNumber, margins, true);
                    }
                    else if(marginPages.Contains(depiction)) {
                        // Margin pages are involve only text, or tables, requiring wider margins
                        var margins = new Dictionary<string, int> { { "top", 15 }, { "bottom", 15 }, { "left", 15 }, { "right", 15 } };
                        tempPdf = ReportHelper.RenderPdf(html, security, pageNumber, margins);
                    }
                    else if (nistPages.Contains(depiction))
                    {
                        // Margin pages are involve only text, or tables, requiring wider margins
                        var margins = new Dictionary<string, int> { { "top", 15 }, { "bottom", 15 }, { "left", 5 }, { "right", 5 } };
                        tempPdf = ReportHelper.RenderPdf(html, security, pageNumber, margins);
                    }
                    else
                    {
                        // Any other report page is a depiction needing thin margins
                        var margins = new Dictionary<string, int> { { "top", 5 }, { "bottom", 5 }, { "left", 5 }, { "right", 5 } };
                        tempPdf = ReportHelper.RenderPdf(html, security, pageNumber, margins, false, null, false);
                    }

                    // Keeping track of page numbers for each section of the report
                    ((CrrViewModel)model).PageNumbers[depiction] = pageNumber;

                    var viewTitle = depiction.ToLower();
                    viewTitle = _viewToTitle.ContainsKey(viewTitle) ? _viewToTitle[viewTitle] : depiction;
                    tempPdf.BookMarks.AddBookMarkAtStart(viewTitle, pageNumber - 1);

                    // Insert table of contents to beginning of report
                    if (viewTitle == "Table of Contents")
                    {
                        pdf.Insert(2, tempPdf);
                    }
                    else 
                    { 
                        pdf.Add(tempPdf);
                    }

                    pageNumber = pageNumber + tempPdf.PageCount;
                }

                // The PDFs are merged after rendering individually
                var finalPdf = pdf.Count > 1 ? ReportHelper.MergePdf(pdf) : pdf.FirstOrDefault();
                return File(finalPdf.BinaryData, "application/pdf", ReportHelper.GetReportName(view));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("reports/crr/crr")]
        public IActionResult CrrReport(string token, string security)
        {
            if (_token.IsTokenValid(token))
            {
              
               return View(CrrHtmlInit(token, security));
            }

            return Unauthorized();
        }

        [HttpGet]
        [Route("reports/crr/deficiency")]
        public IActionResult CrrDeficiencyReport(string token, string security)
        {
            if (_token.IsTokenValid(token))
            {
                return View(CrrHtmlInit(token, security));
            }

            return Unauthorized();
        }

        [HttpGet]
        [Route("reports/crr/comments")]
        public IActionResult CrrCommentsMarked(string token, string security)
        {
            if (_token.IsTokenValid(token))
            {
                return View(CrrHtmlInit(token, security));
            }

            return Unauthorized();
        }

        [HttpGet]
        [Route("api/report/getCrrModel")]
        public IActionResult GetCrrModel()
        {
            var assessmentId = _token.AssessmentForUser();
            var crrModel = GetCrrModel(assessmentId, "", false);
            return Ok(crrModel);
        }

        private object CrrHtmlInit(string token, string security)
        {
            _token.Init(token);
            Request.Headers.Add("Authorization", token);
            var assessmentId = _token.AssessmentForUser();
            HttpContext.Session.Set("assessmentId", Encoding.ASCII.GetBytes(assessmentId.ToString()));
            HttpContext.Session.Set("security", Encoding.ASCII.GetBytes(security));
            return GetCrrModel(assessmentId);
        }


        /// <summary>
        /// Returns the CrrViewModel.  
        /// </summary>
        /// <param name="assessmentId"></param>
        /// <param name="token"></param>
        /// <param name="includeResultsStylesheet"></param>
        /// <returns></returns>
        private object GetCrrModel(int assessmentId, string token = "", bool includeResultsStylesheet = true)
        {
            _crr.InstantiateScoringHelper(assessmentId);
            var detail = _assessment.GetAssessmentDetail(assessmentId, token);

            var demographics = _demographic.GetDemographics(assessmentId);

            //Testing
            _report.SetReportsAssessmentId(assessmentId);

            var deficiencyData = new MaturityBasicReportData()
            {
                Information = _report.GetInformation(),
                DeficienciesList = _report.GetMaturityDeficiencies(),
                Comments = _report.GetCommentsList(),
                MarkedForReviewList = _report.GetMarkedForReviewList(),
                QuestionsList = _report.GetQuestionsList()
            };
            CrrResultsModel crrResultsData = _crr.GetCrrResultsSummary(); //GenerateCrrResults();
            CrrViewModel viewModel = new CrrViewModel(detail, demographics.CriticalService, _crr, deficiencyData);
            viewModel.IncludeResultsStylesheet = includeResultsStylesheet;
            viewModel.ReportChart = _crr.GetPercentageOfPractice();
            viewModel.crrResultsData = crrResultsData;
            return viewModel;
        }

        [HttpGet]
        [Route("api/report/getCrrHtml")]
        public async Task<IActionResult> GetCrrHtml(string view)
        {
            var assessmentId = _token.AssessmentForUser();
            _crr.InstantiateScoringHelper(assessmentId);
            var model = GetCrrModel(assessmentId, "", false);
            string baseUrl = UrlStringHelper.GetBaseUrl(Request);
            var html = new CrrHtml();
            html.Html = await ReportHelper.RenderRazorViewToString(this, view, model, baseUrl, _engine);

            return Ok(html);
        }

        /// <summary>
        /// Generates and returns markup (SVG) for a MIL
        /// heatmap widget.  
        /// </summary>
        /// <param name="mil"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/report/widget/milheatmap")]
        public IActionResult GetWidget([FromQuery] string domain, [FromQuery] string mil, [FromQuery] double? scale = null)
        {
            var assessmentId = _token.AssessmentForUser();
            _crr.InstantiateScoringHelper(assessmentId);


            var xMil = _crr.XDoc.XPathSelectElement($"//Domain[@abbreviation='{domain}']/Mil[@label='{mil}']");
            if (xMil == null)
            {
                return NotFound();
            }

            // populate the widget with the MIL strip and collapse any hidden goal strips
            var heatmap = new Helpers.ReportWidgets.MilHeatMap(xMil, true, true);
            if (scale != null)
            {
                heatmap.Scale((double)scale);
            }

            // return the svg
            return Content(heatmap.ToString(), "image/svg+xml");
        }

        private CrrResultsModel GenerateCrrResults()
        {
            MaturityReportData maturityData = new MaturityReportData(_context);

            maturityData.MaturityModels = _report.GetMaturityModelData();
            maturityData.information = _report.GetInformation();
            maturityData.AnalyzeMaturityData();


            // null out a few navigation properties to avoid circular references that blow up the JSON stringifier
            maturityData.MaturityModels.ForEach(d =>
            {
                d.MaturityQuestions.ForEach(q =>
                {
                    q.Answer.Assessment = null;
                });
            });

            CrrResultsModel retVal = new CrrResultsModel();
            List<DomainStats> cmmcDataDomainLevelStats = maturityData.MaturityModels.FirstOrDefault(d => d.MaturityModelName == "CRR")?.StatsByDomainAndLevel;
            retVal.EvaluateDataList(cmmcDataDomainLevelStats);
            retVal.TrimToNElements(10);
            retVal.GenerateWidthValues(); //If generating wrong values, check inner method values match the ones set in the css
            return retVal;
        }

        private CrrResultsModel GenerateCrrResults(MaturityReportData data)
        {
            //For Testing

            CrrResultsModel retVal = new CrrResultsModel();
            List<DomainStats> cmmcDataDomainLevelStats = data.MaturityModels.Where(d => d.MaturityModelName == "CRR").First().StatsByDomainAndLevel;
            retVal.EvaluateDataList(cmmcDataDomainLevelStats);
            retVal.TrimToNElements(10);
            retVal.GenerateWidthValues(); //If generating wrong values, check inner method values match the ones set in the css
            return retVal;
        }
    }

    public class CrrHtml
    {
        public string Html { get; set; }
    }
}
