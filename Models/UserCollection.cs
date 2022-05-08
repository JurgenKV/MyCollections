using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyCollections.Models
{
    public class UserCollection
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string IdUserCollection { get; set; } 
        public string NameCollection { get; set; }
        public string IdUser { get; set; }

        public ICollection<CollectionItem> Items { get; set; }
    }
}
