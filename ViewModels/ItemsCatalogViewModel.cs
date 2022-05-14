using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCollections.Models;

namespace MyCollections.ViewModels
{
    public class ItemsCatalogViewModel
    {
        public IQueryable<Item> Items;
        public ItemLike ItemLikes;
        public List<UserCollection> TopFiveCollections;
        public List<UserCollection> UserCollections;

    }
}
