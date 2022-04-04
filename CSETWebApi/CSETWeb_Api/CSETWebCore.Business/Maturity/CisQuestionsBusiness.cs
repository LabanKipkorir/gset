﻿using CSETWebCore.DataLayer.Model;
using CSETWebCore.Interfaces.Helpers;
using CSETWebCore.Model.Cis;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSETWebCore.Business.Maturity
{
    /// <summary>
    /// A structured listing of groupings/questions/options
    /// for the CIS maturity model.
    /// </summary>
    public class CisQuestionsBusiness
    {
        private readonly CSETContext _context;
        private readonly IAssessmentUtil _assessmentUtil;
        private readonly int _assessmentId;


        // This class is dedicated to CIS, maturity model 8
        private readonly int _maturityModelId = 8;

        public CisQuestions QuestionsModel;

        private List<MATURITY_QUESTIONS> allQuestions;

        private List<ANSWER> allAnswers;

        private List<MATURITY_GROUPINGS> allGroupings;


        /// <summary>
        /// The consumer can optionally suppress 
        /// grouping descriptions, question text and supplemental info
        /// if they want a smaller response object.
        /// </summary>
        private bool _includeText = true;


        /// <summary>
        /// Returns a populated instance of the maturity grouping
        /// and question structure for a maturity model.
        /// </summary>
        /// <param name="assessmentId"></param>
        public CisQuestionsBusiness(CSETContext context, IAssessmentUtil assessmentUtil, int assessmentId = 0)
        {
            this._context = context;
            this._assessmentUtil = assessmentUtil;
            this._assessmentId = assessmentId;
        }


        /// <summary>
        /// Returns the grouping/question/option structure for a section.
        /// </summary>
        /// <param name="sectionId"></param>
        /// <returns></returns>
        public CisQuestions GetSection(int sectionId)
        {
            LoadStructure(sectionId);

            // include score
            this.QuestionsModel.GroupingScore = CalculateGroupingScore();

            return this.QuestionsModel;
        }


        /// <summary>
        /// Gathers questions and answers and builds them into a basic hierarchy.
        /// </summary>
        private void LoadStructure(int sectionId)
        {
            QuestionsModel = new CisQuestions
            {
                AssessmentId = this._assessmentId
            };

            allQuestions = _context.MATURITY_QUESTIONS
                .Include(x => x.Maturity_LevelNavigation)
                .Include(x => x.MATURITY_REFERENCE_TEXT)
                .Where(q =>
                _maturityModelId == q.Maturity_Model_Id).ToList();


            allAnswers = _context.ANSWER
                .Where(a => a.Question_Type == Constants.Constants.QuestionTypeMaturity && a.Assessment_Id == this._assessmentId)
                .ToList();


            // Get all subgroupings for this maturity model
            allGroupings = _context.MATURITY_GROUPINGS
                .Include(x => x.Type)
                .Where(x => x.Maturity_Model_Id == _maturityModelId).ToList();


            GetSubgroups(QuestionsModel, null, sectionId);
        }


        /// <summary>
        /// Recursive method for traversing the structure.
        /// If the filterId is specified, the subgroups are reduced to that one.
        /// </summary>
        private void GetSubgroups(object oParent, int? parentId, int? filterId = null)
        {
            var mySubgroups = allGroupings.Where(x => x.Parent_Id == parentId).OrderBy(x => x.Sequence).ToList();

            if (filterId != null)
            {
                mySubgroups = allGroupings.Where(x => x.Grouping_Id == filterId).ToList();
            }

            if (mySubgroups.Count == 0)
            {
                return;
            }

            foreach (var sg in mySubgroups)
            {
                var nodeName = System.Text.RegularExpressions
                    .Regex.Replace(sg.Type.Grouping_Type_Name, " ", "_");

                var grouping = new Grouping()
                {
                    GroupType = nodeName,
                    Abbreviation = sg.Abbreviation,
                    GroupingId = sg.Grouping_Id,
                    Title = sg.Title,
                    Description = sg.Description
                };

                if (_includeText)
                {
                    grouping.Description = sg.Description;
                }

                if (oParent is CisQuestions)
                {
                    ((CisQuestions)oParent).Groupings.Add(grouping);
                }

                if (oParent is Grouping)
                {
                    ((Grouping)oParent).Groupings.Add(grouping);
                }



                // are there any questions that belong to this grouping?
                var myQuestions = allQuestions.Where(x => x.Grouping_Id == sg.Grouping_Id
                    && x.Parent_Question_Id == null && x.Parent_Option_Id == null).ToList();

                foreach (var myQ in myQuestions.OrderBy(s => s.Sequence))
                {
                    var answer = allAnswers.FirstOrDefault(x => x.Question_Or_Requirement_Id == myQ.Mat_Question_Id);

                    var question = new Model.Cis.Question()
                    {
                        QuestionId = myQ.Mat_Question_Id,
                        Sequence = myQ.Sequence,
                        DisplayNumber = myQ.Question_Title,
                        ParentQuestionId = myQ.Parent_Question_Id,
                        QuestionType = myQ.Mat_Question_Type,
                        AnswerText = answer?.Answer_Text,
                        AnswerMemo = answer?.Free_Response_Answer,
                        Options = GetOptions(myQ.Mat_Question_Id),
                        Followups = GetFollowupQuestions(myQ.Mat_Question_Id)
                    };

                    if (_includeText)
                    {
                        question.QuestionText = myQ.Question_Text.Replace("\r\n", "<br/>").Replace("\n", "<br/>").Replace("\r", "<br/> ");
                        question.ReferenceText = myQ.MATURITY_REFERENCE_TEXT.FirstOrDefault()?.Reference_Text;
                    }

                    grouping.Questions.Add(question);
                }


                // Recurse down to build subgroupings
                GetSubgroups(grouping, sg.Grouping_Id);
            }
        }


        /// <summary>
        /// Get questions that are followups to QUESTIONS
        /// </summary>
        /// <param name="allQuestions"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private List<Model.Cis.Question> GetFollowupQuestions(int parentId)
        {
            var qList = new List<Model.Cis.Question>();

            var myQuestions = allQuestions.Where(x => x.Parent_Question_Id == parentId && x.Parent_Option_Id == null).ToList();

            foreach (var myQ in myQuestions.OrderBy(s => s.Sequence))
            {
                var answer = allAnswers.FirstOrDefault(x => x.Question_Or_Requirement_Id == myQ.Mat_Question_Id);

                var question = new Model.Cis.Question()
                {
                    QuestionId = myQ.Mat_Question_Id,
                    Sequence = myQ.Sequence,
                    DisplayNumber = myQ.Question_Title,
                    ParentQuestionId = myQ.Parent_Question_Id,
                    QuestionType = myQ.Mat_Question_Type,
                    AnswerText = answer?.Answer_Text,
                    AnswerMemo = answer?.Free_Response_Answer,
                    Options = GetOptions(myQ.Mat_Question_Id),
                    Followups = GetFollowupQuestions(myQ.Mat_Question_Id)
                };

                if (_includeText)
                {
                    question.QuestionText = myQ.Question_Text.Replace("\r\n", "<br/>").Replace("\n", "<br/>").Replace("\r", "<br/> ");
                    question.ReferenceText = myQ.MATURITY_REFERENCE_TEXT.FirstOrDefault()?.Reference_Text;
                }

                qList.Add(question);

                var followups = GetFollowupQuestions(myQ.Mat_Question_Id);
                question.Followups.AddRange(followups);
            }

            return qList;
        }


        /// <summary>
        /// Build options for a question.
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        private List<Option> GetOptions(int questionId)
        {
            var opts = _context.MATURITY_ANSWER_OPTIONS.Where(x => x.Mat_Question_Id == questionId)
                .Include(x => x.ANSWER)
                .OrderBy(x => x.Answer_Sequence)
                .ToList();

            var list = new List<Option>();

            foreach (var o in opts)
            {
                var option = new Option()
                {
                    OptionText = o.Option_Text,
                    OptionId = o.Mat_Option_Id,
                    OptionType = o.Mat_Option_Type,
                    Sequence = o.Answer_Sequence,
                    HasAnswerText = o.Has_Answer_Text,
                    Weight = o.Weight
                };

                var ans = o.ANSWER.FirstOrDefault();
                option.AnswerId = ans?.Answer_Id;
                option.Selected = ans?.Answer_Text == "S";
                option.AnswerText = ans?.Free_Response_Answer;


                // Include questions that are a followup to the OPTION
                var myQuestions = allQuestions.Where(x => x.Parent_Option_Id == o.Mat_Option_Id).ToList();

                foreach (var myQ in myQuestions.OrderBy(s => s.Sequence))
                {
                    var question = new Model.Cis.Question()
                    {
                        QuestionId = myQ.Mat_Question_Id,
                        Sequence = myQ.Sequence,
                        DisplayNumber = myQ.Question_Title,
                        ParentQuestionId = myQ.Parent_Question_Id,
                        ParentOptionId = myQ.Parent_Option_Id,
                        QuestionType = myQ.Mat_Question_Type,
                        Options = GetOptions(myQ.Mat_Question_Id),
                        Followups = GetFollowupQuestions(myQ.Mat_Question_Id)
                    };

                    if (_includeText)
                    {
                        question.QuestionText = myQ.Question_Text.Replace("\r\n", "<br/>").Replace("\n", "<br/>").Replace("\r", "<br/> ");
                        question.ReferenceText = myQ.MATURITY_REFERENCE_TEXT.FirstOrDefault()?.Reference_Text;
                    }

                    option.Followups.Add(question);
                }

                list.Add(option);
            }

            return list;
        }


        /// <summary>
        /// CIS answers are different than normal maturity answers
        /// because Options are involved.  
        /// </summary>
        public Score StoreAnswer(Model.Question.Answer answer)
        {
            var dbOption = _context.MATURITY_ANSWER_OPTIONS.FirstOrDefault(x => x.Mat_Option_Id == answer.OptionId);
            if (dbOption == null)
            {
                return null;
            }

            // is this a radio or checkbox option
            if (dbOption.Mat_Option_Type == "radio")
            {
                return StoreAnswerRadio(answer);
            }

            if (dbOption.Mat_Option_Type == "checkbox")
            {
                return StoreAnswerCheckbox(answer);
            }

            if (dbOption.Mat_Option_Type == "text-first")
            {
                return StoreAnswerCheckbox(answer);
            }

            return CalculateGroupingScore();
        }


        /// <summary>
        /// Stores a "Radio" option answer.  Because radio buttons are
        /// single select, only one ANSWER record is stored for the question with the
        /// selected option's ID.
        /// </summary>
        /// <param name="answer"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private Score StoreAnswerRadio(Model.Question.Answer answer)
        {
            // Find the Maturity Question
            var dbQuestion = _context.MATURITY_QUESTIONS.Where(q => q.Mat_Question_Id == answer.QuestionId).FirstOrDefault();

            if (dbQuestion == null)
            {
                throw new Exception("Unknown question ID: " + answer.QuestionId);
            }


            ANSWER dbAnswer = _context.ANSWER.Where(x => x.Assessment_Id == _assessmentId
                && x.Question_Or_Requirement_Id == answer.QuestionId
                && x.Question_Type == answer.QuestionType).FirstOrDefault();


            if (dbAnswer == null)
            {
                dbAnswer = new ANSWER();
            }


            dbAnswer.Assessment_Id = _assessmentId;
            dbAnswer.Question_Or_Requirement_Id = answer.QuestionId;
            dbAnswer.Question_Type = answer.QuestionType;
            dbAnswer.Question_Number = 0;
            dbAnswer.Mat_Option_Id = answer.OptionId;   // this is the selected option
            dbAnswer.Answer_Text = answer.AnswerText;
            dbAnswer.Alternate_Justification = answer.AltAnswerText;
            dbAnswer.Free_Response_Answer = answer.FreeResponseAnswer;
            dbAnswer.Comment = answer.Comment;
            dbAnswer.FeedBack = answer.Feedback;
            dbAnswer.Mark_For_Review = answer.MarkForReview;
            dbAnswer.Reviewed = answer.Reviewed;
            dbAnswer.Component_Guid = answer.ComponentGuid;

            _context.ANSWER.Update(dbAnswer);
            _context.SaveChanges();

            _assessmentUtil.TouchAssessment(_assessmentId);

            return CalculateGroupingScore();
        }


        /// <summary>
        /// Stores a "Checkbox" option answer.  Because multiple checkboxes
        /// can be selected, one ANSWER record is stored for each selected
        /// option.  When a checkbox is unselected, the existing ANSWER
        /// record is updated, not deleted.
        /// </summary>
        /// <param name="answer"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private Score StoreAnswerCheckbox(Model.Question.Answer answer)
        {
            // Find the Maturity Question
            var dbQuestion = _context.MATURITY_QUESTIONS.Where(q => q.Mat_Question_Id == answer.QuestionId).FirstOrDefault();

            if (dbQuestion == null)
            {
                throw new Exception("Unknown question ID: " + answer.QuestionId);
            }


            ANSWER dbAnswer = _context.ANSWER.Where(x => x.Assessment_Id == _assessmentId
                && x.Question_Or_Requirement_Id == answer.QuestionId
                && x.Mat_Option_Id == answer.OptionId
                && x.Question_Type == answer.QuestionType).FirstOrDefault();


            if (dbAnswer == null)
            {
                dbAnswer = new ANSWER();
            }


            dbAnswer.Assessment_Id = _assessmentId;
            dbAnswer.Question_Or_Requirement_Id = answer.QuestionId;
            dbAnswer.Question_Type = answer.QuestionType;
            dbAnswer.Question_Number = 0;
            dbAnswer.Mat_Option_Id = answer.OptionId;
            dbAnswer.Answer_Text = answer.AnswerText;  // either "S" or "" for a checkbox option answer
            dbAnswer.Alternate_Justification = answer.AltAnswerText;
            dbAnswer.Free_Response_Answer = answer.FreeResponseAnswer;
            dbAnswer.Comment = answer.Comment;
            dbAnswer.FeedBack = answer.Feedback;
            dbAnswer.Mark_For_Review = answer.MarkForReview;
            dbAnswer.Reviewed = answer.Reviewed;
            dbAnswer.Component_Guid = answer.ComponentGuid;

            _context.ANSWER.Update(dbAnswer);
            _context.SaveChanges();

            _assessmentUtil.TouchAssessment(_assessmentId);

            return CalculateGroupingScore();
        }


        /// <summary>
        /// Builds a list of all navigation nodes subordinate to the CIS parent node.
        /// </summary>
        /// <returns></returns>
        public List<NavNode> GetNavStructure()
        {
            var cisGroupings = _context.MATURITY_GROUPINGS.Where(x => x.Maturity_Model_Id == _maturityModelId).ToList();

            var list = new List<NavNode>();

            var topNode = new NavNode()
            {
                Id = null,
                Title = "CIS Questions",
                Level = 1
            };

            GetSubnodes(topNode, ref list, ref cisGroupings);

            return list;
        }


        /// <summary>
        /// 
        /// </summary>
        private int GetSubnodes(NavNode parent, ref List<NavNode> list, ref List<MATURITY_GROUPINGS> cisGroupings)
        {
            var kids = cisGroupings.Where(x => x.Parent_Id == parent.Id).ToList();
            foreach (var kid in kids)
            {
                var sub = new NavNode()
                {
                    Id = kid.Grouping_Id,
                    Title = kid.Title,
                    Level = parent.Level + 1
                };

                list.Add(sub);
                var childCount = GetSubnodes(sub, ref list, ref cisGroupings);

                if (childCount > 0)
                {
                    sub.HasChildren = true;
                }
            }

            return kids.Count;
        }


        /// <summary>
        /// Placeholder for the eventual true scoring calculator.
        /// </summary>
        /// <returns></returns>
        private Score CalculateGroupingScore()
        {
            var rand = new Random();

            var s = new Score
            {
                GroupingScore = rand.Next(101),
                Low = 12,
                Median = 30,
                High = 62
            };
            return s;
        }
    }
}
