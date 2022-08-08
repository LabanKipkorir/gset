﻿//////////////////////////////// 
// 
//   Copyright 2022 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 
using System;
using System.Collections.Generic;
using System.Linq;
using CSETWebCore.DataLayer.Model;
using System.Text.RegularExpressions;
using CSETWebCore.Model.AssessmentIO;
using CSETWebCore.Helpers;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace CSETWebCore.Business.ModuleIO
{
    /// <summary>
    /// This is an alternate way to convert module JSON
    /// and persist to the database.
    /// </summary>
    public class ModuleImporter
    {
        private readonly CSETContext _context;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public ModuleImporter(CSETContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Imports a Module/Standard.
        /// </summary>
        /// <param name="externalStandard"></param>
        public async Task ProcessStandard(ExternalStandard externalStandard)
        {

            //     private static readonly log4net.ILog log
            //= log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            // CsetLogManager.Instance.LogInfoMessage("ModuleImporter.ProcessStandard - basic");


            // CsetLogManager.Instance.LogDebugMessage("ModuleImporter.ProcessStandard");

            SETS_CATEGORY category;
            int? categoryOrder = 0;
            var setname = Regex.Replace(externalStandard.shortName, @"\W", "_");

            var documentImporter = new DocumentImporter();
            var set = new SETS();


            var existingSet = await _context.SETS.FirstOrDefaultAsync(s => s.Set_Name == setname);
            if (existingSet != null)
            {
                // result.LogError("Module already exists.  If this is a new version, please change the ShortName field to reflect this.");
            }
            category = await _context.SETS_CATEGORY.FirstOrDefaultAsync(s => s.Set_Category_Name.Trim().ToLower() == externalStandard.category.Trim().ToLower());

            if (category == null)
            {
                // result.LogError("Module Category is invalid.  Please check the spelling and try again.");
            }
            else
            {
                categoryOrder = category.SETS.Max(s => s.Order_In_Category);
            }

            set.Set_Category_Id = category?.Set_Category_Id;
            set.Order_In_Category = categoryOrder;
            set.Short_Name = externalStandard.shortName;
            set.Set_Name = setname;
            set.Full_Name = externalStandard.name;
            set.Is_Custom = true;
            set.Is_Question = true;
            set.Is_Requirement = true;
            set.Is_Displayed = true;
            set.IsEncryptedModuleOpen = true;
            set.IsEncryptedModule = false;
            set.Is_Deprecated = false;

            set.Standard_ToolTip = externalStandard.summary;

            await _context.SETS.AddAsync(set);
            await _context.SaveChangesAsync();


            try
            {
                ProcessRequirements(externalStandard, set);
            }
            catch (Exception exc)
            {
                log4net.LogManager.GetLogger(this.GetType()).Error($"... {exc}");
            }
        }


        /// <summary>
        /// Save all Requirements in the specified module.
        /// </summary>
        /// <param name="set"></param>
        /// <param name="db"></param>
        private async Task ProcessRequirements(ExternalStandard externalStandard, SETS set)
        {
            // var jsonStandard = System.Text.Json.JsonSerializer.Serialize(externalStandard);
            //JsonConvert.SerializeObject(externalStandard, Formatting.Indented);

            var questionDictionary = new Dictionary<string, DataLayer.Model.NEW_QUESTION>();
            var categoryDictionary = new Dictionary<string, STANDARD_CATEGORY>();
            var requirementList = new List<string>();

            set.NEW_REQUIREMENT = new List<DataLayer.Model.NEW_REQUIREMENT>();
            var requirements = set.NEW_REQUIREMENT;
            int reqSequence = 0;

            foreach (var requirement in externalStandard.requirements)
            {
                //skip duplicates
                if (!requirementList.Any(s => s == requirement.identifier.Trim().ToLower() + "|||" + requirement.text.Trim().ToLower()))
                {
                    reqSequence++;
                    var requirementResult = await SaveRequirement(requirement, set.Set_Name, reqSequence   /*, new ConsoleLogger() */     );


                    if (requirementResult != null)
                    {
                        if (requirementResult.Standard_CategoryNavigation != null)
                        {
                            STANDARD_CATEGORY tempCategory;
                            if (categoryDictionary.TryGetValue(requirementResult.Standard_CategoryNavigation.Standard_Category1, out tempCategory))
                            {
                                requirementResult.Standard_CategoryNavigation = tempCategory;
                            }
                            else
                            {
                                categoryDictionary.Add(requirementResult.Standard_CategoryNavigation.Standard_Category1, requirementResult.Standard_CategoryNavigation);
                            }
                        }

                        foreach (var question in requirementResult.NEW_QUESTIONs().ToList())
                        {
                            DataLayer.Model.NEW_QUESTION existingQuestion;
                            if (questionDictionary.TryGetValue(question.Simple_Question, out existingQuestion))
                            {
                                requirementResult.REQUIREMENT_QUESTIONS.Remove(new REQUIREMENT_QUESTIONS() { Question_Id = question.Question_Id, Requirement_Id = requirementResult.Requirement_Id });
                            }
                            else
                            {
                                questionDictionary.Add(question.Simple_Question, question);
                            }
                        }
                        requirementList.Add(requirementResult.Requirement_Title.Trim().ToLower() + "|||" + requirementResult.Requirement_Text.Trim().ToLower());
                        requirements.Add(requirementResult);
                    }
                    else
                    {
                        // requirementResult.ErrorMessages.ToList().ForEach(s => result.LogError(s));
                    }
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="externalRequirement"></param>
        /// <param name="setName"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public async Task<NEW_REQUIREMENT> SaveRequirement(ExternalRequirement externalRequirement, string setName,
            int sequence
            /* ILogger logger */
            )
        {
            // var result = new ConverterResult<NEW_REQUIREMENT>(logger);
            var result = new ConverterResult<NEW_REQUIREMENT>();
            var newRequirement = result.Result;

            //basic mappings
            newRequirement.Supplemental_Info = externalRequirement.supplemental;
            newRequirement.Requirement_Text = externalRequirement.text;
            newRequirement.Requirement_Title = externalRequirement.identifier;
            newRequirement.Original_Set_Name = setName;
            newRequirement.Weight = externalRequirement.weight;
            newRequirement.REQUIREMENT_LEVELS = new List<REQUIREMENT_LEVELS>();
            newRequirement.REQUIREMENT_REFERENCES = new List<REQUIREMENT_REFERENCES>();
            newRequirement.REQUIREMENT_SETS = new List<REQUIREMENT_SETS>()
            {
                new REQUIREMENT_SETS()
                {
                    Set_Name = setName,
                    Requirement_Sequence = sequence
                }
            };

            QUESTION_GROUP_HEADING questionGroupHeading = null;
            UNIVERSAL_SUB_CATEGORY_HEADINGS uschPairing = null;


            try
            {
                questionGroupHeading = await _context.QUESTION_GROUP_HEADING.FirstOrDefaultAsync(s => s.Question_Group_Heading1.Trim().ToLower() == externalRequirement.heading.Trim().ToLower());
                try
                {
                    var subCat = await _context.UNIVERSAL_SUB_CATEGORIES
                        .FirstOrDefaultAsync(s => s.Universal_Sub_Category.Trim().ToLower() == externalRequirement.subheading.Trim().ToLower());
                    var subcatId =  subCat?.Universal_Sub_Category_Id ?? 0;
                    if (subcatId == 0)
                    {
                        var subcat = new UNIVERSAL_SUB_CATEGORIES() { Universal_Sub_Category = externalRequirement.subheading };
                        await _context.UNIVERSAL_SUB_CATEGORIES.AddAsync(subcat);
                        await _context.SaveChangesAsync();
                        subcatId = subcat.Universal_Sub_Category_Id;
                    }

                    try
                    {
                        uschPairing = await _context.UNIVERSAL_SUB_CATEGORY_HEADINGS.FirstOrDefaultAsync(s => (s.Universal_Sub_Category_Id == subcatId) && (s.Question_Group_Heading_Id == questionGroupHeading.Question_Group_Heading_Id));
                        if (uschPairing == null)
                        {
                            uschPairing = new UNIVERSAL_SUB_CATEGORY_HEADINGS()
                            {
                                Set_Name = "Standards",
                                Universal_Sub_Category_Id = subcatId,
                                Question_Group_Heading_Id = questionGroupHeading.Question_Group_Heading_Id
                            };
                            await _context.UNIVERSAL_SUB_CATEGORY_HEADINGS.AddAsync(uschPairing);
                            await _context.SaveChangesAsync();
                        }
                    }
                    catch (Exception exc)
                    {
                        log4net.LogManager.GetLogger(this.GetType()).Error($"... {exc}");
                        var myExc = exc;
                    }
                }
                catch
                {

                }
            }
            catch
            {

            }


            if (questionGroupHeading == null)
            {
                result.LogError(String.Format("Heading invalid for requirement {0} {1}.  Please double check that the heading is spelled correctly.", externalRequirement.identifier, externalRequirement.text));
            }
            else
            {
                newRequirement.Question_Group_Heading_Id = questionGroupHeading.Question_Group_Heading_Id;
            }


            if (uschPairing == null)
            {
                result.LogError(String.Format("Subheading invalid for requirement {0} {1}.  Please double check that the heading is spelled correctly.", externalRequirement.identifier, externalRequirement.text));
            }

            externalRequirement.category = string.IsNullOrWhiteSpace(externalRequirement.category) ? externalRequirement.heading : externalRequirement.category;
            var category = _context.STANDARD_CATEGORY.FirstOrDefault(s => s.Standard_Category1 == externalRequirement.category);
            if (category == null)
            {
                newRequirement.Standard_CategoryNavigation = new STANDARD_CATEGORY() { Standard_Category1 = externalRequirement.category };
            }
            else
            {
                newRequirement.Standard_Category = category.Standard_Category1;
            }

            newRequirement.Standard_Sub_Category = externalRequirement.subheading;


            // SAL
            foreach (string sal in externalRequirement.securityAssuranceLevels)
            {
                var rl = new REQUIREMENT_LEVELS()
                {
                    Standard_Level = sal,
                    Level_Type = "NST"
                };

                if (newRequirement.REQUIREMENT_LEVELS.Count(x => x.Standard_Level == rl.Standard_Level) == 0)
                {
                    newRequirement.REQUIREMENT_LEVELS.Add(rl);
                }
            }


            var importer = new DocumentImporter();
            if (externalRequirement.references != null)
            {
                foreach (var reference in externalRequirement.references)
                {
                    var reqReference = new REQUIREMENT_REFERENCES();
                    try
                    {
                        reqReference.Destination_String = reference.destination;
                        reqReference.Page_Number = reference.pageNumber;
                        reqReference.Section_Ref = String.IsNullOrEmpty(reference.sectionReference) ? "" : reference.sectionReference;
                        reqReference.Gen_File_Id = importer.LookupGenFileId(reference.fileName);
                    }
                    catch
                    {
                        result.LogError(String.Format("Reference {0} could not be added for requirement {1} {2}.", externalRequirement.source?.fileName, externalRequirement.identifier, externalRequirement.text));
                    }
                    if (reqReference.Gen_File_Id == 0)
                    {
                        result.LogError(String.Format("Reference {0} has not been loaded into CSET.  Please add the file and try again.", externalRequirement.source?.fileName, externalRequirement.identifier, externalRequirement.text));
                    }
                    else
                    {
                        newRequirement.REQUIREMENT_REFERENCES.Add(reqReference);
                    }
                }
            }

            var reqSource = new REQUIREMENT_SOURCE_FILES();

            try
            {
                if (externalRequirement.source != null)
                {
                    reqSource.Gen_File_Id = importer.LookupGenFileId(externalRequirement.source.fileName);
                    reqSource.Page_Number = externalRequirement.source.pageNumber;
                    reqSource.Destination_String = externalRequirement.source.destination;
                    reqSource.Section_Ref = String.IsNullOrEmpty(externalRequirement.source.sectionReference) ? "" : externalRequirement.source.sectionReference;
                    if (reqSource.Gen_File_Id == 0)
                    {
                        result.LogError(String.Format("Source {0} has not been loaded into CSET.  Please add the file and try again.", externalRequirement.source?.fileName, externalRequirement.identifier, externalRequirement.text));
                    }
                    else
                    {
                        newRequirement.REQUIREMENT_SOURCE_FILES.Add(reqSource);
                    }
                }
            }
            catch
            {
                result.LogError(String.Format("Source {0} could not be added for requirement {1} {2}.", externalRequirement.source?.fileName, externalRequirement.identifier, externalRequirement.text));
            }

            try
            {
                await _context.NEW_REQUIREMENT.AddAsync(newRequirement);
                await _context.SaveChangesAsync();
            }
            catch (Exception exc)
            {
                log4net.LogManager.GetLogger(this.GetType()).Error($"... {exc}");
            }


            // Save any Questions associated with the Requirement
            SaveQuestions(setName, externalRequirement, newRequirement,
                questionGroupHeading, uschPairing);


            return newRequirement;
        }


        /// <summary>
        /// 
        /// </summary>
        public async Task SaveQuestions(
            string setName,
            ExternalRequirement externalRequirement,
            NEW_REQUIREMENT newRequirement,
            QUESTION_GROUP_HEADING questionGroupHeading,
            UNIVERSAL_SUB_CATEGORY_HEADINGS uschPairing)
        {
            if (externalRequirement.questions == null || externalRequirement.questions.Count() == 0)
            {
                return;

                // trying to manufacture a question where none was defined could get us into trouble
                // externalRequirement.Questions = new QuestionList() { externalRequirement.Text };
            }

            // get the max Std_Ref_Number for the std_ref
            // each std_ref + std_ref_num must be unique
            int stdRefNum = 1;
            var fellowQuestions = await _context.NEW_QUESTION.Where(x => x.Std_Ref == setName.Replace("_", "")).ToListAsync();
            if (fellowQuestions.Count > 0)
            {
                stdRefNum = fellowQuestions.Max(x => x.Std_Ref_Number) + 1;
            }

            foreach (var question in externalRequirement.questions)
            {
                NEW_QUESTION newQuestion = null;


                // Look for existing question by question text
                newQuestion = await _context.NEW_QUESTION.FirstOrDefaultAsync(s => s.Simple_Question.ToLower().Trim() == question.ToLower().Trim());

                if (newQuestion == null)
                {
                    newQuestion = new NEW_QUESTION();
                    try
                    {
                        newQuestion.Original_Set_Name = setName;
                        newQuestion.Simple_Question = question;
                        newQuestion.Weight = externalRequirement.weight;
                        newQuestion.Question_Group_Id = questionGroupHeading.Question_Group_Heading_Id;
                        newQuestion.Universal_Sal_Level = SalCompare.FindLowestSal(externalRequirement.securityAssuranceLevels);
                        newQuestion.Std_Ref = setName.Replace("_", "");
                        newQuestion.Std_Ref = newQuestion.Std_Ref.Substring(0, Math.Min(newQuestion.Std_Ref.Length, 50));
                        newQuestion.Std_Ref_Number = stdRefNum++;

                        newQuestion.Heading_Pair_Id = uschPairing.Heading_Pair_Id;

                        await _context.NEW_QUESTION.AddAsync(newQuestion);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception exc)
                    {
                        log4net.LogManager.GetLogger(this.GetType()).Error($"... {exc}");
                        // result.LogError(String.Format("Question {0} could not be added for requirement {1} {2}.", question, externalRequirement.Identifier, externalRequirement.Text));
                    }
                }


                var nqs = new NEW_QUESTION_SETS()
                {
                    Question_Id = newQuestion.Question_Id,
                    Set_Name = setName
                };

                if (await _context.NEW_QUESTION_SETS.CountAsync(x => x.Question_Id == nqs.Question_Id && x.Set_Name == nqs.Set_Name) == 0)
                {
                    await _context.NEW_QUESTION_SETS.AddAsync(nqs);
                    await _context.SaveChangesAsync();


                    // attach question SAL levels
                    var nqlList = new List<NEW_QUESTION_LEVELS>();
                    foreach (string sal in externalRequirement.securityAssuranceLevels)
                    {
                        var nql = new NEW_QUESTION_LEVELS()
                        {
                            Universal_Sal_Level = sal.ToString(),
                            New_Question_Set_Id = nqs.New_Question_Set_Id
                        };

                        if (!nqlList.Exists(x =>
                            x.Universal_Sal_Level == nql.Universal_Sal_Level
                            && x.New_Question_Set_Id == nqs.New_Question_Set_Id))
                        {
                            await _context.NEW_QUESTION_LEVELS.AddAsync(nql);
                            nqlList.Add(nql);
                        }
                    }
                }


                try
                {
                    var rqs = new REQUIREMENT_QUESTIONS_SETS()
                    {
                        Question_Id = newQuestion.Question_Id,
                        Set_Name = setName,
                        Requirement_Id = newRequirement.Requirement_Id
                    };

                    if (await _context.REQUIREMENT_QUESTIONS_SETS.CountAsync(x =>
                        x.Question_Id == rqs.Question_Id
                        && x.Set_Name == rqs.Set_Name) == 0)
                    {
                        await _context.REQUIREMENT_QUESTIONS_SETS.AddAsync(rqs);
                    }


                    var rq = new REQUIREMENT_QUESTIONS()
                    {
                        Question_Id = newQuestion.Question_Id,
                        Requirement_Id = newRequirement.Requirement_Id
                    };

                    if (await _context.REQUIREMENT_QUESTIONS_SETS.CountAsync(x => x.Question_Id == rq.Question_Id
                        && x.Requirement_Id == rq.Requirement_Id) == 0)
                    {
                        await _context.REQUIREMENT_QUESTIONS.AddAsync(rq);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (Exception exc)
                {
                    log4net.LogManager.GetLogger(this.GetType()).Error($"... {exc}");
                    throw new Exception("Error saving REQUIREMENT_QUESTIONS_SETS and REQUIREMENT_QUESTIONS");
                }
            }
        }
    }
}