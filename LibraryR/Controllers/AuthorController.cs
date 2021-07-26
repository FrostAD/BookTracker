using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryR.Data;
using LibraryR.Models;
using LibraryR.Service;

namespace LibraryR.Controllers
{
    public class AuthorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;


        public AuthorController(ApplicationDbContext context,IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        // GET: Author
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: Author/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors.Include(a => a.Books)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                ViewBag.Error =  "Author not found";
                return View("Index");
            }

            return View(author);
        }

        // GET: Author/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Author/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Author author)
        {
            if (ModelState.IsValid)
            {
                author.UserId = _userService.GetUserId();
                if (_context.Authors.Contains(author))
                {
                    ViewBag.Error = "Author is already added";
                    return View("Index");
                }

                _context.Add(author);
                await _context.SaveChangesAsync();
                ViewBag.Success = "Author added successfully";
                return View("Index");
            }

            return View(author);
        }

        // GET: Author/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            var author = await _context.Authors.FindAsync(id,_userService.GetUserId());
            if (author == null)
            {
                ViewBag.Error = "Author not found";
                return View("Index");
            }
            return View(author);
        }

        // POST: Author/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,BornYear")] Author author)
        {
            if (id != author.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    author.UserId = _userService.GetUserId();
                    _context.Update(author);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorExists(author.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                ViewBag.Success = "Author edited successfully";

                return View("Details", author);
            }
            return View(author);
        }

        // GET: Author/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {

            var author = await _context.Authors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                ViewBag.Error = "Author not found";
                return View("Index");
            }

            return View(author);
        }

        // POST: Author/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var author = _context.Authors.Include(a => a.Books).FirstOrDefault(a => a.Id == id);
            if (author == null)
            {
                ViewBag.Error = "Author not found";
                return View("Index");
            }
            foreach (var item in author.Books)
            {
                Book book = _context.Books.Include(a => a.Authors).FirstOrDefault(a => a.Id == item.Id);
                var recordsWithBook = _context.Records.Where(r => r.BookId == book.Id.ToString());
                if (recordsWithBook != null)
                    _context.Records.RemoveRange(recordsWithBook);

                if (book.Authors.Count > 1)
                {
                    book.Authors.Remove(author);
                }
                else
                {
                    _context.Books.Remove(book);
                }

            }


            _context.Authors.Remove(author);
            _context.SaveChanges();
            ViewBag.Success = "Author deleted successfully";

            return View("Index");
        }

        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }
    }
}
