﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCollections.Models;

namespace MyCollections.ViewModels
{
    public class ItemSearchViewModel
    {
        private Item CurrentItem;
        private ICollection<ItemComment> itemComments;
        private ICollection<User> users;
    }
}
