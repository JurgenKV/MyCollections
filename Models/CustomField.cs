using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyCollections.Models
{
    public class CustomField
    {
        [Key]
        public int IdCustomField { get; set; }
        public string NameCustomField { get; set; }
        public string Data { get; set; }
        public string IdItem { get; set; }

    }
}
