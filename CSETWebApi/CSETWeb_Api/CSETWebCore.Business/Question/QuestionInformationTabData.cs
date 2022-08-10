﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CSETWebCore.Constants;
using CSETWebCore.DataLayer.Model;
using CSETWebCore.Model.Question;



namespace CSETWebCore.Business.Question
{
    public class QuestionInformationTabData
    {
        private readonly CSETWebCore.Interfaces.Common.IHtmlFromXamlConverter _converter;
        private readonly CSETContext _context;

        public String RequirementFrameworkTitle { get; set; }
        public String RelatedFrameworkCategory { get; set; }
        public bool ShowRequirementFrameworkTitle { get; set; }
        public bool ShowRelatedFrameworkCategory { get { return true; } set { } }
        public int Question_or_Requirement_Id { get; set; }


        public RequirementTabData RequirementsData { get; set; }

        /// <summary>
        /// Documents that appear in the "Source Documents" section of question details.
        /// These are the principal reference documents for the requirement.
        /// </summary>
        public List<CustomDocument> SourceDocumentsList { get; set; }

        /// <summary>
        /// Documents that appear in the "Help Documents" section of question details.
        /// These are additional documents that may be helpful.
        /// </summary>
        public List<CustomDocument> AdditionalDocumentsList { get; set; }

        public List<string> ReferenceTextList { get; set; }

        public string References { get; set; }

        public string ExaminationApproach { get; set; }

        /// <summary>
        /// Contains a list of multiServiceComponent types for the current multiServiceComponent question
        /// </summary>
        public List<ComponentOverrideLinkInfo> ComponentTypes { get; set; }


        /// <summary>
        /// Property to allow for setting visibility on the multiServiceComponent types in the components tab
        /// </summary>     
        public bool ComponentVisibility { get; set; }

        public bool QuestionsVisible { get; set; }

        public bool ShowSALLevel { get; set; }

        public bool ShowRequirementStandards { get; set; }

        public ObservableCollection<FrameworkQuestionItem> FrameworkQuestions { get; set; }

        public String LevelName { get; set; }

        public bool IsCustomQuestion { get; set; }

        public bool IsComponent { get; set; }

        public bool IsMaturity { get; set; }

        public List<String> SetsList { get; set; }

        public List<RelatedQuestion> QuestionsList { get; set; }

