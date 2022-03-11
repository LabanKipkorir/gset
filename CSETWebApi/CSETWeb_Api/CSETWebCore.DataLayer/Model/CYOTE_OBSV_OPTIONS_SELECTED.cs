﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CSETWebCore.DataLayer.Model
{
    public partial class CYOTE_OBSV_OPTIONS_SELECTED
    {
        [Key]
        public int Observable_Id { get; set; }
        [Key]
        [StringLength(50)]
        public string Option_Name { get; set; }
        [StringLength(300)]
        public string Option_Value { get; set; }

        [ForeignKey(nameof(Observable_Id))]
        [InverseProperty(nameof(CYOTE_OBSERVABLES.CYOTE_OBSV_OPTIONS_SELECTED))]
        public virtual CYOTE_OBSERVABLES Observable { get; set; }
        [ForeignKey(nameof(Option_Name))]
        [InverseProperty(nameof(CYOTE_OBSV_OPTIONS.CYOTE_OBSV_OPTIONS_SELECTED))]
        public virtual CYOTE_OBSV_OPTIONS Option_NameNavigation { get; set; }
    }
}