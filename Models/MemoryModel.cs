using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace BabysFirstCalendar.Models
{
    public class MemoryModel
    {
        [Display(Name = "Date")]
        [Required(ErrorMessage = "Please enter the date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Memory")]
        [Required(ErrorMessage = "Please enter a memory")]
        [StringLength(250, ErrorMessage = "Please keep memories 250 characters or less")]
        public string Note { get; set; }

        [DataType(DataType.Upload, ErrorMessage = "Please upload an image")]
        public HttpPostedFileBase Photo { get; set; }
    }
}