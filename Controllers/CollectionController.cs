using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyCollections.Models;

namespace MyCollections.Controllers
{
    public class CollectionController : Controller
    {
        private readonly ILogger<CollectionController> _logger;
        private readonly ApplicationContext _db;


        public CollectionController(ILogger<CollectionController> logger, ApplicationContext context)
        {
            _logger = logger;
            _db = context;
        }

         public IActionResult Items()
        {

            //Item item = new Item
            //{
            //    Tag = "2Book",
            //    Name = "2The picture of Dorian Grey",
            //    Description = "2Top"
            //};

            //_db.Items.Add(item);
            //_db.SaveChanges();



            return View();
        }
    }
}
