using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCollections.Models;

namespace MyCollections.ViewModels
{
    public class AdminMenuViewModel
    {
        public IEnumerable<User> Users;
        public IEnumerable<Item> Items;

    }
}
