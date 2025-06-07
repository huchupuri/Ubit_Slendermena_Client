
//using System;
//using System.Net.WebSockets;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using Moq;
//using FluentAssertions;
//using NUnit.Framework;
//using GameClient.Models;
//using GameClient.Network;
//using Ubit_Slendermena_Client;
//using System.Windows.Forms;
//namespace GameClient.Tests.Network
//{
//    [TestFixture]
//    public class GameClient_MessageTests
//    {
//        private Mock<ClientWebSocket> _mockSocket;
//        private GameClient.Network.GameClient _client;
//        public async Task<bool> ConnectToServerAsync()
//        {
//            try
//            {
//                await _client.ConnectAsync();
//                return true;
//            }
//            catch (Exception ex)
//            {
//                return false;
//            }
//        }
//        [SetUp]
//        public void Setup()
//        {

//            // клиент с подмененным сокетом
//            _client = new GameClient.Network.GameClient("ws://localhost")
//            {
//                _webSocket = _mockSocket.Object,
//                _isConnected = true,
//                _cts = new CancellationTokenSource()
//            };
//        }

//        [Test]
//        public async Task WhenServerSendsCategoryList_ShouldRaiseMessageReceivedWithCategories()
//        {
//            var tcs = new TaskCompletionSource<ServerMessage>();
//            _client.MessageReceived += (sender, msg) => tcs.SetResult(msg);

//            var mockBuffer = Encoding.UTF8.GetBytes(@"{
//                ""Type"": ""Categories"",
//                ""Categories"": [
//                    {""Id"": 1, ""Name"": ""История""},
//                    {""Id"": 2, ""Name"": ""География""}
//                ]
//            }");

//            MockReceiveAsync(mockBuffer);
//            await SimulateIncomingMessage(_client, mockBuffer);
//            var result = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(3));
//            result.Should().NotBeNull();
//            result.Type.Should().Be("Categories");
//            result.Categories.Should().HaveCount(2);
//            result.Categories[0].Name.Should().Be("История");
//            result.Categories[1].Name.Should().Be("География");
//        }

//        [Test]
//        public async Task WhenServerSendsPlayerData_ShouldRaiseMessageReceivedWithPlayer()
//        {
//            // Arrange
//            var tcs = new TaskCompletionSource<ServerMessage>();
//            _client.MessageReceived += (sender, msg) => tcs.SetResult(msg);

//            var mockBuffer = Encoding.UTF8.GetBytes(@"{
//                ""Type"": ""Login"",
//                ""Player"": {
//                    ""Username"": ""testuser"",
//                    ""TotalGames"": 5,
//                    ""Score"": 1000,
//                    ""Wins"": 3
//                }
//            }");

//            MockReceiveAsync(mockBuffer);

//            // Act
//            await SimulateIncomingMessage(_client, mockBuffer);

//            // Assert
//            var result = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(3));
//            result.Should().NotBeNull();
//            result.Type.Should().Be("LoginResult");
//            result.Player.Should().NotBeNull();
//            result.Player.Username.Should().Be("testuser");
//            result.Player.Score.Should().Be(1000);
//            result.Player.Wins.Should().Be(3);
//        }
//        [Test]
//        public async Task WhenServerSendsPlayerData_ShouldRaiseMessageReceivedPlayer()
//        {
//            bool connectionResult = await ConnectToServerAsync();
//            // Arrange
//            var tcs = new TaskCompletionSource<ServerMessage>();
//            _client.MessageReceived += (sender, msg) => tcs.SetResult(msg);

//            var mockBuffer = Encoding.UTF8.GetBytes(@"{
//                ""Type"": ""Login"",
//                ""Player"": {
//                    ""Username"": ""testuser"",
//                    ""TotalGames"": 5,
//                    ""Score"": 1000,
//                    ""Wins"": 3
//                }
//            }");

//            MockReceiveAsync(mockBuffer);

//            // Act
//            await SimulateIncomingMessage(_client, mockBuffer);

//            // Assert
//            var result = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(3));
//            result.Should().NotBeNull();
//            result.Type.Should().Be("LoginResult");
//            result.Player.Should().NotBeNull();
//            result.Player.Username.Should().Be("testuser");
//            result.Player.Score.Should().Be(1000);
//            result.Player.Wins.Should().Be(3);
//        }

//        [Test]
//        public async Task WhenServerSendsAnswerResult_ShouldRaiseMessageReceivedWithCorrectFlag()
//        {
//            // Arrange
//            var tcs = new TaskCompletionSource<ServerMessage>();
//            _client.MessageReceived += (sender, msg) => tcs.SetResult(msg);

