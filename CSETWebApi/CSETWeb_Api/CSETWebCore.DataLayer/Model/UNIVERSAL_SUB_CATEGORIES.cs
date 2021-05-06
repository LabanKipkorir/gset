﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSETWebCore.DataLayer
{
    /// <summary>
    /// A collection of UNIVERSAL_SUB_CATEGORIES records
    /// </summary>
    public partial class UNIVERSAL_SUB_CATEGORIES
    {
        public UNIVERSAL_SUB_CATEGORIES()
        {
            UNIVERSAL_SUB_CATEGORY_HEADINGS = new HashSet<UNIVERSAL_SUB_CATEGORY_HEADINGS>();
        }

        /// <summary>
        /// The Universal Sub Category is used to
        /// </summary>
        [Key]
        [StringLength(100)]
        public string Universal_Sub_Category { get; set; }
        /// <summary>
        /// The Universal Sub Category Id is used to
        /// </summary>
        public int Universal_Sub_Category_Id { get; set; }
        public bool Is_Custom { get; set; }

        public virtual ICollection<UNIVERSAL_SUB_CATEGORY_HEADINGS> UNIVERSAL_SUB_CATEGORY_HEADINGS { get; set; }
    }
}