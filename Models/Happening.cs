using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

public class PastDateAttribute:ValidationAttribute{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext){
        
        if((DateTime)value < DateTime.Now){
            return new ValidationResult("Date should be from Future");
        }
        return ValidationResult.Success;
    }
}
namespace ActivityCenter.Models{
    public class Happening{
        [Key]
        public int HappeningId{get;set;}

        [Required]
        [Display(Name ="Title:")]
        public string Title{get;set;}

        [Required]
        [DataType(DataType.DateTime)]
        [PastDate]
        [Display(Name="Date:")]
        public DateTime HappeningDate{get;set;}

        [Required]
        [DataType(DataType.Time)]
        public DateTime StartTime{get;set;}

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EndTime{get;set;}
        
        [Required]
        [Range(1,Int32.MaxValue)]
        public int Duration{get;set;}


        [Required]
        public string DurationMetric{get;set;}

        [Required]
        public string Description{get;set;}

        public DateTime CreatedAt{get;set;} = DateTime.Now;
        public DateTime UpdatedAt{get;set;} = DateTime.Now;

        public int CreatedById{get;set;}
        public User CreatedBy{get;set;}

        public List<Participation> ParticipantsList{get;set;}

    }
}