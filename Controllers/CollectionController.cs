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
using Microsoft.AspNetCore.Mvc.Rendering;
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

            ItemProfileViewModel itemProfileViewModel = new ItemProfileViewModel
            {
                item = _db.Items.First(i => i.Id == id),
                ItemComments = _db.ItemComments.Where(i=>i.ItemId== id),
                ItemLikes = _db.ItemLikes.Where(i=>i.ItemId == id)
            };

            return View(itemProfileViewModel);

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
        ///update - понять как рпердать данные в форму из которой будет задействована эта функция
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
                if(userCollection.Image != null)
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

        public ActionResult GetCollectionImage(string imagePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(imagePath))
                {
                    return File(imagePath, "image/png");
                }
                else
                {
                    return File("wwwroot/ImageStorage/default", "image/png");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Data);
                
            }

            return null;
        }

        public IActionResult SetComment(string userName, string idItem, string comment)
        {
            //User user = _db.User.First(i => i.UserName == userName);
            ItemComment itemComment = new ItemComment
            {
                ItemId = idItem,
                Comment = comment,
                UserName = User.Identity.Name,
                Date = DateTime.Now.ToString()
            };

            _db.ItemComments.Add(itemComment);
            _db.SaveChanges();

            return RedirectToAction("ItemProfile", new {idItem});
        }

        public string GetUserName(string id)
        {
           User user = _db.User.First(i=>i.Id == id);

           return user.UserName;
        }
    }
}