﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSETWebCore.DataLayer
{
    /// <summary>
    /// A collection of MATURITY_DOMAIN_REMARKS records
    /// </summary>
    public partial class MATURITY_DOMAIN_REMARKS
    {
        [Key]
        public int Grouping_ID { get; set; }
        [Key]
        public int Assessment_Id { get; set; }
        [Required]
        [StringLength(2048)]
        public string DomainRemarks { get; set; }

        [ForeignKey(nameof(Assessment_Id))]
        [InverseProperty(nameof(ASSESSMENTS.MATURITY_DOMAIN_REMARKS))]
        public virtual ASSESSMENTS Assessment_ { get; set; }
        [ForeignKey(nameof(Grouping_ID))]
        [InverseProperty(nameof(MATURITY_GROUPINGS.MATURITY_DOMAIN_REMARKS))]
        public virtual MATURITY_GROUPINGS Grouping_ { get; set; }
    }
}