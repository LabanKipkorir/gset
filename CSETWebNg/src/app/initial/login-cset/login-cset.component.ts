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
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { AlertComponent } from '../../dialogs/alert/alert.component';
import { EjectionComponent } from '../../dialogs/ejection/ejection.component';
import { AssessmentService } from '../../services/assessment.service';
import { AuthenticationService } from '../../services/authentication.service';
import { ConfigService } from '../../services/config.service';
import { EmailService } from '../../services/email.service';
import { JwtParser } from '../../helpers/jwt-parser';
import { LoginService } from '../../services/login.service';

@Component({
  selector: 'app-login-cset',
  templateUrl: './login-cset.component.html',
  styleUrls: ['./login-cset.component.scss']
})
export class LoginCsetComponent implements OnInit {

  /**
   * The current display mode of the page -- LOGIN or SIGNUP
   */
  
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private configSvc: ConfigService,
    private authenticationService: AuthenticationService,
    private emailSvc: EmailService,
    private assessSvc: AssessmentService,
    private dialog: MatDialog,
    public loginSvc: LoginService
  ) {}

  ngOnInit() {
    this.loginSvc.SetupServiceForLoginTransaction(
      this.route,
      this.router,
      this.configSvc,
      this.authenticationService,
      this.emailSvc,
      this.assessSvc,
      this.dialog,
    )
  }
  
}