//            var mockBuffer = Encoding.UTF8.GetBytes(@"{
//                ""Type"": ""AnswerResult"",
//                ""IsCorrect"": true,
//                ""CorrectAnswer"": ""Москва"",
//                ""NewScore"": 500
//            }");

//            MockReceiveAsync(mockBuffer);

//            // Act
//            await SimulateIncomingMessage(_client, mockBuffer);

//            // Assert
//            var result = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(3));
//            result.Should().NotBeNull();
//            result.Type.Should().Be("AnswerResult");
//            result.IsCorrect.Should().BeTrue();
//            result.CorrectAnswer.Should().Be("Москва");
//            result.NewScore.Should().Be(500);
//        }

//        private void MockReceiveAsync(byte[] messageBytes)
//        {
//            _mockSocket.Setup(s => s.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
//                       .ReturnsAsync(new WebSocketReceiveResult(messageBytes.Length, WebSocketMessageType.Text, true));
//        }

//        private async Task SimulateIncomingMessage(GameClient.Network.GameClient client, byte[] messageBytes)
//        {
//            var buffer = new ArraySegment<byte>(messageBytes);
//            var receiveTask = Task.Run(() => client.ReceiveMessagesAsync());

//            // время на обработку
//            await Task.Delay(500);
//        }
//    }
//}
using NUnit.Framework;
using GameClient.Network;
using GameClient.Models;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json;
namespace GameClient.Tests
{
    [TestFixture]
    [Category("Integration")]
    public class GameClientIntegrationNUnitTests
    {
        private GameClient.Network.GameClient _gameClient;
        private string _testServerUrl = "ws://localhost:5000/";

        [SetUp]
        public void Setup()
        {
            _gameClient = new GameClient.Network.GameClient(_testServerUrl);
        }

        [TearDown]
        public void TearDown()
        {
            _gameClient?.Dispose();
        }

        [Test]
        public async Task LoginAsync_WithRealConnection_HandlesServerResponse()
        {
            // Этот тест требует запущенного сервера
            // Arrange
            bool messageReceived = false;
            ServerMessage receivedMessage = null;

            _gameClient.MessageReceived += (sender, message) =>
            {
                receivedMessage = message;
                messageReceived = true;
            };

            try
            {
                // Act
                await _gameClient.ConnectAsync();
                await _gameClient.LoginAsync("testuser", "testpassword");

                // Ждем ответ от сервера
                var timeout = TimeSpan.FromSeconds(5);
                var startTime = DateTime.Now;

                while (!messageReceived && DateTime.Now - startTime < timeout)
                {
                    await Task.Delay(100);
                }

                Assert.That(receivedMessage.Type, Is.EqualTo("LoginSuccess").Or.EqualTo("Error"),
                    "Тип сообщения должен быть LoginSuccess или Error");
            }
            catch (Exception ex) when (ex.Message.Contains("connection") || ex.Message.Contains("подключение"))
            {
                // Пропускаем тест если сервер недоступен
                Assert.Ignore("Тест пропущен: сервер недоступен");
            }
            finally
            {
                await _gameClient.DisconnectAsync();
            }
        }

        [Test]
        public void LoginAsync_InvalidServer_ThrowsException()
        {
            // Arrange
            var invalidClient = new GameClient.Network.GameClient("ws://invalid-server:9999/");

            try
            {
                // Act & Assert
                Assert.ThrowsAsync<Exception>(
                    async () =>
                    {
                        await invalidClient.ConnectAsync();
                        await invalidClient.LoginAsync("testuser", "testpassword");
                    });
            }
            finally
            {
                invalidClient.Dispose();
            }
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        public async Task LoginAsync_MultipleAttempts_HandlesCorrectly(int attemptCount)
        {
            // Arrange
            _gameClient._webSocket = new System.Net.WebSockets.ClientWebSocket();
            _gameClient._isConnected = false;

            // Act & Assert
            for (int i = 0; i < attemptCount; i++)
            {
                Assert.ThrowsAsync<InvalidOperationException>(
                    async () => await _gameClient.LoginAsync($"user{i}", $"pass{i}"));
            }
        }
        [Test]
        [TestCase("user1", "pass1")]
        [TestCase("", "")]
        [TestCase("admin@test.com", "securepass")]
        public void CreateLoginMessage_VariousInputs_SerializesCorrectly(string username, string password)
        {
            // Arrange & Act
            var message = new
            {
                Type = "Login",
                Username = username,
                Password = password
            };

            string json = JsonSerializer.Serialize(message);

            // Assert
            Assert.That(json, Is.Not.Null);
            Assert.That(json, Does.Contain("\"Type\":\"Login\""));
            Assert.That(json, Does.Contain($"\"Username\":\"{username}\""));
            Assert.That(json, Does.Contain($"\"Password\":\"{password}\""));
        }
    }
}
