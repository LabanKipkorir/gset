﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSETWebCore.DataLayer
{
    /// <summary>
    /// A collection of SECURITY_QUESTION records
    /// </summary>
    public partial class SECURITY_QUESTION
    {
        [Key]
        public int SecurityQuestionId { get; set; }
        [Required]
        [StringLength(500)]
        public string SecurityQuestion { get; set; }
        [Required]
        public bool? IsCustomQuestion { get; set; }
    }
}