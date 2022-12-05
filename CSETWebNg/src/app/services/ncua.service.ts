////////////////////////////////
//
//   Copyright 2022 Battelle Energy Alliance, LLC
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
//
////////////////////////////////
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { ConfigService } from './config.service';
import { MatDialog } from '@angular/material/dialog';
import { CharterMismatchComponent } from '../dialogs/charter-mistmatch/charter-mismatch.component';
import { AcetFilteringService } from './filtering/maturity-filtering/acet-filtering.service';
import { AssessmentService } from './assessment.service';
import { MaturityService } from './maturity.service';
import { Question } from '../models/questions.model';
import { ACETService } from './acet.service';
import { IRPService } from './irp.service';
import { QuestionBlockComponent } from '../assessment/questions/question-block/question-block.component';

let headers = {
    headers: new HttpHeaders()
        .set('Content-Type', 'application/json'),
    params: new HttpParams()
};

/**
 * A service that checks for the NCUA examiner's installation switch,
 * and manages various ISE examination variables.
 */
 @Injectable({
    providedIn: 'root'
  })

 export class NCUAService {

  // used to determine whether this is an NCUA installation or not
  apiUrl: string;
  switchStatus: boolean;

  // used for keeping track of which examinations are being merged
  prepForMerge: boolean = false;
  assessmentsToMerge: any[] = [];
  mainAssessCharter: string = "";
  backupCharter: string = "";
  hasShownCharterWarning: boolean = false;

  // Used to determine which kind of ISE exam is needed (SCUEP or CORE/CORE+)
  assetsAsString: string = "0";
  assetsAsNumber: number = 0;
  usingExamLevelOverride: boolean = false;
  proposedExamLevel: string = "SCUEP";
  chosenOverrideLevel: string = "No Override";

  // CORE+ question trigger/state manager
  showCorePlus: boolean = false;

  // CORE+ Only
  showExtraQuestions: boolean = false;

  // Variables to manage ISE issues state
  issuesFinishedLoading: boolean = false;
  questionCheck = new Map();
  issueFindingId = new Map();
  deleteHistory = new Set();

  // Keeps track of Issues with unassigned Types for report notification
  unassignedIssueTitles: any = [];
  unassignedIssues: boolean = false;

  questions: any = null;
  iseIrps: any = null;
  information: any =null;
  jsonString: any = {
    "metaData": [],
    "issuesTotal": {
      "dors": 0,
      "examinerFindings": 0,
      "supplementalFacts": 0,
      "nonReportables": 0
    },
    "examProfileData": [],
    "questionData": []
  };
  examLevel: string = '';
  submitInProgress: boolean = false;

  constructor(
    private http: HttpClient,
    private configSvc: ConfigService,
    public dialog: MatDialog,
    public acetFilteringSvc: AcetFilteringService,
    private maturitySvc: MaturityService,
    private acetSvc: ACETService,
    private irpSvc: IRPService,
    private assessmentSvc: AssessmentService
  ) {
    this.init();
  }

  async init() {
    this.getSwitchStatus();
  }

  /*
  * The master switch for all extra ISE functionality
  */
  getSwitchStatus() {
    this.http.get(this.configSvc.apiUrl + 'isExaminersModule', headers).subscribe((
      response: boolean) => {
        if (this.configSvc.installationMode === 'ACET') {
          this.switchStatus = response;
        }
      }
    );
  }


  /*
  * The following functions are all used for the 'Assessment merge' functionality
  */

  // Opens merge toggle checkboxes on the assessment selection (landing) page
  prepExaminationMerge() {
    if (this.prepForMerge === false) {
      this.prepForMerge = true;
    } else if (this.prepForMerge === true) {
      this.assessmentsToMerge = [];
      this.mainAssessCharter = "";
      this.backupCharter = "";
      this.hasShownCharterWarning = false;
      this.prepForMerge = false;
    }
  }

  // Adds or removes selected ISE examinations to the list to merge
  modifyMergeList(assessment: any, event: any) {
    let tempCharter = "";
    const optionChecked = event.target.checked;

    if (optionChecked) {
      tempCharter = this.pullAssessmentCharter(assessment);

      // Sets a fallback charter number if the user deselects the first exam that they selected
      if (this.assessmentsToMerge.length === 1) {
        this.backupCharter = tempCharter;
      }

      if (this.mainAssessCharter === "") {
        this.mainAssessCharter = tempCharter;
      }

      if (this.mainAssessCharter !== tempCharter && this.hasShownCharterWarning === false) {
        this.openCharterWarning();
      }

      this.assessmentsToMerge.push(assessment.assessmentId);
    } else {
      const index = this.assessmentsToMerge.indexOf(assessment.assessmentId);
      this.assessmentsToMerge.splice(index, 1);

      if (index === 0) {
        this.mainAssessCharter = this.backupCharter;
      }

      if (this.assessmentsToMerge.length === 0) {
        this.mainAssessCharter = "";
        this.backupCharter = "";
      }
    }
  }

  pullAssessmentCharter(assessment: any) {
    // All ISE charters start on the 4th character (after the 'ISE' and space) and are 5 digits long.
    let charterNum = "";
    for (let i = 4; i < 9; i++) {
      charterNum += assessment.assessmentName[i];
    }

    return charterNum;
  }

  openCharterWarning() {
    let dialogRef = this.dialog.open(CharterMismatchComponent, {
      width: '250px',
    });

    dialogRef.afterClosed().subscribe(result => {
      this.hasShownCharterWarning = true;
    });
  }

  // Fires off 2 - 10 assessments to the API to run the stored proc to check for conflicting answers
  getAnswers() {
    let id1 = this.assessmentsToMerge[0];
    let id2 = this.assessmentsToMerge[1];
    let id3 = (this.assessmentsToMerge[2] !== undefined ? this.assessmentsToMerge[2] : 0);
    let id4 = (this.assessmentsToMerge[3] !== undefined ? this.assessmentsToMerge[3] : 0);
    let id5 = (this.assessmentsToMerge[4] !== undefined ? this.assessmentsToMerge[4] : 0);
    let id6 = (this.assessmentsToMerge[5] !== undefined ? this.assessmentsToMerge[5] : 0);
    let id7 = (this.assessmentsToMerge[6] !== undefined ? this.assessmentsToMerge[6] : 0);
    let id8 = (this.assessmentsToMerge[7] !== undefined ? this.assessmentsToMerge[7] : 0);
    let id9 = (this.assessmentsToMerge[8] !== undefined ? this.assessmentsToMerge[8] : 0);
    let id10 = (this.assessmentsToMerge[9] !== undefined ? this.assessmentsToMerge[9] : 0);


    headers.params = headers.params.set('id1', id1).set('id2', id2).set('id3', id3).set('id4', id4)
    .set('id5', id5).set('id6', id6).set('id7', id7).set('id8', id8).set('id9', id9).set('id10', id10);

    return this.http.get(this.configSvc.apiUrl + 'getMergeData', headers)
  }


  /*
  * The following functions are used to help manage some of the ISE Maturity model "state". I realize
  * this probably isn't ideal in an application as big as CSET, but this is the fastest way to iterate
  * over client requests until things solidify and they stop changing things back and forth.
  */

  // Clears necessary variables on assessment drop
  reset() {
    this.questionCheck.clear();
    this.issueFindingId.clear();
    this.deleteHistory.clear();
  }

  // Pull Credit Union filter data to be used in ISE assessment detail filter search
  getCreditUnionData() {
    headers.params = headers.params.set('model', 'ISE');
    return this.http.get(this.configSvc.apiUrl + 'getCreditUnionData', headers);
  }

  //Manage the ISE maturity levels.
  updateAssetSize(amount: string) {
    this.assetsAsString = amount;
    this.assetsAsNumber = parseInt(amount);
    this.getExamLevelFromAssets();
  }

  checkExamLevel() {
    if (this.usingExamLevelOverride === false) {
      this.getExamLevelFromAssets();
    } else if (this.usingExamLevelOverride === true) {
      return this.chosenOverrideLevel;
    }
  }

  getExamLevelFromAssets() {
    if (!this.isIse()) {
      return;
    }

    if (this.assetsAsNumber > 50000000) {
      this.proposedExamLevel = 'CORE';
      if (this.usingExamLevelOverride === false) {
        this.maturitySvc.saveLevel(2).subscribe();
      }
    } else {
      this.proposedExamLevel = 'SCUEP';
      if (this.usingExamLevelOverride === false) {
        this.maturitySvc.saveLevel(1).subscribe();
      }
    }
  }

  updateExamLevelOverride(level: number) {
    if (!this.isIse()) {
      return;
    }

    if (level === 0) {
      this.usingExamLevelOverride = false;
      this.chosenOverrideLevel = "No Override";
      this.getExamLevelFromAssets();
    } else if (level === 1) {
      this.usingExamLevelOverride = true;
      this.chosenOverrideLevel = "SCUEP";
      this.maturitySvc.saveLevel(1).subscribe();
    } else if (level === 2) {
      this.usingExamLevelOverride = true;
      this.chosenOverrideLevel = "CORE";
      this.maturitySvc.saveLevel(2).subscribe();
    }
    this.refreshGroupList(level);
  }

  refreshGroupList(level: number) {
    this.acetFilteringSvc.resetDomainFilters(level);
  }

  getExamLevel() {
    if (this.usingExamLevelOverride === false) {
      return (this.proposedExamLevel);
    } else if (this.usingExamLevelOverride === true) {
      return (this.chosenOverrideLevel);
    }
  }

  showCorePlusOnlySubCats(id: number) {
    if (id >= 2567 && this.getExamLevel() === 'CORE') {
      return true;
    }
  }

  setExtraQuestionStatus(option: boolean) {
    this.showExtraQuestions = option;
  }

  getExtraQuestionStatus() {
    return this.showExtraQuestions;
  }

  toggleExtraQuestionStatus() {
    this.showExtraQuestions = !this.showExtraQuestions;
  }

  // Used to check answer completion for ISE reports
  getIseAnsweredQuestions() {
    return this.http.get(this.apiUrl + 'reports/acet/getIseAnsweredQuestions', headers);
  }

  // translates the maturity_Level_Id into the maturity_Level
  translateExamLevel(matLevelId: number) {
    if(matLevelId === 17) {
      return 'SCUE';
    } else if (matLevelId === 18 || matLevelId === 19) {
      return 'CORE';
    }
  }

  // translates the maturity_Level_Id into the maturity_Level
  translateExamLevelToInt(matLevelString: string) {
    if(matLevelString === 'SCUE') { //SCUEP, but cut off because the substring(0, 4)
      return 17;
    } else if (matLevelString === 'CORE') {
      return 18;
    }
  }

  isParentQuestion(title: string) {
    if ( title.toString().includes('.') ) {
      return false;
    }
    return true;
  }

  isIse() {
    return this.assessmentSvc.assessment?.maturityModel?.modelName === 'ISE';
  }

  submitToMerit(findings: any) {
    this.submitInProgress = true;
    this.questionResponseBuilder(findings);
    this.iseIrpResponseBuilder();
  }

  answerTextToNumber(text: string) {
    switch(text){
      case 'Y':
        return 0;
      case 'N':
        return 1;
      case 'U':
        return 2;
    }
  }

  /**
   * trims the child number '.#' off the given 'title', leaving what the parent 'title' should be
   */ 
  getParentQuestionTitle(title: string) {
    if(!this.isParentQuestion(title)) {
      let endOfTitle = 6;
      // checks if the title is double digits ('Stmt 10' through 'Stmt 22')
      if(title.charAt(6) != '.'){
        endOfTitle = endOfTitle + 1;
      }
      return title.substring(0, endOfTitle);
    }
  }

  /**
   * trims the child number 'Stmt ' off the given 'title', leaving what the statement number the 'title' is from
   */ 
   getParentQuestionTitleNumber(title: string) {
    if(this.isParentQuestion(title)) {
      let spaceIndex = title.indexOf(' ') + 1;

      return title.substring(spaceIndex);
    }
  }

  questionResponseBuilder(findings) {
    this.acetSvc.getIseAllQuestions().subscribe(
      (r: any) => {
        this.questions = r;
        this.examLevel = this.getExamLevel();

        // goes through domains
        for (let i = 0; i < this.questions?.matAnsweredQuestions[0]?.assessmentFactors?.length; i++) { 
          let domain = this.questions?.matAnsweredQuestions[0]?.assessmentFactors[i];
          // goes through subcategories
          for (let j = 0; j < domain.components?.length; j++) {
            let subcat = domain?.components[j];
            let childResponses ={"title": subcat?.questions[0].title,  //uses parent's title
                                 "category": subcat?.title,
                                 "issues": 
                                    {
                                      "dors": 0,
                                      "examinerFindings": 0,
                                      "supplementalFacts": 0,
                                      "nonReportables": 0
                                    },
                                  "children":[]
                                };
            // goes through questions
            for (let k = 0; k < subcat?.questions?.length; k++) {
              let question = subcat?.questions[k];
              if (k != 0) { //don't want parent questions being included with children
                if (this.examLevel === 'SCUEP' && question.maturityLevel !== 'SCUEP') {
                  question.answerText = 'U';
                }
                
                else if (this.examLevel === 'CORE' || this.examLevel === 'CORE+') {
                  if (question.maturityLevel === 'CORE+' && question.answerText !== 'U') {
                    this.examLevel = 'CORE+';
                  }
                  else if (question.maturityLevel === 'SCUEP') {
                    question.answerText = 'U';
                  }
                }

                childResponses.children.push({"title":question.title, "response": this.answerTextToNumber(question.answerText)});
              } else { //if it's a parent question, deal with possible issues
                for (let m = 0; m < findings?.length; m++) {
                  if (findings[m]?.question?.mat_Question_Id == question.matQuestionId) {
                    console.log('in for '+ question.title + ' | ' + findings[m]?.finding?.type)
                    switch (findings[m]?.finding?.type) {
                      case "DOR":
                        childResponses.issues.dors++;
                        this.jsonString.issuesTotal.dors++;
                        break;
                      case "Examiner Finding":
                        childResponses.issues.examinerFindings++;
                        this.jsonString.issuesTotal.examinerFindings++;
                        break;
                      case "Supplemental Fact":
                        childResponses.issues.supplementalFacts++;
                        this.jsonString.issuesTotal.supplementalFacts++;
                        break;
                      case "Non-reportable":
                        childResponses.issues.nonReportables++;
                        this.jsonString.issuesTotal.nonReportables++;
                        break;
                      default:
                        break;
                    }
                  }
                }
              }
            }

            this.jsonString.questionData.push(childResponses);
          }
        }

        this.metaDataBuilder();
      }
    );
  }

  iseIrpResponseBuilder() {
    this.irpSvc.getIRPList().subscribe(
      (r: any) => {
        this.iseIrps = r.headerList[5]; //these are all the IRPs for ISE. If this changes in the future, this will need updated

        for (let i = 0; i < this.iseIrps?.irpList?.length; i++) {
          let currentIrp = this.iseIrps?.irpList[i];

          let irpResponse = {"examProfileNumber": currentIrp.item_Number, "response": currentIrp.response};
          this.jsonString.examProfileData.push(irpResponse);
        }

      }
    );
  }

  metaDataBuilder() { 
    let info = this.questions?.information;
    let metaDataInfo = {
      "assessmentName": info.assessment_Name,
      "examiner": info.assessor_Name.trim(),
      "creationDate": info.assessment_Date,
      "examLevel": this.examLevel,
      "guid": 'TBD'
    };

    this.jsonString.metaData.push(metaDataInfo);

    this.saveToJsonFile(JSON.stringify(this.jsonString, null, '\t'), info.assessment_Name + '.json');

    this.jsonString = { // reset the string
      "metaData": [],
      "issuesTotal": {
        "dors": 0,
        "examinerFindings": 0,
        "supplementalFacts": 0,
        "nonReportables": 0
      },
      "examProfileData": [],
      "questionData": []
    }; 
  }

  saveToJsonFile(text: string, filename: string){
    let a = document.createElement('a');
    a.setAttribute('href', 'data:text/plain;charset=utf-u,'+encodeURIComponent(text));
    a.setAttribute('download', filename);
    a.click();
    this.submitInProgress = false;
  }

}
