﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CSETWebCore.DataLayer.Model;

[PrimaryKey("Assessment_Id", "Aggregation_Id")]
public partial class AGGREGATION_ASSESSMENT
{
    [Key]
    public int Assessment_Id { get; set; }

    [Key]
    public int Aggregation_Id { get; set; }

    public int? Sequence { get; set; }

    [StringLength(50)]
    public string Alias { get; set; }

    [ForeignKey("Aggregation_Id")]
    [InverseProperty("AGGREGATION_ASSESSMENT")]
    public virtual AGGREGATION_INFORMATION Aggregation { get; set; }

    [ForeignKey("Assessment_Id")]
    [InverseProperty("AGGREGATION_ASSESSMENT")]
    public virtual ASSESSMENTS Assessment { get; set; }
}