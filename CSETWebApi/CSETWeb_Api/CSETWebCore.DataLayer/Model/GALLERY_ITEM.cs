﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CSETWebCore.DataLayer.Model
{
    public partial class GALLERY_ITEM
    {
        public GALLERY_ITEM()
        {
            GALLERY_GROUP_DETAILS = new HashSet<GALLERY_GROUP_DETAILS>();
        }

        [Key]
        public int Gallery_Item_Id { get; set; }
        [StringLength(100)]
        public string Icon_File_Name_Small { get; set; }
        [StringLength(100)]
        public string Icon_File_Name_Large { get; set; }
        [Required]
        public string Configuration_Setup { get; set; }
        public string Configuration_Setup_Client { get; set; }
        [Required]
        [StringLength(1000)]
        public string Description { get; set; }
        [StringLength(200)]
        public string Title { get; set; }

        [InverseProperty("Gallery_Item")]
        public virtual ICollection<GALLERY_GROUP_DETAILS> GALLERY_GROUP_DETAILS { get; set; }
    }
}