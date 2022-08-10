﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSETWebCore.Model.Assessment;
using CSETWebCore.Model.Question;

namespace CSETWebCore.Model.Cis
{

    public class ModelStructure
    {
        public string ModelName { get; set; }
        public string ModelTitle { get; set; }
        public int ModelId { get; set; }
        public List<Grouping> Groupings { get; set; } = new List<Grouping>();
    }

    public class CisQuestions
    {
        public int AssessmentId { get; set; }

        public List<Grouping> Groupings { get; set; } = new List<Grouping> { };

        public Score GroupingScore { get; set; }


        public int? BaselineAssessmentId { get; set; }
        public string BaselineAssessmentName { get; set; }
        public Score BaselineGroupingScore { get; set; }
    }

    public class Grouping
    {
        public string GroupType { get; set; }
        public string Description { get; set; }
        public string Abbreviation { get; set; }
        public int GroupingId { get; set; }
        public string Prefix { get; set; }
        public string Title { get; set; }

        /// <summary>
        /// The grouping's score based on answers/options
        /// </summary>
        public Score Score { get; set; }

        public Aggregation.HorizBarChart Chart { get; set; }

        public List<Grouping> Groupings { get; set; } = new List<Grouping>();
        public List<Question> Questions { get; set; } = new List<Question>();
    }

    public class Question
    {
        public int QuestionId { get; set; }
        public string QuestionType { get; set; }
        public int Sequence { get; set; }
        public string DisplayNumber { get; set; }
        public string QuestionText { get; set; }

        public string AnswerText { get; set; }
        public string AnswerMemo { get; set; }
        public string ReferenceText { get; set; }

        public List<CustomDocument> SourceDocuments { get; set; }
        public List<CustomDocument> AdditionalDocuments { get; set; }

        public int? ParentQuestionId { get; set; }
        public int? ParentOptionId { get; set; }
        public List<Option> Options { get; set; } = new List<Option>();
        public List<Question> Followups { get; set; } = new List<Question>();

        public string Comment { get; set; }
        public string Feedback { get; set; }
        public bool MarkForReview { get; set; }
        public List<int> DocumentIds { get; set; } = new List<int>();

        public string BaselineAnswerText { get; set; }
        public string BaselineAnswerMemo { get; set; }
    }

    public class Option
    {
        public int OptionId { get; set; }
        public string OptionType { get; set; }
        public string OptionText { get; set; }
        public int Sequence { get; set; }

        public decimal? Weight { get; set; }

        /// <summary>
        /// Identifies an option that should not be selected
        /// if any of its peers is selected, e.g., "none of the above"
        /// </summary>
        public bool IsNone { get; set; }

        public bool Selected { get; set; }

        public int? AnswerId { get; set; }

        /// <summary>
        /// Indicates if the option also renders an input field.
        /// </summary>
        public bool HasAnswerText { get; set; }

        /// <summary>
        /// The user's answer.
        /// </summary>
        public string AnswerText { get; set; }

        /// <summary>
        /// Baseline selections can be populated for comparison against
        /// the current assessment's option.
        /// </summary>
        public bool BaselineSelected { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BaselineAnswerText { get; set; }



        public List<Question> Followups { get; set; } = new List<Question>();
    }


    public class IntegrityCheckOption 
    {
        public int OptionId { get; set; }
        public bool Selected { get; set; }
        public List<InconsistentOption> InconsistentOptions { get; set; } = new List<InconsistentOption>();
    }


    public class InconsistentOption
    {
        public int OptionId { get; set; }
        public string ParentQuestionText { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class Score
    {
        public int GroupingId { get; set; }
        public int GroupingScore { get; set; }
        public int High { get; set; }
        public int Median { get; set; }
        public int Low { get; set; }
    }


    /// <summary>
    /// Represents a node in the navigation tree.  This is created
    /// for use with the CIS model because the tree is large
    /// and not worth hard coding.  This class could be promoted
    /// to some place more general for other usage if needed.
    /// </summary>
    public class NavNode
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }
        public bool HasChildren { get; set; } = false;
    }

    public class FlatQuestion
    {
        public string QuestionText { get; set; }
        public decimal? Weight { get; set; }
        public bool Selected { get; set; }
        public string Type { get; set; }
    }

    public class GroupedQuestions
    {
        public string QuestionText { get; set; }
        public List<FlatQuestion> OptionQuestions { get; set; }
    }

    public class RollupOptions
    {
        public string Type { get; set; }
        public decimal? Weight { get; set; }
    }

    public class CisAssessmentsResponse
    {
        public int? BaselineAssessmentId { get; set; }
        public List<AssessmentDetail> MyCisAssessments { get; set; } = new List<AssessmentDetail>();
    }

    public class CisImportRequest
    {
        public int Dest { get; set; }
        public int Source { get; set; }
    }
}
