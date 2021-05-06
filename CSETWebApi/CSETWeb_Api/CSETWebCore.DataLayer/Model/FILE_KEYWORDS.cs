﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSETWebCore.DataLayer
{
    /// <summary>
    /// A collection of FILE_KEYWORDS records
    /// </summary>
    public partial class FILE_KEYWORDS
    {
        /// <summary>
        /// The Gen File Id is used to
        /// </summary>
        [Key]
        public int Gen_File_Id { get; set; }
        /// <summary>
        /// The Keyword is used to
        /// </summary>
        [Key]
        [StringLength(60)]
        public string Keyword { get; set; }

        [ForeignKey(nameof(Gen_File_Id))]
        [InverseProperty(nameof(GEN_FILE.FILE_KEYWORDS))]
        public virtual GEN_FILE Gen_File_ { get; set; }
    }
}