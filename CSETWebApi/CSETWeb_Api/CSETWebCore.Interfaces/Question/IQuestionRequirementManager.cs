using System.Collections.Generic;
using System.Threading.Tasks;
using CSETWebCore.DataLayer.Model;
using CSETWebCore.Model.Question;

namespace CSETWebCore.Interfaces.Question
{
    public interface IQuestionRequirementManager
    {
        public List<SubCategoryAnswersPlus> SubCatAnswers { get; set; }
        public int AssessmentId { get; set; }
        public string StandardLevel { get; set; }
        public List<string> SetNames { get; set; }
        public string ApplicationMode { get; set; }
        Task InitializeManager(int assessmentId);
        void InitializeSubCategoryAnswers();
        Task InitializeApplicationMode();
        void InitializeSalLevel();
        void InitializeStandardsForAssessment();
        Task SetApplicationMode(string mode);
        Task<string> GetApplicationMode(int assessmentId);
        Task<int> StoreComponentAnswer(Answer answer);
        Task<int> StoreAnswer(Answer answer);
        Task BuildComponentsResponse(QuestionResponse resp);
        void BuildOverridesOnly(QuestionResponse resp);
        void AddResponseComponentOverride(QuestionResponse resp, List<Answer_Components_Base> list, string listname);

        void AddResponse(QuestionResponse resp, List<Answer_Components_Base> list, string listname);

        string DetermineQuestionType(bool is_requirement, bool is_component, bool is_framework, bool is_maturity);
        string FormatLineBreaks(string s);
        Task<int> NumberOfRequirements();
        Task<int> NumberOfQuestions();
    }
}