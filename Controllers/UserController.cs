using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCollections.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyCollections.ViewModels;

namespace MyCollections.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationContext _db;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager,
            ILogger<UserController> logger, ApplicationContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _db = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            foreach (var u in users)
            {
                if (User.Identity.Name == u.UserName)
                {
                    return RedirectToAction("Login", "User");
                }
            }

            return View();
        }

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    UserName = model.Login,
                    Email = model.Email,
                    IsActive = true,
                    AdminRoot = false,
                    IsWhiteTheme = false
                };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("ItemsCatalog", "Collection");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [Route("User/Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.Email);
                    if (user == null)
                    {
                        return NotFound("Unable to load user for update last login.");
                    }

                    var lastLoginResult = await _userManager.UpdateAsync(user);
                    if (!lastLoginResult.Succeeded)
                    {
                        throw new InvalidOperationException($"Unexpected error occurred setting the last login date" +
                                                            $" ({lastLoginResult}) for user with ID '{user.Id}'.");
                    }

                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return RedirectToAction("ItemsCatalog", "Collection");
                    }
                    else
                    {
                        return RedirectToAction("Index", "User");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Login or Password is incorrect or you are blocked");
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Registration", "User");
        }

       
        public IActionResult AdminMenu()
        {
            AdminMenuViewModel adminMenuViewModel = new AdminMenuViewModel();

            var users = _userManager.Users.ToList();
            if (users.Any(u => User.Identity.Name == u.UserName && u.AdminRoot && u.IsActive))
            {
                adminMenuViewModel.Users = _userManager.Users.ToList();
                adminMenuViewModel.Items = _db.Items.ToList();

                return View(adminMenuViewModel);

            }

            return RedirectToAction("Login", "User");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MultiplyBlock(string usersId)
        {
            bool toOut = false;
            if (usersId != null)
            {
                var user = await _userManager.FindByIdAsync(usersId);
                if (user != null)
                {
                    user.IsActive = false;
                    await _userManager.UpdateAsync(user);
                }
                else
                {
                    ModelState.AddModelError("", "User Not Found");
                }
            }

            return RedirectToAction("AdminMenu", "User");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MultiplyUnblock(string usersId)
        {
            if (usersId != null)
            {
                var user = await _userManager.FindByIdAsync(usersId);
                    if (user != null)
                    {
                        user.IsActive = true;
                        await _userManager.UpdateAsync(user);
                    }
                    else
                    {
                        ModelState.AddModelError("", "User Not Found");
                    }
            }

            return RedirectToAction("AdminMenu");
        }

        public async Task<IActionResult> MultiplySetUserRoot(string usersId)
        {
            bool toOut = false;
            if (usersId != null)
            {

                    var user = await _userManager.FindByIdAsync(usersId);
                    if (user != null)
                    {
                        user.AdminRoot = false;
                        await _userManager.UpdateAsync(user);
                    }
                    else
                    {
                        ModelState.AddModelError("", "User Not Found");
                    }
            }

            return RedirectToAction("AdminMenu", "User");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MultiplySetAdminRoot(string usersId)
        {
            if (usersId != null)
            {

                    var user = await _userManager.FindByIdAsync(usersId);
                    if (user != null)
                    {
                        user.AdminRoot = true;
                        await _userManager.UpdateAsync(user);
                    }
                    else
                    {
                        ModelState.AddModelError("", "User Not Found");
                    }

            }

            return RedirectToAction("AdminMenu");
        }


        [HttpPost]
        public async Task<IActionResult> MultiplyDelete(string usersId)
        {
            bool toOut = false;
            if (usersId != null)
            {
                var user = await _userManager.FindByIdAsync(usersId);
                if (user != null)
                {
                    List<UserCollection> userCollections =
                        _db.UserCollections.Where(i => i.UserId == user.Id).ToList();
                    foreach (var collection in userCollections)
                    {
                        List<CollectionItem> collectionItems = _db.CollectionItems
                            .Where(i => i.UserCollectionId == collection.Id).ToList();
                        foreach (var collectionItem in collectionItems)
                        {
                            _db.CollectionItems.Remove(collectionItem);
                        }

                        _db.UserCollections.Remove(collection);
                    }

                    List<ItemComment> itemComments =
                        _db.ItemComments.Where(i => i.UserName == user.UserName).ToList();
                    foreach (var comment in itemComments)
                    {
                        _db.ItemComments.Remove(comment);
                    }

                    List<ItemLike> itemLikes =
                        _db.ItemLikes.Where(i => i.UserId == user.Id).ToList();
                    foreach (var itemLike in itemLikes)
                    {
                        _db.ItemLikes.Remove(itemLike);
                    }

                    await _userManager.DeleteAsync(user);

                    await _db.SaveChangesAsync();

                    if (User.Identity.Name == user.UserName)
                    {
                        toOut = true;
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User Not Found");
                }
            }

            if (toOut)
            {
                return await Logout();
            }

           return RedirectToAction("AdminMenu");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult UserProfile(string name,string str)
        {
            UserProfileViewModel userProfile = new UserProfileViewModel();
            User user;
            if (!string.IsNullOrEmpty(name))
            {
                user = _db.User.First(i => i.UserName == name);
            }
            else
            {
                user = _db.User.First(i => i.UserName == User.Identity.Name);
            }

            userProfile.UserCollections = _db.UserCollections.Where(coll => coll.UserId.Equals(user.Id)).ToList();

            userProfile.UserCollections = !string.IsNullOrEmpty(str)
                ? userProfile.UserCollections.Where(collection => collection.Tag.Contains(str) || collection.Name.Contains(str)).ToList()
                : userProfile.UserCollections;

            userProfile.User = user;
            userProfile.ExtendedFields = new ExtendedField();

            return View(userProfile);
        }
    }
}