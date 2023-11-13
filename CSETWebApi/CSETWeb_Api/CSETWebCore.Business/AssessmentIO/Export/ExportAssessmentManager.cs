//////////////////////////////// 
// 
//   Copyright 2023 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 
using CSETWebCore.DataLayer.Model;
using Microsoft.EntityFrameworkCore;
using Nelibur.ObjectMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using CSETWebCore.Business.ImportAssessment.Models.Version_10_1;
using CSETWebCore.Model.AssessmentIO;
using CSETWebCore.Helpers;
using Ionic.Zip;
using Microsoft.IdentityModel.Tokens;

namespace CSETWebCore.Business.AssessmentIO.Export
{
    public class AssessmentExportManager
    {
        private readonly CSETContext _context;
        private readonly IDictionary<int, string> questionGroupHeadings;
        private readonly IDictionary<int, string> universalSubHeadings;

        public AssessmentExportManager(CSETContext context)
        {
            this._context = context;

            questionGroupHeadings = _context.QUESTION_GROUP_HEADING.Select(t => new { t.Question_Group_Heading_Id, t.Question_Group_Heading1 })
                .ToDictionary(t => t.Question_Group_Heading_Id, t => t.Question_Group_Heading1);

            universalSubHeadings = _context.UNIVERSAL_SUB_CATEGORIES.Select(t => new { t.Universal_Sub_Category_Id, t.Universal_Sub_Category })
                .ToDictionary(t => t.Universal_Sub_Category_Id, t => t.Universal_Sub_Category);

            SetupBindings();
        }

