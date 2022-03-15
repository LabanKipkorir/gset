﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CSETWebCore.DataLayer.Model
{
    public partial class CIST_CSI_SERVICE_DEMOGRAPHICS
    {
        [Key]
        public int Assessment_Id { get; set; }
        [StringLength(50)]
        public string Critical_Service_Name { get; set; }
        [StringLength(50)]
        public string Critical_Service_Description { get; set; }
        [StringLength(50)]
        public string IT_ICS_Name { get; set; }
        public bool Multi_Site { get; set; }
        [StringLength(50)]
        public string Multi_Site_Description { get; set; }
        [StringLength(50)]
        public string Budget_Basis { get; set; }
        [StringLength(25)]
        public string Authorized_Organizational_User_Count { get; set; }
        [StringLength(25)]
        public string Authorized_Non_Organizational_User_Count { get; set; }
        [StringLength(25)]
        public string Customers_Count { get; set; }

        [ForeignKey(nameof(Assessment_Id))]
        [InverseProperty(nameof(ASSESSMENTS.CIST_CSI_SERVICE_DEMOGRAPHICS))]
        public virtual ASSESSMENTS Assessment { get; set; }
        [ForeignKey(nameof(Authorized_Non_Organizational_User_Count))]
        [InverseProperty(nameof(CIST_CSI_USER_COUNTS.CIST_CSI_SERVICE_DEMOGRAPHICS))]
        public virtual CIST_CSI_USER_COUNTS Authorized_Non_Organizational_User_CountNavigation { get; set; }
        [ForeignKey(nameof(Budget_Basis))]
        [InverseProperty(nameof(CIST_CSI_BUDGET_BASES.CIST_CSI_SERVICE_DEMOGRAPHICS))]
        public virtual CIST_CSI_BUDGET_BASES Budget_BasisNavigation { get; set; }
        [ForeignKey(nameof(Customers_Count))]
        [InverseProperty(nameof(CIST_CSI_CUSTOMER_COUNTS.CIST_CSI_SERVICE_DEMOGRAPHICS))]
        public virtual CIST_CSI_CUSTOMER_COUNTS Customers_CountNavigation { get; set; }
    }
}