﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSETWebCore.DataLayer.Model
{
    public partial class usp_getComponentsSummaryResult
    {
        public string Answer_Text { get; set; }
        public string Answer_Full_Name { get; set; }
        public int vcount { get; set; }
        [Column(TypeName = "decimal(18,1)")]
        public decimal value { get; set; }
    }
}
