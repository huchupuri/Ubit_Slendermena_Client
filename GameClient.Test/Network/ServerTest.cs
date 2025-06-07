//using NUnit.Framework;
//using Moq;
//using GameClient.Network;
//using GameClient.Models;
//using System;
//using System.Net.WebSockets;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Text;
//using System.Text.Json;
//using System.Reflection;
//using GameClient.Network;
//using GameClient.Models;
//using System;
//using System.Text.Json;
//using NUnit.Framework;
//using NUnit.Framework.Constraints;

//namespace GameClient.Tests
//{
//    namespace GameClient.Tests
//    {
//        /// <summary>
//        /// Вспомогательные методы для тестирования с NUnit
//        /// </summary>
//        public static class NUnitTestHelpers
//        {
//            /// <summary>
//            /// Создает тестовое сообщение от сервера
//            /// </summary>
//            public static ServerMessage CreateTestServerMessage(string type, string username = null, string message = null)
//            {
//                return new ServerMessage
//                {
//                    Type = type,
//                    Username = username,
//                    Message = message,
//                    Id = Guid.NewGuid(),
//                    TotalGames = 0,
//                    TotalScore = 0,
//                    Wins = 0
//                };
//            }

//            /// <summary>
//            /// Создает JSON строку для тестового сообщения
//            /// </summary>
//            public static string CreateTestMessageJson(string type, string username, string password)
//            {
//                var message = new
//                {
//                    Type = type,
//                    Username = username,
//                    Password = password
//                };
//                return JsonSerializer.Serialize(message);
//            }

//            /// <summary>
//            /// Проверяет корректность JSON сообщения для логина
//            /// </summary>
//            public static bool ValidateLoginMessage(byte[] messageBytes, string expectedUsername, string expectedPassword)
//            {
//                try
//                {
//                    string messageJson = System.Text.Encoding.UTF8.GetString(messageBytes);
//                    var messageObject = JsonSerializer.Deserialize<JsonElement>(messageJson);

//                    return messageObject.GetProperty("Type").GetString() == "Login" &&
//                           messageObject.GetProperty("Username").GetString() == expectedUsername &&
//                           messageObject.GetProperty("Password").GetString() == expectedPassword;
//                }
//                catch
//                {
//                    return false;
//                }
//            }

//            /// <summary>
//            /// Создает тестовые данные для параметризованных тестов
//            /// </summary>
//            public static object[] GetTestCredentials()
//            {
//                return new object[]
//                {
//                new object[] { "user1", "password1" },
//                new object[] { "admin", "admin123" },
//                new object[] { "test@example.com", "securepass" },
//                new object[] { "русский_пользователь", "пароль123" },
//                new object[] { "user with spaces", "pass with spaces" },
//                new object[] { "special!@#$%", "chars^&*()" }
//                };
//            }
//        }

//        /// <summary>
//        /// Пользовательские ограничения для NUnit
//        /// </summary>
//        public static class CustomConstraints
//        {
//            /// <summary>
//            /// Проверяет, что JSON содержит корректное сообщение логина
//            /// </summary>
//            public static LoginMessageConstraint ContainsValidLoginMessage(string expectedUsername, string expectedPassword)
//            {
//                return new LoginMessageConstraint(expectedUsername, expectedPassword);
//            }
//        }

//        public class LoginMessageConstraint : NUnit.Framework.Constraints.Constraint
//        {
//            private readonly string _expectedUsername;
//            private readonly string _expectedPassword;

//            public LoginMessageConstraint(string expectedUsername, string expectedPassword)
//            {
//                _expectedUsername = expectedUsername;
//                _expectedPassword = expectedPassword;
//            }

//            public override ConstraintResult ApplyTo<TActual>(TActual actual)
//            {
//                if (actual is byte[] messageBytes)
//                {
//                    bool isValid = NUnitTestHelpers.ValidateLoginMessage(messageBytes, _expectedUsername, _expectedPassword);
//                    return new ConstraintResult(this, actual, isValid);
//                }

