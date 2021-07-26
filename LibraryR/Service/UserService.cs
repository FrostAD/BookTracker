using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LibraryR.Service
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContext;

        public UserService(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }
        public string GetUserId()
        {
            string id = _httpContext.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier).ToString();
            id = id.Substring(id.LastIndexOf(':')+2);
            return id;
        }
    }
}
