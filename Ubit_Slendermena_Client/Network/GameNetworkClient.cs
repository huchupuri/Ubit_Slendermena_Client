using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using GameClient.Models;

namespace GameClient.Network;

public class GameNetworkClient
{
    private TcpClient? _client;
    private NetworkStream? _stream;
    private bool _isConnected;
    private Thread? _receiveThread;

    public event Action<ServerMessage>? MessageReceived;
    public event Action? Disconnected;

    public bool IsConnected => _isConnected;

    public async Task<bool> ConnectAsync(string serverAddress, int port)
    {
        try
        {
            _client = new TcpClient();
            await _client.ConnectAsync(serverAddress, port);
            _stream = _client.GetStream();
            _isConnected = true;

            _receiveThread = new Thread(ReceiveMessages) { IsBackground = true };
            _receiveThread.Start();

            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    public void Disconnect()
    {
        _isConnected = false;
        _stream?.Close();
        _client?.Close();
        Disconnected?.Invoke();
    }

    public async Task SendMessageAsync(object message)
    {
        if (!_isConnected || _stream == null) return;

        try
        {
            string json = JsonSerializer.Serialize(message);
            byte[] data = Encoding.UTF8.GetBytes(json);
            await _stream.WriteAsync(data);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка отправки сообщения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ReceiveMessages()
    {
        byte[] buffer = new byte[4096];

        try
        {
            while (_isConnected && _stream != null)
            {
                int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    MessageBox.Show("Соединение закрыто сервером.", "Разрыв соединения", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                }

                // Копируем только реально полученные байты
                byte[] receivedData = new byte[bytesRead];
                Buffer.BlockCopy(buffer, 0, receivedData, 0, bytesRead);

                string json = Encoding.UTF8.GetString(receivedData);

                try
                {
                    var message = JsonSerializer.Deserialize<ServerMessage>(json);
                    

                    if (message != null)
                    {
                        MessageReceived?.Invoke(message);
                    }

                }
                catch (JsonException ex)
                {
                    MessageBox.Show($"Ошибка разбора JSON: {ex.Message}\nДанные: {json}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        catch (IOException)
        {
            MessageBox.Show("Соединение потеряно.", "Ошибка сети", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Непредвиденная ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Disconnect();
        }
    }
}