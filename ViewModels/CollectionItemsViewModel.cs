using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCollections.Models;

namespace MyCollections.ViewModels
{
    public class CollectionItemsViewModel
    {
        public User User;
        public UserCollection UserCollection;
        public List<Item> Items;
        public List<ExtendedField> ExtendedFields;
        public List<DataField> DataFields;
    }
}