        public bool ShowNoQuestionInformation { get; set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="converter"></param>
        /// <param name="context"></param>
        public QuestionInformationTabData(CSETWebCore.Interfaces.Common.IHtmlFromXamlConverter converter, CSETContext context)
        {
            SourceDocumentsList = new List<CustomDocument>();
            this.ComponentVisibility = false;
            this.ComponentTypes = new List<ComponentOverrideLinkInfo>();
            this.FrameworkQuestions = new ObservableCollection<FrameworkQuestionItem>();
            _converter = converter;
            _context = context;
        }


        public void BuildQuestionTab(QuestionInfoData infoData, SETS set)
        {
            ShowRequirementFrameworkTitle = true;
            BuildFromNewQuestion(infoData, set);
        }


        internal void BuildRelatedQuestionTab(RelatedQuestionInfoData questionInfoData, SETS set)
        {
            BuildFromNewQuestion(questionInfoData, set);
            ShowRelatedFrameworkCategory = true;
            ShowRequirementFrameworkTitle = true;
            RelatedFrameworkCategory = questionInfoData.Category;
        }

        private NEW_QUESTION BuildFromNewQuestion(BaseQuestionInfoData infoData, SETS set)
        {
            NEW_QUESTION question = infoData.Question;
            NEW_REQUIREMENT requirement = null;
            RequirementTabData tabData = new RequirementTabData();
            Question_or_Requirement_Id = infoData.QuestionID;
            this.LevelName = (from a in _context.NEW_QUESTION_SETS.Where(t => t.Question_Id == infoData.QuestionID && t.Set_Name == infoData.Set.Set_Name)
                              join l in _context.NEW_QUESTION_LEVELS on a.New_Question_Set_Id equals l.New_Question_Set_Id
                              join u in _context.UNIVERSAL_SAL_LEVEL on l.Universal_Sal_Level equals u.Universal_Sal_Level1
                              orderby u.Sal_Level_Order
                              select u.Full_Name_Sal).FirstOrDefault();

            // Gets requirements for the current standard (Set)
            IEnumerable<NEW_REQUIREMENT> requires = null;
            if (set.Is_Custom)
            {
                //Legacy only
                var tempRequires = new List<NEW_REQUIREMENT>();
                foreach (var setName in set.CUSTOM_STANDARD_BASE_STANDARDBase_StandardNavigation.Select(s => s.Base_Standard).ToList())
                {
                    tempRequires = tempRequires.Concat(question.NEW_REQUIREMENTs().Where(t => t.REQUIREMENT_SETS.Select(s => s.Set_Name).Contains(setName)).ToList()).ToList();
                }
                requires = tempRequires;
            }
            if (requires == null || !requires.Any())
            {

                requires = from a in _context.NEW_REQUIREMENT
                           join b in _context.REQUIREMENT_QUESTIONS_SETS on a.Requirement_Id equals b.Requirement_Id
                           where b.Question_Id == infoData.QuestionID && b.Set_Name == set.Set_Name
                           select a;
            }

            requirement = requires.FirstOrDefault();
            if (requirement != null)
            {
                tabData.Set_Name = set.Set_Name;
                tabData.Text = FormatRequirementText(requirement.Requirement_Text);
                tabData.RequirementID = requirement.Requirement_Id;
                if (!IsComponent)
                    RequirementFrameworkTitle = requirement.Requirement_Title;


                RelatedFrameworkCategory = requirement.Standard_Sub_Category;

                if (RelatedFrameworkCategory == null)
                {
                    var query = from qgh in _context.QUESTION_GROUP_HEADING
                                join usch in _context.UNIVERSAL_SUB_CATEGORY_HEADINGS on qgh.Question_Group_Heading_Id equals usch.Question_Group_Heading_Id
                                join q in _context.NEW_QUESTION on usch.Heading_Pair_Id equals q.Heading_Pair_Id
                                where q.Question_Id == question.Question_Id
                                select qgh.Question_Group_Heading1;
                    RelatedFrameworkCategory = query.FirstOrDefault();
                }

                tabData.SupplementalInfo = requires.FirstOrDefault(s => !String.IsNullOrEmpty(s.Supplemental_Info))?.Supplemental_Info;
                tabData.SupplementalInfo = FormatSupplementalInfo(tabData.SupplementalInfo);


                var refBuilder = new Helpers.ReferencesBuilder(_context);
                refBuilder.BuildReferenceDocuments(requirement.Requirement_Id,
                    out List<CustomDocument> sourceDocList,
                    out List<CustomDocument> additionalDocList);
                SourceDocumentsList = sourceDocList;
                AdditionalDocumentsList = additionalDocList;

                ReferenceTextList = refBuilder.BuildReferenceTextForRequirement(requirement.Requirement_Id);
            }

            QuestionsVisible = false;
            ShowRequirementStandards = false;
            ShowSALLevel = true;
            RequirementsData = tabData;
            return question;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="requirementData"></param>
        /// <param name="levelManager"></param>
        /// <param name="controlContext"></param>
        public void BuildRequirementInfoTab(RequirementInfoData requirementData, CSETWebCore.Interfaces.Standards.IStandardSpecficLevelRepository levelManager)
        {
            ShowRequirementFrameworkTitle = true;

            var requirement = requirementData.Requirement;
            Question_or_Requirement_Id = requirement.Requirement_Id;

            this.SetsList = new List<string>(requirementData.Sets.Select(s => s.Value.Short_Name));

            // Get related questions
            var query = from rq in _context.REQUIREMENT_QUESTIONS
                        join q in _context.NEW_QUESTION on rq.Question_Id equals q.Question_Id
                        where rq.Requirement_Id == requirementData.RequirementID
                        select new RelatedQuestion
                        {
                            QuestionID = q.Question_Id,
                            QuestionText = q.Simple_Question
                        };

            this.QuestionsList = query.ToList();


            RequirementTabData tabData = new RequirementTabData();
            tabData.RequirementID = requirement.Requirement_Id;
            tabData.Text = FormatRequirementText(requirement.Requirement_Text);
            tabData.SupplementalInfo = FormatSupplementalInfo(requirement.Supplemental_Info);
            tabData.Set_Name = requirementData.SetName;

            RequirementsData = tabData;
            int requirement_id = requirement.Requirement_Id;
            // var questions = requirement.NEW_QUESTION;
            SETS set;
            if (!requirementData.Sets.TryGetValue(requirementData.SetName, out set))
            {
                set = _context.SETS.Where(x => x.Set_Name == requirementData.SetName).FirstOrDefault();
            }

            if (!IsComponent)
                RequirementFrameworkTitle = requirement.Requirement_Title;

            RelatedFrameworkCategory = requirement.Standard_Sub_Category;

            if (requirementData.SetName == StandardConstants.CNSSI_1253_DB || requirementData.SetName == StandardConstants.CNSSI_ICS_PIT_DB
                || requirementData.SetName == StandardConstants.CNSSI_ICS_V1_DB || requirementData.SetName == StandardConstants.CNSSI_1253_V2_DB)
            {

                var levels = requirement.REQUIREMENT_LEVELS.Select(x => new
                {
                    FullName = levelManager.GetFullName(levelManager.GetStandard(x.Standard_Level), levelManager.GetLevelOrder(x.Standard_Level)),
                    LevelOrder = levelManager.GetLevelOrder(x.Standard_Level),
                    LevelType = x.Level_Type
                }).ToList();

                Tuple<int, string> availableLevel = Tuple.Create(-1, "None");
                Tuple<int, string> confidenceLevel = Tuple.Create(-1, "None");
                Tuple<int, string> integretityLevel = Tuple.Create(-1, "None");

                foreach (var item in levels)
                {
                    if (item.LevelType == "A")
                    {
                        if (availableLevel.Item1 == -1)
                        {
                            availableLevel = Tuple.Create(item.LevelOrder, item.FullName);
                        }
                        else
                        {
                            if (availableLevel.Item1 > item.LevelOrder)
                            {
                                availableLevel = Tuple.Create(item.LevelOrder, item.FullName);
                            }
                        }

                    }
                    else if (item.LevelType == "C")
                    {
                        if (confidenceLevel.Item1 == -1)
                        {
                            confidenceLevel = Tuple.Create(item.LevelOrder, item.FullName);
                        }
                        else
                        {
                            if (confidenceLevel.Item1 > item.LevelOrder)
                            {
                                confidenceLevel = Tuple.Create(item.LevelOrder, item.FullName);
                            }
                        }
                    }
                    else if (item.LevelType == "I")
                    {
                        if (integretityLevel.Item1 == -1)
                        {
                            integretityLevel = Tuple.Create(item.LevelOrder, item.FullName);
                        }
                        else
                        {
                            if (integretityLevel.Item1 > item.LevelOrder)
                            {
                                integretityLevel = Tuple.Create(item.LevelOrder, item.FullName);
                            }
                        }
                    }

                }

                this.LevelName = "Availability: " + availableLevel.Item2 + "\n" +
                    "Confidentiality: " + confidenceLevel.Item2 + "\n" +
                   "Integrity:" + integretityLevel.Item2;
            }
            else
            {
                var parameters = requirementData.Requirement.REQUIREMENT_LEVELS.Select(s => new Tuple<string, int>(levelManager.GetStandard(s.Standard_Level), levelManager.GetLevelOrder(s.Standard_Level))).OrderBy(s => s.Item2).FirstOrDefault();
                if (parameters != null)
                    this.LevelName = levelManager.GetFullName(parameters.Item1, parameters.Item2);
            }


            QuestionsVisible = true;
            ShowRequirementStandards = true;
            ShowSALLevel = true;
            ExaminationApproach = requirement.ExaminationApproach;

            var refBuilder = new Helpers.ReferencesBuilder(_context);
            refBuilder.BuildReferenceDocuments(requirement.Requirement_Id,
                    out List<CustomDocument> sourceDocList,
                    out List<CustomDocument> additionalDocList);
            SourceDocumentsList = sourceDocList;
            AdditionalDocumentsList = additionalDocList;

            ReferenceTextList = refBuilder.BuildReferenceTextForRequirement(requirementData.RequirementID);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="frameworkData"></param>
        /// <param name="controlContext"></param>
        public void BuildFrameworkInfoTab(FrameworkInfoData frameworkData)
        {
            QuestionsList = new List<RelatedQuestion>();
            IsCustomQuestion = frameworkData.IsCustomQuestion;
            References = frameworkData.References;
            if (!IsComponent)
                RequirementFrameworkTitle = frameworkData.Title;
            RelatedFrameworkCategory = frameworkData.Category;
            ShowRequirementFrameworkTitle = true;


            if (String.IsNullOrWhiteSpace(References))
                References = "None";
            Question_or_Requirement_Id = frameworkData.RequirementID;

            RequirementTabData tabData = new RequirementTabData();
            if (frameworkData.IsCustomQuestion)
            {
                tabData.Text = frameworkData.Question;
                tabData.SupplementalInfo = FormatSupplementalInfo(frameworkData.SupplementalInfo);
                if (String.IsNullOrWhiteSpace(tabData.SupplementalInfo))
                    tabData.SupplementalInfo = "None";
                QuestionsVisible = false;
                ShowRequirementStandards = false;
                ShowSALLevel = false;
            }
            else
            {
                var requirement = _context.NEW_REQUIREMENT.Where(x => x.Requirement_Id == frameworkData.RequirementID).Select(t => new
                {
                    Question_or_Requirement_Id = t.Requirement_Id,
                    Text = FormatRequirementText(t.Requirement_Text),
                    SupplementalInfo = FormatSupplementalInfo(t.Supplemental_Info),
                    Questions = t.NEW_QUESTIONs().Select(s => new RelatedQuestion
                    {
                        QuestionID = s.Question_Id,
                        QuestionText = s.Simple_Question
                    }),
                    LevelName = t.REQUIREMENT_LEVELS.Select(s => s.Standard_LevelNavigation).OrderBy(s => s.Level_Order).Select(s => s.Full_Name).FirstOrDefault()
                }).FirstOrDefault();
                if (requirement != null)
                {
                    QuestionsList = requirement.Questions.ToList();
                    this.LevelName = requirement.LevelName;
                    tabData.RequirementID = requirement.Question_or_Requirement_Id;
                    tabData.Text = requirement.Text;
                    tabData.SupplementalInfo = FormatSupplementalInfo(requirement.SupplementalInfo);

                    QuestionsVisible = false;

                    ShowSALLevel = true;

                    var refBuilder = new Helpers.ReferencesBuilder(_context);
                    refBuilder.BuildReferenceDocuments(frameworkData.RequirementID, out List<CustomDocument> sourceDocList, out List<CustomDocument> additionalDocList);

                    SetFrameworkQuestions(frameworkData.RequirementID);
                }
            }
            RequirementsData = tabData;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="controlContext"></param>
        public void BuildComponentInfoTab(ComponentQuestionInfoData info)
        {
            try
            {
                IsComponent = true;
                ShowRequirementFrameworkTitle = false;
                this.RequirementFrameworkTitle = "Components";
                NEW_QUESTION question = BuildFromNewQuestion(info, info.Set);
                ComponentVisibility = true;
                // Build multiServiceComponent types list if any
                ComponentTypes.Clear();
                int salLevel = _context.UNIVERSAL_SAL_LEVEL.Where(x => x.Universal_Sal_Level1 == question.Universal_Sal_Level).First().Sal_Level_Order;

                List<ComponentOverrideLinkInfo> tmpList = new List<ComponentOverrideLinkInfo>();


                foreach (COMPONENT_QUESTIONS componentType in _context.COMPONENT_QUESTIONS.Where(x => x.Question_Id == info.QuestionID))
                {
                    bool enabled = info.HasComponentsForTypeAtSal(componentType.Component_Symbol_Id, salLevel);
                    COMPONENT_SYMBOLS componentTypeData = info.DictionaryComponentInfo[componentType.Component_Symbol_Id];
                    tmpList.Add(new ComponentOverrideLinkInfo()
                    {
                        Component_Symbol_Id = componentTypeData.Component_Symbol_Id,
                        Symbol_Name = componentTypeData.Symbol_Name,
                        Enabled = enabled
                    });
                }

                ComponentTypes = tmpList.OrderByDescending(x => x.Enabled).ThenBy(x => x.Symbol_Name).ToList();
                var reqid = _context.REQUIREMENT_QUESTIONS.Where(x => x.Question_Id == info.QuestionID).First().Requirement_Id;


                var refBuilder = new Helpers.ReferencesBuilder(_context);
                refBuilder.BuildReferenceDocuments(reqid, 
                    out List<CustomDocument> sourceDocList,
                    out List<CustomDocument> additionalDocList);
                SourceDocumentsList = sourceDocList;
                AdditionalDocumentsList = additionalDocList;


                var requirement = _context.NEW_REQUIREMENT.Where(x => x.Requirement_Id == reqid).Select(t => new
                {
                    Question_or_Requirement_Id = t.Requirement_Id,
                    Text = FormatRequirementText(t.Requirement_Text),
                    SupplementalInfo = FormatSupplementalInfo(t.Supplemental_Info)
                }).FirstOrDefault();
                if (requirement != null)
                {
                    RequirementsData = new RequirementTabData()
                    {
                        RequirementID = requirement.Question_or_Requirement_Id,
                        Text = requirement.Text,
                        SupplementalInfo = FormatSupplementalInfo(requirement.SupplementalInfo),
                    };
                    QuestionsVisible = false;
                    ShowSALLevel = true;
                }
            }
            catch (Exception exc)
            {
                log4net.LogManager.GetLogger(this.GetType()).Error($"... {exc}");
            }
        }


        /// <summary>s
        /// Builds info tab information for a maturity question.
        /// References are hooked up differently than to questions and requirements.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="controlContext"></param>
        public void BuildMaturityInfoTab(MaturityQuestionInfoData info)
        {
            try
            {
                ShowRequirementFrameworkTitle = false;
                RequirementFrameworkTitle = info.MaturityQuestion.Question_Title;
                ShowRequirementStandards = true;

                var l = _context.MATURITY_LEVELS.Where(x => x.Level == info.MaturityQuestion.Maturity_Level).FirstOrDefault();
                if (l != null)
                {
                    LevelName = l.Level_Name;
                }

                IsMaturity = true;


                RequirementTabData tabData = new RequirementTabData();
                tabData.SupplementalInfo = info.MaturityQuestion.Supplemental_Info;
                tabData.SupplementalInfo = FormatSupplementalInfo(tabData.SupplementalInfo);

                tabData.ExaminationApproach = info.MaturityQuestion.Examination_Approach;

                RequirementsData = tabData;


                var refBuilder = new Helpers.ReferencesBuilder(_context);
                refBuilder.BuildDocumentsForMaturityQuestion(info.QuestionID,
                    out List<CustomDocument> sourceDocList,
                    out List<CustomDocument> additionalDocList);
                SourceDocumentsList = sourceDocList;
                AdditionalDocumentsList = additionalDocList;


                ReferenceTextList = refBuilder.BuildReferenceTextForMaturityQuestion(info.QuestionID);
            }
            catch (Exception exc)
            {
                log4net.LogManager.GetLogger(this.GetType()).Error($"... {exc}");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="requirement_ID"></param>
        /// <param name="controlEntity"></param>
        internal void SetFrameworkQuestions(int requirement_ID)
        {
            this.FrameworkQuestions.Clear();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();


            var newQuestionItems = (from nr in _context.NEW_REQUIREMENT
                                    from newquestions in nr.NEW_QUESTIONs()
                                    join newquestionSets in _context.NEW_QUESTION_SETS on newquestions.Question_Id equals newquestionSets.Question_Id into questionSets
                                    join level in _context.UNIVERSAL_SAL_LEVEL on newquestions.Universal_Sal_Level equals level.Universal_Sal_Level1
                                    join subheading in _context.UNIVERSAL_SUB_CATEGORY_HEADINGS on newquestions.Heading_Pair_Id equals subheading.Heading_Pair_Id
                                    join questionGroupHeading in _context.QUESTION_GROUP_HEADING on subheading.Question_Group_Heading_Id equals questionGroupHeading.Question_Group_Heading_Id
                                    where nr.Requirement_Id == requirement_ID
                                    select new
                                    {
                                        NewRequirement = nr,
                                        Question = newquestions,
                                        Level = level,
                                        QuestionGroupHeading = questionGroupHeading,
                                        QuestionSets = questionSets
                                    });
            foreach (var item in newQuestionItems)
            {

                FrameworkQuestionItem questionItem = new FrameworkQuestionItem();
                questionItem.QuestionText = item.Question.Simple_Question;
                questionItem.RequirementID = item.Question.Question_Id;
                questionItem.SALLevel = item.Level;
                questionItem.QuestionGroupHeading = item.QuestionGroupHeading;

                SETS set = item.QuestionSets.OrderBy(x => x.Set_NameNavigation.Order_Framework_Standards).Select(x => x.Set_NameNavigation).FirstOrDefault();
                questionItem.Standard = set.Short_Name;
                questionItem.SetName = set;
                questionItem.Question = item.Question;
                questionItem.Reference = "Ref";
                this.FrameworkQuestions.Add(questionItem);
            }

            stopWatch.Stop();
            //CSETLogger.Info("Time to get framework questions Time: " + stopWatch.ElapsedMilliseconds);
        }


        /// <summary>
        /// Formats a SupplementalInfo for browser display as follows:
        ///  - if null, returns null.
        ///  - if it is a XAML FlowDocument, it is converted to HTML.
        ///  - any CRLF linefeed characters in the string are converted to <br /> tags.
        /// </summary>
        /// <param name="supp"></param>
        /// <returns></returns>
        private string FormatSupplementalInfo(string supp)
        {
            if (string.IsNullOrEmpty(supp))
            {
                return "(no supplemental guidance available)";
            }

            if (supp.StartsWith("<FlowDocument"))
            {
                string html = _converter.ConvertXamlToHtml(supp);
                return html.Replace("margin:0 0 0 0;", "").Replace("padding:0 0 0 0;", "");
            }

            // Convert any linefeed characters to HTML line break tags
            const string pattern = "</?\\w+((\\s+\\w+(\\s*=\\s*(?:\".*?\"|'.*?'|[^'\">\\s]+))?)+\\s*|\\s*)/?>";
            Regex reg = new Regex(pattern);
            var matches = reg.Matches(supp);
            if (matches.Count > 0)
            {
                return supp;
            }

            return supp.Replace("\r\n", "<br/>").Replace("\n", "<br/>").Replace("\r", "<br/>");
        }


        /// <summary>
        /// Converts text for browser HTML display.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string FormatRequirementText(string text)
        {
            return text.Replace("\r\n", "<br/>").Replace("\n", "<br/>").Replace("\r", "<br/>");
        }
    }
}