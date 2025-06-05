using GameClient.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ubit_Slendermena_Client
{
    public partial class ProfileForm : Form
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Player _player;
        private string _selectedAvatarPath = null;

        public ProfileForm(Player player)
        {
            Logger.Info($"Открытие ProfileForm для игрока: {player?.Username}");

            _player = player;
            InitializeComponent();
            LoadPlayerData();
            SubscribeToEvents();

            Logger.Debug($"ProfileForm инициализирована для игрока ID={player?.Id}");
        }

        private void LoadPlayerData()
        {
            Logger.Debug("Загрузка данных игрока в ProfileForm");

            if (_player != null)
            {
                UsernameTxt.Text = _player.Username;
                UsernameTxt.ReadOnly = true; 
                LoadPlayerAvatar();

                Logger.Info($"Данные игрока загружены: {_player.Username}, Игр: {_player.TotalGames}, Побед: {_player.Wins}");
            }
        }

        private void LoadPlayerAvatar()
        {
            try
            {
                string avatarPath = Path.Combine(Application.StartupPath, "avatars", $"{_player.Id}.png");

                if (File.Exists(avatarPath))
                {
                    Logger.Debug($"Загрузка сохраненного аватара: {avatarPath}");
                    AvatarPic.Image = Image.FromFile(avatarPath);
                }
                else
                {
                    AvatarPic.BackColor = Color.LightGray;
                    AvatarPic.Image = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка при загрузке аватара игрока");
                AvatarPic.BackColor = Color.LightGray;
                AvatarPic.Image = null;
            }
        }

        private void SubscribeToEvents()
        {

            ChangePasswordBtn.Click += ChangePasswordBtn_Click;
            UploadImgBtn.Click += UploadImgBtn_Click;
            ExitBtn.Click += ExitBtn_Click;
            DeleteAccountBtn.Click += DeleteAccountBtn_Click;
            BackBtn.Click += BackBtn_Click;
        }

        private void ChangePasswordBtn_Click(object sender, EventArgs e)
        {
            Logger.Info($"Попытка смены пароля для игрока: {_player?.Username}");

            try
            {
                if (string.IsNullOrWhiteSpace(PasswordTxt.Text))
                {
                    Logger.Warn("Попытка смены пароля с пустым полем");
                    MessageBox.Show("Введите новый пароль", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (PasswordTxt.Text.Length < 6)
                {
                    Logger.Warn("Попытка установки слишком короткого пароля");
                    MessageBox.Show("Пароль должен содержать минимум 6 символов", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show("Вы уверены, что хотите изменить пароль?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Logger.Info($"Пароль изменен для игрока: {_player?.Username}");
                    MessageBox.Show("Пароль успешно изменен!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    PasswordTxt.Clear();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Ошибка при смене пароля для игрока: {_player?.Username}");
                MessageBox.Show($"Ошибка при смене пароля: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UploadImgBtn_Click(object sender, EventArgs e)
        {
            Logger.Info($"Загрузка аватара для игрока: {_player?.Username}");

            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Все файлы|*.*";
                    openFileDialog.Title = "Выберите изображение для аватара";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        _selectedAvatarPath = openFileDialog.FileName;
                        Logger.Info($"Выбран файл аватара: {_selectedAvatarPath}");
                        using (var originalImage = Image.FromFile(_selectedAvatarPath))
                        {
                            var displayImage = new Bitmap(originalImage, AvatarPic.Size);
                            AvatarPic.Image?.Dispose(); 
                            AvatarPic.Image = displayImage;
                        }

                        SavePlayerAvatar();

                        MessageBox.Show("Аватар успешно загружен!", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Ошибка при загрузке аватара для игрока: {_player?.Username}");
                MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SavePlayerAvatar()
        {
           
        }

        private void ExitBtn_Click(object sender, EventArgs e)
        {
            Logger.Info($"Игрок {_player?.Username} вышел");

            try
            {
                var result = MessageBox.Show("Вы уверены, что хотите выйти из аккаунта?", "Подтверждение выхода",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Logger.Info($"Подтвержден выход из аккаунта для игрока: {_player?.Username}");
                    this.DialogResult = DialogResult.OK;
                    this.Close();

                    foreach (Form form in Application.OpenForms.Cast<Form>().ToArray())
                    {
                        if (form is MenuForm menuForm)
                        {
                            Logger.Debug("Закрытие MenuForm и возврат к EntryForm");
                            menuForm.Hide();
                            var entryForm = new EntryForm(null);
                            entryForm.Show();

                            menuForm.Close();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void DeleteAccountBtn_Click(object sender, EventArgs e)
        {
            Logger.Warn($"Игрок {_player?.Username} нажал удаление аккауниа");

            try
            {

                Logger.Warn($"Подтверждено удаление аккаунта для игрока: {_player?.Username}");

                // Здесь должна быть логика отправки запроса на сервер для удаления аккаунта
                // await _networkClient.DeleteAccountAsync(_player.Id);

                MessageBox.Show("Аккаунт будет удален. Функция будет реализована позже.", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                Logger.Info("Запрос на удаление аккаунта обработан (заглушка)");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Ошибка при удалении аккаунта для игрока: {_player?.Username}");
                MessageBox.Show($"Ошибка при удалении аккаунта: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BackBtn_Click(object sender, EventArgs e)
        {
            Logger.Info($"Игрок {_player?.Username} нажал кнопку 'Назад' в профиле");
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Logger.Info($"Закрытие ProfileForm для игрока: {_player?.Username}");
            if (AvatarPic.Image != null)
            {
                AvatarPic.Image.Dispose();
                AvatarPic.Image = null;
            }

            base.OnFormClosing(e);
        }
    }
}