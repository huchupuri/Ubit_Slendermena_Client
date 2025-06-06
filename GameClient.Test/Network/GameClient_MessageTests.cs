
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

namespace GameClient.Tests.Network
{
    [TestFixture]
    public class GameClient_MessageTests
    {
        private Mock<ClientWebSocket> _mockSocket;
        private GameClient.Network.GameClient _client;

        [SetUp]
        public void Setup()
        {
            _mockSocket = new Mock<ClientWebSocket>();

            // клиент с подмененным сокетом
            _client = new GameClient.Network.GameClient("ws://localhost")
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
            _client.MessageReceived += (sender, msg) => tcs.SetResult(msg);

            var mockBuffer = Encoding.UTF8.GetBytes(@"{
                ""Type"": ""Categories"",
                ""Categories"": [
                    {""Id"": 1, ""Name"": ""История""},
                    {""Id"": 2, ""Name"": ""География""}
                ]
            }");

            MockReceiveAsync(mockBuffer);

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
            _client.MessageReceived += (sender, msg) => tcs.SetResult(msg);

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

            MockReceiveAsync(mockBuffer);

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
            _client.MessageReceived += (sender, msg) => tcs.SetResult(msg);

            var mockBuffer = Encoding.UTF8.GetBytes(@"{
                ""Type"": ""AnswerResult"",
                ""IsCorrect"": true,
                ""CorrectAnswer"": ""Москва"",
                ""NewScore"": 500
            }");

            MockReceiveAsync(mockBuffer);

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

        private void MockReceiveAsync(byte[] messageBytes)
        {
            _mockSocket.Setup(s => s.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(new WebSocketReceiveResult(messageBytes.Length, WebSocketMessageType.Text, true));
        }

        private async Task SimulateIncomingMessage(GameClient.Network.GameClient client, byte[] messageBytes)
        {
            var buffer = new ArraySegment<byte>(messageBytes);
            var receiveTask = Task.Run(() => client.ReceiveMessagesAsync());

            // время на обработку
            await Task.Delay(500);
        }
    }
}
