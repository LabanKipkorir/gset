import { NestedTreeControl } from '@angular/cdk/tree';
import { MatTreeNestedDataSource } from '@angular/material/tree';
import { Router } from '@angular/router';
import { AssessmentService } from '../assessment.service';
import { EventEmitter, Injectable, Output, Directive } from "@angular/core";
import { of as observableOf, BehaviorSubject } from "rxjs";
import { ConfigService } from '../config.service';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { AnalyticsService } from '../analytics.service';
import { QuestionsService } from '../questions.service';
import { MaturityService } from '../maturity.service';
import { CisService } from '../cis.service';
import { PageVisibilityService } from '../navigation/page-visibility.service';
import { NavTreeService } from './nav-tree.service';


export interface NavTreeNode {
  children?: NavTreeNode[];
  label?: string;
  value?: any;
  DatePublished?: string;
  HeadingTitle?: string;
  HeadingText?: string;
  DocId?: string;
  elementType?: string;
  isCurrent?: boolean;
  expandable: boolean;
  visible: boolean;
  index?: number;
}

@Injectable({
  providedIn: 'root'
})
export class NavigationService {

  /**
   * The workflow is stored in a DOM so that we can easily navigate around the tree
   */
  workflow: Document;

  currentPage = '';



  @Output()
  navItemSelected = new EventEmitter<any>();

  @Output()
  scrollToQuestion = new EventEmitter<string>();


  isNavLoading = false;

  activeResultsView: string;

  frameworkSelected = false;
  acetSelected = false;
  diagramSelected = true;

  analyticsIsUp = false;
  cisSubnodes = null;



  /**
   *
   */
  constructor(
    private assessSvc: AssessmentService,
    private configSvc: ConfigService,
    private router: Router,
    private http: HttpClient,
    private analyticsSvc: AnalyticsService,
    private questionsSvc: QuestionsService,
    private maturitySvc: MaturityService,
    private cisSvc: CisService,
    private pageVisibliltySvc: PageVisibilityService,
    private navTreeSvc: NavTreeService
  ) {
    this.setWorkflow('omni');

    this.analyticsSvc.pingAnalyticsService().subscribe(data => {
      this.analyticsIsUp = true;
    });
  }

  private getChildren = (node: NavTreeNode) => { return observableOf(node.children); };


  /**
   * Generates a random 'magic number'.
   */
  getMagic() {
    this.navTreeSvc.magic = (Math.random() * 1e5).toFixed(0);
    return this.navTreeSvc.magic;
  }



  /**
   *
   */
  getFramework() {
    return this.http.get(this.configSvc.apiUrl + "standard/IsFramework");
  }

  setACETSelected(acet: boolean) {
    this.acetSelected = acet;
    localStorage.removeItem('tree');
    this.navTreeSvc.buildTree(this.workflow, this.getMagic());
  }

  setFrameworkSelected(framework: boolean) {
    this.frameworkSelected = framework;
    localStorage.removeItem('tree');
    this.navTreeSvc.buildTree(this.workflow, this.getMagic());
  }



  /**
   * 
   */
  setWorkflow(name: string) {
    const url = 'assets/navigation/workflow-' + name + '.xml';
    this.http.get(url, { responseType: 'text' }).subscribe((xml: string) => {

      // build the workflow DOM
      let d = new DOMParser();
      this.workflow = d.parseFromString(xml, 'text/xml');


      // populate displaytext for CIS and MVRA using API-sourced grouping titles
      this.maturitySvc.mvraGroupings.forEach(t => {
        const e = this.workflow.getElementById('maturity-questions-nested-' + t.id);
        if (!!e) {
          e.setAttribute('displaytext', t.title);
        }
      });

      this.maturitySvc.cisGroupings.forEach(t => {
        const e = this.workflow.getElementById('maturity-questions-nested-' + t.id);
        if (!!e) {
          e.setAttribute('displaytext', t.title);
        }
      });


      // build the sidenav tree
      localStorage.removeItem('tree');
      this.navTreeSvc.buildTree(this.workflow, this.getMagic());
    },
      (err: HttpErrorResponse) => {
        console.log(err);
      });
  }

