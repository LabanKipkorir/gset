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
    /// A collection of MATURITY_QUESTIONS records
    /// </summary>
    public partial class MATURITY_QUESTIONS
    {
        public MATURITY_QUESTIONS()
        {
            InverseParent_Question = new HashSet<MATURITY_QUESTIONS>();
            MATURITY_ANSWER_OPTIONS = new HashSet<MATURITY_ANSWER_OPTIONS>();
            MATURITY_REFERENCES = new HashSet<MATURITY_REFERENCES>();
            MATURITY_REFERENCE_TEXT = new HashSet<MATURITY_REFERENCE_TEXT>();
            MATURITY_SOURCE_FILES = new HashSet<MATURITY_SOURCE_FILES>();
        }

        [Key]
        public int Mat_Question_Id { get; set; }
        [StringLength(250)]
        public string Question_Title { get; set; }
        [Required]
        public string Question_Text { get; set; }
        public string Supplemental_Info { get; set; }
        [StringLength(250)]
        public string Category { get; set; }
        [StringLength(250)]
        public string Sub_Category { get; set; }
        public int Maturity_Level_Id { get; set; }
        public int Sequence { get; set; }
        [MaxLength(20)]
        public byte[] Text_Hash { get; set; }
        public int Maturity_Model_Id { get; set; }
        public int? Parent_Question_Id { get; set; }
        public int? Ranking { get; set; }
        public int? Grouping_Id { get; set; }
        public string Examination_Approach { get; set; }
        [StringLength(80)]
        public string Short_Name { get; set; }
        [StringLength(50)]
        public string Mat_Question_Type { get; set; }
        public int? Parent_Option_Id { get; set; }

        [ForeignKey("Grouping_Id")]
        [InverseProperty("MATURITY_QUESTIONS")]
        public virtual MATURITY_GROUPINGS Grouping { get; set; }
        [ForeignKey("Mat_Question_Type")]
        [InverseProperty("MATURITY_QUESTIONS")]
        public virtual MATURITY_QUESTION_TYPES Mat_Question_TypeNavigation { get; set; }
        [ForeignKey("Maturity_Level_Id")]
        [InverseProperty("MATURITY_QUESTIONS")]
        public virtual MATURITY_LEVELS Maturity_LevelNavigation { get; set; }
        [ForeignKey("Maturity_Model_Id")]
        [InverseProperty("MATURITY_QUESTIONS")]
        public virtual MATURITY_MODELS Maturity_Model { get; set; }
        [ForeignKey("Parent_Option_Id")]
        [InverseProperty("MATURITY_QUESTIONS")]
        public virtual MATURITY_ANSWER_OPTIONS Parent_Option { get; set; }
        [ForeignKey("Parent_Question_Id")]
        [InverseProperty("InverseParent_Question")]
        public virtual MATURITY_QUESTIONS Parent_Question { get; set; }
        [InverseProperty("Question")]
        public virtual ISE_ACTIONS ISE_ACTIONS { get; set; }
        [InverseProperty("Parent_Question")]
        public virtual ICollection<MATURITY_QUESTIONS> InverseParent_Question { get; set; }
        [InverseProperty("Mat_Question")]
        public virtual ICollection<MATURITY_ANSWER_OPTIONS> MATURITY_ANSWER_OPTIONS { get; set; }
        [InverseProperty("Mat_Question")]
        public virtual ICollection<MATURITY_REFERENCES> MATURITY_REFERENCES { get; set; }
        [InverseProperty("Mat_Question")]
        public virtual ICollection<MATURITY_REFERENCE_TEXT> MATURITY_REFERENCE_TEXT { get; set; }
        [InverseProperty("Mat_Question")]
        public virtual ICollection<MATURITY_SOURCE_FILES> MATURITY_SOURCE_FILES { get; set; }
    }
}