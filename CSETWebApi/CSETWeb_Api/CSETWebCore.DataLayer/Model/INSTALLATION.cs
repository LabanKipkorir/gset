﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CSETWebCore.DataLayer.Model
{
    /// <summary>
    /// A collection of INSTALLATION records
    /// </summary>
    public partial class INSTALLATION
    {
        [StringLength(200)]
        public string JWT_Secret { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Generated_UTC { get; set; }
        [Key]
        [StringLength(200)]
        public string Installation_ID { get; set; }
    }
}