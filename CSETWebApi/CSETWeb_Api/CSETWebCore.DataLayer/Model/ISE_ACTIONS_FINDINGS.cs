﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CSETWebCore.DataLayer.Model
{
    public partial class ISE_ACTIONS_FINDINGS
    {
        [Key]
        public int Finding_Id { get; set; }
        [Key]
        public int Mat_Question_Id { get; set; }
        [Required]
        [StringLength(1000)]
        public string Action_Items_Override { get; set; }

        [ForeignKey("Finding_Id")]
        [InverseProperty("ISE_ACTIONS_FINDINGS")]
        public virtual FINDING Finding { get; set; }
        [ForeignKey("Mat_Question_Id")]
        [InverseProperty("ISE_ACTIONS_FINDINGS")]
        public virtual ISE_ACTIONS Mat_Question { get; set; }
    }
}