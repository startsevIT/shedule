using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Domain.Entity;
using Domain.Messages;
using Domain.Messages.Messages;

namespace API.Controllers;





public class WebSocketHub()
{
    private readonly Dictionary<Guid, List<WebSocket>> _connections = new();

    public async Task OpenConnectionAsync(WebSocket ws, Guid userId, Guid chatId, EntityStorage entityStorage)
    {
        AddToChat(chatId, ws);

        var buffer = new byte[4096];
        while (ws.State == WebSocketState.Open)
        {
            var msg = await ws.GetMessageAsync(buffer, chatId, userId);
            entityStorage.Create(msg);
            await SendToChatAsync(chatId, msg);
        }

        LeaveFromChat(chatId, ws);
    }

    private void AddToChat(Guid chatId, WebSocket ws)
    {
        if (!_connections.TryGetValue(chatId, out var list))
            _connections[chatId] = list ??= [];
        list.Add(ws);
    }

    private void LeaveFromChat(Guid chatId, WebSocket ws)
    {
        _connections[chatId].Remove(ws);
        if (_connections[chatId].Count == 0)
            _connections.Remove(chatId);
    }

    private async Task SendToChatAsync(Guid chatId, Message message)
    {
        var tasks = _connections[chatId]
            .Where(s => s.State == WebSocketState.Open)
            .Select(s => s.SendMessageAsync(message));
        await Task.WhenAll(tasks);
    }
}

internal static class WebSocketExtensions
{
    public static async Task<Message> GetMessageAsync(this WebSocket ws,byte[] buffer, Guid chatId, Guid authorId)
    {
        var result = await ws.ReceiveAsync(buffer, CancellationToken.None);
        var text = Encoding.UTF8.GetString(buffer, 0, result.Count);
        return Message.Create(chatId, authorId, text);
    }

    public static async Task SendMessageAsync(this WebSocket ws, Message msg)
    {
        var jsonString = JsonSerializer.Serialize(msg);
        var bytes = Encoding.UTF8.GetBytes(jsonString);
        await ws.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
    }
}