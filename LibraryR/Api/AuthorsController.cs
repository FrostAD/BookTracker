using LibraryR.Data;
using LibraryR.Service;
using LibraryR.ViewModels;
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
    public class AuthorsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IUserService userService;

        public AuthorsController(ApplicationDbContext context, IUserService userService)
        {
            this.context = context;
            this.userService = userService;
        }
        //DataTables
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
                int authorsTotal = 0;

                IQueryable<Models.Author> authorsData = null;
                if (User.IsInRole("Admin"))
                {
                    authorsData = (from author in context.Authors select author);
                }
                else
                {

                authorsData = (from author in context.Authors.Where(a => a.UserId == userService.GetUserId()) select author);
                }

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    authorsData = authorsData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                int year = 0;
                bool isYear = int.TryParse(searchValue, out year);
                if (!string.IsNullOrEmpty(searchValue))
                {
                    authorsData = authorsData.Where(m => m.FirstName.ToLower().Contains(searchValue.ToLower()) || m.LastName.ToLower().Contains(searchValue.ToLower()) || m.BornYear == year);
                }
                authorsTotal = authorsData.Count();
                var data = authorsData.Skip(skip).Take(pageSize).ToList();
                var jsonData = new { draw = draw, recordsFiltered = authorsTotal, recordsTotal = authorsTotal, data = data };
                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //Select2
        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> Search(string term)
        {
            if (!string.IsNullOrEmpty(term))
            {
                var authors = await context.Authors.Where(a => a.UserId == userService.GetUserId()).ToListAsync();
                int year = 0;
                bool isYear = int.TryParse(term, out year);
                var data = authors.Where(a => a.FirstName.ToLower().Contains(term.ToLower())
                || (a.LastName is null ? false : a.LastName.ToLower().Contains(term.ToLower())) || (a.BornYear is null ? false : a.BornYear == year)).ToList().AsReadOnly();
                return Ok(data);
            }
            else
            {
                var authors = await context.Authors.Where(a => a.UserId == userService.GetUserId()).ToListAsync();
                var data = authors.AsReadOnly();
                return Ok(data);
            }
        }
    }
}
