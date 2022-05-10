using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyCollections.Models;
using MyCollections.ViewModels;

namespace MyCollections.Controllers
{
    public class CollectionController : Controller
    {
        private readonly ILogger<CollectionController> _logger;
        private readonly ApplicationContext _db;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;


        public CollectionController(ILogger<CollectionController> logger, ApplicationContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _logger = logger;
            _db = context;
            _userManager = userManager;
            _signInManager = signInManager;

        }

        public IActionResult Items(string str)
        {
            ICollection<Item> itemsList = _db.Items.ToList();

            if (!string.IsNullOrEmpty(str))
            {
                var item = itemsList.Where(item => item.Tag == str);
                return View(item.ToList());
            }

            ///
            var users = _userManager.Users.ToList();
            string currIdUser = "";

            foreach (var u in users)
            {
                if (User.Identity.Name == u.UserName)
                {
                    currIdUser = u.Id;
                }
            }

            //ItemLike itt = new ItemLike
            //{
            //    IdUser = currIdUser,

            //};
            //Item it = _db.Items.Where(i => i.Name == "Crazy Maks").FirstOrDefault();
            //if (it != null)
            //{
            //    ItemLike itt = new ItemLike
            //    {
            //        IdUser = currIdUser,
            //        IdItem = it.Id_item

            //    };
            //    if (it.ItemLikes == null)
            //        it.ItemLikes = new List<ItemLike>();

            //    it.ItemLikes.Add(itt);

            //    _db.SaveChangesAsync();

            //}
            return View(itemsList.ToList());
        }

        public IActionResult ItemProfile(string id)
         {
             
            return View(_db.Items.Find(id));
         }

        public IActionResult SetItemLike(string user)
        {
            var users = _userManager.Users.ToList();
            string currIdUser = "";

            foreach (var u in users)
            {
                if (user == u.UserName)
                {
                    currIdUser = u.Id;
                }
            }

            //foreach (var i in _db.Items)
            //{

            //    i.ItemLikes = new List<ItemLike>
            //    {
            //        new ItemLike
            //        {
            //            IdUser = currIdUser,
            //            IdItem = i.Id_item

            //        }
            //    };
                

            //    _db.Items.Update(i);
            //    _db.SaveChanges();
            //    break;
            //}
            ItemLike itt = new ItemLike
            {
                IdUser = currIdUser,

            };
            var it = _db.Items.Find("Crazy Maks");
            if (it != null)
            {
                it.ItemLikes.Add(itt);
                _db.SaveChanges();
            }
            
            return RedirectToAction("Items", "Collection");
        }


    }
}
