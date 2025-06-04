using GameClient.Models;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace GameClient.Network
{
    public class GameClient
    {
        private ClientWebSocket _webSocket;
        private readonly string _serverUrl;
        private bool _isConnected;
        private CancellationTokenSource _cts;


        public event EventHandler<ServerMessage> MessageReceived;
        public event EventHandler<string> ConnectionClosed;
        public event EventHandler<Exception> ErrorOccurred;

        public bool IsConnected => _isConnected && _webSocket?.State == WebSocketState.Open;

        public GameClient(string serverUrl)
        {
            _serverUrl = serverUrl;
            _cts = new CancellationTokenSource();
        }

        public async Task ConnectAsync()
        {
            try
            {
                // Создаем новый WebSocket если предыдущий был закрыт
                if (_webSocket?.State != WebSocketState.Open)
                {
                    _webSocket?.Dispose();
                    _webSocket = new ClientWebSocket();
                    _cts?.Cancel();
                    _cts = new CancellationTokenSource();
                }

                await _webSocket.ConnectAsync(new Uri(_serverUrl), _cts.Token);
                _isConnected = true;

                // Запускаем прослушивание сообщений
                _ = Task.Run(ReceiveMessagesAsync);
            }
            catch (Exception ex)
            {
                _isConnected = false;
                ErrorOccurred?.Invoke(this, ex);
                throw;
            }
        }

        public async Task SendMessageAsync(object message)
        {
            if (!IsConnected)
                throw new InvalidOperationException($"Клиент {_isConnected}, {_webSocket?.State} не подключен к серверу");

            try
            {
                string jsonMessage = JsonSerializer.Serialize(message);
                byte[] messageBytes = Encoding.UTF8.GetBytes(jsonMessage);

                await _webSocket.SendAsync(new ArraySegment<byte>(messageBytes),
                    WebSocketMessageType.Text, true, _cts.Token);
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, ex);
                throw;
            }
        }

        private async Task ReceiveMessagesAsync()
        {
            byte[] buffer = new byte[4096];

            try
            {
                while (_isConnected && _webSocket.State == WebSocketState.Open && !_cts.Token.IsCancellationRequested)
                {
                    WebSocketReceiveResult result = await _webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer), _cts.Token);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                            "Закрытие по запросу сервера", CancellationToken.None);
                        _isConnected = false;
                        ConnectionClosed?.Invoke(this, "Соединение закрыто сервером");
                        Console.WriteLine("закрыто");
                        break;
                    }

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string messageText = Encoding.UTF8.GetString(buffer, 0, result.Count);

                        // Преобразуем строку в объект ServerMessage
                        ServerMessage serverMessage = ServerMessage.FromJson(messageText);

                        // Вызываем событие с объектом ServerMessage
                        MessageReceived?.Invoke(this, serverMessage);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Нормальная отмена операции

                _isConnected = false;
            }
            catch (WebSocketException ex)
            {
                if (!IsConnected)
                    throw new InvalidOperationException($"Клиент {_isConnected}, {_webSocket?.State} не подключен к серверу");
                _isConnected = false;
                ConnectionClosed?.Invoke(this, $"Соединение прервано: {ex.Message}");
            }
            catch (Exception ex)
            {
                _isConnected = false;
                ErrorOccurred?.Invoke(this, ex);
            }
        }

        public async Task LoginAsync(string username, string password)
        {
            await SendMessageAsync(new
            {
                Type = "Login",
                Username = username,
                Password = password
            });
        }

        public async Task StartGameAsync(byte playerCount)
        {
            await SendMessageAsync(new
            {
                Type = "StartGame",
                playerCount
            });
        }

        public async Task SelectQuestionAsync(int categoryId)
        {
            await SendMessageAsync(new
            {
                Type = "SelectQuestion",
                CategoryId = categoryId
            });
        }

        public async Task SubmitAnswerAsync(int questionId, string answer)
        {
            await SendMessageAsync(new
            {
                Type = "Answer",
                QuestionId = questionId,
                Answer = answer
            });
        }

        public async Task DisconnectAsync()
        {
            if (_isConnected && _webSocket?.State == WebSocketState.Open)
            {
                try
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                        "Клиент отключается", CancellationToken.None);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при отключении: {ex.Message}");
                }
            }

            _isConnected = false;
            _cts?.Cancel();
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _webSocket?.Dispose();
            _cts?.Dispose();
        }
    }
}
