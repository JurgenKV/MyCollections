﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyCollections.Models
{
    public class Item
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string Id_item { get; set; }
        public string Tag { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Likes { get; set; }
        public string Picture { get; set; }

        public virtual ICollection<CollectionItem> CollectionItems{ get; set; }
        public virtual ICollection<CustomField> CustomFields { get; set; }
    }
}