using Domain.Entity;
using Domain.Messages.Chats;
using Domain.Messages.ChatUsers;
using Domain.Messages.Messages;
using Domain.Users;

namespace Application.Services.Messenger;

public class MessengerService(
    EntityStorage entityStorage)
{
    public void Create(string title, Guid creatorId, List<ChatUserCreateDto> userDtos)
    {
        var chat = Chat.Create(title);
        entityStorage.Create(chat);
        entityStorage.Create([
            ChatUser.Create(chat.Id, creatorId, ChatRoleEnum.Creator),
            ..userDtos.Select(chatUserDto => ChatUser.Create(chatUserDto.UserId, chat.Id, chatUserDto.Role))
        ]);
    }

    public void Update(Guid id, ChatUpdateDto dto)
    {
        var chat = entityStorage.Read<Chat>(id);
        if (chat is null)
            throw new KeyNotFoundException();
        chat.Update(dto);
        entityStorage.Update(chat);

        var chatUsers = entityStorage.Select<ChatUser>().Where(x => x.ChatId == chat.Id).ToArray();
        if (dto.RemovedUsersIds != null)
            entityStorage.Delete(chatUsers.Where(x => dto.RemovedUsersIds.Contains(x.UserId)));

        if (dto.AddedUsers != null)
            entityStorage.Create(dto.AddedUsers.Select(x => ChatUser.Create(id, x.UserId, x.Role)));
    }

    public bool UserInChat(Guid userId, Guid chatId)
        => entityStorage.Select<ChatUser>()
            .Where(x => x.ChatId == chatId)
            .Any(x => x.UserId == userId);

    public ChatReadDto Read(Guid id)
    {
        var chat = entityStorage.Read<Chat>(id);
        if (chat is null)
            throw new KeyNotFoundException();
        var messages = entityStorage.Select<Message>()
            .Where(x => x.ChatId == id)
            .ToArray();
        var users = entityStorage.Read<User>(messages.Select(x => x.AuthorId).Distinct().ToArray())
            .ToDictionary(x => x.Id, x => x);

        var result = new ChatReadDto(
            chat.Title,
            messages.Select(x
                => new MessageReadDto(users[x.AuthorId].FullName, x.Text, x.CreationTime)).ToArray());
        return result;
    }
}

public record ChatReadDto(string Title, MessageReadDto[] Messages);


