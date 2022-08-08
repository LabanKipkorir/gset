﻿using CSETWebCore.Business.Sal;
using CSETWebCore.Business.Standards;
using CSETWebCore.DataLayer.Model;
using CSETWebCore.Helpers;
using CSETWebCore.Model.Sal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nelibur.ObjectMapper;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CSETWebCore.Interfaces.Helpers;
using CSETWebCore.Interfaces.Standards;

namespace CSETWebCore.Api.Controllers
{
    [ApiController]
    public class SalController : ControllerBase
    {
        private readonly CSETContext _context;
        private readonly ITokenManager _token;
        private readonly IAssessmentModeData _assessment;
        private readonly IAssessmentUtil _assessmentUtil;
        private readonly IStandardsBusiness _standard;
        private readonly IStandardSpecficLevelRepository _standardRepo;

        public SalController(CSETContext context, ITokenManager token, IAssessmentModeData assessment, 
            IStandardsBusiness standard, IAssessmentUtil assessmentUtil, IStandardSpecficLevelRepository standardRepo)
        {
            _context = context;
            _token = token;
            _assessment = assessment;
            _standard = standard;
            _assessmentUtil = assessmentUtil;
            _standardRepo = standardRepo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/SAL")]
        public async Task<IActionResult> GetSTANDARD_SELECTION()
        {
            try
            {
                int asssessmentId = await _token.AssessmentForUser();

                TinyMapper.Bind<STANDARD_SELECTION, Sals>();
                TinyMapper.Bind<Sals, STANDARD_SELECTION>();

                STANDARD_SELECTION sTANDARD_SELECTION = await _context.STANDARD_SELECTION.FindAsync(asssessmentId);
                Sals rsal;
                if (sTANDARD_SELECTION == null)
                {
                    rsal = new Sals()
                    {
                        Selected_Sal_Level = "Low",
                        Last_Sal_Determination_Type = "Simple",
                        CLevel = "Low",
                        ALevel = "Low",
                        ILevel = "Low"
                    };
                    sTANDARD_SELECTION = TinyMapper.Map<STANDARD_SELECTION>(rsal);
                    sTANDARD_SELECTION.Assessment_Id = asssessmentId;
                    sTANDARD_SELECTION.Application_Mode = _assessment.DetermineDefaultApplicationMode();
                    await _context.STANDARD_SELECTION.AddAsync(sTANDARD_SELECTION);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    rsal = TinyMapper.Map<Sals>(sTANDARD_SELECTION);

                }

                LevelManager lm = new LevelManager(asssessmentId, _context);
                lm.RetrieveOtherLevels(rsal);
                StandardRepository sr = new StandardRepository(_standard,_assessment,_context,_assessmentUtil, _standardRepo);
                sr.InitializeStandardRepository(asssessmentId);
                sr.Confidence_Level = rsal.CLevel;
                sr.Integrity_Level = rsal.ILevel;
                sr.Availability_Level = rsal.ALevel;

                return Ok(rsal);

            }
            catch (Exception exc)
            {
                log4net.LogManager.GetLogger(this.GetType()).Error($"... {exc}");

                return Conflict();
            }

        }

        [HttpGet]
        [Route("api/SAL/Type")]
        public async Task<IActionResult> GetType(String newType)
        {
            try
            {
                int asssessmentId = await _token.AssessmentForUser();
                STANDARD_SELECTION sTANDARD_SELECTION = await _context.STANDARD_SELECTION.FindAsync(asssessmentId);
                if (sTANDARD_SELECTION != null)
                {
                    sTANDARD_SELECTION.Last_Sal_Determination_Type = newType;
                    await _context.SaveChangesAsync();
                }
                return Ok();

            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }

        [HttpPost]
        [Route("api/SAL")]
        public async Task<IActionResult> PostSAL(Sals tmpsal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int assessmentId = await _token.AssessmentForUser();
            TinyMapper.Bind<Sals, STANDARD_SELECTION>();
            STANDARD_SELECTION sTANDARD_SELECTION = await _context.STANDARD_SELECTION.Where(x => x.Assessment_Id == assessmentId).FirstOrDefaultAsync();
            if (sTANDARD_SELECTION != null)
            {
                sTANDARD_SELECTION = TinyMapper.Map<Sals, STANDARD_SELECTION>(tmpsal, sTANDARD_SELECTION);
            }
            else
            {
                sTANDARD_SELECTION = TinyMapper.Map<STANDARD_SELECTION>(tmpsal);
            }
            sTANDARD_SELECTION.Assessment_Id = assessmentId;


            _context.Entry(sTANDARD_SELECTION).State = EntityState.Modified;
            LevelManager lm = new LevelManager(assessmentId, _context);
            lm.SaveOtherLevels(assessmentId, tmpsal);
            lm.Init(sTANDARD_SELECTION);
            if (tmpsal.SelectedSALOverride)
            {
                lm.SaveSALLevel(tmpsal.Selected_Sal_Level);
            }

            try
            {
                await _context.SaveChangesAsync();

                StandardRepository sr = new StandardRepository(_standard, _assessment, _context, _assessmentUtil, _standardRepo);
                sr.InitializeStandardRepository(assessmentId);
                sr.Confidence_Level = tmpsal.CLevel;
                sr.Integrity_Level = tmpsal.ILevel;
                sr.Availability_Level = tmpsal.ALevel;

                // save the newly-calculated overall value
                if (!tmpsal.SelectedSALOverride)
                {
                    tmpsal.Selected_Sal_Level = sr.Selected_Sal_Level;
                    lm.SaveSALLevel(tmpsal.Selected_Sal_Level);
                }

                return Ok(tmpsal);
            }
            catch (DbUpdateConcurrencyException dbe)
            {
                var standExist = await STANDARD_SELECTIONExists(assessmentId);
                if (!standExist)
                {
                    return NotFound();
                }
                else
                {
                    log4net.LogManager.GetLogger(this.GetType()).Error($"... {dbe}");

                    throw;
                }
            }
            catch (Exception e)
            {
                BadRequest(e);
            }

            return NoContent();
        }

        // POST: api/SAL
        [HttpPost]
        [Route("api/Sal_what_is_this")]
        public async Task<IActionResult> PostSTANDARD_SELECTION(STANDARD_SELECTION sTANDARD_SELECTION)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.STANDARD_SELECTION.AddAsync(sTANDARD_SELECTION);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException exc)
            {
                log4net.LogManager.GetLogger(this.GetType()).Error($"... {exc}");

                if (await STANDARD_SELECTIONExists(sTANDARD_SELECTION.Assessment_Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = sTANDARD_SELECTION.Assessment_Id }, sTANDARD_SELECTION);
        }

        [HttpGet]
        [Route("api/SAL/NistData")]
        public async Task<NistModel> GetNistData()
        {
            int assessmentId = await _token.AssessmentForUser();
            NistSalBusiness nistSal = new NistSalBusiness(_context, _assessmentUtil);
            NistModel rvalue = new NistModel()
            {
                models = nistSal.GetInformationTypes(assessmentId),
                questions = await nistSal.GetNistQuestions(assessmentId),
                specialFactors = nistSal.GetSpecialFactors(assessmentId)
            };

            return rvalue;
        }

        [HttpPost]
        [Route("api/SAL/NistData")]
        public async Task<Sals> PostNistData([FromBody] NistSalModel updateValue)
        {
            int assessmentId = await _token.AssessmentForUser();
            NistSalBusiness nistSal = new NistSalBusiness(_context, _assessmentUtil);
            return await nistSal.UpdateSalValue(updateValue, assessmentId);


        }

        [HttpPost]
        [Route("api/SAL/NistDataQuestions")]
        public async Task<Sals> PostNistDataQuestions([FromBody] NistQuestionsAnswers updateValue)
        {
            int assessmentId = await _token.AssessmentForUser();
            NistSalBusiness nistSal = new NistSalBusiness(_context, _assessmentUtil);
            return await nistSal.SaveNistQuestions(assessmentId, updateValue);
        }

        [HttpPost]
        [Route("api/SAL/NistDataSpecialFactor")]
        public async Task<Sals> PostNistDataSpecialFactor([FromBody] NistSpecialFactor updateValue)
        {
            int assessmentId = await _token.AssessmentForUser();
            NistSalBusiness nistSal = new NistSalBusiness(_context, _assessmentUtil);
            return await nistSal.SaveNistSpecialFactor(assessmentId, updateValue);
        }

        private async Task<bool> STANDARD_SELECTIONExists(int id)
        {
            return await _context.STANDARD_SELECTION.CountAsync(e => e.Assessment_Id == id) > 0;
        }
    }
}
