using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Quotes.Models
{
    public class Quotation
    {
        public virtual Category Category { get; set; }
        [DisplayName("Category")]
        public int CategoryID { get; set; }
        public int QuotationID { get; set; }
        [Required]
        public string Quote { get; set; }
        [Required]
        public string Author { get; set; }

        [DisplayFormat(DataFormatString="{0:MM/dd/yyyy}")]
        public DateTime Date { get; set; }


        
    }
}