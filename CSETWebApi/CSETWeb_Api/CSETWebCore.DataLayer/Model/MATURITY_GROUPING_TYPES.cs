﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSETWebCore.DataLayer
{
    /// <summary>
    /// A collection of MATURITY_GROUPING_TYPES records
    /// </summary>
    public partial class MATURITY_GROUPING_TYPES
    {
        public MATURITY_GROUPING_TYPES()
        {
            MATURITY_GROUPINGS = new HashSet<MATURITY_GROUPINGS>();
        }

        [Key]
        public int Type_Id { get; set; }
        [StringLength(100)]
        public string Grouping_Type_Name { get; set; }

        [InverseProperty("Type_")]
        public virtual ICollection<MATURITY_GROUPINGS> MATURITY_GROUPINGS { get; set; }
    }
}