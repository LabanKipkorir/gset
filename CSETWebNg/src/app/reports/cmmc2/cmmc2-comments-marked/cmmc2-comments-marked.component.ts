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
import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ConfigService } from '../../../services/config.service';
import { MaturityService } from '../../../services/maturity.service';

@Component({
  selector: 'app-cmmc2-comments-marked',
  templateUrl: './cmmc2-comments-marked.component.html',
  styleUrls: ['./../../crr/crr-report/crr-report.component.scss']
})
export class Cmmc2CommentsMarkedComponent implements OnInit {

  model: any;
  loading: boolean = false;
  logoPath: string = '';
  keyToCategory: any;

  commentsList = [];
  markedForReviewList = [];

  constructor(
  public configSvc: ConfigService,
  private titleService: Title,
  private maturitySvc: MaturityService
  ){}

  ngOnInit() {
    this.loading = true;
    this.keyToCategory = this.maturitySvc.keyToCategory;
    this.titleService.setTitle("CMMC 2.0 Comments and Marked for Review - CSET");
    let appCode = this.configSvc.installationMode;

    if (!appCode || appCode === 'CSET') {
      this.logoPath = "assets/images/CISA_Logo_1831px.png";
    }
    else if (appCode === 'CSET-TSA') {
      this.logoPath = "assets/images/TSA/tsa_insignia_rgbtransparent.png";
    }

    this.maturitySvc.getCmmcReportData().subscribe(
      (r: any) => {
        this.model = r;

        // Build up comments list
        this.model.reportData.comments.forEach(matAns => {
          const domain = matAns.mat.question_Title.split('.')[0];
          console.log(domain);
          const cElement = this.commentsList.find(e => e.cat === this.keyToCategory[domain]);
          if (!cElement) {
            this.commentsList.push({ cat: this.keyToCategory[domain], matAnswers: [matAns] });
          } else {
            cElement.matAnswers.push(matAns);
          }
        });

        // mark questions followed by a child for border display
        this.commentsList.forEach(e => {
          for (let i = 0; i < e.matAnswers.length; i++) {
            if (e.matAnswers[i + 1]?.mat.parent_Question_Id != null) {
              e.matAnswers[i].isFollowedByChild = true;
            }
          }
        });

        // Build up marked for review list
        this.model.reportData.markedForReviewList.forEach(matAns => {
          const domain = matAns.mat.question_Title.split('.')[0];
          const mfrElement = this.markedForReviewList.find(e => e.cat === this.keyToCategory[domain]);

          if (!mfrElement) {
            this.markedForReviewList.push({ cat: this.keyToCategory[domain], matAnswers: [matAns] });
          } else {
            mfrElement.matAnswers.push(matAns);
          }
        });

        // mark questions followed by a child for border display
        this.markedForReviewList.forEach(e => {
          for (let i = 0; i < e.matAnswers.length; i++) {
            if (e.matAnswers[i + 1]?.mat.parent_Question_Id != null) {
              e.matAnswers[i].isFollowedByChild = true;
            }
          }
        });

        this.loading = false;
      },
      error => console.log('CMMC 2.0 Comments and Marked for Review Report Error: ' + (<Error>error).message)
    );
  }

  getFullAnswerText(abb: string) {
    return this.configSvc.config['answerLabel' + abb];
  }

  printReport() {
    window.print();
  }
}
