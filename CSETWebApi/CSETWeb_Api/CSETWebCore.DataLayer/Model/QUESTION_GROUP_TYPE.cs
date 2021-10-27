﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CSETWebCore.DataLayer.Model
{
    /// <summary>
    /// A collection of QUESTION_GROUP_TYPE records
    /// </summary>
    [Index(nameof(Group_Name), Name = "IX_QUESTION_GROUP_TYPE", IsUnique = true)]
    public partial class QUESTION_GROUP_TYPE
    {
        /// <summary>
        /// The Question Group Id is used to
        /// </summary>
        [Key]
        public int Question_Group_Id { get; set; }
        /// <summary>
        /// The Group Name is used to
        /// </summary>
        [Required]
        [StringLength(10)]
        public string Group_Name { get; set; }
        /// <summary>
        /// The Scoring Group is used to
        /// </summary>
        [Required]
        [StringLength(10)]
        public string Scoring_Group { get; set; }
        /// <summary>
        /// The Scoring Type is used to
        /// </summary>
        [Required]
        [StringLength(10)]
        public string Scoring_Type { get; set; }
        /// <summary>
        /// The Group Header is used to
        /// </summary>
        [StringLength(2000)]
        public string Group_Header { get; set; }
    }
}