        private void SetupBindings()
        {
         
            TinyMapper.Bind<ACCESS_KEY_ASSESSMENT, jACCESS_KEY_ASSESSMENT>();
            TinyMapper.Bind<ADDRESS, jADDRESS>();
            TinyMapper.Bind<ANSWER, jANSWER>();
            TinyMapper.Bind<ANSWER_PROFILE, jANSWER_PROFILE>();
            TinyMapper.Bind<ASSESSMENT_CONTACTS, jASSESSMENT_CONTACTS>();
            TinyMapper.Bind<ASSESSMENT_DIAGRAM_COMPONENTS, jASSESSMENT_DIAGRAM_COMPONENTS>();
            TinyMapper.Bind<ASSESSMENT_IRP, jASSESSMENT_IRP>();
            TinyMapper.Bind<ASSESSMENT_IRP_HEADER, jASSESSMENT_IRP_HEADER>();
            TinyMapper.Bind<ASSESSMENT_SELECTED_LEVELS, jASSESSMENT_SELECTED_LEVELS>();
            TinyMapper.Bind<ASSESSMENTS, jASSESSMENTS>();
            TinyMapper.Bind<ASSESSMENTS_REQUIRED_DOCUMENTATION, jASSESSMENTS_REQUIRED_DOCUMENTATION>();
            TinyMapper.Bind<AVAILABLE_MATURITY_MODELS, jAVAILABLE_MATURITY_MODELS>();
            TinyMapper.Bind<AVAILABLE_STANDARDS, jAVAILABLE_STANDARDS>();
            TinyMapper.Bind<CIS_CSI_ORGANIZATION_DEMOGRAPHICS, jCIS_CSI_ORGANIZATION_DEMOGRAPHICS>();
            TinyMapper.Bind<CIS_CSI_SERVICE_COMPOSITION, jCIS_CSI_SERVICE_COMPOSITION>();
            TinyMapper.Bind<CIS_CSI_SERVICE_COMPOSITION_SECONDARY_DEFINING_SYSTEMS, jCIS_CSI_SERVICE_COMPOSITION_SECONDARY_DEFINING_SYSTEMS>();
            TinyMapper.Bind<CIS_CSI_SERVICE_DEMOGRAPHICS, jCIS_CSI_SERVICE_DEMOGRAPHICS>();
            TinyMapper.Bind<CIS_CSI_USER_COUNTS, jCIS_CSI_USER_COUNTS>();
            TinyMapper.Bind<CNSS_CIA_JUSTIFICATIONS, jCNSS_CIA_JUSTIFICATIONS>();
            TinyMapper.Bind<COUNTY_ANSWERS, jCOUNTY_ANSWERS>();
            TinyMapper.Bind<CSAF_FILE, jCSAF_FILE>();
            TinyMapper.Bind<CSET_VERSION, jCSET_VERSION>();
            TinyMapper.Bind<CUSTOM_BASE_STANDARDS, jCUSTOM_BASE_STANDARDS>();
            TinyMapper.Bind<CUSTOM_QUESTIONAIRE_QUESTIONS, jCUSTOM_QUESTIONAIRE_QUESTIONS>();
            TinyMapper.Bind<CUSTOM_QUESTIONAIRES, jCUSTOM_QUESTIONAIRES>();
            TinyMapper.Bind<DEMOGRAPHIC_ANSWERS, jDEMOGRAPHIC_ANSWERS>();
            TinyMapper.Bind<DEMOGRAPHICS, jDEMOGRAPHICS>();
            TinyMapper.Bind<DETAILS_DEMOGRAPHICS, jDETAILS_DEMOGRAPHICS>();
            TinyMapper.Bind<DIAGRAM_CONTAINER, jDIAGRAM_CONTAINER>();
            TinyMapper.Bind<DOCUMENT_ANSWERS, jDOCUMENT_ANSWERS>();
            TinyMapper.Bind<DOCUMENT_FILE, jDOCUMENT_FILE>();
            TinyMapper.Bind<FINANCIAL_ASSESSMENT_VALUES, jFINANCIAL_ASSESSMENT_VALUES>();
            TinyMapper.Bind<FINANCIAL_HOURS, jFINANCIAL_HOURS>();
            TinyMapper.Bind<FINDING, jFINDING>();
            TinyMapper.Bind<FINDING_CONTACT, jFINDING_CONTACT>();
            TinyMapper.Bind<FRAMEWORK_TIER_TYPE_ANSWER, jFRAMEWORK_TIER_TYPE_ANSWER>();
            TinyMapper.Bind<GENERAL_SAL, jGENERAL_SAL>();
            TinyMapper.Bind<HYDRO_DATA_ACTIONS, jHYDRO_DATA_ACTIONS>();
            TinyMapper.Bind<INFORMATION, jINFORMATION>();
            TinyMapper.Bind<METRO_ANSWERS, jMETRO_ANSWERS>();
            TinyMapper.Bind<NETWORK_WARNINGS, jNETWORK_WARNINGS>();
            TinyMapper.Bind<NIST_SAL_INFO_TYPES, jNIST_SAL_INFO_TYPES>();
            TinyMapper.Bind<NIST_SAL_QUESTION_ANSWERS, jNIST_SAL_QUESTION_ANSWERS>();
            TinyMapper.Bind<PARAMETER_ASSESSMENT, jPARAMETER_ASSESSMENT>();
            TinyMapper.Bind<PARAMETER_VALUES, jPARAMETER_VALUES>();
            TinyMapper.Bind<REGION_ANSWERS, jREGION_ANSWERS>();
            TinyMapper.Bind<REPORT_DETAIL_SECTION_SELECTION, jREPORT_DETAIL_SECTION_SELECTION>();
            TinyMapper.Bind<REPORT_OPTIONS_SELECTION, jREPORT_OPTIONS_SELECTION>();
            TinyMapper.Bind<STANDARD_SELECTION, jSTANDARD_SELECTION>();
            TinyMapper.Bind<SUB_CATEGORY_ANSWERS, jSUB_CATEGORY_ANSWERS>();
            TinyMapper.Bind<USER_DETAIL_INFORMATION, jUSER_DETAIL_INFORMATION>();
            TinyMapper.Bind<NIST_SAL_QUESTIONS, jNIST_SAL_QUESTION_ANSWERS>(config =>
            {
                config.Ignore(x => x.Question_Text);
            });
            TinyMapper.Bind<AVAILABLE_MATURITY_MODELS, jAVAILABLE_MATURITY_MODELS>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="assessmentId"></param>
        /// <returns></returns>
        private UploadAssessmentModel CopyForExport(int assessmentId)
        {
            var assessmentDate = DateTime.MinValue;
            var model = new UploadAssessmentModel();
            foreach (var item in _context.ANSWER_PROFILE.Where(x => x.Asessment_Id == assessmentId))
            {
                model.jANSWER_PROFILE.Add(TinyMapper.Map<ANSWER_PROFILE,jANSWER_PROFILE>(item));
               
            }

            foreach (var item in _context.CIS_CSI_ORGANIZATION_DEMOGRAPHICS.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jCIS_CSI_ORGANIZATION_DEMOGRAPHICS.Add(TinyMapper.Map<CIS_CSI_ORGANIZATION_DEMOGRAPHICS, jCIS_CSI_ORGANIZATION_DEMOGRAPHICS>(item));

            }

            foreach (var item in _context.CIS_CSI_SERVICE_COMPOSITION.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jCIS_CSI_SERVICE_COMPOSITION.Add(TinyMapper.Map<CIS_CSI_SERVICE_COMPOSITION, jCIS_CSI_SERVICE_COMPOSITION>(item));

            }
            foreach (var item in _context.CIS_CSI_SERVICE_COMPOSITION_SECONDARY_DEFINING_SYSTEMS.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jCIS_CSI_SERVICE_COMPOSITION_SECONDARY_DEFINING_SYSTEMS.Add(TinyMapper.Map<CIS_CSI_SERVICE_COMPOSITION_SECONDARY_DEFINING_SYSTEMS, jCIS_CSI_SERVICE_COMPOSITION_SECONDARY_DEFINING_SYSTEMS>(item));

            }
            foreach (var item in _context.CIS_CSI_SERVICE_DEMOGRAPHICS.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jCIS_CSI_SERVICE_DEMOGRAPHICS.Add(TinyMapper.Map<CIS_CSI_SERVICE_DEMOGRAPHICS, jCIS_CSI_SERVICE_DEMOGRAPHICS>(item));

            }
            var userCount = _context.CIS_CSI_USER_COUNTS.ToList();
            foreach (var item in userCount)
            {
                model.jCIS_CSI_USER_COUNTS.Add(TinyMapper.Map<CIS_CSI_USER_COUNTS, jCIS_CSI_USER_COUNTS>(item));
            }
            foreach (var item in _context.COUNTY_ANSWERS.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jCOUNTY_ANSWERS.Add(TinyMapper.Map<COUNTY_ANSWERS, jCOUNTY_ANSWERS>(item));

            }
            foreach (var item in _context.CSAF_FILE)
            {
                model.jCSAF_FILE.Add(TinyMapper.Map<CSAF_FILE, jCSAF_FILE>(item));

            }
            foreach (var item in _context.DEMOGRAPHIC_ANSWERS.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jDEMOGRAPHIC_ANSWERS.Add(TinyMapper.Map<DEMOGRAPHIC_ANSWERS, jDEMOGRAPHIC_ANSWERS>(item));

            }
            foreach (var item in _context.DETAILS_DEMOGRAPHICS.Where(x => x.Assessment_Id == assessmentId)) 
            {
                model.jDETAILS_DEMOGRAPHICS.Add(TinyMapper.Map<DETAILS_DEMOGRAPHICS, jDETAILS_DEMOGRAPHICS>(item));
            }
            foreach (var item in _context.METRO_ANSWERS.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jMETRO_ANSWERS.Add(TinyMapper.Map<METRO_ANSWERS, jMETRO_ANSWERS>(item));

            }
            foreach (var item in _context.NETWORK_WARNINGS.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jNETWORK_WARNINGS.Add(TinyMapper.Map<NETWORK_WARNINGS, jNETWORK_WARNINGS>(item));

            }
            foreach (var item in _context.REGION_ANSWERS.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jREGION_ANSWERS.Add(TinyMapper.Map<REGION_ANSWERS, jREGION_ANSWERS>(item));
            }
            foreach (var item in _context.REPORT_DETAIL_SECTION_SELECTION.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jREPORT_DETAIL_SECTION_SELECTION.Add(TinyMapper.Map<REPORT_DETAIL_SECTION_SELECTION, jREPORT_DETAIL_SECTION_SELECTION>(item));
            }
            foreach (var item in _context.REPORT_OPTIONS_SELECTION.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jREPORT_OPTIONS_SELECTION.Add(TinyMapper.Map<REPORT_OPTIONS_SELECTION, jREPORT_OPTIONS_SELECTION>(item));
            }
        
            foreach (var item in _context.ASSESSMENTS.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jASSESSMENTS.Add(TinyMapper.Map<ASSESSMENTS, jASSESSMENTS>(item));
                assessmentDate = item.Assessment_Date;
            }


            foreach (var item in _context.ASSESSMENT_CONTACTS.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jASSESSMENT_CONTACTS.Add(TinyMapper.Map<ASSESSMENT_CONTACTS,jASSESSMENT_CONTACTS>(item));
            }
            foreach (var accessKey in _context.ACCESS_KEY_ASSESSMENT.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jACCESS_KEY_ASSESSMENT.Add(TinyMapper.Map<ACCESS_KEY_ASSESSMENT, jACCESS_KEY_ASSESSMENT>(accessKey));

            }
            foreach (var item in _context.ANSWER
                .Include(x => x.FINDING).ThenInclude(x => x.ISE_ACTIONS_FINDINGS)
                .Include(x => x.FINDING).ThenInclude(x => x.FINDING_CONTACT)
                .Include(x => x.HYDRO_DATA_ACTIONS)
                .Where(x => x.Assessment_Id == assessmentId))
            {
                model.jANSWER.Add(TinyMapper.Map<ANSWER,jANSWER>(item));

                if (item.HYDRO_DATA_ACTIONS != null)
                {
                    model.jHYDRO_DATA_ACTIONS.Add(TinyMapper.Map<HYDRO_DATA_ACTIONS, jHYDRO_DATA_ACTIONS>(item.HYDRO_DATA_ACTIONS));
                }
                
                foreach (var f in item.FINDING)
                {
                    var obs = TinyMapper.Map<FINDING, jFINDING>(f);
                    model.jFINDING.Add(obs);

                    foreach (var fc in f.FINDING_CONTACT)
                    {
                        model.jFINDING_CONTACT.Add(TinyMapper.Map<FINDING_CONTACT,jFINDING_CONTACT>(fc));
                    }

                    foreach (var action in f.ISE_ACTIONS_FINDINGS)
                    {
                        if (action != null)
                        {
                            model.jISE_ACTIONS_FINDINGS.Add(TinyMapper.Map<ISE_ACTIONS_FINDINGS, jISE_ACTIONS_FINDINGS>(action));
                        }
                    }
                }
            }

            foreach (var item in _context.ASSESSMENT_SELECTED_LEVELS.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jASSESSMENT_SELECTED_LEVELS.Add(TinyMapper.Map<ASSESSMENT_SELECTED_LEVELS,jASSESSMENT_SELECTED_LEVELS>(item));
            }

            foreach (var item in _context.AVAILABLE_STANDARDS.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jAVAILABLE_STANDARDS.Add(TinyMapper.Map<AVAILABLE_STANDARDS,jAVAILABLE_STANDARDS>(item));
            }

            foreach (var item in _context.CNSS_CIA_JUSTIFICATIONS.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jCNSS_CIA_JUSTIFICATIONS.Add(TinyMapper.Map<CNSS_CIA_JUSTIFICATIONS,jCNSS_CIA_JUSTIFICATIONS>(item));
            }

            foreach (var item in _context.CSET_VERSION)
            {
                model.jCSET_VERSION.Add(TinyMapper.Map<CSET_VERSION,jCSET_VERSION>(item));
            }
     
            foreach (var item in _context.CUSTOM_BASE_STANDARDS)
            {
                model.jCUSTOM_BASE_STANDARDS.Add(TinyMapper.Map<CUSTOM_BASE_STANDARDS,jCUSTOM_BASE_STANDARDS>(item));
            }

            foreach (var item in _context.CUSTOM_QUESTIONAIRES)
            {
                model.jCUSTOM_QUESTIONAIRES.Add(TinyMapper.Map<CUSTOM_QUESTIONAIRES,jCUSTOM_QUESTIONAIRES>(item));
            }

            foreach (var item in _context.CUSTOM_QUESTIONAIRE_QUESTIONS)
            {
                model.jCUSTOM_QUESTIONAIRE_QUESTIONS.Add(TinyMapper.Map<CUSTOM_QUESTIONAIRE_QUESTIONS,jCUSTOM_QUESTIONAIRE_QUESTIONS>(item));
            }

            foreach (var item in _context.DEMOGRAPHICS.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jDEMOGRAPHICS.Add(TinyMapper.Map<DEMOGRAPHICS,jDEMOGRAPHICS>(item));
            }

            foreach (var item in _context.DOCUMENT_FILE.Include(x => x.DOCUMENT_ANSWERS).ThenInclude(x => x.Answer).Where(x => x.Assessment_Id == assessmentId))
            {
                model.jDOCUMENT_FILE.Add(TinyMapper.Map<DOCUMENT_FILE,jDOCUMENT_FILE>(item));
                foreach (var a in item.ANSWERs(_context))
                {
                    model.jDOCUMENT_ANSWERS.Add(new jDOCUMENT_ANSWERS()
                    {
                        Answer_Id = a.Answer_Id,
                        Document_Id = item.Document_Id
                    });
                }
            }

            foreach (var item in _context.FRAMEWORK_TIER_TYPE_ANSWER.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jFRAMEWORK_TIER_TYPE_ANSWER.Add(TinyMapper.Map<FRAMEWORK_TIER_TYPE_ANSWER,jFRAMEWORK_TIER_TYPE_ANSWER>(item));
            }

            foreach (var item in _context.INFORMATION.Where(x => x.Id == assessmentId))
            {
                var oInfo = TinyMapper.Map<INFORMATION,jINFORMATION>(item);
                oInfo.Assessment_Date = assessmentDate;
                oInfo.Baseline_Assessment_Id = null;
                model.jINFORMATION.Add(oInfo);
            }

            foreach (var item in _context.NIST_SAL_INFO_TYPES.Where(x => x.Selected == true && x.Assessment_Id == assessmentId))
            {
                model.jNIST_SAL_INFO_TYPES.Add(TinyMapper.Map<NIST_SAL_INFO_TYPES,jNIST_SAL_INFO_TYPES>(item));
            }

            foreach (var item in _context.NIST_SAL_QUESTION_ANSWERS.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jNIST_SAL_QUESTION_ANSWERS.Add(TinyMapper.Map<NIST_SAL_QUESTION_ANSWERS,jNIST_SAL_QUESTION_ANSWERS>(item));
            }

            var parameterslist = from a in _context.ASSESSMENTS
                                 join an in _context.ANSWER on a.Assessment_Id equals an.Assessment_Id
                                 join p in _context.PARAMETER_VALUES on an.Answer_Id equals p.Answer_Id
                                 where a.Assessment_Id == assessmentId
                                 select p;
            foreach (var item in parameterslist.Where(x => x.Parameter_Is_Default == false))
            {
                model.jPARAMETER_VALUES.Add(TinyMapper.Map<PARAMETER_VALUES,jPARAMETER_VALUES>(item));
            }

            foreach (var item in _context.PARAMETER_ASSESSMENT.Where(x => x.Assessment_ID == assessmentId))
            {
                model.jPARAMETER_ASSESSMENTs.Add(TinyMapper.Map<PARAMETER_ASSESSMENT,jPARAMETER_ASSESSMENT>(item));
            }

            foreach (var item in _context.STANDARD_SELECTION.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jSTANDARD_SELECTION.Add(TinyMapper.Map<STANDARD_SELECTION,jSTANDARD_SELECTION>(item));
            }

            foreach (var item in _context.GENERAL_SAL.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jGENERAL_SAL.Add(TinyMapper.Map<GENERAL_SAL,jGENERAL_SAL>(item));
            }

            foreach (var item in _context.SUB_CATEGORY_ANSWERS.Where(x => x.Assessement_Id == assessmentId))
            {
                model.jSUB_CATEGORY_ANSWERS.Add(TinyMapper.Map<SUB_CATEGORY_ANSWERS,jSUB_CATEGORY_ANSWERS>(item));
            }

            // NCUA data
            foreach (var item in _context.FINANCIAL_HOURS.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jFINANCIAL_HOURS.Add(TinyMapper.Map<FINANCIAL_HOURS,jFINANCIAL_HOURS>(item));
            }

            foreach (var item in _context.FINANCIAL_ASSESSMENT_VALUES.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jFINANCIAL_ASSESSMENT_VALUES.Add(TinyMapper.Map<FINANCIAL_ASSESSMENT_VALUES,jFINANCIAL_ASSESSMENT_VALUES>(item));
            }

            foreach (var item in _context.ASSESSMENTS_REQUIRED_DOCUMENTATION.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jASSESSMENTS_REQUIRED_DOCUMENTATION.Add(TinyMapper.Map<ASSESSMENTS_REQUIRED_DOCUMENTATION,jASSESSMENTS_REQUIRED_DOCUMENTATION>(item));
            }

            foreach (var item in _context.ASSESSMENT_IRP_HEADER.Where(x => x.ASSESSMENT_ID == assessmentId))
            {
                model.jASSESSMENT_IRP_HEADER.Add(TinyMapper.Map<ASSESSMENT_IRP_HEADER,jASSESSMENT_IRP_HEADER>(item));
            }

            foreach (var item in _context.ASSESSMENT_IRP.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jASSESSMENT_IRP.Add(TinyMapper.Map<ASSESSMENT_IRP,jASSESSMENT_IRP>(item));
            }

            foreach (var item in _context.ASSESSMENT_DIAGRAM_COMPONENTS.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jASSESSMENT_DIAGRAM_COMPONENTS.Add(TinyMapper.Map<ASSESSMENT_DIAGRAM_COMPONENTS,jASSESSMENT_DIAGRAM_COMPONENTS>(item));
            }

            foreach (var item in _context.DIAGRAM_CONTAINER.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jDIAGRAM_CONTAINER.Add(TinyMapper.Map<DIAGRAM_CONTAINER,jDIAGRAM_CONTAINER>(item));
            }

            foreach (var item in _context.AVAILABLE_MATURITY_MODELS.Where(x => x.Assessment_Id == assessmentId))
            {
                model.jAVAILABLE_MATURITY_MODELS.Add(TinyMapper.Map<AVAILABLE_MATURITY_MODELS,jAVAILABLE_MATURITY_MODELS>(item));
            }
            return model;
        }

        private Stream ArchiveStream(int assessmentId, string password, string passwordHint)
        {
            var archiveStream = new MemoryStream();
            var model = CopyForExport(assessmentId);

            using (var archive = new ZipFile())
            {
                if (password != null && password != "")
                {
                    archive.Password = password;
                }

                foreach (var standard in model.jAVAILABLE_STANDARDS)
                {
                    if (!standard.Selected)
                    {
                        continue;
                    }

                    var set = _context.SETS
                        .Include(s => s.Set_Category)
                        .Where(s => s.Set_Name == standard.Set_Name).FirstOrDefault();
                    if (set == null || !set.Is_Custom)
                    {
                        continue;
                    }


                    var qq = from nq in _context.NEW_QUESTION
                             join nqs in _context.NEW_QUESTION_SETS on nq.Question_Id equals nqs.Question_Id
                             where nqs.Set_Name == standard.Set_Name
                             select nq;

                    var questions = qq.ToList();
                    set.NEW_QUESTION = new List<NEW_QUESTION>(questions);


                    var rq = _context.REQUIREMENT_SETS
                        .Include(s => s.Requirement)
                        .ThenInclude(s => s.REQUIREMENT_LEVELS)
                        .Where(s => s.Set_Name == standard.Set_Name)
                        .Select(s => s.Requirement);

                    set.NEW_REQUIREMENT = new List<NEW_REQUIREMENT>(rq.ToList());



                    var extStandard = StandardConverter.ToExternalStandard(set, _context);
                    var setname = Regex.Replace(extStandard.shortName, @"\W", "_");

                    // Export Set
                    //var standardEntry = archive.CreateEntry($"{setname}.json");
                    var jsonStandard = JsonConvert.SerializeObject(extStandard, Formatting.Indented);

                    ZipEntry standardEntry = archive.AddEntry($"{setname}.json", jsonStandard);


                    //Set the GUID at time of export so we are sure it's right!!!
                    model.jANSWER = model.jANSWER.Where(s => s.Is_Requirement).GroupJoin(set.NEW_REQUIREMENT, s => s.Question_Or_Requirement_Id, s => s.Requirement_Id, (t, s) =>
                    {
                        var req = s.FirstOrDefault();
                        if (req != null)
                        {
                            var buffer = Encoding.Default.GetBytes($"{extStandard.shortName}|||{req.Requirement_Title}|||{req.Requirement_Text}");
                            t.Custom_Question_Guid = new Guid(MD5.Create().ComputeHash(buffer)).ToString();
                        }
                        return t;
                    }).Concat(model.jANSWER.Where(s => !s.Is_Requirement).GroupJoin(questions, s => s.Question_Or_Requirement_Id, s => s.Question_Id, (t, s) =>
                    {
                        var req = s.FirstOrDefault();
                        if (req != null)
                        {
                            var buffer = Encoding.Default.GetBytes(req.Simple_Question);
                            t.Custom_Question_Guid = new Guid(MD5.Create().ComputeHash(buffer)).ToString();
                        }
                        return t;
                    })).ToList();

                    model.CustomStandards.Add(setname);

                    var files = extStandard.requirements.SelectMany(s => s.references.Concat(new ExternalResource[] { s.source })).OfType<ExternalResource>().Distinct();
                    foreach (var file in files)
                    {
                        var genFile = _context.GEN_FILE.FirstOrDefault(s => s.File_Name == file.fileName && (s.Is_Uploaded ?? false));
                        if (genFile == null || model.CustomStandardDocs.Contains(file.fileName))
                            continue;

                        var doc = genFile.ToExternalDocument();
                        var jsonDoc = JsonConvert.SerializeObject(doc, Formatting.Indented);

                        ZipEntry docEntry = archive.AddEntry($"{doc.ShortName}.json", jsonDoc);

                        model.CustomStandardDocs.Add(file.fileName);
                    }
                }

                model.ExportDateTime = DateTime.UtcNow;

                var json = JsonConvert.SerializeObject(model, Formatting.Indented);
                ZipEntry jsonEntry = archive.AddEntry("model.json", json);
                ZipEntry hint = archive.AddEntry($"{passwordHint}.hint", passwordHint);
                archive.Save(archiveStream);
            }


            archiveStream.Seek(0, SeekOrigin.Begin);
            return archiveStream;
        }

        /// <summary>
        /// Export an assessment by its ID. 
        /// Can optionally provide a password and password hint that will be used during import process.
        /// </summary>
        /// <param name="assessmentId">The ID of the assessment to export</param>
        /// <param name="fileExtension">The extension of the export file</param>
        /// <param name="password">If not empty, this password will be required to import the assessment</param>
        /// <param name="passwordHint">An optional password hint</param>
        /// <returns>An AssessmentExportFile object containing the file name and the file contents</returns>
        public AssessmentExportFile ExportAssessment(int assessmentId, string fileExtension, string password = "", string passwordHint = "") 
        {
            // determine file name
            var fileName = $"{assessmentId}{fileExtension}";
            var assessmentName = _context.INFORMATION.Where(x => x.Id == assessmentId).FirstOrDefault()?.Assessment_Name;
            if (!string.IsNullOrEmpty(assessmentName))
            {
                fileName = $"{assessmentName}{fileExtension}";
            }

            // export the assessment
            Stream assessmentFileContents = ArchiveStream(assessmentId, password, passwordHint);
            return new AssessmentExportFile(fileName, assessmentFileContents);
        }

        /// <summary>
        /// Exports access key assessments in the current DB context and returns them in a zip archive.
        /// </summary>
        /// <param name="guids">Array of assessment guids to export</param>
        /// <param name="fileExtension">The extension of the export files</param>
        /// <returns></returns>
        public Stream BulkExportAssessmentsbyGuid(Guid[] guids, string fileExtension) 
        {

            var archiveStream = new MemoryStream();

            var exportAssessments = _context.ASSESSMENTS.Where(a => guids.Contains(a.Assessment_GUID)).ToList();

            // Assessments with provided guids do not exist.
            if (exportAssessments.IsNullOrEmpty()) 
            {
                return null;
            }

            // Zip all the assessments into one archive
            using (var archive = new ZipFile()) 
            {
                // export the assessments
                foreach (ASSESSMENTS a in exportAssessments)
                {
                    AssessmentExportFile exportFile = ExportAssessment(a.Assessment_Id, fileExtension);
                    archive.AddEntry(exportFile.FileName, exportFile.FileContents);
                }

                archive.Save(archiveStream);
            }

            archiveStream.Seek(0, SeekOrigin.Begin);
            return archiveStream;
        }
    }
}
