﻿using System.Collections.Generic;
using CSETWebCore.Model.Question;


namespace CSETWebCore.Business.Question
{
    public interface IQuestionBusiness
    {
        void SetQuestionAssessmentId(int assessmentId);
        QuestionResponse GetQuestionListWithSet(string questionGroupName);
        QuestionResponse GetQuestionList(string questionGroupName);
        List<AnalyticsQuestionAnswer> GetAnalyticQuestionAnswers(QuestionResponse questionResponse);
        List<int> GetActiveAnswerIds();
        QuestionDetails GetDetails(int questionId, string questionType);
        QuestionResponse BuildResponse();
        Task StoreSubcategoryAnswers(SubCategoryAnswers subCatAnswerBlock);
    }
}