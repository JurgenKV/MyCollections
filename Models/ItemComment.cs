using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyCollections.Models
{
    public class ItemComment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string Comment { get; set; }
        public string UserId { get; set; }
        public string ItemId { get; set; }
        public string Date { get; set; }
    }
}
