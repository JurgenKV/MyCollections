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

        public IActionResult ItemsCatalog(string str)
        {
            ItemsCatalogViewModel itemsCatalogViewModel = new ItemsCatalogViewModel();
            itemsCatalogViewModel.TopFiveCollections = new List<UserCollection>();
            itemsCatalogViewModel.Items = !string.IsNullOrEmpty(str) ? _db.Items.Where(item => item.Tag == str || item.Name == str) : _db.Items;

            if (User.Identity.IsAuthenticated)
            {
                User user = _db.User.First(i => i.UserName == User.Identity.Name);
                itemsCatalogViewModel.UserCollections = _db.UserCollections.Where(col => col.UserId == user.Id).ToList();
            }

            itemsCatalogViewModel.ItemLikes = new ItemLike();
            if (_db.UserCollections != null && _db.UserCollections.Count() > 5)
            {
                //IQueryable<UserCollection> userCollections = _db.UserCollections;
                List<UserCollection> orderByDescending = _db.UserCollections.OrderBy(i => i.Items.Count()).ToList();

                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        itemsCatalogViewModel.TopFiveCollections.Add(orderByDescending[i]);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Data);
                    }
                }
            }

            return View(itemsCatalogViewModel);
        }

        [HttpPost]
        public IActionResult ItemProfile(string id)
        {
            ItemProfileViewModel itemProfileViewModel = new ItemProfileViewModel
            {
                Item = _db.Items.First(i => i.Id == id),
                ItemComments = _db.ItemComments.Where(i => i.ItemId == id),
                ItemLikes = _db.ItemLikes.Where(i => i.ItemId == id)
            };

            return View(itemProfileViewModel);
        }

        public IActionResult ItemProfile(string id, string userName)
        {
            ItemProfileViewModel itemProfileViewModel = new ItemProfileViewModel
            {
                Item = _db.Items.First(i => i.Id == id),
                ItemComments = _db.ItemComments.Where(i => i.ItemId == id),
                ItemLikes = _db.ItemLikes.Where(i => i.ItemId == id)
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

            return RedirectToAction("ItemsCatalog", "Collection");
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
                if (userCollection.Image != null)
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
                    return File("wwwroot/ImageStorage/default.png", "image/png");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Data);
            }

            return null;
        }

        public IActionResult SetItemComment(string userName, string idItem, string comment)
        {
            if (comment == null) return RedirectToAction("ItemProfile", new { id = idItem, userName });

            ItemComment itemComment = new ItemComment
            {
                ItemId = idItem,
                Comment = comment,
                UserName = User.Identity.Name,
                Date = DateTime.Now.ToString(CultureInfo.InvariantCulture)
            };

            _db.ItemComments.Add(itemComment);
            _db.SaveChanges();

            return RedirectToAction("ItemProfile", new { id = idItem, userName });
        }

        public string GetUserName(string id)
        {
            User user = _db.User.First(i => i.Id == id);

            return user.UserName;
        }

        
        public IActionResult CollectionItems(string IdCollection, string IdUser)
        {
            CollectionItemsViewModel collectionItemsViewModel = new CollectionItemsViewModel();
            collectionItemsViewModel.User = new User();
            collectionItemsViewModel.Items = new List<Item>();
            collectionItemsViewModel.UserCollection = new UserCollection();

            collectionItemsViewModel.User = _db.User.First(i => i.Id == IdUser);
            collectionItemsViewModel.UserCollection = _db.UserCollections.First(i => i.Id == IdCollection);
            List<CollectionItem> collectionItemList = _db.CollectionItems.Where(i => i.UserCollectionId == IdCollection).ToList();

            foreach (var item in collectionItemList)
            {
                collectionItemsViewModel.Items.Add(_db.Items.First(i => i.Id == item.ItemId));
            }

            collectionItemsViewModel.CustomFields = new List<CustomField>();

            return View(collectionItemsViewModel);
        }

        public IActionResult AddCollectionItem(string IdCollection, string IdItem)
        {
            CollectionItem collectionItem = new CollectionItem
            {
                ItemId = IdItem,
                UserCollectionId = IdCollection
            };
            int a = _db.CollectionItems.Count(i => i.ItemId == IdItem && i.UserCollectionId == IdCollection);
            if (a == 0)
            {
                _db.CollectionItems.Add(collectionItem);
                _db.SaveChanges();
            }

            return RedirectToAction("ItemsCatalog");
        }

    }
}