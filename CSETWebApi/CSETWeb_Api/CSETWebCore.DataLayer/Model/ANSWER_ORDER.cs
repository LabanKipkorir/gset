﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CSETWebCore.DataLayer.Model
{
    public partial class ANSWER_ORDER
    {
        [Key]
        [StringLength(50)]
        public string Answer_Text { get; set; }
        [Column("answer_order")]
        public int answer_order1 { get; set; }
    }
}