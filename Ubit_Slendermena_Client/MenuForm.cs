using GameClient;
using GameClient.Models;
using GameClient.Network;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ubit_Slendermena_Client
{
    public partial class MenuForm : Form
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Player _player;
        private readonly GameClient.Network.GameClient _client;

        public MenuForm(Player player, GameClient.Network.GameClient client)
        {
            Logger.Info($"Инициализация MenuForm для игрока: {player?.Username}");

            InitializeComponent();
            _player = player;
            _client = client;

            Logger.Debug($"MenuForm создана для игрока ID={player?.Id}, Username={player?.Username}");
            
            // Отображаем информацию о игроке в заголовке или статусной строке
            UpdatePlayerInfo();
            SubscribeToClientEvents();
        }

        private void UpdatePlayerInfo()
        {
            if (_player != null)
            {
                // Обновляем заголовок окна с информацией о игроке
                this.Text = $"Главное меню - {_player.Username} (Побед: {_player.Wins}/{_player.TotalGames})";
                Logger.Debug($"Обновлена информация о игроке в заголовке: {_player.Username}");
            }
        }

        private void SubscribeToClientEvents()
        {
            Logger.Debug("Подписка на события клиента в MenuForm");
            if (_client != null)
            {
                _client.ConnectionClosed += OnConnectionClosed;
                _client.ErrorOccurred += OnErrorOccurred;
            }
        }

        private void UnsubscribeFromClientEvents()
        {
            Logger.Debug("Отписка от событий клиента в MenuForm");
            if (_client != null)
            {
                _client.ConnectionClosed -= OnConnectionClosed;
                _client.ErrorOccurred -= OnErrorOccurred;
            }
        }

        private void HandleGameStarted()
        {
            Logger.Info("Переход к форме создания новой игры");
            var gameForm = new NewGame(_player, _client);
            UnsubscribeFromClientEvents();
            this.Hide();
            gameForm.ShowDialog();
            this.Close();
        }

        private void OnConnectionClosed(object sender, string reason)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, string>(OnConnectionClosed), sender, reason);
                return;
            }

            Logger.Warn($"Соединение с сервером потеряно в MenuForm: {reason}");
            MessageBox.Show($"Соединение с сервером потеряно: {reason}", "Соединение потеряно",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            ReturnToEntryForm();
        }

        private void OnErrorOccurred(object sender, Exception ex)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, Exception>(OnErrorOccurred), sender, ex);
                return;
            }

            Logger.Error(ex, "Произошла ошибка в MenuForm");
            MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ReturnToEntryForm()
        {
            try
            {
                Logger.Info("Возврат к форме входа из MenuForm");
                UnsubscribeFromClientEvents();
                this.Hide();

                var entryForm = new EntryForm(_client);
                entryForm.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, "Критическая ошибка при возврате к форме входа");
                MessageBox.Show($"Ошибка при возврате к форме входа: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void ProfileBtn_Click(object sender, EventArgs e)
        {
            Logger.Info($"Открытие профиля игрока: {_player?.Username}");
            
            try
            {
                var profileForm = new ProfileForm(_player);
                var result = profileForm.ShowDialog();
                
                // Если пользователь вышел из аккаунта через профиль
                if (result == DialogResult.OK)
                {
                    Logger.Info("Пользователь вышел из аккаунта через профиль");
                    // Форма профиля уже обработала выход, просто закрываем MenuForm
                    this.Close();
                }
                else
                {
                    // Обновляем информацию о игроке на случай изменений
                    UpdatePlayerInfo();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Ошибка при открытии профиля для игрока: {_player?.Username}");
                MessageBox.Show($"Ошибка при открытии профиля: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LocalizationBtn_Click(object sender, EventArgs e)
        {
            Logger.Info("Нажата кнопка локализации");
            
            try
            {
                // Показываем диалог выбора языка
                var languageDialog = MessageBox.Show(
                    "Выберите язык:\n\nДа - Русский\nНет - English\nОтмена - Отменить",
                    "Выбор языка",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                switch (languageDialog)
                {
                    case DialogResult.Yes:
                        Logger.Info("Выбран русский язык");
                        MessageBox.Show("Русский язык уже установлен", "Язык",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case DialogResult.No:
                        Logger.Info("Выбран английский язык");
                        MessageBox.Show("English language will be implemented later", "Language",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    default:
                        Logger.Debug("Выбор языка отменен");
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка при смене языка");
                MessageBox.Show($"Ошибка при смене языка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void PlayBtn_Click(object sender, EventArgs e)
        {
            Logger.Info($"Игрок {_player?.Username} нажал кнопку 'Играть'");

            try
            {
                // Отключаем кнопку во время обработки
                PlayBtn.Enabled = false;
                PlayBtn.Text = "Создание игры...";

                // Проверяем соединение с сервером
                if (!_client.IsConnected)
                {
                    Logger.Warn("Попытка создания игры без соединения с сервером");
                    MessageBox.Show("Нет соединения с сервером", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Logger.Debug("Открытие формы создания новой игры");
                // Открываем форму создания новой игры
                var newGameForm = new NewGame(_player, _client);
                UnsubscribeFromClientEvents();
                this.Hide();
                newGameForm.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Ошибка при создании игры для игрока {_player?.Username}");
                MessageBox.Show($"Ошибка при создании игры: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Восстанавливаем состояние кнопки
                PlayBtn.Enabled = true;
                PlayBtn.Text = "Играть";
                Logger.Debug("Состояние кнопки 'Играть' восстановлено");
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Logger.Info($"Закрытие MenuForm для игрока: {_player?.Username}");

            // Отписываемся от событий и отключаемся от сервера при закрытии формы
            UnsubscribeFromClientEvents();

            if (_client != null && _client.IsConnected)
            {
                try
                {
                    Logger.Debug("Отключение от сервера при закрытии MenuForm");
                    _client.DisconnectAsync().Wait(1000); // Ждем максимум 1 секунду
                    Logger.Info("Успешное отключение от сервера");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Ошибка при отключении от сервера в MenuForm");
                    Console.WriteLine($"Ошибка при отключении: {ex.Message}");
                }
            }

            base.OnFormClosing(e);
        }
    }
}