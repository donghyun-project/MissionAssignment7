using Microsoft.AspNetCore.Mvc;
using MissionAssignment7.Models;
using MissionAssignment7.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MissionAssignment7.Controllers
{
    public class HomeController : Controller
    {
        // declare private book repository and gets data from there, works like OOP
        private IBookstoreRepository repo;

        public HomeController(IBookstoreRepository temp)
        {
            repo = temp;
        }

        // index view page with book data
        public IActionResult Index(int pageNum = 1)
        {
            int pageSize = 10;

            // show list of 10 books for each page
            var x = new BooksViewModel
            {
                Books = repo.Books
                    .OrderBy(pageNum => pageNum.Title)
                     .Skip((pageNum - 1) * pageSize)
                    .Take(pageSize),

                // page information
                PageInfo = new PageInfo
                {
                    TotalNumBooks = repo.Books.Count(),
                    BooksPerPage = pageSize,
                    CurrentPage = pageNum
                }
            };

            return View(x);
        }
    }
}
