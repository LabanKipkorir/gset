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
import { Component, OnInit, Inject } from '@angular/core';
import { FindingsService } from '../../../services/findings.service';
import { AssessmentService } from '../../../services/assessment.service';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Finding, Importance, FindingContact } from './findings.model';
import * as _ from 'lodash';
import { Router } from '@angular/router';

@Component({
  selector: 'app-findings',
  templateUrl: './findings.component.html',
  host: {
    'style': 'max-width: 100%'
  }
})
export class FindingsComponent implements OnInit {

  finding: Finding;
  importances: Importance[];
  contactsmodel: any[];
  answerID: number;
  questionID: number;

  constructor(
    private findSvc: FindingsService,
    private dialog: MatDialogRef<FindingsComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Finding,
    private router: Router,
    private assessSvc: AssessmentService
  ) {
    this.finding = data;
    this.answerID = data.answer_Id;
    this.questionID = data.question_Id;
  }

  ngOnInit() {
    this.dialog.backdropClick()
      .subscribe(() => {
        this.update();
      });

    // send the finding to the server
    // if it is empty or new let the server
    // worry about it
    this.findSvc.getImportance().subscribe((result: Importance[]) => {
      this.importances = result;
      let questionType = localStorage.getItem('questionSet');
      this.findSvc.getFinding(this.finding.answer_Id, this.finding.finding_Id, this.finding.question_Id, questionType)
        .subscribe((response: Finding) => {
          this.finding = response;
          this.answerID = this.finding.answer_Id;
          this.questionID = this.finding.question_Id;
          this.contactsmodel = _.map(_.filter(this.finding.finding_Contacts,
            { 'selected': true }),
            'Assessment_Contact_Id');
          this.data.answer_Id = this.answerID;
        });
    });
  }

  refreshContacts():void{
    let questionType = localStorage.getItem('questionSet');
    this.findSvc.getFinding(this.finding.answer_Id, this.finding.finding_Id, this.finding.question_Id, questionType)
        .subscribe((response: Finding) => {
          this.finding = response;
          this.contactsmodel = _.map(_.filter(this.finding.finding_Contacts,
            { 'selected': true }),
            'Assessment_Contact_Id');
        });
  }

  clearMulti() {
    this.finding.finding_Contacts.forEach(c => {
      c.selected = false;
    });
  }

  checkFinding(finding: Finding) {
    // and a bunch of fields together
    // if they are all null then false
    // else true;
    let findingCompleted = true;

    findingCompleted = (finding.impact == null);
    findingCompleted = (finding.importance == null) && (findingCompleted);
    findingCompleted = (finding.issue == null) && (findingCompleted);
    findingCompleted = (finding.recommendations == null) && (findingCompleted);
    findingCompleted = (finding.resolution_Date == null) && (findingCompleted);
    findingCompleted = (finding.summary == null) && (findingCompleted);
    findingCompleted = (finding.vulnerabilities == null) && (findingCompleted);

    return !finding;
  }


  update() {
    this.finding.answer_Id = this.answerID;
    this.finding.question_Id = this.questionID;
    this.findSvc.saveDiscovery(this.finding).subscribe(() => {
      this.dialog.close(true);
    });
  }

  updateImportance(importid) {
    this.finding.importance_Id = importid;
  }

  updateContact(contactid) {
    this.finding.finding_Contacts.forEach((fc: FindingContact) => {
      if (fc.assessment_Contact_Id === contactid.assessment_Contact_Id) {
        fc.selected = contactid.selected;
      }
    });
  }
}
