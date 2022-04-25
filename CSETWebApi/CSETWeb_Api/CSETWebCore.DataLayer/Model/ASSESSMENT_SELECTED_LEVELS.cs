﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CSETWebCore.DataLayer.Model
{
    /// <summary>
    /// A collection of ASSESSMENT_SELECTED_LEVELS records
    /// </summary>
    public partial class ASSESSMENT_SELECTED_LEVELS
    {
        /// <summary>
        /// The Id is used to
        /// </summary>
        [Key]
        public int Assessment_Id { get; set; }
        /// <summary>
        /// The Level Name is used to
        /// </summary>
        [Key]
        [StringLength(50)]
        public string Level_Name { get; set; }
        /// <summary>
        /// The Standard Specific Sal Level is used to
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Standard_Specific_Sal_Level { get; set; }

        [ForeignKey(nameof(Assessment_Id))]
        [InverseProperty(nameof(STANDARD_SELECTION.ASSESSMENT_SELECTED_LEVELS))]
        public virtual STANDARD_SELECTION Assessment { get; set; }
        [ForeignKey(nameof(Level_Name))]
        [InverseProperty(nameof(LEVEL_NAMES.ASSESSMENT_SELECTED_LEVELS))]
        public virtual LEVEL_NAMES Level_NameNavigation { get; set; }
    }
}