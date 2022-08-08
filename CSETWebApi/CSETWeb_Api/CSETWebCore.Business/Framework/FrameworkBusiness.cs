﻿using System.Linq;
using System.Threading.Tasks;
using CSETWebCore.DataLayer.Model;
using CSETWebCore.Interfaces.Framework;
using CSETWebCore.Interfaces.Helpers;
using CSETWebCore.Model.Framework;
using Microsoft.EntityFrameworkCore;

namespace CSETWebCore.Business.Framework
{
    public class FrameworkBusiness : IFrameworkBusiness
    {
        private CSETContext _context;
        private readonly IAssessmentUtil _assessmentUtil;

        public FrameworkBusiness(CSETContext context, IAssessmentUtil assessmentUtil)
        {
            _context = context;
            _assessmentUtil = assessmentUtil;
        }

        /// <summary>
        /// Returns all known Cybersecurity Framework questions and tiers along
        /// with any selected tiers.
        /// </summary>
        /// <param name="assessmentId"></param>
        /// <returns></returns>
        public FrameworkResponse GetFrameworks(int assessmentId)
        {
            FrameworkResponse resp = new FrameworkResponse();

          
            // get any existing answers for this assessment
            var answers = _context.FRAMEWORK_TIER_TYPE_ANSWER.Where(ans => ans.Assessment_Id == assessmentId);
            // TODO:  If we add a 'sequence' column to this table, we can sort the sections
            //        because they appear in a different order in CSET 8.1.  Does the order even matter?


            foreach (FRAMEWORK_TIER_TYPE ftt in _context.FRAMEWORK_TIER_TYPE.ToList())
            {
                FrameworkTierType f = new FrameworkTierType()
                {
                    TierType = ftt.TierType
                };

                // Create a group name for radio button behavior
                f.GroupName = f.TierType.Replace(" ", "-").ToLower();

                resp.TierTypes.Add(f);


                var tierDefs = from ftd in _context.FRAMEWORK_TIER_DEFINITIONS
                               join ft in _context.FRAMEWORK_TIERS on ftd.Tier equals ft.Tier
                               orderby ftd.TierType, ft.TierOrder
                               where ftd.TierType == f.TierType
                               select ftd;

                var listTierDefinitions = tierDefs.ToList();


                foreach (FRAMEWORK_TIER_DEFINITIONS tierDef in listTierDefinitions)
                {
                    bool selected = answers.Where(x => x.TierType == f.TierType && x.Tier == tierDef.Tier).Any();

                    FrameworkTier tier = new FrameworkTier()
                    {
                        TierName = tierDef.Tier,
                        Question = tierDef.TierQuestion,
                        Selected = selected
                    };

                    // mark the selected tier in this definition
                    if (selected)
                    {
                        f.SelectedTier = tier.TierName;
                    }

                    // create a unique control ID for use on the front end
                    tier.ControlId = (f.TierType.Replace(" ", "-") + "-" + tier.TierName.Replace(" ", "-")).ToLower();



                    f.Tiers.Add(tier);
                }

                // default Tier 1 if no tier is selected
                if (string.IsNullOrEmpty(f.SelectedTier))
                {
                    f.SelectedTier = "Tier 1";
                }
            }

            return resp;
        }


        /// <summary>
        /// Persists the selected tier value to the database.
        /// </summary>
        /// <param name="assessmentId"></param>
        /// <param name="selectedTier"></param>
        public async Task PersistSelectedTierAnswer(int assessmentId, TierSelection selectedTier)
        {
            // save to FRAMEWORK_TIER_TYPE_ANSWER table
            var answer = await _context.FRAMEWORK_TIER_TYPE_ANSWER.Where(x => x.Assessment_Id == assessmentId && x.TierType == selectedTier.TierType).FirstOrDefaultAsync();

            if (answer == null)
            {
                answer = new FRAMEWORK_TIER_TYPE_ANSWER();
            }

            answer.Assessment_Id = assessmentId;
            answer.TierType = selectedTier.TierType;
            answer.Tier = selectedTier.TierName;

            if (await _context.FRAMEWORK_TIER_TYPE_ANSWER.FindAsync(answer.Assessment_Id, answer.TierType) == null)
            {
                await _context.FRAMEWORK_TIER_TYPE_ANSWER.AddAsync(answer);
            }
            else
            {
                _context.FRAMEWORK_TIER_TYPE_ANSWER.Update(answer);
            }
            await _context.SaveChangesAsync();

            await _assessmentUtil.TouchAssessment(assessmentId);
        }
    }
}