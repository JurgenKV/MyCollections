using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyCollections.Models
{
    public class CollectionItem
    {
        [Key]
        public string IdCollection { get; set; }
        public string IdItem { get; set; }
        public virtual Item Item { get; set; }
        public virtual UserCollection UserCollection { get; set; }

    }
}
