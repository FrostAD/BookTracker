using LibraryR.Data;
using LibraryR.Models;
using LibraryR.Service;
using LibraryR.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryR.Controllers
{
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserService _userService;


        public BookController(ApplicationDbContext db,IUserService userService)
        {
            _db = db;
            _userService = userService;
        }
        public IActionResult Index()
        {
            return View();
        }
        //GET
        public IActionResult Create()
        {
            BookVM book = new BookVM()
            {
                Book = new Book(),
                AuthorDropDown = _db.Authors.Select(a => new SelectListItem
                {
                    Text = a.FirstName + " " + a.LastName,
                    Value = a.Id.ToString()
                })
            };

            return View(book);
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BookVM b)
        {
            var selectedAuthors = Request.Form["ajaxMultiSelect2"];
            if (selectedAuthors.Count == 0)
            {
                ViewBag.Error = "Select at least 1 author";
                return View();
            }
            List<Author> list = new List<Author>();
            Author selectedAuthor;
            foreach (var author in selectedAuthors)
            {
                selectedAuthor = _db.Authors.Find(int.Parse(author), _userService.GetUserId());
                list.Add(selectedAuthor);
            }

            b.Book.Authors = list;
            b.Book.UserId = _userService.GetUserId();
            _db.Books.Add(b.Book);
            _db.SaveChanges();
            ViewBag.Success = "Book added successfully";

            return View("Index");
        }

        // GET: Book/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            var book = await _db.Books.Include(a => a.Authors)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                ViewBag.Error = "Book not found";
                return View("Index");
            }

            return View(book);
        }

        //GET
        public IActionResult Edit(int? id)
        {
            Book selectedBook = _db.Books.Include(b => b.Authors).FirstOrDefault(b => b.Id == id);

            if (selectedBook == null)
            {
                ViewBag.Error = "Book not found";
                return View("Index");
            }

            BookVM book = new BookVM()
            {
                Book = selectedBook,
                AuthorDropDown = selectedBook.Authors.Select(a => new SelectListItem
                {
                    Text = a.FirstName + " " + a.LastName,
                    Value = a.Id.ToString(),
                    Selected = true
                })
            };
            return View(book);
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BookVM bk)
        {
            var selectedAuthors = Request.Form["ajaxMultiSelect2"];
            List<Author> list = new List<Author>();
            Author selectedAuthor;
            foreach (var author in selectedAuthors)
            {
                selectedAuthor = _db.Authors.Find(int.Parse(author), _userService.GetUserId());
                list.Add(selectedAuthor);
            }

            Book real = _db.Books.Include(b => b.Authors).FirstOrDefault(b => b.Id == bk.Book.Id);
            real.Authors.Clear();
            _db.SaveChanges();
            _db.Entry(real).State = EntityState.Detached;

            bk.Book.Authors = list;

            _db.Books.Update(bk.Book);
            _db.SaveChanges();

            ViewBag.Success = "Book edited successfully";

            return View("Details", bk.Book);

        }

        // GET: Book/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {

            var book = await _db.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                ViewBag.Error = "Book not found";
                return View("Index");
            }

            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _db.Books.FindAsync(id, _userService.GetUserId());
            var recordWithBook = _db.Records.Where(r => r.BookId == book.Id.ToString());
            _db.Records.RemoveRange(recordWithBook);
            _db.Books.Remove(book);
            await _db.SaveChangesAsync();
            ViewBag.Success = "Book deleted successfully";

            return View("Index");
        }

        private bool BookExists(int id)
        {
            return _db.Books.Any(e => e.Id == id);
        }
    }
}
