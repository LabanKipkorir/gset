﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CSETWebCore.DataLayer.Model
{
    /// <summary>
    /// A collection of MATURITY_REFERENCES records
    /// </summary>
    public partial class MATURITY_REFERENCES
    {
        [Key]
        public int Mat_Question_Id { get; set; }
        [Key]
        public int Gen_File_Id { get; set; }
        [Key]
        [StringLength(850)]
        public string Section_Ref { get; set; }
        public int? Page_Number { get; set; }
        [StringLength(2000)]
        public string Destination_String { get; set; }

        [ForeignKey(nameof(Gen_File_Id))]
        [InverseProperty(nameof(GEN_FILE.MATURITY_REFERENCES))]
        public virtual GEN_FILE Gen_File { get; set; }
        [ForeignKey(nameof(Mat_Question_Id))]
        [InverseProperty(nameof(MATURITY_QUESTIONS.MATURITY_REFERENCES))]
        public virtual MATURITY_QUESTIONS Mat_Question { get; set; }
    }
}