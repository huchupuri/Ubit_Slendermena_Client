using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using FluentAssertions;
using NUnit.Framework;
using GameClient.Models;
using GameClient.Network;
using System.Text.Json;
using Ubit_Slendermena_Client.Models;

namespace GameClient.Tests.Network
{
    [TestFixture]
    public class GameClient_MessageTests
    {
        private Mock<ClientWebSocket> _mockSocket;
        private GameClient _client;

        [SetUp]
        public void Setup()
        {
            // Мокаем WebSocket
            _mockSocket = new Mock<ClientWebSocket>();

            // Подменяем метод ReceiveAsync через Reflection
            var mockReceiveMethod = typeof(WebSocket).GetMethod("ReceiveAsync",
                new[] { typeof(ArraySegment<byte>), typeof(CancellationToken) });

            _mockSocket.Setup(m => mockReceiveMethod.Invoke(m.Object, It.IsAny<object[]>()))
                .ReturnsAsync(new WebSocketReceiveResult(4, WebSocketMessageType.Text, false));

            // Создаем клиент с подмененным сокетом
            _client = new GameClient("ws://localhost")
            {
                _webSocket = _mockSocket.Object,
                _isConnected = true,
                _cts = new CancellationTokenSource()
            };
        }

        [Test]
        public async Task WhenServerSendsCategoryList_ShouldRaiseMessageReceivedWithCategories()
        {
            // Arrange
            var tcs = new TaskCompletionSource<ServerMessage>();
            ServerMessage receivedMessage = null!;

            _client.MessageReceived += (sender, msg) =>
            {
                receivedMessage = msg;
                tcs.SetResult(msg);
            };

            // Подменяем ReceiveAsync — имитируем получение сообщения
            var mockBuffer = Encoding.UTF8.GetBytes(@"{
                ""Type"": ""Categories"",
                ""Categories"": [
                    {""Id"": 1, ""Name"": ""История""},
                    {""Id"": 2, ""Name"": ""География""}
                ]
            }");

            _mockSocket.Setup(s => s.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new WebSocketReceiveResult(mockBuffer.Length, WebSocketMessageType.Text, true));

            // Act
            await SimulateIncomingMessage(_client, mockBuffer);

            // Assert
            var result = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(3));
            result.Should().NotBeNull();
            result.Type.Should().Be("Categories");
            result.Categories.Should().HaveCount(2);
            result.Categories[0].Name.Should().Be("История");
            result.Categories[1].Name.Should().Be("География");
        }

        [Test]
        public async Task WhenServerSendsPlayerData_ShouldRaiseMessageReceivedWithPlayer()
        {
            // Arrange
            var tcs = new TaskCompletionSource<ServerMessage>();
            ServerMessage receivedMessage = null!;

            _client.MessageReceived += (sender, msg) =>
            {
                receivedMessage = msg;
                tcs.SetResult(msg);
            };

            var mockBuffer = Encoding.UTF8.GetBytes(@"{
                ""Type"": ""LoginResult"",
                ""Player"": {
                    ""Id"": ""9f6579dd-2d8b-4d3e-ba1f-20ad8a32f58b"",
                    ""Username"": ""testuser"",
                    ""TotalGames"": 5,
                    ""Score"": 1000,
                    ""Wins"": 3
                }
            }");

            _mockSocket.Setup(s => s.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new WebSocketReceiveResult(mockBuffer.Length, WebSocketMessageType.Text, true));

            // Act
            await SimulateIncomingMessage(_client, mockBuffer);

            // Assert
            var result = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(3));
            result.Should().NotBeNull();
            result.Type.Should().Be("LoginResult");
            result.Player.Should().NotBeNull();
            result.Player.Username.Should().Be("testuser");
            result.Player.Score.Should().Be(1000);
            result.Player.Wins.Should().Be(3);
        }

        [Test]
        public async Task WhenServerSendsAnswerResult_ShouldRaiseMessageReceivedWithCorrectFlag()
        {
            // Arrange
            var tcs = new TaskCompletionSource<ServerMessage>();
            ServerMessage receivedMessage = null!;

            _client.MessageReceived += (sender, msg) =>
            {
                receivedMessage = msg;
                tcs.SetResult(msg);
            };

            var mockBuffer = Encoding.UTF8.GetBytes(@"{
                ""Type"": ""AnswerResult"",
                ""IsCorrect"": true,
                ""CorrectAnswer"": ""Москва"",
                ""NewScore"": 500
            }");

            _mockSocket.Setup(s => s.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new WebSocketReceiveResult(mockBuffer.Length, WebSocketMessageType.Text, true));

            // Act
            await SimulateIncomingMessage(_client, mockBuffer);

            // Assert
            var result = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(3));
            result.Should().NotBeNull();
            result.Type.Should().Be("AnswerResult");
            result.IsCorrect.Should().BeTrue();
            result.CorrectAnswer.Should().Be("Москва");
            result.NewScore.Should().Be(500);
        }

        private async Task SimulateIncomingMessage(GameClient client, byte[] messageBytes)
        {
            var buffer = new ArraySegment<byte>(messageBytes);
            var receiveTask = Task.Run(async () =>
            {
                await client.ReceiveMessagesAsync();
            });

            // Даем время на обработку
            await Task.Delay(500);
        }
    }
}