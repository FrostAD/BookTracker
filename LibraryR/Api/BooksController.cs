using LibraryR.Data;
using LibraryR.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;    
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace LibraryR.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IUserService userService;

        public BooksController(ApplicationDbContext context, IUserService userService)
        {
            this.context = context;
            this.userService = userService;
        }
        [HttpPost]
        public IActionResult GetAuthors()
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int booksTotal = 0;

                IQueryable<Models.Book> booksData = null;
                if (User.IsInRole("Admin"))
                {
                    booksData = (from book in context.Books select book);
                }
                else
                {
                    booksData = (from book in context.Books.Where(b => b.UserId == userService.GetUserId()) select book);

                }

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    booksData = booksData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                int year;
                bool isYear = int.TryParse(searchValue, out year);
                if (!string.IsNullOrEmpty(searchValue))
                {
                    booksData = booksData.Where(m => m.Title.ToLower().Contains(searchValue.ToLower()) || m.Year == year);
                }
                booksTotal = booksData.Count();
                var data = booksData.Skip(skip).Take(pageSize).ToList();
                var jsonData = new { draw = draw, recordsFiltered = booksTotal, recordsTotal = booksTotal, data = data };
                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> Search(string term)
        {
            if (!string.IsNullOrEmpty(term))
            {
                var books = await context.Books.Where(b => b.UserId == userService.GetUserId()).ToListAsync();
                int year = 0;
                bool isYear = int.TryParse(term, out year);
                var data = books.Where(a => a.Title.ToLower().Contains(term.ToLower()) || a.Year == year
                ).ToList().AsReadOnly();
                return Ok(data);
            }
            else
            {
                var books = await context.Books.Where(b => b.UserId == userService.GetUserId()).ToListAsync();
                var data = books.AsReadOnly();
                return Ok(data);
            }
        }
    }
}
