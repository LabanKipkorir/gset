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

import {
  Component,
  EventEmitter,
  OnInit,
  Output,
  ViewChild,
  HostListener
} from '@angular/core';
import { MatSidenav } from '@angular/material/sidenav';
import { ActivatedRoute, Router } from '@angular/router';
import { AssessmentService } from '../services/assessment.service';
import { LayoutService } from '../services/layout.service';
import { NavTreeService } from '../services/navigation/nav-tree.service';
import { NavigationService } from '../services/navigation/navigation.service';

@Component({
  selector: 'app-assessment',
  styleUrls: ['./assessment.component.scss'],
  templateUrl: './assessment.component.html',
  // tslint:disable-next-line:use-host-property-decorator
  host: { class: 'd-flex flex-column flex-11a w-100' }
})
export class AssessmentComponent implements OnInit {
  innerWidth: number;
  innerHeight: number;

  /**
   * Indicates whether the nav panel is visible (true)
   * or hidden (false).
   */
  expandNav = false;

  /**
   * Indicates whether the nav stays visible (true)
   * or auto-hides when the screen is narrow (false).
   */
  lockNav = true;

  widthBreakpoint = 960;
  scrollTop = 0;


  @Output() navSelected = new EventEmitter<string>();

  @ViewChild('sNav')
  sideNav: MatSidenav;

  @HostListener('window:resize', ['$event'])
  onResize(event) {
    this.evaluateWindowSize();
  }

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    public assessSvc: AssessmentService,
    public navSvc: NavigationService,
    public navTreeSvc: NavTreeService,
    public layoutSvc: LayoutService
  ) {
    this.assessSvc.getAssessmentToken(+this.route.snapshot.params['id']);
    this.assessSvc.getMode();
    this.setTab('prepare');
    this.navSvc.activeResultsView = null;
    if (localStorage.getItem('tree')) {
      this.navSvc.buildTree();
    }
  }

  ngOnInit(): void {
    this.evaluateWindowSize();
  }

  setTab(tab) {
    this.assessSvc.currentTab = tab;
  }

  checkActive(tab) {
    return this.assessSvc.currentTab === tab;
  }

  /**
   * Determines how to display the sidenav.
   */
  sidenavMode() {
    if (this.layoutSvc.hp) {
      this.lockNav = false;
      return 'over';
    }

    return this.innerWidth < this.widthBreakpoint ? 'over' : 'side';
  }

  /**
   * Evaluates sidenav drawer behavior based on window size
   */
  evaluateWindowSize() {
    this.innerWidth = window.innerWidth;
    this.innerHeight = window.innerHeight;

    // show/hide lock/unlock the nav drawer based on available width
    if (this.innerWidth < this.widthBreakpoint) {
      this.expandNav = false;
      this.lockNav = false;
    } else {
      this.expandNav = true;
      this.lockNav = true;
    }
  }

  /**
   * Called when the user clicks an item
   * in the nav.
   */
  selectNavItem(target: string) {
    if (!this.lockNav) {
      this.expandNav = false;
    } else {
      this.expandNav = true;
    }

    this.navSvc.navDirect(target);
  }

  toggleNav() {
    this.expandNav = !this.expandNav;
  }

  handleScroll(component: string) {
    const element = document.getElementById(component);
    if (
      element.scrollTop < this.scrollTop &&
      document.scrollingElement.scrollTop > 0
    ) {
      document.scrollingElement.scrollTo({ behavior: 'smooth', top: 0 });
    }
    this.scrollTop = element.scrollTop;
  }

  /**
   * Returns the text for the Requirements label.  It might be Statements for ACET assessments.
   */
  requirementsLabel() {
    return 'Requirements';
  }

  /**
   * Fired when the sidenav's opened state changes.
   * @param e
   */
  openStateChange(e) {
    this.expandNav = e;
  }

  isTocLoading(s) {
    return s === "Please wait" || s === "Loading questions";
  }

  goHome() {
    this.assessSvc.dropAssessment();
    this.router.navigate(['/home']);
  }
}
