using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using MyCollections.Models;
using MyCollections.ViewModels;
using Microsoft.Extensions.FileProviders;

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

        public async Task<IActionResult> CreateItem(string ItemTag, string ItemName, string ItemDescription,
            IFormFile image)
        {
            string path = null;

            if (image != null)
            {
                path = "wwwroot/ImageStorage/ItemImage/" + Path.GetFileName(image.FileName);
                DirectoryInfo dirInfo = new DirectoryInfo("wwwroot/ImageStorage/ItemImage/");

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

            Item item = new Item
            {
                Name = ItemName,
                Tag = ItemTag,
                Description = ItemDescription,
                Image = Strings.Replace(path, "wwwroot/", "~/")
            };

            _db.Items.Add(item);
            await _db.SaveChangesAsync();

            return RedirectToAction("AdminMenu", "User");
        }

        public IActionResult ItemsCatalog(string str)
        {
            ItemsCatalogViewModel itemsCatalogViewModel = new ItemsCatalogViewModel();
            itemsCatalogViewModel.TopFiveCollections = new List<UserCollection>();
            itemsCatalogViewModel.ItemLikes = new List<ItemLike>();

            List<ItemLike> Likes = _db.ItemLikes.ToList();
            itemsCatalogViewModel.Items = !string.IsNullOrEmpty(str)
                ? _db.Items.Where(item => item.Tag.Contains(str) || item.Name.Contains(str))
                : _db.Items;

            if (User.Identity.IsAuthenticated)
            {
                itemsCatalogViewModel.User = _db.User.First(i => i.UserName == User.Identity.Name);
                itemsCatalogViewModel.UserCollections = _db.UserCollections
                    .Where(col => col.UserId == itemsCatalogViewModel.User.Id).ToList();
                itemsCatalogViewModel.ItemLikes = Likes.Where(i => i.UserId == itemsCatalogViewModel.User.Id).ToList();
            }

            if (_db.UserCollections != null && _db.UserCollections.Count() > 5)
            {
                List<UserCollection> orderByDescending =
                    _db.UserCollections.OrderByDescending(i => i.Items.Count()).ToList();
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

            if (Likes != null)
            {
                foreach (var item in itemsCatalogViewModel.Items)
                {
                    item.ItemLikes = new List<ItemLike>();
                    item.ItemLikes = Likes.Where(like => like.ItemId == item.Id).ToList();
                }
            }

            List<string> Tags = new List<string>();
            foreach (var item in _db.Items)
            {
                Tags.Add(item.Tag);
            }

            itemsCatalogViewModel.TagCloude = Tags.Distinct();
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

        public IActionResult SetItemLike(string UserId, string ItemId)
        {
            ItemLike itemLike = new ItemLike
            {
                UserId = UserId,
                ItemId = ItemId
            };

            ItemLike item = null;
            try
            {
                item = _db.ItemLikes.First(i => (i.UserId == UserId) && (i.ItemId == ItemId));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Data);
            }

            if (item != null)
            {
                _db.ItemLikes.Remove(item);
                _db.SaveChanges();
            }
            else
            {
                _db.ItemLikes.Add(itemLike);
                _db.SaveChanges();
            }

            return RedirectToAction("ItemsCatalog", "Collection");
        }

        [HttpPost]
        public async Task<IActionResult> AddCollection(string idUser, string name, string tag, string description,
            IFormFile image, string[] Fields)
        {
            string path = null;

            if (image != null)
            {
                path = "wwwroot/ImageStorage/CollectionImage/" + idUser + "/" + Path.GetFileName(image.FileName);
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
                Image = Strings.Replace(path, "wwwroot/", "~/")
            };
            _db.UserCollections.Add(userCollection);

            foreach (var Field in Fields)
            {
                ExtendedField ExtendedField = new ExtendedField()
                {
                    Name = Field,
                    UserCollectionId = userCollection.Id
                };
                _db.ExtendedFields.Add(ExtendedField); //Снять
            }


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
            string path = "~/ImageStorage/CollectionImage/" + user.Id + "/" + image.FileName;

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
            if (_db.UserCollections.Any(i => i.Id == id))
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

                List<CollectionItem> collection =
                    _db.CollectionItems.Where(i => i.UserCollectionId == userCollection.Id).ToList();
                for (int i = 0; i < collection.Count(); i++)
                {
                    _db.CollectionItems.Remove(collection[i]);
                }

                List<ExtendedField> extendedFields =
                    _db.ExtendedFields.Where(i => i.UserCollectionId == userCollection.Id).ToList();
                for (int i = 0; i < extendedFields.Count(); i++)
                {
                    _db.ExtendedFields.Remove(extendedFields[i]);
                }

                _db.UserCollections.Remove(userCollection);
                _db.SaveChanges();
            }

            return RedirectToAction("UserProfile", "User", new { userName });
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
            List<CollectionItem> collectionItemList =
                _db.CollectionItems.Where(i => i.UserCollectionId == IdCollection).ToList();
            collectionItemsViewModel.ExtendedFields =
                _db.ExtendedFields.Where(i => i.UserCollectionId == IdCollection).ToList();
            collectionItemsViewModel.DataFields = new List<DataField>();

            foreach (var item in collectionItemList)
            {
                collectionItemsViewModel.Items.Add(_db.Items.First(i => i.Id == item.ItemId));
            }

            foreach (var Field in collectionItemsViewModel.ExtendedFields)
            {
                collectionItemsViewModel.DataFields.AddRange(_db.DataFields.Where(i => i.ExtendedFieldId == Field.Id));
            }

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


        public IActionResult DeleteCollectionItem(string IdCollection, string IdItem, string IdUser)
        {
            CollectionItem collectionItem =
                _db.CollectionItems.First(i => i.ItemId == IdItem && i.UserCollectionId == IdCollection);

            List<ExtendedField> extendedFields =
                _db.ExtendedFields.Where(i => i.UserCollectionId == IdCollection).ToList();

            List<DataField> dataFields = new List<DataField>();
            foreach (var extended in extendedFields)
            {
                dataFields.AddRange(_db.DataFields.Where(i => i.ExtendedFieldId == extended.Id));
            }

            _db.DataFields.RemoveRange(dataFields);
            _db.CollectionItems.Remove(collectionItem);


            _db.SaveChanges();
            return RedirectToAction("CollectionItems", new { IdCollection, IdUser });
        }

        public IActionResult UpdateDataField(string Data, string FieldId, string IdUser, string IdCollection,
            string IdItem)
        {
            if (FieldId != null)
            {
                DataField dataField =
                    _db.DataFields.FirstOrDefault(i => i.ItemId == IdItem && i.ExtendedFieldId == Int32.Parse(FieldId));
                if (dataField == null)
                {
                    DataField newDataField = new DataField
                    {
                        Data = Data,
                        ExtendedFieldId = Int32.Parse(FieldId),
                        ItemId = IdItem
                    };
                    _db.DataFields.Add(newDataField);
                }
                else
                {
                    dataField.Data = Data;
                    _db.DataFields.Update(dataField);
                }

                _db.SaveChanges();
            }

            return RedirectToAction("CollectionItems", new { IdCollection, IdUser });
        }


        public  FileResult ExportCSV(string CollectionId, string name)
        {
            List<CollectionItem> collectionItems = _db.CollectionItems.Where(i => i.UserCollectionId == CollectionId).ToList();
            UserCollection userCollection = _db.UserCollections.First(i => i.Id == CollectionId);
            List<Item> items = new List<Item>();

            foreach (var collectionItem in collectionItems)
            {
                items.Add(_db.Items.First(i => i.Id == collectionItem.ItemId));
            }
            var csv = new StringBuilder();

            //for (int i = 0; i < items.Count(); i++)
            //{
            //    var first = items[i].ToString();

            //    var newLine = string.Format("{0},{1}", first);
            //    csv.AppendLine(newLine);
            //}

            string path = "C:\\Users\\JurgenKV\\source\\repos\\MyCollections\\wwwroot\\";

            using (var w = new StreamWriter(path))
            {
                for (int i = 0; i < items.Count(); i++)
                {
                    var first = items[i].ToString();

                    var line = string.Format("{0},{1},{2}", first);
                    w.WriteLine(line);
                    w.Flush();
                }
            }

            

            string fileName = userCollection.Name + ".csv";

           // File.AppendAllText(path, csv.ToString());
            

           // File.WriteAllText("", csv.ToString());

           // return File(csv, "text/csv", fileName); // this is the key!

           return null;
        }

       
    }
}