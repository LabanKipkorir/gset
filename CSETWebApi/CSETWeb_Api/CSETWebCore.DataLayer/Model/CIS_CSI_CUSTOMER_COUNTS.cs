﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CSETWebCore.DataLayer.Model;

public partial class CIS_CSI_CUSTOMER_COUNTS
{
    [Key]
    [StringLength(50)]
    public string Customer_Count { get; set; }

    public int Sequence { get; set; }

    [InverseProperty("Customers_CountNavigation")]
    public virtual ICollection<CIS_CSI_SERVICE_DEMOGRAPHICS> CIS_CSI_SERVICE_DEMOGRAPHICS { get; set; } = new List<CIS_CSI_SERVICE_DEMOGRAPHICS>();
}