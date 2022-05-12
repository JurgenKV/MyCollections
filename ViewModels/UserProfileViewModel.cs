using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyCollections.Models;

namespace MyCollections.ViewModels
{
    public class UserProfileViewModel
    {
        public User User;
        public IQueryable<UserCollection> UserCollections;
    }
}
