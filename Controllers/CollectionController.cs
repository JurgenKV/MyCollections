using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

            return View(itemsList.ToList());
        }

        [HttpPost]
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

            return RedirectToAction("Items", "Collection");
        }

        public IActionResult SetItemComment(string itemId, string comment)
        {

            return View("ItemProfile");
        }
        [HttpPost]
        public async Task<IActionResult> AddCollection(string idUser, string name, string tag, string description, IFormFile image )
        {
            string path = null;

            if (image != null)
            {
                // путь к папке Files
                path = "/ImageStorage/CollectionImage/" + image.FileName;
                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }
            }


            UserCollection userCollection = new UserCollection
            {
                Name = name,
                IdUser = idUser,
                Description = description,
                Tag = tag,
                Image = path
            };


            _db.UserCollections.Add(userCollection);
            _db.SaveChanges();

            return RedirectToAction("UserProfile", "User");
        }
        [HttpPost]
        public IActionResult UpdateCollection()
        {
            return RedirectToAction("UserProfile", "User");
        }
        [HttpPost]
        public IActionResult DeleteCollection()
        {
            return RedirectToAction("UserProfile", "User");
        }




    }
}
