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
import { Injectable } from '@angular/core';
import { AssessmentService } from '../assessment.service';
import { NavTreeNode } from './navigation.service';
import { PageVisibilityService } from './page-visibility.service';
import { MatTreeNestedDataSource } from '@angular/material/tree';
import { NestedTreeControl } from '@angular/cdk/tree';
import { of as observableOf, BehaviorSubject } from "rxjs";
import { TranslocoService } from '@ngneat/transloco';

@Injectable({
  providedIn: 'root'
})
export class NavTreeService {

  dataSource: MatTreeNestedDataSource<NavTreeNode> = new MatTreeNestedDataSource<NavTreeNode>();
  dataChange: BehaviorSubject<NavTreeNode[]> = new BehaviorSubject<NavTreeNode[]>([]);
  tocControl: NestedTreeControl<NavTreeNode>;

  workflow: Document

  public currentPage: string;

  public magic: string;

  public isNavLoading: boolean;

  constructor(
    private assessSvc: AssessmentService,
    private pageVisibliltySvc: PageVisibilityService,
    private tSvc: TranslocoService
  ) {
    // set up the mat tree control and its data source
    this.tocControl = new NestedTreeControl<NavTreeNode>(this.getChildren);
    this.dataSource = new MatTreeNestedDataSource<NavTreeNode>();
    this.dataChange.subscribe(data => {
      this.dataSource.data = data;
      this.tocControl.dataNodes = data;
      this.tocControl.expandAll();
    });
  }

  private getChildren = (node: NavTreeNode) => { return observableOf(node.children); };

  /**
   *
   * @param magic
   */
  buildTree(workflow: Document, magic: string) {
    if (this.magic !== magic) {
      console.warn('buildTree - magic compare failed');
      return;
    }

    this.workflow = workflow;

    this.dataSource.data = this.buildTocData();
    this.tocControl.dataNodes = this.dataSource.data;

    this.setQuestionsTree();

    this.tocControl.expandAll();

    this.isNavLoading = false;
  }

  /**
   * Returns a tree structure of TOC nodes
   */
  buildTocData(): NavTreeNode[] {
    const toc: NavTreeNode[] = [];
    if(this.workflow.documentElement.children)
      this.domToNav(this.workflow.documentElement.children, toc);

    return toc;
  }


  /**
   * Converts workflow DOM nodes to NavTreeNodes.
   * Calls itself recursively to populate children.
   */
  domToNav(domNodes: HTMLCollection, navNodes: NavTreeNode[]) {
    Array.from(domNodes).forEach((workflowNode: HTMLElement) => {

      // nodes without a 'displaytext' or 'd' attribute are ignored
      if (!!workflowNode.attributes['displaytext'] || !!workflowNode.attributes['d']) {

        let displaytext = workflowNode.attributes['displaytext']?.value;
        let d = workflowNode.attributes['d']?.value;

        // localize the 'd' attribute
        if (!!d) {
          displaytext = this.tSvc.translate(`titles.${d}`);
        }

        const navNode: NavTreeNode = {
          label: displaytext,
          value: workflowNode.id ?? 0,
          children: [],
          expandable: true,
          visible: true,
          enabled: true   
        };        
        navNode.visible = this.pageVisibliltySvc.showPage(workflowNode);
        if(navNode.visible){
          navNode.enabled = this.pageVisibliltySvc.isEnabled(workflowNode);
        }
        // the node might need tweaking based on certain factors
        this.adjustNavNode(navNode);

        if (this.currentPage === navNode.value) {
          navNode.isCurrent = true;
        }

        navNodes.push(navNode);

        if (workflowNode.children.length > 0) {
          this.domToNav(workflowNode.children, navNode.children);
        }
      }
    });
  }

  /**
   * Any runtime adjustments can be made here.
   */
  adjustNavNode(node: NavTreeNode) {
    if (node.value == 'maturity-questions') {
      // Models may use a specific term (alias) for "questions" or "practices"
      const alias = this.assessSvc.assessment?.maturityModel?.questionsAlias?.toLowerCase();
      if (!!alias) {
        node.label = this.tSvc.translate(`titles.${alias}`);
      }
    }

    if (node.value == 'standard-questions') {
      const mode = this.assessSvc.applicationMode?.toLowerCase();
      if (mode == 'q') {
        node.label = this.tSvc.translate('titles.standard questions');
      }
      if (mode == 'r') {
        node.label = this.tSvc.translate('titles.standard requirements');
      }
    }    
  }

  /**
   *
   * @param magic
   */
  clearTree(magic: string) {
    if (this.magic === magic) {
      this.isNavLoading = true;
      this.dataSource.data = null;
      this.tocControl.dataNodes = this.dataSource.data;
    }
  }



  /**
   * Replaces the children of the Questions node with
   * the values supplied.
   * There can be up to 3 sources for questions --
   *   - Maturity
   *   - Questions/Requirements
   *   - Diagram
   *
   * This functionality has been put on hold.  It needs to be reevaluated
   * to see if it is useful to the user or is just clutter in the nav tree.
   * For now, there's no need to make API calls whose response will not be used.
   */
  setQuestionsTree() {
    const d = this.dataSource.data;
    this.dataSource.data = d;
  }

  /**
   * Called from the resource-library component.
   */
  setTree(tree: NavTreeNode[], magic: string, collapsible: boolean = true) {
    if (this.magic === magic) {
      this.tocControl = new NestedTreeControl<NavTreeNode>((node: NavTreeNode) => {
        return observableOf(node.children);
      });
      this.dataSource = new MatTreeNestedDataSource<NavTreeNode>();
      this.dataSource.data = tree;
      this.tocControl.dataNodes = tree;

      this.tocControl.expandAll();

      this.isNavLoading = false;
    }
  }

  /**
   *
   */
  hasNestedChild(_: number, node: NavTreeNode) {
    return node.children?.length > 0;
  }


  /**
  * Clear any current page and mark the new one.
  */
  setCurrentPage(id: string) {
    this.clearCurrentPage(this.dataSource?.data);

    let delay = !!this.dataSource.data ? 0 : 1000;

    setTimeout(() => {
      const currentNode = this.findInTree(this.dataSource.data, id);

      if (!!currentNode) {
        currentNode.isCurrent = true;
        this.currentPage = currentNode.value;
      }
    }, delay);
  }


  /**
   * Sets all nodes in the tree to NOT be current
   */
  clearCurrentPage(tree: NavTreeNode[]) {
    this.currentPage = '';

    if (!tree) {
      return;
    }

    for (let i = 0; i < tree.length; i++) {
      let node = tree[i];
      node.isCurrent = false;

      if (node.children.length > 0) {
        this.clearCurrentPage(node.children);
      }
    }
  }

  /**
   * Recurses a list of NavTreeNode and their children for a
   * className.
   */
  findInTree(tree: NavTreeNode[], className: string): NavTreeNode {
    let result = null;

    if (!!tree) {
      for (let i = 0; i < tree.length; i++) {
        let node = tree[i];
        if (node.value === className) {
          return node;
        }

        if (node.children.length > 0) {
          result = this.findInTree(node.children, className);
          if (!!result) {
            return result;
          }
        }
      }
    }

    return result;
  }
}
