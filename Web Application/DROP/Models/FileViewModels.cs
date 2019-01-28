using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DROP.Models
{
    public class FileViewModels
    {
        [Required]
        [Display(Name = "Quarter")]
        [RegularExpression(@"^(?:[1-4]?)$",
            ErrorMessage = "Enter 1-4 only.")]
        public int quarter { get; set; }

        [Required]
        [Display(Name = "Year")]
        [RegularExpression(@"^(?:[0-2][0-9][0-2][0-9]?)$",
            ErrorMessage = "Incorrect Format")]
        public int year { get; set; }

        public string so_desc { get; set; }

        public string coursename { get; set; }

        public int coppiat_id { get; set; }

        public string filename { get; set; }

        public DateTime cdate { get; set; }

        public int acc_id { get; set; }

        public int pid { get; set; }

        public string fname { get; set; }

        public string lname { get; set; }
    }
}