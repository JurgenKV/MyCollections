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
        public string Id { get; set; } 
        public string Name { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public string Tag { get; set; }
        public string Image { get; set; }

        
        public ICollection<CollectionItem> Items { get; set; }
    }
}
