﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CSETWebCore.DataLayer.Model;

[PrimaryKey("Layout_Name", "Row_Index")]
public partial class GALLERY_ROWS
{
    [Key]
    [StringLength(50)]
    public string Layout_Name { get; set; }

    [Key]
    public int Row_Index { get; set; }

    public int Group_Id { get; set; }

    [ForeignKey("Group_Id")]
    [InverseProperty("GALLERY_ROWS")]
    public virtual GALLERY_GROUP Group { get; set; }

    [ForeignKey("Layout_Name")]
    [InverseProperty("GALLERY_ROWS")]
    public virtual GALLERY_LAYOUT Layout_NameNavigation { get; set; }
}