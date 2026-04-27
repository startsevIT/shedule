using Application.Services.Pairs;
using Domain.Pairs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController, Route("api/[controller]")]
public class PairController(PairService service) : ControllerBase
{
    [HttpPost("/excel")]
    public async Task<IActionResult> PostExcel(IFormFile? file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Файл не выбран или пустой");
        var allowedExtensions = new[] { ".xlsx", ".xls" };

        var fileName = file.FileName;
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            return BadRequest($"Разрешены только файлы: {string.Join(", ", allowedExtensions)}");

        var filePath = Path.Combine("/", $"{fileName}.xls");

        await using (var stream = new FileStream(filePath, FileMode.Create))
            await file.CopyToAsync(stream);

        try
        {
            var data = ExcelParser.Parse(filePath);
            System.IO.File.Delete(filePath);
            try
            {
                service.Create(data.ToArray());
                return Created();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        catch (Exception ex)
        {
            System.IO.File.Delete(filePath);
            return StatusCode(500, $"Ошибка парсинга: {ex.Message}");
        }
    }

    [HttpPost("/filter"), Authorize]
    public IActionResult Get(GetPairModel model) =>
        ResultByUserId(id =>
            model.UserId == null
                ? service.Read(model with { UserId = id })
                : service.Read(model));

    [HttpPost]
    public IActionResult Post(PairCreateDto[] dtos)
    {
        try
        {
            service.Create(dtos);
            return Created();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}