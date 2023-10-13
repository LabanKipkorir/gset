﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CSETWebCore.DataLayer.Model;

public partial class MODES_SETS_MATURITY_MODELS
{
    [Key]
    public int App_Code_Id { get; set; }

    [Required]
    [StringLength(50)]
    public string AppCode { get; set; }

    [StringLength(50)]
    public string Set_Name { get; set; }

    [StringLength(100)]
    public string Model_Name { get; set; }

    public bool? Is_Included { get; set; }

    [ForeignKey("AppCode")]
    [InverseProperty("MODES_SETS_MATURITY_MODELS")]
    public virtual APP_CODE AppCodeNavigation { get; set; }

    public virtual MATURITY_MODELS Model_NameNavigation { get; set; }

    [ForeignKey("Set_Name")]
    [InverseProperty("MODES_SETS_MATURITY_MODELS")]
    public virtual SETS Set_NameNavigation { get; set; }
}