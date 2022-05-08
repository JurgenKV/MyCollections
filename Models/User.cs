using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MyCollections.Models
{
    public class User : IdentityUser
    {
        public bool IsActive { get; set; }
        public bool AdminRoot { get; set; }
        public bool IsWhiteTheme { get; set; }

        public virtual ICollection<UserCollection> UserCollections { get; set; }
    }
}
