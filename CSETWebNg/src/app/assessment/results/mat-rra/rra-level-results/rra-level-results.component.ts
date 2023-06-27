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
import { NavigationService } from '../../../../services/navigation/navigation.service';
import { Title, DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { MaturityService } from '../../../../services/maturity.service';

@Component({
  selector: 'app-rra-level-results',
  templateUrl: './rra-level-results.component.html',
  styleUrls: ['rra-level-results.component.scss'],
  // eslint-disable-next-line
  host: { class: 'd-flex flex-column flex-11a' },
})
export class RraLevelResultsComponent implements OnInit {

  initialized = false;
  dataError = false;

  response;
  cmmcModel;
  
  whiteText= "rgba(255,255,255,1)"
  blueText= "rgba(31,82,132,1)"

  constructor(
    public navSvc: NavigationService,
    public maturitySvc: MaturityService,
    private titleService: Title,
  ) { }

  ngOnInit(): void {
  }

}
