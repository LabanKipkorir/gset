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
import { map } from 'rxjs/operators';
import { timer, Observable } from 'rxjs';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { ComponentFactoryResolver, Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';


import { JwtParser } from '../helpers/jwt-parser';
import { ChangePassword } from '../models/reset-pass.model';
import { CreateUser } from './../models/user.model';
import { ConfigService } from './config.service';
import { environment } from '../../environments/environment';
import { parse } from 'querystring';

export interface LoginResponse {
    token: string;
    resetRequired: boolean;
    isSuperUser: boolean;
    userLastName: string;
    userFirstName: string;
    userId: number;
    email: string;
    exportExtension: string;
    importExtensions: string;
    linkerTime: string;
}

const headers = {
    headers: new HttpHeaders()
        .set('Content-Type', 'application/json'),
    params: new HttpParams()
};

@Injectable()
export class AuthenticationService {
    isLocal: boolean;
    private initialized = false;
    private parser = new JwtParser();

    constructor(private http: HttpClient, private router: Router, private configSvc: ConfigService, public dialog: MatDialog) {
        if (!this.initialized) {
            this.initialized = true;

        }
    }


    checkLocal() {
        return this.http.post(this.configSvc.apiUrl + 'auth/login/standalone',
            JSON.stringify(
                {
                    TzOffset: new Date().getTimezoneOffset().toString(),
                    // If InstallationMode isn't empty, use it.  Otherwise default to environment.appCode
                    Scope: (this.configSvc.installationMode || '') !== '' ? this.configSvc.installationMode : environment.appCode
                }
            ), headers)
            .toPromise().then(
                (response: LoginResponse) => {

                    if (response?.email === null || response?.email === undefined) {
                        this.isLocal = false;
                    } else {
                        this.isLocal = true;
                        this.storeUserData(response);
                    }

                    localStorage.setItem('cset.isLocal', (this.isLocal + ''));

                    localStorage.setItem('cset.linkerDate', response?.linkerTime);
                },
                error => {
                    console.warn('Error getting stand-alone status. Assuming non-stand-alone mode.');
                    this.isLocal = false;
                });
    }

    /**
     * Calls the API to find out whether this is a local install
     */
    checkLocalInstallStatus() {
        return this.http.get(this.configSvc.apiUrl + 'auth/islocal', headers);
    }

    /**
     *
     * @param user
     */
    storeUserData(user: LoginResponse) {
        // if (localStorage.getItem('userToken') != null) {

        //     console.log("userToken is not being reset");
        //     console.log(localStorage.getItem('userToken'));
        //     console.log("user token is:")
        //     console.log(user.token);
        //     let uid = parseInt(localStorage.getItem("userId"));
        //     let uuid = parseInt(this.parser.decodeToken(localStorage.getItem('userToken')).userid);
        //     console.log("localStorage:"+uid+" TokenID:"+uuid);
        //     console.log(uuid);

        //     // if(isNaN(uid)||isNaN(uuid))
        //     // {
        //     //     console.log("really skipping the swapping of tokens");
        //     // }else if(uid!=uuid){
        //     //     localStorage.setItem('userToken', user.token);
        //     // }
        // }
        // else
        if (user.token != null) {
            localStorage.setItem('userToken', user.token);
        }
        localStorage.setItem('firstName', user.userFirstName);
        localStorage.setItem('lastName', user.userLastName);
        localStorage.setItem('superUser', '' + user.isSuperUser);
        localStorage.setItem('userId', '' + user.userId);
        localStorage.setItem('email', user.email);
        localStorage.setItem('exportExtension', user.exportExtension);
        localStorage.setItem('importExtensions', user.importExtensions)
        localStorage.setItem('developer', String(false));


        // schedule the first token refresh event
        this.scheduleTokenRefresh(this.http, user.token);
    }

    /**
     *
     * @param email
     * @param password
     */
    login(email: string, password: string) {
        localStorage.clear();
        localStorage.setItem('email', email);

        // set the scope (application)
        let scope: string;

        switch (this.configSvc.installationMode || '') {
            case 'ACET':
                scope = 'ACET';
                break;
            case 'TSA':
                scope = 'TSA';
                break;
            case 'RRA':
                scope = 'RRA';
                break;
            default:
                scope = environment.appCode
        }

        return this.http.post(this.configSvc.apiUrl + 'auth/login',
            JSON.stringify(
                {
                    Email: email,
                    Password: password,
                    TzOffset: new Date().getTimezoneOffset().toString(),
                    Scope: scope
                }
            ), headers).pipe(
                map((user: LoginResponse) => {
                    // store user details and jwt token in local storage to keep user logged in between page refreshes
                    this.storeUserData(user);

                    return user;
                }));
    }


    logout() {
        // remove user from session storage to log user out
        localStorage.clear();
        this.router.navigate(['/home/login'], { queryParamsHandling: "preserve" });
    }


    /**
      * Schedules an HTTP transaction to refresh the JWT.
      * @param http The current HttpClient instance.
      * @param token A JWT string.
      */
    scheduleTokenRefresh(http: HttpClient, token: string) {
        const refresh = timer(this.calcTokenRefreshTimeout(token));
        refresh.subscribe(
            val => {
                // only schedule a refresh if the user is currently logged on
                if (localStorage.getItem('userToken') != null) {

                    http.get(this.configSvc.apiUrl + 'auth/token?refresh')
                        .subscribe((resp: LoginResponse) => {
                            localStorage.removeItem('userToken');
                            localStorage.setItem('userToken', resp.token);

                            // schedule the next refresh
                            this.scheduleTokenRefresh(this.http, resp.token);
                        }, error => {
                            console.log(<Error>error.message);
                        });
                }
            });
    }


    /**
      * Returns the timeout interval in milliseconds
      * @param token A JWT string.
      */
    calcTokenRefreshTimeout(token: string): number {
        // extract the expiration timestamp from the token
        const jwt = new JwtParser();
        const parsedToken = jwt.decodeToken(token);
        const expTimeUnix = parsedToken.exp;

        const nowUtcUnix = Math.floor((new Date()).getTime() / 1000);

        // how many seconds from now until expiry?
        const secondsUntilExpiration = expTimeUnix - nowUtcUnix;

        // refresh at 60 seconds lead time before expiry
        const leadSeconds = 60;
        const refreshIntervalMs = (secondsUntilExpiration - leadSeconds) * 1000;

        return refreshIntervalMs;
    }

    /**
     * Requests a JWT with a short lifespan.
     */
    getShortLivedToken() {
        return this.http.get(this.configSvc.apiUrl + 'auth/token?expSeconds=30000');
    }

    getShortLivedTokenForAssessment(assessment_id: number) {
        return this.http.get(this.configSvc.apiUrl + 'auth/token?assessmentId=' + assessment_id + '&expSeconds=30000');
    }

    changePassword(data: ChangePassword) {
        return this.http.post(this.configSvc.apiUrl + 'ResetPassword/ChangePassword', JSON.stringify(data), { 'headers': headers.headers, params: headers.params, responseType: 'text' });
    }

    updateUser(data: CreateUser): Observable<CreateUser> {
        return this.http.post(this.configSvc.apiUrl + 'contacts/UpdateUser', data, headers);
    }

    getUserInfo(): Observable<CreateUser> {
        return this.http.get(this.configSvc.apiUrl + 'contacts/GetUserInfo');
    }

    passwordStatus() {
        return this.http.get(this.configSvc.apiUrl + 'ResetPassword/ResetPasswordStatus/', headers);
    }

    getSecurityQuestionsList(email: string) {
        return this.http.get(this.configSvc.apiUrl + 'ResetPassword/SecurityQuestions?email=' + email + '&appCode=' + environment.appCode);
    }

    getSecurityQuestionsPotentialList() {
        return this.http.get(this.configSvc.apiUrl + 'ResetPassword/PotentialQuestions');
    }

    userToken() {
        return localStorage.getItem('userToken');
    }

    userId(): number {
        return parseInt(localStorage.getItem('userId'), 10);
    }

    email() {
        return localStorage.getItem('email');
    }

    firstName() {
        return localStorage.getItem('firstName');
    }

    lastName() {
        return localStorage.getItem('lastName');
    }

    setUserInfo(info: CreateUser) {
        localStorage.setItem('firstName', info.firstName);
        localStorage.setItem('lastName', info.lastName);
        localStorage.setItem('email', info.primaryEmail);
    }

}
