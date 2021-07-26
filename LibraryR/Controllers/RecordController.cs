using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryR.Data;
using LibraryR.Models;
using LibraryR.ViewModels;
using Microsoft.AspNetCore.Identity;
using LibraryR.Service;

namespace LibraryR.Controllers
{
    public class RecordController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        public RecordController(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        // GET: Record
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: Record/Details/5
        [HttpGet("record/details")]
        public async Task<IActionResult> Details(string? bookId, string? userId)
        {
            Record record = _context.Records.Find(bookId, userId);
            if (record is null)
            {
                ViewBag.Error = "Record not found";
                return View("Index");
            }

            RecordIndexVM recIndex = new RecordIndexVM()
            {
                Record = record,
                BookTitle = _context.Books.Find(int.Parse(record.BookId),_userService.GetUserId()).Title,
                StatusString = _context.StatusTypes.Find(int.Parse(record.StatusTypeId)).Type
            };

            return View(recIndex);
        }

        // GET: Record/Create
        public IActionResult Create()
        {
            RecordVM record = new RecordVM()
            {
                Record = new Record(),
                BookDropDown = _context.Books.Select(a => new SelectListItem
                {
                    Text = a.Title,
                    Value = a.Id.ToString()
                }),
                StatusDropDown = _context.StatusTypes.Select(a => new SelectListItem
                {
                    Text = a.Type,
                    Value = a.Id.ToString()
                })
            };
            return View(record);
        }

        // POST: Record/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RecordVM r)
        {
            var sBook = Request.Form["ajaxSelect2Book"];
            var sStatus = Request.Form["ajaxSelect2"];
            if (sBook.Count == 0)
            {
                ViewBag.Error = "Select book";
                return View(r);
            }else if(sStatus.Count == 0)
            {
                ViewBag.Error = "Set status";
                return View(r);
            }

            Book selectedBook = _context.Books.Find(Int32.Parse(sBook),_userService.GetUserId());
            

            r.Record.UserId = _userService.GetUserId();
            r.Record.BookId = sBook;
            r.Record.StatusTypeId = sStatus;
            if (_context.Records.Find(r.Record.BookId,r.Record.UserId) != null)
            {
                ViewBag.Error = "Record is already added";
                return View("Index");

            }

            _context.Records.Add(r.Record);
            _context.SaveChanges();
            ViewBag.Success = "Record added successfully";

            return View("Index");
        }

        // GET: Record/Edit/5
        [HttpGet("record/edit")]
        public async Task<IActionResult> Edit(string? bookId, string? userId)
        {
            Record selectedRecord = _context.Records.Find(bookId, userId);
            if (selectedRecord == null)
            {
                ViewBag.Error = "Record not found";
                return View("Index");
            }

            Book book = _context.Books.Find(int.Parse(selectedRecord.BookId),_userService.GetUserId());
            StatusType status = _context.StatusTypes.Find(int.Parse(selectedRecord.StatusTypeId));
            List<Book> lBook = new List<Book>();
            lBook.Add(book);
            List<StatusType> lStatus = new List<StatusType>();
            lStatus.Add(status);
            RecordVM record = new RecordVM()
            {
                Record = selectedRecord,
                BookDropDown = lBook.Select(a => new SelectListItem
                {
                    Text = a.Title,
                    Value = a.Id.ToString(),
                    Selected = true
                }),
                StatusDropDown = lStatus.Select(a => new SelectListItem
                {
                    Text = a.Type,
                    Value = a.Id.ToString(),
                    Selected = true
                })

            };

            return View(record);
        }

        // POST: Record/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RecordVM recordVm)
        {
            var selectedBook = Request.Form["ajaxSelect2RecordEditBook"];
            var selectedStatus = Request.Form["ajaxSelect2RecordEditStatus"];

            recordVm.Record.BookId = selectedBook;
            recordVm.Record.StatusTypeId = selectedStatus;

            _context.Update(recordVm.Record);
            _context.SaveChanges();
            ViewBag.Success = "Record edited successfully";

            RecordIndexVM recIndex = new RecordIndexVM()
            {
                Record = recordVm.Record,
                BookTitle = _context.Books.Find(int.Parse(recordVm.Record.BookId), _userService.GetUserId()).Title,
                StatusString = _context.StatusTypes.Find(int.Parse(recordVm.Record.StatusTypeId)).Type
            };

            return View("Details", recIndex);
        }

        // GET: Record/Delete/5
        [HttpGet("record/delete")]

        public async Task<IActionResult> Delete(string? bookId, string? userId)
        {
            Record record = _context.Records.Find(bookId, userId);
            if (record == null)
            {
                ViewBag.Error = "Record not found";
                return View("Index");
            }
            return View(@record);
        }

        // POST: Record/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string? bookId, string? userId)
        {
            Record record = _context.Records.Find(bookId, userId);

            _context.Records.Remove(record);
            _context.SaveChanges();
            ViewBag.Success = "Record deleted successfully";

            return View("Index");
        }

        private bool RecordExists(string id)
        {
            return _context.Records.Any(e => e.BookId == id);
        }
    }
}
