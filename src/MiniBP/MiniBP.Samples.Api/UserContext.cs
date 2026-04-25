using System;
using Common;
using Microsoft.AspNetCore.Http;

namespace Api.Model;

public class UserContext : IUserContext
{
    //private readonly IHttpContextAccessor contextAccessor;

    public UserContext()
    {
        //this.contextAccessor = contextAccessor;
    }

    // public UserDto CurrentUser => new UserDto(Guid.Parse(contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value),
    // 	contextAccessor.HttpContext.User.FindFirst("preferred_username").Value,
    // 	Convert.ToInt32(contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "org_info_id")?.Value));

    public UserDto CurrentUser => new UserDto(Guid.Parse("8c95a960-33d8-49d6-945d-693ce9db1419"), "rmn", 1);
}
