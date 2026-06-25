using Application.Services.Messenger;
using Domain.Entity;
using Domain.Messages;
using Domain.Messages.Chats;
using Domain.Messages.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/* todo
Создание чата +
Добавление пользователей в чат +
Вход в чат ??
Проверка что пользователь в чате +
Подгрузка старых сообщений +
Подключение к чату +
    Проверка, что пользователь в чате +
    Отправка сообщения +
    Сохранение сообщения в базе +
    Рассылка сообщения всем в чате +
*/

[Route("api/[controller]")]
public class MessengerController(
    MessengerService service,
    EntityStorage entityStorage,
    WebSocketHub hub) : ControllerBase
{
    [HttpPost, Authorize]
    public void Post([FromBody] ChatCreateDto dto)
        => service.Create(dto.Title, GetUserIdByClaims(), dto.AddedUsers);

    //todo Добавление пользователей, удаление, обновление заголовка - стоит разделить, думаю
    [HttpPut("{chatId:guid}"), Authorize]
    public void Put([FromBody] ChatUpdateDto dto, Guid chatId)
        => service.Update(chatId, dto);

    [HttpGet("{chatId:guid}"), Authorize]
    public IActionResult Get(Guid chatId)
    {
        var userId = GetUserIdByClaims();
        if (!service.UserInChat(chatId, userId))
            return BadRequest();
        return Ok(service.Read(chatId));
    }

    [HttpGet("connect/{chatId:guid}"), Authorize]
    public async Task Connect(Guid chatId)
    {
        var userId = GetUserIdByClaims();
        if (!service.UserInChat(userId, chatId))
            return;

        var webSocketManager = HttpContext.WebSockets;
        if (!webSocketManager.IsWebSocketRequest)
            return;

        var ws = await webSocketManager.AcceptWebSocketAsync();
        await hub.OpenConnectionAsync(ws, userId, chatId, entityStorage);
    }
}

public record ChatCreateDto(string Title, List<ChatUserCreateDto> AddedUsers);