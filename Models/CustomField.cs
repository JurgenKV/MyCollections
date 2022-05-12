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
        public int Id { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }
        public string ItemId { get; set; }

    }
}
