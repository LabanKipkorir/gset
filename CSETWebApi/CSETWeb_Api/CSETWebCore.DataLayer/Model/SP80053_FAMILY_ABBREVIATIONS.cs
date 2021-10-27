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
    /// A collection of SP80053_FAMILY_ABBREVIATIONS records
    /// </summary>
    public partial class SP80053_FAMILY_ABBREVIATIONS
    {
        [Key]
        [StringLength(2)]
        public string ID { get; set; }
        [Required]
        [StringLength(250)]
        public string Standard_Category { get; set; }
        public int Standard_Order { get; set; }
    }
}