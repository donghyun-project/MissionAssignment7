using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MissionAssignment7.Infrastructure;
using MissionAssignment7.Models;

namespace MissionAssignment7.Pages
{
    public class ShoppingCartModel : PageModel
    {
        private IBookstoreRepository repo { get; set; }

        // this is shopping cart model
        public ShoppingCartModel(IBookstoreRepository temp)
        {
            repo = temp;
        }

        // this is basket
        public Basket basket { get; set; }

        // this is return URL
        public string ReturnUrl { get; set; }
        
        // function to get return url and store into basket
        public void OnGet(string returnUrl)
        {
            ReturnUrl = returnUrl ?? "/";
            basket = HttpContext.Session.GetJson<Basket>("basket") ?? new Basket(); ;
        }

        // returning to the book list
        public IActionResult OnPost(int bookId, string returnUrl)
        {
            Book b = repo.Books.FirstOrDefault(x => x.BookId == bookId);

            // ?? null-coalescing operator
            basket = HttpContext.Session.GetJson<Basket>("basket") ?? new Basket();
            basket.AddItem(b, 1);

            HttpContext.Session.SetJson("basket", basket);

            return RedirectToPage(new { ReturnUrl = returnUrl });
        }
    }
}
