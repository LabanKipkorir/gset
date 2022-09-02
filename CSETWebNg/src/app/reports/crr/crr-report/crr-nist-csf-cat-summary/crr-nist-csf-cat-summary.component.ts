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
import { CrrService } from './../../../../services/crr.service';

@Component({
  selector: 'app-crr-nist-csf-cat-summary',
  templateUrl: './crr-nist-csf-cat-summary.component.html',
  styleUrls: ['./../crr-report.component.scss']
})
export class CrrNistCsfCatSummaryComponent implements OnInit {

  @Input() model: CrrReportModel;

  bodyData: any[] = [];
  legend: string = '';

  constructor(private crrSvc: CrrService) { }

  ngOnInit(): void {
    this.crrSvc.getNistCsfCatSummaryBodyData().subscribe((resp: any[]) => {
      this.bodyData = resp;
    })

    this.crrSvc.getMil1PerformanceSummaryLegendWidget().subscribe((resp: string) => {
      this.legend = resp;
    })
  }
}
