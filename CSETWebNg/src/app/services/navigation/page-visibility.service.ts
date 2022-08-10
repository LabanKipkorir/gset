import { Injectable } from '@angular/core';
import { AssessmentService } from '../assessment.service';
import { ConfigService } from '../config.service';

/**
 * Analyzes assessment 
 */
@Injectable({
  providedIn: 'root'
})
export class PageVisibilityService {

  constructor(
    private assessSvc: AssessmentService,
    private configSvc: ConfigService
  ) { }

  /**
   * Indicates if you can "land on" the page.
   * Parent nodes can't be landed on
   * because they aren't displayable pages.  They function
   * as collapsible nodes in the nav tree.
   */
  canLandOn(page: HTMLElement): boolean {
    // pages without a path can't be landed on/navigated to
    if (!page.hasAttribute('path')) {
      return false;
    }

    return true;
  }

  /**
   * Evaluates conditions where a page should be hidden and
   * ignored in the TOC and next/back workflow.
   */
  showPage(page: HTMLElement): boolean {
    // look for a condition on the current page or its nearest parent
    let nnnn = page.closest('[condition]');
    let conditionAttrib = nnnn?.attributes['condition']?.value;

    // if no conditions are applicable, show the page
    if (!conditionAttrib || conditionAttrib.length === 0) {
      return true;
    }

    let show = true;

    // All conditions must be true (AND).  
    // Start with true and if any fail, result is false.
    let conditions = conditionAttrib.toUpperCase().split(' ');
    conditions.forEach(c => {

      // if 'HIDE' is present, this trumps everything else
      if (c == 'HIDE') {
        show = false;
      }


      // SOURCE checks the assessment's 'workflow' value (the skin it was born in)
      if (c.startsWith('SOURCE:')) {
        let target = c.substring(c.indexOf(':') + 1);
        show = show && (this.assessSvc.assessment?.workflow == target);
      }

      if (c.startsWith('SOURCE-NOT:')) {
        let target = c.substring(c.indexOf(':') + 1);
        show = show && (this.assessSvc.assessment?.workflow !== target);
      }

      if (c.startsWith('SOURCE-ANY(')) {
        let found = false;
        let p1 = c.indexOf('(');
        let p2 = c.indexOf(')');
        let targetText = c.substring(p1 + 1, p2);
        var targets = targetText.split(',');
        targets.forEach(t => {
          found = found || (this.assessSvc.assessment?.workflow == t);
        });
        show = show && found;
      }

      
      // Installation Mode
      if (c.startsWith('INSTALL-MODE:')) {
        let target = c.substring(c.indexOf(':') + 1);
        show = show && (this.configSvc.installationMode == target);
      }

      if (c.startsWith('INSTALL-MODE-NOT:')) {
        let target = c.substring(c.indexOf(':') + 1);
        show = show && (this.configSvc.installationMode !== target);
      }

      if (c.startsWith('OPTION-ANY(')) {
        let found = false;
        let p1 = c.indexOf('(');
        let p2 = c.indexOf(')');
        let targetText = c.substring(p1 + 1, p2);
        var targets = targetText.split(',');
        targets.forEach(t => {
          switch (t.toUpperCase()) {
            case 'MATURITY':
              found = found || !!this.assessSvc.assessment && this.assessSvc.assessment.useMaturity;
              break;
            case 'STANDARD':
              found = found || !!this.assessSvc.assessment && this.assessSvc.assessment.useStandard;
              break;
            case 'DIAGRAM':
              found = found || !!this.assessSvc.assessment && this.assessSvc.assessment.useDiagram;
              break;
          }
        });
        show = show && found;
      }


      // OPTION is which features are included in the assessment: maturity, standard or diagram
      if (c === 'OPTION:MATURITY') {
        show = show && (
          !!this.assessSvc.assessment
          && this.assessSvc.assessment.useMaturity);
      }

      if (c === 'OPTION:STANDARD') {
        show = show && (
          !!this.assessSvc.assessment
          && this.assessSvc.assessment.useStandard);
      }

      if (c === 'OPTION:DIAGRAM') {
        show = show && (
          !!this.assessSvc.assessment
          && this.assessSvc.assessment.useDiagram);
      }


      // look for the specified maturity model
      if (c.startsWith('MATURITY:')) {
        let target = c.substring(c.indexOf(':') + 1);

        show = show && (
          !!this.assessSvc.assessment
          && this.assessSvc.assessment.useMaturity
          && this.assessSvc.usesMaturityModel(target));
      }

      if (c.startsWith('MATURITY-NOT:')) {
        let target = c.substring(c.indexOf(':') + 1);

        show = show && (
          !!this.assessSvc.assessment
          && this.assessSvc.assessment.useMaturity
          && !this.assessSvc.usesMaturityModel(target));
      }

      if (c.startsWith('MATURITY-ANY(')) {
        let found = false;
        let p1 = c.indexOf('(');
        let p2 = c.indexOf(')');
        let targetText = c.substring(p1 + 1, p2);
        var targets = targetText.split(',');
        targets.forEach(t => {
          found = found || (
            !!this.assessSvc.assessment
            && this.assessSvc.assessment.useMaturity
            && this.assessSvc.usesMaturityModel(t.trim()));
        });

        show = show && found;
      }


      // Look for a maturity target level greater than X
      if (c.startsWith('TARGET-LEVEL-GT:')) {
        let target = c.substring(c.indexOf(':') + 1);
        show = show && this.assessSvc.assessment.maturityModel.maturityTargetLevel > Number.parseInt(target);
      }


      if (c == 'SHOW-EXEC-SUMMARY') {
        show = show && this.showExecSummaryPage();
      }

    });

    return show;
  }


  /**
   * Indicates when the Executive Summary page should be included in the navigation.
   */
  showExecSummaryPage(): boolean {
    let assessment = this.assessSvc.assessment;
    return assessment?.useDiagram || assessment?.useStandard;
  }
}
