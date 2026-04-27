namespace Application.Services.Users;

public class EmailBodyGeneratedService
{
    private string StudyReference { get; set; } = ""; //todo ссылка
    public string GenerateEmailBody(string fullName, string email, string password)
    {
        return $"""

                            <html>
                            <body>
                                <h2>Добро пожаловать в студенческий сервис!</h2>
                                <p>Уважаемый(ая) {fullName},</p>
                                <p>Ваши учётные данные для входа:</p>
                                <ul>
                                    <li><strong>Логин:</strong> {email}</li>
                                    <li><strong>Пароль:</strong> {password}</li>
                                </ul>
                                <p>Для входа перейдите на <a href='{StudyReference}'>наш сайт</a>.</p>
                                <p><em>Пожалуйста, сохраните эти данные в надёжном месте.</em></p>
                                <hr>
                                <p>С уважением, команда студенческого сервиса</p>
                            </body>
                            </html>
                """;
    }
}