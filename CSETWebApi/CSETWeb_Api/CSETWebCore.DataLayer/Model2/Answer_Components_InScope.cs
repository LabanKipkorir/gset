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
    public partial class Answer_Components_InScope
    {
        public int Assessment_Id { get; set; }
        public int Answer_Id { get; set; }
        public int Question_Or_Requirement_Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Answer_Text { get; set; }
        [StringLength(1000)]
        public string Comment { get; set; }
        [StringLength(1000)]
        public string Alternate_Justification { get; set; }
        public int? Question_Number { get; set; }
        [StringLength(7338)]
        public string QuestionText { get; set; }
        [StringLength(200)]
        public string ComponentName { get; set; }
        public int Component_Symbol_Id { get; set; }
        public int? Layer_Id { get; set; }
        [StringLength(250)]
        public string LayerName { get; set; }
        public int? Container_Id { get; set; }
        [StringLength(250)]
        public string ZoneName { get; set; }
        [StringLength(10)]
        public string SAL { get; set; }
        public bool Is_Component { get; set; }
        public Guid Component_Guid { get; set; }
        [StringLength(50)]
        public string Custom_Question_Guid { get; set; }
        public int? Old_Answer_Id { get; set; }
        public bool Reviewed { get; set; }
        public bool? Mark_For_Review { get; set; }
        public bool Is_Requirement { get; set; }
        public bool Is_Framework { get; set; }
    }
}