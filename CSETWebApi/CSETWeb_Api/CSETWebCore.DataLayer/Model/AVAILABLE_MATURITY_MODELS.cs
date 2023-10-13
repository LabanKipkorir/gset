﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CSETWebCore.DataLayer.Model;

/// <summary>
/// A collection of AVAILABLE_MATURITY_MODELS records
/// </summary>
[PrimaryKey("Assessment_Id", "model_id")]
public partial class AVAILABLE_MATURITY_MODELS
{
    [Key]
    public int Assessment_Id { get; set; }

    public bool Selected { get; set; }

    [Key]
    public int model_id { get; set; }

    [ForeignKey("Assessment_Id")]
    [InverseProperty("AVAILABLE_MATURITY_MODELS")]
    public virtual ASSESSMENTS Assessment { get; set; }

    [ForeignKey("model_id")]
    [InverseProperty("AVAILABLE_MATURITY_MODELS")]
    public virtual MATURITY_MODELS model { get; set; }
}