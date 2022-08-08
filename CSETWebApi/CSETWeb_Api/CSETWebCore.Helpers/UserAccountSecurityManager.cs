﻿//////////////////////////////// 
// 
//   Copyright 2022 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 
using CSETWebCore.DataLayer.Model;
using CSETWebCore.Model.Contact;
using CSETWebCore.Model.Password;
using CSETWebCore.Model.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using CSETWebCore.Interfaces.User;
using CSETWebCore.Interfaces.Notification;
using System.Threading.Tasks;

namespace CSETWebCore.Helpers
{
    public class UserAccountSecurityManager
    {
        private readonly CSETContext _context;
        private readonly IUserBusiness _userBusiness;
        private readonly INotificationBusiness _notificationBusiness;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public UserAccountSecurityManager(CSETContext context, IUserBusiness userBusiness, INotificationBusiness notificationBusiness)
        {
            _context = context;
            _userBusiness = userBusiness;
            _notificationBusiness = notificationBusiness;
        }


        /// <summary>
        /// Creates a new user and sends them an email containing a temporary password.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<bool> CreateUserSendEmail(CreateUser info)
        {
            try
            {
                // Create the USER and USER_DETAIL_INFORMATION records for this new user
                var ud = new UserDetail()
                {
                    UserId = info.UserId,
                    Email = info.PrimaryEmail,
                    FirstName = info.FirstName,
                    LastName = info.LastName
                };
                UserCreateResponse userCreateResponse = await _userBusiness.CreateUser(ud,_context);

                await _context.USER_SECURITY_QUESTIONS.AddAsync(new USER_SECURITY_QUESTIONS()
                {
                    UserId = userCreateResponse.UserId,
                    SecurityQuestion1 = info.SecurityQuestion1,
                    SecurityQuestion2 = info.SecurityQuestion2,
                    SecurityAnswer1 = info.SecurityAnswer1,
                    SecurityAnswer2 = info.SecurityAnswer2
                });

                await _context.SaveChangesAsync();

                // Send the new temp password to the user
                _notificationBusiness.SendPasswordEmail(userCreateResponse.PrimaryEmail, info.FirstName, info.LastName, userCreateResponse.TemporaryPassword, info.AppCode);

                return true;
            }
            catch (ApplicationException app)
            {
                throw new Exception("This account already exists. Please request a new password using the Forgot Password link if you have forgotten your password.", app);
            }
            catch (DbUpdateException due)
            {
                throw new Exception("This account already exists. Please request a new password using the Forgot Password link if you have forgotten your password.", due);
            }
            catch (Exception exc)
            {
                log4net.LogManager.GetLogger(this.GetType()).Error($"... {exc}");

                return false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="changePass"></param>
        /// <returns></returns>
        public async Task<bool> ChangePassword(ChangePassword changePass)
        {
            try
            {
                var user = await _context.USERS.Where(x => x.PrimaryEmail == changePass.PrimaryEmail).FirstOrDefaultAsync();
                var info = await _context.USER_DETAIL_INFORMATION.Where(x => x.PrimaryEmail == user.PrimaryEmail).FirstOrDefaultAsync();

                new PasswordHash().HashPassword(changePass.NewPassword, out string hash, out string salt);
                user.Password = hash;
                user.Salt = salt;
                user.PasswordResetRequired = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception exc)
            {
                log4net.LogManager.GetLogger(this.GetType()).Error($"... {exc}");

                return false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="subject"></param>
        /// <param name="appCode"></param>
        /// <returns></returns>
        public async Task<bool> ResetPassword(string email, string subject, string appCode)
        {
            /**
             * get the user and make sure they exist
             * set the reset password flag
             * generate the new password
             * send the email 
             * return 
             */
            try
            {
                var user = await _context.USERS.Where(x => x.PrimaryEmail == email).FirstOrDefaultAsync();

                user.PasswordResetRequired = true;
                var password = UniqueIdGenerator.Instance.GetBase32UniqueId(10);


#if DEBUG
                // set the password back to 'abc' for consistency/predictability when debugging
                if (user.PrimaryEmail == "a@b.com")
                {
                    password = "abc";
                }
#endif
                string hash;
                string salt;
                new PasswordHash().HashPassword(password, out hash, out salt);
                user.Password = hash;
                user.Salt = salt;

                _notificationBusiness.SendPasswordResetEmail(user.PrimaryEmail, user.FirstName, user.LastName, password, subject, appCode);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception exc)
            {
                log4net.LogManager.GetLogger(this.GetType()).Error($"... {exc}");

                return false;
            }
        }


        /// <summary>
        /// Returns a list of potential security questions.
        /// </summary>
        /// <returns></returns>
        public List<PotentialQuestions> GetSecurityQuestionList()
        {
            List<PotentialQuestions> questions =
                (from a in _context.SECURITY_QUESTION
                 select new PotentialQuestions()
                 {
                     SecurityQuestion = a.SecurityQuestion,
                     SecurityQuestionId = a.SecurityQuestionId
                 }).ToList<PotentialQuestions>();
            return questions;
        }
    }
}
