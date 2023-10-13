﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CSETWebCore.DataLayer.Model;

/// <summary>
/// A collection of GLOBAL_PROPERTIES records
/// </summary>
public partial class GLOBAL_PROPERTIES
{
    /// <summary>
    /// The Property is used to
    /// </summary>
    [Key]
    [StringLength(50)]
    public string Property { get; set; }

    /// <summary>
    /// The Property Value is used to
    /// </summary>
    public string Property_Value { get; set; }
}