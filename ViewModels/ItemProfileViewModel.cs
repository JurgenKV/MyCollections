using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCollections.Models;

namespace MyCollections.ViewModels
{
    public class ItemProfileViewModel
    {
        public Item item;
        public IQueryable<ItemLike> ItemLikes;
        public IQueryable<ItemComment> ItemComments;
    }
}
