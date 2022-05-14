using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCollections.Models;

namespace MyCollections.ViewModels
{
    public class ItemProfileViewModel
    {
        public Item Item;
        public IQueryable<ItemLike> ItemLikes;
        public IQueryable<ItemComment> ItemComments;
    }
}
