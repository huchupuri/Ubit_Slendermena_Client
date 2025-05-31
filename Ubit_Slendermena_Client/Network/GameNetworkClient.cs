using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using GameClient.Models;

namespace GameClient.Network;

public class GameNetworkClient
{
    private TcpClient? _client; private NetworkStream? _stream; private bool _isConnected; private Thread? _receiveThread;

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
        var buffer = new byte[4096];
        var stringBuilder = new StringBuilder();

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

                // Добавляем полученные данные к строковому буферу
                string chunk = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                stringBuilder.Append(chunk);

                string fullBuffer = stringBuilder.ToString();
                int newLineIndex;

                // Обрабатываем каждое JSON-сообщение по строкам (\n)
                while ((newLineIndex = fullBuffer.IndexOf('\n')) >= 0)
                {
                    string line = fullBuffer[..newLineIndex].Trim(); // строка без \n
                    fullBuffer = fullBuffer[(newLineIndex + 1)..];   // остаток
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    try
                    {
                        var message = JsonSerializer.Deserialize<ServerMessage>(line);
                        if (message != null)
                        {
                            MessageReceived?.Invoke(message);
                        }
                    }
                    catch (JsonException ex)
                    {
                        MessageBox.Show($"Ошибка разбора JSON: {ex.Message}\nДанные: {line}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                // Обновляем stringBuilder новым остатком
                stringBuilder.Clear();
                stringBuilder.Append(fullBuffer);
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
