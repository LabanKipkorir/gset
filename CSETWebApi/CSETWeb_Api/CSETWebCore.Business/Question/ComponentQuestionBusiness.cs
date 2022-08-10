using CSETWebCore.DataLayer.Model;
using CSETWebCore.Interfaces.Helpers;
using CSETWebCore.Interfaces.Question;
using CSETWebCore.Model.Question;
using Microsoft.EntityFrameworkCore;
using Nelibur.ObjectMapper;
using Snickler.EFCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSETWebCore.Business.Question
{
    public class ComponentQuestionBusiness
    {

        private CSETContext _context;
        private readonly IAssessmentUtil _assessmentUtil;
        private readonly ITokenManager _tokenManager;
        private readonly IQuestionRequirementManager _questionRequirement;

        /// <summary>
        /// 
        /// </summary>
        public List<SubCategoryAnswersPlus> SubCatAnswers;

        /// <summary>
        /// 
        /// </summary>
        protected string ApplicationMode = "";


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="assessmentID"></param>
        public ComponentQuestionBusiness(CSETContext context, IAssessmentUtil assessmentUtil, ITokenManager tokenManager, IQuestionRequirementManager questionRequirement) 
        {
            _context = context;
            _assessmentUtil = assessmentUtil;
            _tokenManager = tokenManager;
            _questionRequirement = questionRequirement;
        }


        /// <summary>
        /// Gathers applicable questions for the assessment's network components as defined the by Diagram.
        /// </summary>
        /// <param name="resp"></param>        
        public async Task<QuestionResponse> GetResponse()
        {
            int assessmentId = await _tokenManager.AssessmentForUser();

            var resp = new QuestionResponse();

            // Ideally, we would not call this proc each time we fetch the questions.
            // Is there a quick way to tell if all the diagram answers have already been filled?
            _context.FillNetworkDiagramQuestions(assessmentId);
            var spList = await  _context.usp_Answer_Components_Default(assessmentId);
            var list = spList.Cast<Answer_Components_Base>().ToList();

            AddResponse(resp, list, "Component Defaults");
            await BuildOverridesOnly(resp);


            return resp;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="context"></param>
        public async Task BuildOverridesOnly(QuestionResponse resp)
        {
            int assessmentId = await _tokenManager.AssessmentForUser();

            // Because these are only override questions and the lists are short, don't bother grouping by group header.  Just subcategory.
            List<Answer_Components_Base> dlist = null;
            await _context.LoadStoredProc("[usp_getAnswerComponentOverrides]")
              .WithSqlParam("assessment_id", assessmentId)
              .ExecuteStoredProcAsync((handler) =>
              {
                  dlist = handler.ReadToList<Answer_Components_Base>()
                    .OrderBy(x => x.Symbol_Name).ThenBy(x => x.ComponentName).ThenBy(x => x.Component_Guid)
                    .ThenBy(x => x.Universal_Sub_Category)
                    .ToList();
              });

            AddResponseComponentOverride(resp, dlist, "Component Overrides");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="context"></param>
        /// <param name="list"></param>
        /// <param name="listname"></param>
        private void AddResponseComponentOverride(QuestionResponse resp, List<Answer_Components_Base> list, string listname)
        {
            List<QuestionGroup> groupList = new List<QuestionGroup>();
            QuestionGroup qg = new QuestionGroup();
            QuestionSubCategory sc = new QuestionSubCategory();
            QuestionAnswer qa = new QuestionAnswer();

            string symbolType = null;
            string componentName = null;
            string curGroupHeading = null;
            string curSubHeading = null;
            int prevQuestionId = 0;
            QuestionSubCategoryComparator comparator = new QuestionSubCategoryComparator();

            int displayNumber = 0;

            //push a new group if component_type, component_name, or question_group_heading changes

            foreach (var dbQ in list)
            {
                if ((dbQ.Symbol_Name != symbolType)
                    || (dbQ.ComponentName != componentName))
                {
                    qg = new QuestionGroup()
                    {
                        GroupHeadingText = dbQ.Question_Group_Heading,
                        GroupHeadingId = dbQ.GroupHeadingId,
                        StandardShortName = listname,
                        Symbol_Name = dbQ.Symbol_Name,
                        ComponentName = dbQ.ComponentName,
                        IsOverride = true

                    };
                    groupList.Add(qg);
                    symbolType = dbQ.Symbol_Name;
                    componentName = dbQ.ComponentName;

                    curGroupHeading = qg.GroupHeadingText;
                    // start numbering again in new group
                    displayNumber = 0;
                }

                // new subcategory -- break on pairing ID to separate 'base' and 'custom' pairings
                if ((dbQ.Universal_Sub_Category != curSubHeading) || (dbQ.Question_Id == prevQuestionId))
                {
                    sc = new QuestionSubCategory()
                    {
                        GroupHeadingId = dbQ.GroupHeadingId,
                        SubCategoryId = dbQ.SubCategoryId,
                        SubCategoryHeadingText = dbQ.Universal_Sub_Category,
                        HeaderQuestionText = dbQ.Sub_Heading_Question_Description,
                        SubCategoryAnswer = this.SubCatAnswers?.Where(x => x.HeadingId == dbQ.heading_pair_id).FirstOrDefault()?.AnswerText
                    };

                    qg.SubCategories.Add(sc);
                    curSubHeading = dbQ.Universal_Sub_Category;
                }
                prevQuestionId = dbQ.Question_Id;
                qa = new QuestionAnswer()
                {
                    DisplayNumber = (++displayNumber).ToString(),
                    QuestionId = dbQ.Question_Id,
                    QuestionType = "Component",
                    QuestionText = _questionRequirement.FormatLineBreaks(dbQ.Simple_Question),
                    Answer = dbQ.Answer_Text,
                    Answer_Id = dbQ.Answer_Id,
                    AltAnswerText = dbQ.Alternate_Justification,
                    freeResponseAnswer=dbQ.Free_Response_Answer,
                    Comment = dbQ.Comment,
                    MarkForReview = dbQ.Mark_For_Review ?? false,
                    Reviewed = dbQ.Reviewed ?? false,
                    Feedback = dbQ.Feedback,
                    ComponentGuid = dbQ.Component_Guid ?? Guid.Empty
                };

                sc.Questions.Add(qa);
            }


            resp.Categories.AddRange(groupList);
            resp.QuestionCount += list.Count;
            resp.DefaultComponentsCount = list.Count;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="resp"></param>
        /// <param name="context"></param>
        /// <param name="list"></param>
        /// <param name="listname"></param>
        private void AddResponse(QuestionResponse resp, List<Answer_Components_Base> list, string listname)
        {
            List<QuestionGroup> groupList = new List<QuestionGroup>();
            QuestionGroup qg = new QuestionGroup();
            QuestionSubCategory sc = new QuestionSubCategory();
            QuestionAnswer qa = new QuestionAnswer();

            string curGroupHeading = null;
            int curHeadingPairId = 0;


            int displayNumber = 0;


            foreach (var dbQ in list)
            {
                if (dbQ.Question_Group_Heading != curGroupHeading)
                {
                    qg = new QuestionGroup()
                    {
                        GroupHeadingText = dbQ.Question_Group_Heading,
                        GroupHeadingId = dbQ.GroupHeadingId,
                        StandardShortName = listname,
                        Symbol_Name = dbQ.Symbol_Name,
                        ComponentName = dbQ.ComponentName
                    };
                    groupList.Add(qg);
                    curGroupHeading = qg.GroupHeadingText;
                    // start numbering again in new group
                    displayNumber = 0;
                }

                // new subcategory -- break on pairing ID to separate 'base' and 'custom' pairings
                if (dbQ.heading_pair_id != curHeadingPairId)
                {
                    sc = new QuestionSubCategory()
                    {
                        GroupHeadingId = dbQ.GroupHeadingId,
                        SubCategoryId = dbQ.SubCategoryId,
                        SubCategoryHeadingText = dbQ.Universal_Sub_Category,
                        HeaderQuestionText = dbQ.Sub_Heading_Question_Description??string.Empty,
                        SubCategoryAnswer = this.SubCatAnswers?.Where(x => x.HeadingId == dbQ.heading_pair_id).FirstOrDefault()?.AnswerText
                    };

                    qg.SubCategories.Add(sc);

                    curHeadingPairId = dbQ.heading_pair_id;
                }

                qa = new QuestionAnswer()
                {
                    DisplayNumber = (++displayNumber).ToString(),
                    QuestionId = dbQ.Question_Id,
                    QuestionType = "Component",
                    QuestionText = _questionRequirement.FormatLineBreaks(dbQ.Simple_Question),
                    Answer = dbQ.Answer_Text,
                    Answer_Id = dbQ.Answer_Id,
                    AltAnswerText = dbQ.Alternate_Justification,
                    freeResponseAnswer = dbQ.Free_Response_Answer,
                    Comment = dbQ.Comment,
                    MarkForReview = dbQ.Mark_For_Review ?? false,
                    Reviewed = dbQ.Reviewed ?? false,
                    ComponentGuid = dbQ.Component_Guid ?? Guid.Empty,
                    Feedback = dbQ.Feedback
                };

                sc.Questions.Add(qa);
            }

            resp.Categories.AddRange(groupList);
            resp.QuestionCount += list.Count;
            resp.DefaultComponentsCount = list.Count;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="assessmentId"></param>
        /// <param name="question_id"></param>
        /// <param name="Component_Symbol_Id"></param>
        /// <returns></returns>
        public List<Answer_Components_Exploded_ForJSON> GetOverrideQuestions(int assessmentId, int question_id, int Component_Symbol_Id)
        {
            List<Answer_Components_Exploded_ForJSON> rlist = new List<Answer_Components_Exploded_ForJSON>();

                List<usp_getExplodedComponent> questionlist = null;

                _context.LoadStoredProc("[usp_getExplodedComponent]")
                  .WithSqlParam("assessment_id", assessmentId)
                  .ExecuteStoredProc((handler) =>
                  {
                      questionlist = handler.ReadToList<usp_getExplodedComponent>().Where(c => c.Question_Id == question_id
                                    && c.Component_Symbol_Id == Component_Symbol_Id).ToList();
                  });

                IQueryable<Answer_Components> answeredQuestionList = _context.Answer_Components.Where(a =>
                    a.Assessment_Id == assessmentId && a.Question_Or_Requirement_Id == question_id);


                foreach (var question in questionlist.ToList())
                {
                    Answer_Components_Exploded_ForJSON tmp = null;
                    TinyMapper.Bind<usp_getExplodedComponent, Answer_Components_Exploded_ForJSON>();
                    tmp = TinyMapper.Map<Answer_Components_Exploded_ForJSON>(question);
                    tmp.Component_GUID = question.Component_GUID.ToString();
                    rlist.Add(tmp);
                }

                return rlist;            
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public QuestionResponse GetOverrideListOnly()
        {
            QuestionResponse resp = new QuestionResponse
            {
                ApplicationMode = this.ApplicationMode
            };

            resp.Categories = new List<QuestionGroup>();
            resp.QuestionCount = 0;
            resp.RequirementCount = 0;

            BuildOverridesOnly(resp);
            return resp;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="answer"></param>
        /// <returns></returns>
        public async Task<int> StoreAnswer(Answer answer)
        {
            int assessmentId = await _tokenManager.AssessmentForUser();

            // Find the Question or Requirement
            var question = await _context.NEW_QUESTION.Where(q => q.Question_Id == answer.QuestionId).FirstOrDefaultAsync();

            if (question == null)
            {
                throw new Exception("Unknown question or requirement ID: " + answer.QuestionId);
            }

            // in case a null is passed, store 'unanswered'
            if (string.IsNullOrEmpty(answer.AnswerText))
            {
                answer.AnswerText = "U";
            }

            int answerId;

            ANSWER dbAnswer = null;
            if (answer != null)
            {
                dbAnswer = await _context.ANSWER.Where(x => x.Assessment_Id == assessmentId
                            && x.Question_Or_Requirement_Id == answer.QuestionId
                            && x.Is_Requirement == false && x.Component_Guid == answer.ComponentGuid).FirstOrDefaultAsync();
            }


            if (dbAnswer == null)
            {
                dbAnswer = new ANSWER();                
                dbAnswer.Assessment_Id = assessmentId;
                await _context.ANSWER.AddAsync(dbAnswer);
                await _context.SaveChangesAsync();
                answerId = dbAnswer.Answer_Id;
            }
            else
            {
                answerId = dbAnswer.Answer_Id;
            }


            dbAnswer.Answer_Id = answerId;
            dbAnswer.Question_Or_Requirement_Id = answer.QuestionId;
            dbAnswer.Question_Number = int.Parse(answer.QuestionNumber);
            dbAnswer.Is_Requirement = false;
            dbAnswer.Answer_Text = answer.AnswerText;
            dbAnswer.Alternate_Justification = answer.AltAnswerText;
            dbAnswer.Free_Response_Answer = answer.FreeResponseAnswer;
            dbAnswer.Comment = answer.Comment;
            dbAnswer.FeedBack = answer.Feedback;
            dbAnswer.Mark_For_Review = answer.MarkForReview;
            dbAnswer.Reviewed = answer.Reviewed;
            dbAnswer.Component_Guid = answer.ComponentGuid;
            dbAnswer.Is_Component = true;

            _context.ANSWER.Update(dbAnswer);
            await _context.SaveChangesAsync();

            await _assessmentUtil.TouchAssessment(assessmentId);

            return dbAnswer.Answer_Id;
        }


        /// <summary>
        /// get the exploded view where assessment
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="shouldSave"></param>
        public async Task HandleGuid(Guid guid, bool shouldSave)
        {
            int assessmentId = await _tokenManager.AssessmentForUser();

            if (shouldSave)
            {
                var componentName = await _context.ASSESSMENT_DIAGRAM_COMPONENTS.Where(x => x.Component_Guid == guid).FirstOrDefaultAsync();
                if (componentName != null)
                {
                    var creates = from a in _context.COMPONENT_QUESTIONS
                                  where a.Component_Symbol_Id == componentName.Component_Symbol_Id
                                  select a;

                    var alreadyThere = await (from a in _context.ANSWER
                                        where a.Assessment_Id == assessmentId
                                        && a.Component_Guid == guid

                                        select a).ToDictionaryAsync(x => x.Question_Or_Requirement_Id, x => x);
                    var questions = await creates.ToListAsync();
                    foreach (var c in questions)
                    {
                        if (!alreadyThere.ContainsKey(c.Question_Id))
                        {
                            _context.ANSWER.Add(new ANSWER()
                            {
                                Answer_Text = Constants.Constants.UNANSWERED,
                                Assessment_Id = assessmentId,
                                Component_Guid = guid,
                                Question_Type = "Component",
                                Is_Component = true,
                                Is_Requirement = false,
                                Question_Or_Requirement_Id = c.Question_Id
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new ApplicationException("could not find component for guid:" + guid);
                }
            }
            else
            {
                foreach (var a in await _context.ANSWER.Where(x => x.Component_Guid == guid).ToListAsync())
                {
                    _context.ANSWER.Remove(a);
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}