//                return new ConstraintResult(this, actual, false);
//            }

//            public override string Description => $"valid login message with username '{_expectedUsername}' and password '{_expectedPassword}'";
//        }
//    }

//    [TestFixture]
//    public class GameClientNUnitTestsFixed
//    {
//        private GameClient.Network.GameClient _gameClient;
//        private string _testServerUrl = "ws://localhost:5000/";

//        [SetUp]
//        public void Setup()
//        {
//            _gameClient = new GameClient.Network.GameClient(_testServerUrl);
//        }

//        [TearDown]
//        public void TearDown()
//        {
//            _gameClient?.Dispose();
//        }

//        [Test]
//        public void LoginAsync_NotConnected_ThrowsInvalidOperationException()
//        {
//            // Arrange
//            string username = "testuser";
//            string password = "testpassword";

//            // GameClient по умолчанию не подключен
//            Assert.That(_gameClient.IsConnected, Is.False);

//            // Act & Assert
//            var exception = Assert.ThrowsAsync<InvalidOperationException>(
//                async () => await _gameClient.LoginAsync(username, password));

//            Assert.That(exception.Message, Does.Contain("не подключен к серверу"));
//        }

//        [Test]
//        public async Task LoginAsync_EmptyUsername_SendsEmptyUsername()
//        {
//            // Arrange
//            string username = "";
//            string password = "testpassword";

//            // Создаем тестируемый клиент с mock WebSocket
//            var testableClient = new TestableGameClient(_testServerUrl);

//            // Act
//            await testableClient.LoginAsync(username, password);

//            // Assert
//            var sentMessage = testableClient.GetLastSentMessage();
//            Assert.That(sentMessage, Is.Not.Null);
//            Assert.That(sentMessage.GetProperty("Type").GetString(), Is.EqualTo("Login"));
//            Assert.That(sentMessage.GetProperty("Username").GetString(), Is.EqualTo(""));
//            Assert.That(sentMessage.GetProperty("Password").GetString(), Is.EqualTo(password));
//        }

//        [Test]
//        public async Task LoginAsync_NullCredentials_SendsNullValues()
//        {
//            // Arrange
//            string username = null;
//            string password = null;

//            var testableClient = new TestableGameClient(_testServerUrl);

//            // Act
//            await testableClient.LoginAsync(username, password);

//            // Assert
//            var sentMessage = testableClient.GetLastSentMessage();
//            Assert.That(sentMessage, Is.Not.Null);
//            Assert.That(sentMessage.GetProperty("Type").GetString(), Is.EqualTo("Login"));
//            Assert.That(sentMessage.GetProperty("Username").ValueKind, Is.EqualTo(JsonValueKind.Null));
//            Assert.That(sentMessage.GetProperty("Password").ValueKind, Is.EqualTo(JsonValueKind.Null));
//        }

//        [Test]
//        public async Task LoginAsync_ValidCredentials_SendsCorrectMessage()
//        {
//            // Arrange
//            string username = "testuser";
//            string password = "testpassword";

//            var testableClient = new TestableGameClient(_testServerUrl);

//            // Act
//            await testableClient.LoginAsync(username, password);

//            // Assert
//            var sentMessage = testableClient.GetLastSentMessage();
//            Assert.That(sentMessage, Is.Not.Null);
//            Assert.That(sentMessage.GetProperty("Type").GetString(), Is.EqualTo("Login"));
//            Assert.That(sentMessage.GetProperty("Username").GetString(), Is.EqualTo(username));
//            Assert.That(sentMessage.GetProperty("Password").GetString(), Is.EqualTo(password));
//        }

//        [Test]
//        public async Task LoginAsync_SpecialCharacters_HandlesCorrectly()
//        {
//            // Arrange
//            string username = "тест@user#123";
//            string password = "пароль$%^&*()";

//            var testableClient = new TestableGameClient(_testServerUrl);

