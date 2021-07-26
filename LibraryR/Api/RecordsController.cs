using LibraryR.Data;
using LibraryR.Models;
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
    public class RecordsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IUserService userService;

        public RecordsController(ApplicationDbContext context, IUserService userService)
        {
            this.context = context;
            this.userService = userService;
        }
        [HttpPost]
        public IActionResult GetCustomers()
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
                int recordsTotal = 0;
                IQueryable<Models.Record> recs = null;
                if (User.IsInRole("Admin"))
                {
                    recs = context.Records;

                }
                else
                {
                    recs = context.Records.Where(a => a.UserId == userService.GetUserId());
                }
                List<RecordIndexVM> recsIndex = new List<RecordIndexVM>();
                foreach (var r in recs)
                {
                    StatusType status = context.StatusTypes.Find(int.Parse(r.StatusTypeId));
                    recsIndex.Add(new RecordIndexVM
                    {
                        Record = r,
                        BookTitle = context.Books.Find(int.Parse(r.BookId), r.UserId).Title,
                        StatusString = status.Type
                    });

                }

                var recordsData = (from record in recsIndex.AsQueryable() select record);
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    recordsData = recordsData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    recordsData = recordsData.Where(m => m.BookTitle.ToLower().Contains(searchValue.ToLower()) || m.StatusString.ToLower().Contains(searchValue.ToLower()));
                }
                recordsTotal = recordsData.Count();
                var data = recordsData.Skip(skip).Take(pageSize).ToList();
                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data };
                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //GETS STATUS NOT RECORDS
        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> Search(string term)
        {
            if (!string.IsNullOrEmpty(term))
            {
                var status = await context.StatusTypes.ToListAsync();
                var data = status.Where(a => a.Type.Contains(term, StringComparison.OrdinalIgnoreCase)
                ).ToList().AsReadOnly();
                return Ok(data);
            }
            else
            {
                var status = await context.StatusTypes.ToListAsync();
                var data = status.AsReadOnly();
                return Ok(data);
            }
        }
    }
}
