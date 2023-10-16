﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CSETWebCore.DataLayer.Model;

/// <summary>
/// A collection of NEW_REQUIREMENT records
/// </summary>
public partial class NEW_REQUIREMENT
{
    [Key]
    public int Requirement_Id { get; set; }

    [StringLength(250)]
    public string Requirement_Title { get; set; }

    [Required]
    public string Requirement_Text { get; set; }

    public string Supplemental_Info { get; set; }

    [StringLength(250)]
    public string Standard_Category { get; set; }

    [StringLength(250)]
    public string Standard_Sub_Category { get; set; }

    public int? Weight { get; set; }

    public string Implementation_Recommendations { get; set; }

    [Required]
    [StringLength(50)]
    public string Original_Set_Name { get; set; }

    [MaxLength(20)]
    public byte[] Text_Hash { get; set; }

    public int? NCSF_Cat_Id { get; set; }

    public int? NCSF_Number { get; set; }

    [MaxLength(32)]
    public byte[] Supp_Hash { get; set; }

    public int? Ranking { get; set; }

    public int Question_Group_Heading_Id { get; set; }

    public string ExaminationApproach { get; set; }

    public int? Old_Id_For_Copy { get; set; }

    [InverseProperty("Requirement")]
    public virtual ICollection<FINANCIAL_REQUIREMENTS> FINANCIAL_REQUIREMENTS { get; set; } = new List<FINANCIAL_REQUIREMENTS>();

    [ForeignKey("NCSF_Cat_Id")]
    [InverseProperty("NEW_REQUIREMENT")]
    public virtual NCSF_CATEGORY NCSF_Cat { get; set; }

    [InverseProperty("Requirement")]
    public virtual ICollection<NERC_RISK_RANKING> NERC_RISK_RANKING { get; set; } = new List<NERC_RISK_RANKING>();

    [ForeignKey("Original_Set_Name")]
    [InverseProperty("NEW_REQUIREMENT")]
    public virtual SETS Original_Set_NameNavigation { get; set; }

    [InverseProperty("Requirement")]
    public virtual ICollection<PARAMETER_REQUIREMENTS> PARAMETER_REQUIREMENTS { get; set; } = new List<PARAMETER_REQUIREMENTS>();

    public virtual QUESTION_GROUP_HEADING Question_Group_Heading { get; set; }

    [InverseProperty("Requirement")]
    public virtual ICollection<REQUIREMENT_LEVELS> REQUIREMENT_LEVELS { get; set; } = new List<REQUIREMENT_LEVELS>();

    [InverseProperty("Requirement")]
    public virtual ICollection<REQUIREMENT_QUESTIONS> REQUIREMENT_QUESTIONS { get; set; } = new List<REQUIREMENT_QUESTIONS>();

    [InverseProperty("Requirement")]
    public virtual ICollection<REQUIREMENT_QUESTIONS_SETS> REQUIREMENT_QUESTIONS_SETS { get; set; } = new List<REQUIREMENT_QUESTIONS_SETS>();

    [InverseProperty("Requirement")]
    public virtual ICollection<REQUIREMENT_REFERENCES> REQUIREMENT_REFERENCES { get; set; } = new List<REQUIREMENT_REFERENCES>();

    [InverseProperty("Requirement")]
    public virtual ICollection<REQUIREMENT_SETS> REQUIREMENT_SETS { get; set; } = new List<REQUIREMENT_SETS>();

    [InverseProperty("Requirement")]
    public virtual ICollection<REQUIREMENT_SOURCE_FILES> REQUIREMENT_SOURCE_FILES { get; set; } = new List<REQUIREMENT_SOURCE_FILES>();

    [ForeignKey("Standard_Category")]
    [InverseProperty("NEW_REQUIREMENT")]
    public virtual STANDARD_CATEGORY Standard_CategoryNavigation { get; set; }
}