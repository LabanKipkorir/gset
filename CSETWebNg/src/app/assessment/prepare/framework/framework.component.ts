////////////////////////////////
//
//   Copyright 2023 Battelle Energy Alliance, LLC
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
import { Router } from '@angular/router';
import { AssessmentService } from '../../../services/assessment.service';
import { FrameworkService } from '../../../services/framework.service';
import { Frameworks, SelectedTier } from '../../../models/frameworks.model';
import { NavigationService } from '../../../services/navigation/navigation.service';

@Component({
  selector: 'app-framework',
  templateUrl: './framework.component.html',
  // eslint-disable-next-line
  host: {class: 'd-flex flex-column flex-11a'}
})
export class FrameworkComponent implements OnInit {
  frameworks: Frameworks;

  selectedTier: SelectedTier;

  constructor(
    public navSvc: NavigationService,
    private assessSvc: AssessmentService,
    private frameworkSvc: FrameworkService
    ) { }

  ngOnInit() {
    this.loadFrameworks();
  }

  loadFrameworks() {
    this.frameworkSvc.getFrameworksList().subscribe(
      (data: Frameworks) => {
        this.frameworks = data;
      },
      error => {
        console.log('Error getting all frameworks: ' + (<Error>error).name + (<Error>error).message);
        console.log('Error getting all frameworks: ' + (<Error>error).stack);
      });
  }

  submit(group, tier) {
    this.selectedTier = {
      tierType: group.tierType,
      tierName: tier.tierName
    };

    this.frameworkSvc.postSelections(this.selectedTier).subscribe();
  }
}
