using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Quotes.Models
{
    public class Category
    {
        public virtual List<Quotation> Quotations { get; set; }
        [DisplayName("Category")]
        public int CategoryID { get; set; }
        [Required]
        public string Name { get; set; }

    }
}