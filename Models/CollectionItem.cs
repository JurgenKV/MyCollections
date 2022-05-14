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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ItemId { get; set; }
        public string UserCollectionId { get; set; }
        //public virtual Item Item { get; set; }
        //public virtual UserCollection UserCollection { get; set; }
    }
}
