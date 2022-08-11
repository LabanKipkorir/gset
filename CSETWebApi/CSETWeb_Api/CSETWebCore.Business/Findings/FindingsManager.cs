﻿using CSETWebCore.DataLayer.Model;
using CSETWebCore.Model.Findings;
using Microsoft.EntityFrameworkCore;
using Nelibur.ObjectMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSETWebCore.Business.Findings
{
    public class FindingsManager
    {
        private int _assessmentId;

        /** I need to get the list of contacts
        *  provide a list of all the available contacts 
        *  allow the user to select from the list
        *  restrict this to one finding only
        */

        private CSETContext _context;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="assessmentId"></param>
        public FindingsManager(CSETContext context, int assessmentId)
        {
            _assessmentId = assessmentId;
            _context = context;

            TinyMapper.Bind<FINDING, Finding>();
            TinyMapper.Bind<FINDING_CONTACT, FindingContact>();
            TinyMapper.Bind<IMPORTANCE, Importance>();
            TinyMapper.Bind<ASSESSMENT_CONTACTS, FindingContact>();
            TinyMapper.Bind<Finding, FindingContact>();
            TinyMapper.Bind<Finding, Finding>();
            TinyMapper.Bind<FindingContact, FindingContact>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="finding"></param>
        public async Task DeleteFinding(Finding finding)
        {
            FindingData fm = new FindingData(finding, _context);
            await fm.Delete();
            await fm.Save();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="finding"></param>
        public async Task UpdateFinding(Finding finding)
        {
            FindingData fm = new FindingData(finding, _context);
            await fm.Save();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="answerId"></param>
        /// <returns></returns>
        public List<Finding> AllFindings(int answerId)
        {
            List<Finding> findings = new List<Finding>();

            var xxx = _context.FINDING
                .Where(x => x.Answer_Id == answerId)
                .Include(i => i.Importance)
                .Include(k => k.FINDING_CONTACT)
                .ToList();

            foreach (FINDING f in xxx)
            {
                Finding webF = new Finding();
                webF.Finding_Contacts = new List<FindingContact>();
                webF.Summary = f.Summary;
                webF.Finding_Id = f.Finding_Id;
                webF.Answer_Id = answerId;
                TinyMapper.Map(f, webF);
                if (f.Importance == null)
                    webF.Importance = new Importance()
                    {
                        Importance_Id = 1,
                        Value = Constants.Constants.SAL_LOW
                    };
                else
                    webF.Importance = TinyMapper.Map<IMPORTANCE, Importance>(f.Importance);

                foreach (FINDING_CONTACT fc in f.FINDING_CONTACT)
                {
                    FindingContact webFc = TinyMapper.Map<FINDING_CONTACT, FindingContact>(fc);

                    webFc.Selected = (fc != null);
                    webF.Finding_Contacts.Add(webFc);
                }
                findings.Add(webF);
            }
            return findings;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="findingId"></param>
        /// <param name="answerId"></param>
        /// <returns></returns>
        public async Task<Finding> GetFinding(int findingId, int answerId = 0)
        {
            Finding webF;

            if (findingId != 0)
            {
                FINDING f = await _context.FINDING
                    .Where(x => x.Finding_Id == findingId)
                    .Include(fc => fc.FINDING_CONTACT)
                    .FirstOrDefaultAsync();

                var q = await _context.ANSWER.Where(x => x.Answer_Id == f.Answer_Id).FirstOrDefaultAsync();

                webF = TinyMapper.Map<Finding>(f);
                webF.Question_Id = q != null ? q.Question_Or_Requirement_Id : 0;
                webF.Finding_Contacts = new List<FindingContact>();
                var contactList = await _context.ASSESSMENT_CONTACTS.Where(x => x.Assessment_Id == _assessmentId).ToListAsync();
                foreach (var contact in contactList)
                {
                    FindingContact webContact = TinyMapper.Map<FindingContact>(contact);
                    webContact.Name = contact.PrimaryEmail + " -- " + contact.FirstName + " " + contact.LastName;
                    webContact.Selected = (f.FINDING_CONTACT.Where(x => x.Assessment_Contact_Id == contact.Assessment_Contact_Id).FirstOrDefault() != null);
                    webF.Finding_Contacts.Add(webContact);
                }
            }
            else
            {
                var q = await _context.ANSWER.Where(x => x.Answer_Id == answerId).FirstOrDefaultAsync();

                FINDING f = new FINDING()
                {
                    Answer_Id = answerId
                };


                await _context.FINDING.AddAsync(f);
                await _context.SaveChangesAsync();
                webF = TinyMapper.Map<Finding>(f);
                webF.Finding_Contacts = new List<FindingContact>();
                var contactList = await _context.ASSESSMENT_CONTACTS.Where(x => x.Assessment_Id == _assessmentId).ToListAsync();
                foreach (var contact in contactList)
                {
                    FindingContact webContact = TinyMapper.Map<FindingContact>(contact);
                    webContact.Finding_Id = f.Finding_Id;
                    webContact.Name = contact.PrimaryEmail + " -- " + contact.FirstName + " " + contact.LastName;
                    webContact.Selected = false;
                    webF.Finding_Contacts.Add(webContact);
                }
            }

            return webF;
        }
    }
}