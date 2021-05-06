﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSETWebCore.DataLayer
{
    /// <summary>
    /// A collection of STANDARD_CATEGORY_SEQUENCE records
    /// </summary>
    public partial class STANDARD_CATEGORY_SEQUENCE
    {
        [Key]
        [StringLength(250)]
        public string Standard_Category { get; set; }
        [Key]
        [StringLength(50)]
        public string Set_Name { get; set; }
        [Column("Standard_Category_Sequence")]
        public int Standard_Category_Sequence1 { get; set; }

        [ForeignKey(nameof(Set_Name))]
        [InverseProperty(nameof(SETS.STANDARD_CATEGORY_SEQUENCE))]
        public virtual SETS Set_NameNavigation { get; set; }
        [ForeignKey(nameof(Standard_Category))]
        [InverseProperty(nameof(STANDARD_CATEGORY.STANDARD_CATEGORY_SEQUENCE))]
        public virtual STANDARD_CATEGORY Standard_CategoryNavigation { get; set; }
    }
}