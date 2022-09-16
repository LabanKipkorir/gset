import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-landing-page-tabs',
  templateUrl: './landing-page-tabs.component.html',
  styleUrls: ['./landing-page-tabs.component.scss'],
  host: { class: 'd-flex flex-column flex-11a w-100' }
})
export class LandingPageTabsComponent implements OnInit {
  
  currentTab:string;
  isSearch: boolean= false;
  constructor() { }

  ngOnInit(): void {
    this.currentTab="newAssessment";
  }

  setTab(tab) {
    this.currentTab = tab;
  }

  checkActive(tab) {
    return this.currentTab === tab;
  }

  changeToSearch(){
    this.isSearch = !this.isSearch;
  }
}
