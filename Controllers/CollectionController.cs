﻿using Microsoft.AspNetCore.Mvc;
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


        public CollectionController(ILogger<CollectionController> logger, ApplicationContext context,
            UserManager<User> userManager, SignInManager<User> signInManager)
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
        public async Task<IActionResult> AddCollection(string idUser, string name, string tag, string description,
            IFormFile image)
        {
            string path = null;

            if (image != null)
            {
                path = "wwwroot/ImageStorage/CollectionImage/" + idUser + "/" + image.FileName;
                DirectoryInfo dirInfo = new DirectoryInfo("wwwroot/ImageStorage/CollectionImage/" + idUser);
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                    await using var fileStream = new FileStream(path, FileMode.Create);
                    await image.CopyToAsync(fileStream);
                }
                else
                {
                    await using var fileStream = new FileStream(path, FileMode.Create);
                    await image.CopyToAsync(fileStream);
                }
                
            }

            UserCollection userCollection = new UserCollection
            {
                Name = name,
                UserId = idUser,
                Description = description,
                Tag = tag,
                Image = path
            };


            _db.UserCollections.Add(userCollection);
            await _db.SaveChangesAsync();
            User user = _db.User.First(i => i.Id == idUser);

            return RedirectToAction("UserProfile", "User", new { user.UserName });
        }

        [HttpPost]
        public IActionResult UpdateCollection(string idUser, string id, string name, string tag, string description,
            IFormFile image)
        {
            User user = _db.User.First(i => i.Id == idUser);
            UserCollection userCollection = _db.UserCollections.First(i => i.Id == id);
            string path = "wwwroot/ImageStorage/CollectionImage/" + user.Id + "/" + image.FileName;

            if (!string.IsNullOrEmpty(name))
                userCollection.Name = name;

            if (!string.IsNullOrEmpty(tag))
                userCollection.Tag = tag;

            if (!string.IsNullOrEmpty(description))
                userCollection.Description = description;

            if (!string.IsNullOrEmpty(image.FileName))
            {
                if (!path.Equals(userCollection.Image))
                {
                    System.IO.File.Delete(userCollection.Image);
                }

                using var fileStream = new FileStream(path, FileMode.Create);
                image.CopyTo(fileStream);
            }

            _db.SaveChanges();
            return RedirectToAction("UserProfile", "User", new { user.UserName });
        }

        [HttpPost]
        public IActionResult DeleteCollection(string userName, string id)
        {
            UserCollection userCollection = _db.UserCollections.First(i => i.Id == id);

            try
            {
                if(!userCollection.Image.Equals(null))
                    System.IO.File.Delete(userCollection.Image);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            _db.UserCollections.Remove(userCollection);
            _db.SaveChanges();
            return RedirectToAction("UserProfile", "User", new { userName });
        }
    }
}