//            // Act
//            await testableClient.LoginAsync(username, password);

//            // Assert
//            var sentMessage = testableClient.GetLastSentMessage();
//            Assert.That(sentMessage.GetProperty("Username").GetString(), Is.EqualTo(username));
//            Assert.That(sentMessage.GetProperty("Password").GetString(), Is.EqualTo(password));
//        }

//        [Test]
//        [TestCase("user1", "pass1")]
//        [TestCase("admin", "admin123")]
//        [TestCase("test@example.com", "password")]
//        [TestCase("", "password")]
//        [TestCase("username", "")]
//        public async Task LoginAsync_VariousCredentials_SendsCorrectData(string username, string password)
//        {
//            // Arrange
//            var testableClient = new TestableGameClient(_testServerUrl);

//            // Act
//            await testableClient.LoginAsync(username, password);

//            // Assert
//            var sentMessage = testableClient.GetLastSentMessage();
//            Assert.That(sentMessage.GetProperty("Type").GetString(), Is.EqualTo("Login"));

//            if (username == null)
//                Assert.That(sentMessage.GetProperty("Username").ValueKind, Is.EqualTo(JsonValueKind.Null));
//            else
//                Assert.That(sentMessage.GetProperty("Username").GetString(), Is.EqualTo(username));

//            if (password == null)
//                Assert.That(sentMessage.GetProperty("Password").ValueKind, Is.EqualTo(JsonValueKind.Null));
//            else
//                Assert.That(sentMessage.GetProperty("Password").GetString(), Is.EqualTo(password));
//        }

//        [Test]
//        public void LoginAsync_WebSocketSendThrowsException_PropagatesException()
//        {
//            // Arrange
//            string username = "testuser";
//            string password = "testpassword";

//            var faultyClient = new FaultyGameClient(_testServerUrl);

//            // Act & Assert
//            var exception = Assert.ThrowsAsync<InvalidOperationException>(
//                async () => await faultyClient.LoginAsync(username, password));

//            Assert.That(exception.Message, Does.Contain("Test exception"));
//        }
//    }

//    // Тестируемый класс, который наследует от GameClient и позволяет перехватывать отправленные сообщения
//    public class TestableGameClient : GameClient.Network.GameClient
//    {
//        private JsonElement? _lastSentMessage;

//        public TestableGameClient(string serverUrl) : base(serverUrl)
//        {
//            // Симулируем подключенное состояние
//            SetConnectedState(true);
//        }

//        public override async Task SendMessageAsync(object message)
//        {
//            // Перехватываем сообщение вместо отправки
//            string jsonMessage = JsonSerializer.Serialize(message);
//            _lastSentMessage = JsonSerializer.Deserialize<JsonElement>(jsonMessage);

//            // Симулируем успешную отправку
//            await Task.CompletedTask;
//        }

//        public JsonElement? GetLastSentMessage()
//        {
//            return _lastSentMessage;
//        }

//        private void SetConnectedState(bool connected)
//        {
//            // Используем рефлексию для установки приватного поля
//            var field = typeof(GameClient.Network.GameClient).GetField("_isConnected",
//                BindingFlags.NonPublic | BindingFlags.Instance);
//            field?.SetValue(this, connected);
//        }
//    }

//    // Класс для тестирования исключений
//    public class FaultyGameClient : GameClient.Network.GameClient
//    {
//        public FaultyGameClient(string serverUrl) : base(serverUrl)
//        {
//            SetConnectedState(true);
//        }

//        public override async Task SendMessageAsync(object message)
//        {
//            await Task.Delay(1); // Симулируем асинхронную операцию
//            throw new InvalidOperationException("Test exception");
//        }

//        private void SetConnectedState(bool connected)
//        {
//            var field = typeof(GameClient.Network.GameClient).GetField("_isConnected",
//                BindingFlags.NonPublic | BindingFlags.Instance);
//            field?.SetValue(this, connected);
//        }
//    }
//}
