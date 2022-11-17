import { Component, ElementRef, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { filter } from 'rxjs/operators';
import { PasswordStatusResponse } from '../../models/reset-pass.model';
import { AuthenticationService } from '../../services/authentication.service';
import { ChangePasswordComponent } from "../../dialogs/change-password/change-password.component";
import { ConfigService } from '../../services/config.service';


@Component({
  selector: 'app-landing-page-tabs',
  templateUrl: './landing-page-tabs.component.html',
  styleUrls: ['./landing-page-tabs.component.scss'],
  host: { class: 'd-flex flex-column flex-11a w-100' }
})
export class LandingPageTabsComponent implements OnInit, AfterViewInit {

  currentTab: string;
  isSearch: boolean= false;
  searchString:string="";
  @ViewChild('tabs') tabsElementRef: ElementRef;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    public authSvc: AuthenticationService,
    public dialog: MatDialog,
    private configSvc: ConfigService
    ) { }

  ngOnInit(): void {
    this.setTab('myAssessments');



    // setting the tab when we get a query parameter.
    this.route.queryParamMap.pipe(filter(params => params.has('tab'))).subscribe(params => {
      this.setTab(params.get('tab'));
      // clear the query parameters from the url.
      this.router.navigate([], { queryParams: {} });
    });
  }

  ngAfterViewInit(): void {
    // Only implementing sticky tabs on main CSET installation mode for now.
    const tabsEl = this.tabsElementRef.nativeElement;
    tabsEl.classList.add('sticky-tabs');
    if (this.authSvc.isLocal) {
      tabsEl.style.top = '81px';
    } else {
      tabsEl.style.top = '62px';
    }
  }

  setTab(tab) {
    this.currentTab = tab;
  }

  checkActive(tab) {
    return this.currentTab === tab;
  }
  changeToSearch(val){
    this.isSearch = true;
    this.searchString = val;
  }
  cancelSearch(){
    this.isSearch = false;
    this.searchString = '';
  }

  hasPath(rpath: string) {
    if (rpath != null) {
      localStorage.removeItem("returnPath");
      this.router.navigate([rpath], { queryParamsHandling: "preserve" });
    }
  }

}
