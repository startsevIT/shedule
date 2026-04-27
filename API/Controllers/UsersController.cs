using Application.Services.Users;
using Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class UsersController(UserService service) : ControllerBase
{
    [HttpPost("/entrants")]
    public async Task<IActionResult> Register([FromBody] EntrantRegisterDto[] dtos)
    {
        await service.RegisterEntrant(dtos);
        return Created();
    }

    [HttpPost("/manager")]//TODO, Authorize(Roles = Role.Workers)]
    public async Task<IActionResult> Register([FromBody] ManagerRegisterDto[] dtos)
    {
        await service.RegisterManager(dtos);
        return Created();
    }

    [HttpPost("/students")]//TODO, Authorize(Roles = Role.Workers)]
    public async Task<IActionResult> Register([FromBody] StudentRegisterDto[] dtos)
    {
        await service.RegisterStudents(dtos);
        return Created();
    }

    [HttpPost("/teachers")]//TODO, Authorize(Roles = Role.Workers)]
    public async Task<IActionResult> Register([FromBody] TeacherRegisterDto[] dtos)
    {
        await service.RegisterTeachers(dtos);
        return Created();
    }

    [HttpPost("/login")]
    public IActionResult Login([FromBody] UserLoginDto dto)
    {
        try
        {
            var token = service.Login(dto);
            return Ok(token);
        }
        catch
        {
            return Unauthorized();
        }
    }

    [HttpGet("/account"), Authorize]
    public IActionResult GetAccount()
        => Ok(service.GetAccount(GetUserIdByClaims()));

    [HttpPut("/account/password"), Authorize]
    public IActionResult UpdatePassword(PasswordUpdateDto dto)
    {
        service.UpdatePassword(GetUserIdByClaims(), dto);
        return Ok();
    }

    [HttpPut("/account/{id:guid}")]//TODO, Authorize(Roles = Role.Workers)]
    public IActionResult Update(Guid id, UserUpdateDto dto)
    {
        service.Update(id, dto);
        return Ok();
    }
}