  /**
   * 
   */
  buildTree() {
    this.navTreeSvc.buildTree(this.workflow, this.getMagic());
  }

  /**
   * 
   */
  setQuestionsTree() {
    this.navTreeSvc.setQuestionsTree();
  }

  /**
   * Crawls the workflow document to determine the next viewable page.
   */
  navNext(cur: string) {
    const currentPage = this.workflow.getElementById(cur);

    if (currentPage == null) {
      return;
    }

    if (currentPage.children.length == 0 && currentPage.nextElementSibling == null && currentPage.parentElement.tagName == 'nav') {
      // we are at the last page, nothing to do
      return;
    }

    let target = currentPage;

    do {
      if (target.children.length > 0) {
        target = <HTMLElement>target.firstElementChild;
      } else {
        while (!target.nextElementSibling) {
          target = <HTMLElement>target.parentElement;
        }
        target = <HTMLElement>target.nextElementSibling;
      }
    } while (!this.pageVisibliltySvc.canLandOn(target) || !this.pageVisibliltySvc.showPage(target));

    this.routeToTarget(target);
  }

  /**
   * Crawls the workflow document to determine the previous viewable page.
   */
  navBack(cur: string) {
    const currentPage = this.workflow.getElementById(cur);

    if (currentPage == null) {
      return;
    }

    if (currentPage.previousElementSibling == null && currentPage.parentElement.tagName == 'nav') {
      // we are at the first page, nothing to do
      return;
    }

    let target = currentPage;

    do {
      if (target.children.length > 0) {
        target = <HTMLElement>target.lastElementChild;
      } else {
        while (!target.previousElementSibling) {
          target = <HTMLElement>target.parentElement;
        }
        target = <HTMLElement>target.previousElementSibling;
      }
    } while (!this.pageVisibliltySvc.canLandOn(target) || !this.pageVisibliltySvc.showPage(target));

    this.routeToTarget(target);
  }

  /**
   * Routes to the path configured for the specified pageId.
   * @param value
   */
  navDirect(id: string) {
    // if the target is a simple string, find it in the pages structure
    // and navigate to its path
    if (typeof id == 'string') {
      let target = this.workflow.getElementById(id);

      if (target == null) {
        console.error('Cannot find \'' + id + ' \'in workflow document');
        return;
      }

      // if they clicked on a tab there won't be a path -- nudge them to the next page
      if (!target.hasAttribute('path')) {
        this.navNext(id);
        return;
      }

      this.routeToTarget(target);
      return;
    }
  }

  /**
   * Navigates to the path specified in the target node.
   */
  routeToTarget(target: HTMLElement) {
    this.navTreeSvc.setCurrentPage(target.id);

    // determine the route path
    const targetPath = target.attributes['path'].value.replace('{:id}', this.assessSvc.id().toString());
    this.router.navigate([targetPath]);
  }

  /**
   * Determines if the specified page is the last visible page in the nav flow.
   * Used to hide the "Next" button.
   * @returns
   */
  isLastVisiblePage(id: string): boolean {
    let target = this.workflow.getElementById(id);

    do {
      if (target.children.length > 0) {
        target = <HTMLElement>target.firstElementChild;
      } else if (!target.nextElementSibling) {
        while (!target.nextElementSibling && target.tagName != 'nav') {
          target = <HTMLElement>target.parentElement;
        }
        target = <HTMLElement>target.nextElementSibling;
      } else {
        target = <HTMLElement>target.nextElementSibling;
      }
    } while (!!target && !this.pageVisibliltySvc.showPage(target));

    if (!target) {
      return true;
    }

    return false;
  }

  /**
   * 
   */
  setCurrentPage(id: string) {
    this.navTreeSvc.setCurrentPage(id);
  }
}