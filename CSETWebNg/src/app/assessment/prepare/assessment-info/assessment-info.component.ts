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
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { AssessmentDetail } from '../../../models/assessment-info.model';
import { AssessmentService } from '../../../services/assessment.service';
import { ConfigService } from '../../../services/config.service';
import { NavigationService } from '../../../services/navigation/navigation.service';

@Component({
  selector: 'app-assessment-info',
  templateUrl: './assessment-info.component.html',
  // tslint:disable-next-line:use-host-property-decorator
  host: { class: 'd-flex flex-column flex-11a' }
})
export class AssessmentInfoComponent implements OnInit {
  constructor(
    public assessSvc: AssessmentService,
    public configSvc: ConfigService,
    public navSvc: NavigationService

    ) { }


  ngOnInit() {

    // this simple assignment is temporary.  I think we are going to 
    // need an "omni workflow" that supports everything, in case there
    // is a mixture of features/options.
    // The omni workflow will still work in a single-option assessment.
    // const assessment = this.assessSvc.assessment;
    // if (assessment.useStandard) {
    //   this.navSvc.setWorkflow('classic');
    // } 
    // if (assessment.useMaturity) {
    //   this.navSvc.setWorkflow('maturity');
    // }

    this.navSvc.setWorkflow('omni');
  }
}
