
CREATE VIEW [dbo].[vFinancialGroups]
AS
SELECT        dbo.FINANCIAL_GROUPS.FinancialGroupId, dbo.FINANCIAL_DOMAINS.Domain, dbo.FINANCIAL_MATURITY.MaturityLevel, dbo.FINANCIAL_ASSESSMENT_FACTORS.AssessmentFactor, 
                         dbo.FINANCIAL_COMPONENTS.FinComponent
FROM            dbo.FINANCIAL_GROUPS INNER JOIN
                         dbo.FINANCIAL_DOMAINS ON dbo.FINANCIAL_GROUPS.DomainId = dbo.FINANCIAL_DOMAINS.DomainId INNER JOIN
                         dbo.FINANCIAL_MATURITY ON dbo.FINANCIAL_GROUPS.Financial_Level_Id = dbo.FINANCIAL_MATURITY.Financial_Level_Id INNER JOIN
                         dbo.FINANCIAL_ASSESSMENT_FACTORS ON dbo.FINANCIAL_GROUPS.AssessmentFactorId = dbo.FINANCIAL_ASSESSMENT_FACTORS.AssessmentFactorId INNER JOIN
                         dbo.FINANCIAL_COMPONENTS ON dbo.FINANCIAL_GROUPS.FinComponentId = dbo.FINANCIAL_COMPONENTS.FinComponentId
