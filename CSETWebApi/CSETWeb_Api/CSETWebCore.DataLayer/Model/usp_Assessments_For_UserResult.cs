﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSETWebCore.DataLayer.Model
{
    public partial class usp_Assessments_For_UserResult
    {
        public int AssessmentId { get; set; }
        public string AssessmentName { get; set; }
        public DateTime AssessmentDate { get; set; }
        public DateTime AssessmentCreatedDate { get; set; }
        public string CreatorName { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool MarkedForReview { get; set; }
        public bool UseDiagram { get; set; }
        public bool UseStandard { get; set; }
        public bool UseMaturity { get; set; }
        public string workflow { get; set; }
        public string SelectedMaturityModel { get; set; }
        public string SelectedStandards { get; set; }
        public bool? AltTextMissing { get; set; }
        public int? UserId { get; set; }
    }
}
