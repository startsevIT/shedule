using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public abstract class ControllerBase : Controller
{
    protected ActionResult ResultByUserId<T>(Func<Guid, T> func) where T : class
    {
        var userId = GetUserIdByClaims();

        return Ok(() => func(userId));
    }

    protected Guid GetUserIdByClaims()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            throw new Exception("User ID not found or invalid");
        return userId;
    }
}