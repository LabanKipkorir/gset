﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CSETWebCore.DataLayer.Model2
{
    [Keyless]
    public partial class vParameters
    {
        public int Parameter_ID { get; set; }
        [Required]
        [StringLength(350)]
        public string Parameter_Name { get; set; }
        [Required]
        [StringLength(2000)]
        public string Default_Value { get; set; }
        public int? Assessment_ID { get; set; }
    }
}