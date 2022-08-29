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
import { Component, Input, OnInit } from '@angular/core';
import { CrrReportModel } from '../../../../models/reports.model';
import { CrrService } from '../../../../services/crr.service';

@Component({
  selector: 'app-crr-mil1-performance-summary',
  templateUrl: './crr-mil1-performance-summary.component.html',
  styleUrls: ['./../crr-report.component.scss']
})
export class CrrMil1PerformanceSummaryComponent implements OnInit {

  @Input() model: CrrReportModel;
  mil1FullAnswerDistribChart: string = '';
  legend: string = '';
  scoreBarCharts: string[] = [];
  stackedBarCharts: any[] = [];

  constructor(private crrSvc: CrrService) { }

  ngOnInit(): void {
    this.crrSvc.getMil1FullAnswerDistribWidget().subscribe((resp: string) => {
      this.mil1FullAnswerDistribChart = resp;
    })

    this.crrSvc.getMil1PerformanceSummaryLegendWidget().subscribe((resp: string) => {
      this.legend = resp;
    })

    this.crrSvc.getMil1PerformanceSummaryBodyCharts().subscribe((resp: any) => {
      this.scoreBarCharts = resp.scoreBarCharts;
      this.stackedBarCharts = resp.stackedBarCharts;
    })
  }

  // This function splits strings like
  // "Goal 6 - Post-incident lessons learned are translated into improvement strategies."
  // and
  // "Goal 3-Risks are identified."
  stringSplitter(str: string) {
    return str.split(" - ")[1] ?? str.split("-")[1];
  }

  getStackedBarChart(goalTitle: string) {
    return this.stackedBarCharts.find(c => c.title === goalTitle).chart;
  }

  filterMilDomainGoals(goals) {
    return goals.filter(g => !g.title.startsWith('MIL'));
  }
}
