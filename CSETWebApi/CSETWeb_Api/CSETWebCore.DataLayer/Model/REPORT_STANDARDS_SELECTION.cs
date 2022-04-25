﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CSETWebCore.DataLayer.Model
{
    public partial class REPORT_STANDARDS_SELECTION
    {
        [Key]
        public int Assesment_Id { get; set; }
        [Key]
        [StringLength(50)]
        public string Report_Set_Entity_Name { get; set; }
        public int Report_Section_Order { get; set; }
        public bool Is_Selected { get; set; }

        [ForeignKey(nameof(Assesment_Id))]
        [InverseProperty(nameof(ASSESSMENTS.REPORT_STANDARDS_SELECTION))]
        public virtual ASSESSMENTS Assesment { get; set; }
        [ForeignKey(nameof(Report_Set_Entity_Name))]
        [InverseProperty(nameof(SETS.REPORT_STANDARDS_SELECTION))]
        public virtual SETS Report_Set_Entity_NameNavigation { get; set; }
    }
}