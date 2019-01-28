using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DROP.Models
{
    public class HomeViewModel
    {
        public int acc_id { get; set; }

        public int type_id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FName { get; set; }

        [Required]
        [Display(Name = "Middle Name")]
        public string MName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LName { get; set; }

        [Required]
        [Display(Name = "Username")]
        [Remote("checkusername", "Home", HttpMethod = "POST", ErrorMessage = "Username Already Exists")]
        public string username { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$",
            ErrorMessage = "Requried: uppercase, lowercase, numeric, and special character with minimum length of 8.")]
        [Display(Name = "Password")]
        public string password { get; set; }

        public string LogErrorMsg { get; set; }

        public int SelectedType { get; set; }

        public int SelectedID { get; set; }

        public List<usertype> Type
        {
            get
            {
                projectEntities db = new projectEntities();
                return db.usertypes.ToList();
            }
        }

        public int input_id { get; set; }

        public string name_in { get; set; }

        [Required(ErrorMessage = "File upload required")]
        public IEnumerable<HttpPostedFileBase> excel_in { get; set; }

        public virtual usertype usertype { get; set; }

        [Required]
        [RegularExpression(@"^0*(?:[1-9][0-9]?|100)$",
            ErrorMessage = "Enter 1-100 only.")]
        [Display(Name = "Target")]
        public string target { get; set; }

        [Required(ErrorMessage ="Choose a Student Outcome")]
        public int so_id { get; set; }

        [Required(ErrorMessage = "Choose a Performance Indicator")]
        public int pi_id { get; set; }

        [Required(ErrorMessage = "Choose a Course")]
        public int course_id { get; set; }

        [Required(ErrorMessage = "Choose an Assessment Tool")]
        public int at_id { get; set; }
    }

}