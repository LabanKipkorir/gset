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
    /// A collection of SETS records
    /// </summary>
    public partial class SETS
    {
        public SETS()
        {
            AVAILABLE_STANDARDS = new HashSet<AVAILABLE_STANDARDS>();
            CUSTOM_STANDARD_BASE_STANDARDBase_StandardNavigation = new HashSet<CUSTOM_STANDARD_BASE_STANDARD>();
            CUSTOM_STANDARD_BASE_STANDARDCustom_Questionaire_NameNavigation = new HashSet<CUSTOM_STANDARD_BASE_STANDARD>();
            MODES_SETS_MATURITY_MODELS = new HashSet<MODES_SETS_MATURITY_MODELS>();
            NEW_QUESTION = new HashSet<NEW_QUESTION>();
            NEW_QUESTION_SETS = new HashSet<NEW_QUESTION_SETS>();
            NEW_REQUIREMENT = new HashSet<NEW_REQUIREMENT>();
            REPORT_STANDARDS_SELECTION = new HashSet<REPORT_STANDARDS_SELECTION>();
            REQUIREMENT_QUESTIONS_SETS = new HashSet<REQUIREMENT_QUESTIONS_SETS>();
            REQUIREMENT_SETS = new HashSet<REQUIREMENT_SETS>();
            SECTOR_STANDARD_RECOMMENDATIONS = new HashSet<SECTOR_STANDARD_RECOMMENDATIONS>();
            SET_FILES = new HashSet<SET_FILES>();
            STANDARD_CATEGORY_SEQUENCE = new HashSet<STANDARD_CATEGORY_SEQUENCE>();
            STANDARD_SOURCE_FILE = new HashSet<STANDARD_SOURCE_FILE>();
            UNIVERSAL_SUB_CATEGORY_HEADINGS = new HashSet<UNIVERSAL_SUB_CATEGORY_HEADINGS>();
        }

        /// <summary>
        /// The Set Name is used to
        /// </summary>
        [Key]
        [StringLength(50)]
        [Unicode(false)]
        public string Set_Name { get; set; }
        /// <summary>
        /// The Full Name is used to
        /// </summary>
        [Required]
        [StringLength(250)]
        [Unicode(false)]
        public string Full_Name { get; set; }
        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string Short_Name { get; set; }
        [Required]
        public bool? Is_Displayed { get; set; }
        /// <summary>
        /// The Is Pass Fail is used to
        /// </summary>
        public bool Is_Pass_Fail { get; set; }
        /// <summary>
        /// The Old Std Name is used to
        /// </summary>
        [StringLength(50)]
        [Unicode(false)]
        public string Old_Std_Name { get; set; }
        public int? Set_Category_Id { get; set; }
        public int? Order_In_Category { get; set; }
        public int? Report_Order_Section_Number { get; set; }
        public int? Aggregation_Standard_Number { get; set; }
        public bool Is_Question { get; set; }
        public bool Is_Requirement { get; set; }
        public int Order_Framework_Standards { get; set; }
        [StringLength(800)]
        [Unicode(false)]
        public string Standard_ToolTip { get; set; }
        public bool Is_Deprecated { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string Upgrade_Set_Name { get; set; }
        public bool Is_Custom { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Date { get; set; }
        public bool IsEncryptedModule { get; set; }
        [Required]
        public bool? IsEncryptedModuleOpen { get; set; }
        public bool IsACET { get; set; }

        [ForeignKey("Set_Category_Id")]
        [InverseProperty("SETS")]
        public virtual SETS_CATEGORY Set_Category { get; set; }
        [InverseProperty("Set_NameNavigation")]
        public virtual ICollection<AVAILABLE_STANDARDS> AVAILABLE_STANDARDS { get; set; }
        [InverseProperty("Base_StandardNavigation")]
        public virtual ICollection<CUSTOM_STANDARD_BASE_STANDARD> CUSTOM_STANDARD_BASE_STANDARDBase_StandardNavigation { get; set; }
        [InverseProperty("Custom_Questionaire_NameNavigation")]
        public virtual ICollection<CUSTOM_STANDARD_BASE_STANDARD> CUSTOM_STANDARD_BASE_STANDARDCustom_Questionaire_NameNavigation { get; set; }
        [InverseProperty("Set_NameNavigation")]
        public virtual ICollection<MODES_SETS_MATURITY_MODELS> MODES_SETS_MATURITY_MODELS { get; set; }
        [InverseProperty("Original_Set_NameNavigation")]
        public virtual ICollection<NEW_QUESTION> NEW_QUESTION { get; set; }
        [InverseProperty("Set_NameNavigation")]
        public virtual ICollection<NEW_QUESTION_SETS> NEW_QUESTION_SETS { get; set; }
        [InverseProperty("Original_Set_NameNavigation")]
        public virtual ICollection<NEW_REQUIREMENT> NEW_REQUIREMENT { get; set; }
        [InverseProperty("Report_Set_Entity_NameNavigation")]
        public virtual ICollection<REPORT_STANDARDS_SELECTION> REPORT_STANDARDS_SELECTION { get; set; }
        [InverseProperty("Set_NameNavigation")]
        public virtual ICollection<REQUIREMENT_QUESTIONS_SETS> REQUIREMENT_QUESTIONS_SETS { get; set; }
        [InverseProperty("Set_NameNavigation")]
        public virtual ICollection<REQUIREMENT_SETS> REQUIREMENT_SETS { get; set; }
        [InverseProperty("Set_NameNavigation")]
        public virtual ICollection<SECTOR_STANDARD_RECOMMENDATIONS> SECTOR_STANDARD_RECOMMENDATIONS { get; set; }
        [InverseProperty("SetNameNavigation")]
        public virtual ICollection<SET_FILES> SET_FILES { get; set; }
        [InverseProperty("Set_NameNavigation")]
        public virtual ICollection<STANDARD_CATEGORY_SEQUENCE> STANDARD_CATEGORY_SEQUENCE { get; set; }
        [InverseProperty("Set_NameNavigation")]
        public virtual ICollection<STANDARD_SOURCE_FILE> STANDARD_SOURCE_FILE { get; set; }
        [InverseProperty("Set_NameNavigation")]
        public virtual ICollection<UNIVERSAL_SUB_CATEGORY_HEADINGS> UNIVERSAL_SUB_CATEGORY_HEADINGS { get; set; }
    }
}