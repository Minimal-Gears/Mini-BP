using System;
using Common;
using Microsoft.AspNetCore.Http;

namespace Api.Model
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor contextAccessor;

        public UserContext(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        // public UserDto CurrentUser => new UserDto(Guid.Parse(contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value),
        // 	contextAccessor.HttpContext.User.FindFirst("preferred_username").Value,
        // 	Convert.ToInt32(contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "org_info_id")?.Value));

        public UserDto CurrentUser => new UserDto(Guid.Empty, "rmn", 1);
    }
}