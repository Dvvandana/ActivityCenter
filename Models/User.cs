using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

namespace ActivityCenter.Models{
    public class User{
        [Key]
        public int UserId{get;set;}

        [Required]
        [MinLength(2,ErrorMessage="First Name should be atleast 2 characters long")]
        [Display(Name = "Name:")]
        public string Name{get;set;}


        [Required]
        [EmailAddress]
        [Display(Name = "Email:")]
        public string Email{get;set;}

        [Required]
        [MinLength(8,ErrorMessage="Password must be 8 characters long")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$",ErrorMessage="Please Enter one chr one int and one special")]
        public string Password{get;set;}
        public DateTime CreatedAt{get;set;} = DateTime.Now;
        public DateTime UpdatedAt{get;set;} = DateTime.Now;

        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]

        public string Confirm{get;set;}

        public List<Participation> ActivitiesToAttend{get;set;}
        public List<Happening> createdPlan{get;set;}
    }
}