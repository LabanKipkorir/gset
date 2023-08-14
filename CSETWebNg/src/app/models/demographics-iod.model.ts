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


export interface DemographicsIod {
    assessmentId?: number;
    organizationType?: string;
    organizationName?: string;
    businessUnit?: string;
    sector?: string;
    subsector?: string;
    numberEmployeesTotal?: string;
    numberEmployeesUnit?: string;
    annualRevenue?: string;
    criticalServiceRevenuePercent?: string;
    numberPeopleServedByCritSvc?: string;
    disruptedSector1?: string;
    disruptedSector2?: string;
    usesStandard?: boolean;
    standard1?: string;
    standard2?: string;
    requiredToComply?: boolean;
    regulationType1?: string;
    reg1Other?: string;
    regulationType2?: string;
    reg2Other?: string;

    /**
     * Lists the orgs 
     */
    shareOrgs?: string[];
    shareOther?: string;

    barrier1?: string;
    barrier2?: string;

    listOrgTypes?: any[];
    listSectors?: any[];
    listSubsectors?: any[];
    listNumberEmployeeTotal?: any[];
    listNumberEmployeeUnit?: any[];
    listRevenueAmounts?: any[];
    listRevenuePercentages?: any[];
    listNumberPeopleServed?: any[];
    listCISectors?: any[];
    listRegulationTypes?: any[];
    listShareOrgs?: any[];
    listBarriers?: any[];
}