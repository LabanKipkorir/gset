﻿using CSETWebCore.DataLayer.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CSETWebCore.Model.Demographic
{
    public class DemographicIod
    {
        public int AssessmentId { get; set; }

        public string OrganizationType { get; set; }
        public string NumberEmployees { get; set; }
        public string AnnualRevenue { get; set; }

        public string CriticalServiceRevenuePercent { get; set; }

        public string NumberPeopleServedByCritSvc { get; set; }

        public string DisruptedSector1 { get; set; }
        public string DisruptedSector2 { get; set; }


        /// <summary>
        /// Does your organization use a body of practice, industry
        /// standard, or similar resource to support or inform its
        /// cybersecurity efforts?
        /// </summary>
        public bool StandardUsed { get; set; }
        public string Standard1 { get; set; }
        public string Standard2 { get; set; }


        /// <summary>
        /// Is your organization required to comply with mandatory,
        /// cybersecurity-related regulation?
        /// </summary>
        public bool RegulationRequired { get; set; }
        public string RegulationType1 { get; set; }
        public string Reg1Other { get; set; }
        public string RegulationType2 { get; set; }
        public string Reg2Other { get; set; }


        /// <summary>
        /// The following fields indicate with whom the organization
        /// shares with or obtains cybersecurity-related information from.
        /// </summary>
        public bool ShareISAC { get; set; }
        public bool ShareFBI { get; set; }
        public bool ShareCyberConsultants { get; set; }
        public bool ShareDHS { get; set; }
        public bool ShareLocalGovt { get; set; }
        public bool ShareNCFTA { get; set; }
        public string ShareOther { get; set; }


        public string Barrier1 { get; set; }
        public string Barrier2 { get; set; }
    }
}
