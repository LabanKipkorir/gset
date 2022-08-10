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
import { NavigationService } from '../../../../services/navigation/navigation.service';
import { Title, DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { MaturityService } from '../../../../../app/services/maturity.service';
import { ChartService } from '../../../../services/chart.service';
import { AnyRecord } from 'dns';

@Component({
  selector: 'app-cmmc-level-drilldown',
  templateUrl: './cmmc-level-drilldown.component.html',  
  styleUrls: ['../../../../../sass/cmmc-results.scss'],
  // tslint:disable-next-line:use-host-property-decorator
  host: { class: 'd-flex flex-column flex-11a' }
})
export class CmmcLevelDrilldownComponent implements OnInit {

  initialized = false;
  dataError = false;

  response;
  cmmcModel;
  statsByLevel;
  pieColorNo = "#990000";

  //Level descriptions for pie charts
  levelDescriptions = {
    1: "Safeguard Federal Contract Information (FCI)",
    2: "Serves as a transition step in cybersecurity maturity progression to protect CUI",
    3: "Protect Controlled Unclassified Information (CUI)",
    4: "Protect CUI and reduce risk of Advanced Persistent Threats (APTs)",
    5: "Protect CUI and reduce risk of Advanced Persistent Threats (APTs)",
  }

  constructor(
    public navSvc: NavigationService,
    public maturitySvc: MaturityService,
    public chartSvc: ChartService,
    private titleService: Title,
  ) { }


  ngOnInit(): void {
    this.maturitySvc.getResultsData('sitesummarycmmc').subscribe(
      (r: any) => {
        this.response = r;
        if (r.maturityModels) {
          r.maturityModels.forEach(model => {
            if (model.maturityModelName === 'CMMC') {
              this.cmmcModel = model
              this.statsByLevel = this.generateStatsByLevel(this.cmmcModel.statsByLevel)

              this.statsByLevel.forEach(level => {            
                if (this.isWithinModelLevel(level)) {
                  this.buildNewPie(level);
                }
              });
            }
          });
          window.dispatchEvent(new Event('resize'));
        }
        this.initialized = true;
      },
      error => {
        this.dataError = true;
        this.initialized = true;
        console.log('Site Summary report load Error: ' + (<Error>error).message)
      }
    ), (finish) => {
    };
  }

  /**
   * 
   * @param data 
   * @returns 
   */
  generateStatsByLevel(data) {
    let outputData = data?.filter(obj => obj.modelLevel != "Aggregate");
    outputData?.sort((a, b) => (a.modelLevel < b.modelLevel) ? 1 : -1);
    let totalAnsweredCount = 0;
    let totalUnansweredCount = 0;
    outputData?.forEach(element => {
      totalUnansweredCount += element.questionUnAnswered;
      totalAnsweredCount += element.questionAnswered;
      element["totalUnansweredCount"] = totalUnansweredCount;
      element["totalAnsweredCount"] = totalAnsweredCount;
    });

    return outputData;
  }

  getRadi(i) {
    let degreeOfNo = Math.round(i.questionUnAnswered / i.questionCount * 360)
    let val = {
      backgroundImage: `conic-gradient(${this.pieColorNo} ${degreeOfNo}deg, rgba(0,0,0,0) 0 1deg)`
    }
    return val
  }

  getBorder(input) {
    return `solid ${input} black`
  }

  getBarSettings(input) {
    let width = Math.round(input.questionAnswered / input.questionCount * 100)
    let color = "linear-gradient(5deg, rgba(100,100,100,1) 0%, rgba(200,200,200,1) 100%)"
    if (input.modelLevel < this.cmmcModel.targetLevel) {
      color = this.getGradient("blue");
    } else if (input.modelLevel == this.cmmcModel.targetLevel) {
      color = this.getGradient("green");
    } else {
      color = this.getGradient("grey");
    }
    let val = {
      width: `${width}%`,
      background: color
    }
    return val
  }

  isWithinModelLevel(level) {
    if (level.modelLevel == "CMMC") { return false; }
    let val = Number(level.modelLevel)
    if (!isNaN(val)) {
      if (val <= this.cmmcModel.targetLevel) {
        return true;
      }
    }
    return false;
  }


  getGradient(color, alpha = 1, reverse = false) {
    let vals = {
      color_one: "",
      color_two: ""
    }
    alpha = 1
    switch (color) {
      case "blue":
      case "blue-1": {
        vals["color_one"] = `rgba(31,82,132,${alpha})`
        vals["color_two"] = `rgba(58,128,194,${alpha})`
        break;
      }
      case "blue-2": {
        vals["color_one"] = `rgba(75,116,156,${alpha})`
        vals["color_two"] = `rgba(97,153,206,${alpha})`
        break;
      }
      case "blue-3": {
        vals["color_one"] = `rgba(120,151,156,${alpha})`
        vals["color_two"] = `rgba(137,179,218,${alpha})`
        break;
      }
      case "blue-4": {
        vals["color_one"] = `rgba(165,185,205,${alpha})`
        vals["color_two"] = `rgba(176,204,230,${alpha})`
        break;
      }
      case "blue-5": {
        vals["color_one"] = `rgba(210,220,230,${alpha})`
        vals["color_two"] = `rgba(216,229,243,${alpha})`
        break;
      }
      case "green": {
        vals["color_one"] = `rgba(98,154,109,${alpha})`
        vals["color_two"] = `rgba(31,77,67,${alpha})`
        break;
      }
      case "grey": {
        vals["color_one"] = `rgba(98,98,98,${alpha})`
        vals["color_two"] = `rgba(120,120,120,${alpha})`
        break;
      }
      case "orange": {
        vals["color_one"] = `rgba(255,190,41,${alpha})`
        vals["color_two"] = `rgba(224,217,98,${alpha})`
        break;
      }
    }
    if (reverse) {
      let tempcolor = vals["color_one"]
      vals["color_one"] = vals["color_two"]
      vals["color_two"] = tempcolor
    }
    return `linear-gradient(5deg,${vals['color_one']} 0%, ${vals['color_two']} 100%)`
  }


  /**
   * 
   */
  buildNewPie(level: any) {
    // build the data object for Chart
    var x: any = {};
    x.label = '';
    x.labels = ['Compliant', 'Noncompliant'];
    x.data = [100 * (level.questionAnswered / level.questionCount), 100 * (level.questionUnAnswered / level.questionCount)];
    x.colors = [this.chartSvc.segmentColor('Y'), this.chartSvc.segmentColor('U')];

    var canvasId = 'level' + level.modelLevel;

    setTimeout(() => {
      level.chart = this.chartSvc.buildDoughnutChart(canvasId, x);
    }, 10);
  }
}
