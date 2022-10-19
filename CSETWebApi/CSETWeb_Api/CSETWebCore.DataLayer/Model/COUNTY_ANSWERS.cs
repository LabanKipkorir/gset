﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CSETWebCore.DataLayer.Model
{
    public partial class COUNTY_ANSWERS
    {
        [Key]
        public int Assessment_Id { get; set; }
        [Key]
        [StringLength(10)]
        public string County_FIPS { get; set; }

        [ForeignKey("Assessment_Id")]
        [InverseProperty("COUNTY_ANSWERS")]
        public virtual ASSESSMENTS Assessment { get; set; }
        [ForeignKey("County_FIPS")]
        [InverseProperty("COUNTY_ANSWERS")]
        public virtual COUNTIES County_FIPSNavigation { get; set; }
    }
}