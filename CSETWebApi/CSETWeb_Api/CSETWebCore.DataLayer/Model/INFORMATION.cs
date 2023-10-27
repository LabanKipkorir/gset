﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CSETWebCore.DataLayer.Model;

/// <summary>
/// A collection of INFORMATION records
/// </summary>
public partial class INFORMATION
{
    /// <summary>
    /// The Id is used to
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// The Assessment Name is used to
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Assessment_Name { get; set; }

    /// <summary>
    /// The Facility Name is used to
    /// </summary>
    [StringLength(100)]
    public string Facility_Name { get; set; }

    /// <summary>
    /// The City Or Site Name is used to
    /// </summary>
    [StringLength(100)]
    public string City_Or_Site_Name { get; set; }

    /// <summary>
    /// The State Province Or Region is used to
    /// </summary>
    [StringLength(100)]
    public string State_Province_Or_Region { get; set; }

    /// <summary>
    /// The Assessor Name is used to
    /// </summary>
    [StringLength(100)]
    public string Assessor_Name { get; set; }

    /// <summary>
    /// The Assessor Email is used to
    /// </summary>
    [StringLength(100)]
    public string Assessor_Email { get; set; }

    /// <summary>
    /// The Assessor Phone is used to
    /// </summary>
    [StringLength(100)]
    public string Assessor_Phone { get; set; }

    /// <summary>
    /// The Assessment Description is used to
    /// </summary>
    [StringLength(4000)]
    public string Assessment_Description { get; set; }

    /// <summary>
    /// The Additional Notes And Comments is used to
    /// </summary>
    [StringLength(4000)]
    public string Additional_Notes_And_Comments { get; set; }

    /// <summary>
    /// The Additional Contacts is used to
    /// </summary>
    [Column(TypeName = "ntext")]
    public string Additional_Contacts { get; set; }

    /// <summary>
    /// The Executive Summary is used to
    /// </summary>
    [Column(TypeName = "ntext")]
    public string Executive_Summary { get; set; }

    /// <summary>
    /// The Enterprise Evaluation Summary is used to
    /// </summary>
    [Column(TypeName = "ntext")]
    public string Enterprise_Evaluation_Summary { get; set; }

    [StringLength(70)]
    public string Real_Property_Unique_Id { get; set; }

    public int? eMass_Document_Id { get; set; }

    public bool? IsAcetOnly { get; set; }

    [StringLength(30)]
    public string Workflow { get; set; }

    public int? Baseline_Assessment_Id { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Origin { get; set; }

    [StringLength(20)]
    public string Postal_Code { get; set; }

    public int? Region_Code { get; set; }

    public bool? Ise_Submitted { get; set; }
    public DateTime? Submitted_Date { get; set; }

    [ForeignKey("Id")]
    [InverseProperty("INFORMATION")]
    public virtual ASSESSMENTS IdNavigation { get; set; }

    [ForeignKey("eMass_Document_Id")]
    [InverseProperty("INFORMATION")]
    public virtual DOCUMENT_FILE eMass_Document { get; set; }
}