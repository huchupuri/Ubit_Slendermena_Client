using Ubit_Slendermena_Client.Network;

namespace Ubit_Slendermena_Client.Forms;

public partial class LoginForm : Form
{
    private TextBox txtServerAddress;
    private TextBox txtPort;
    private TextBox txtPlayerName;
    private Button btnConnect;
    private Label lblServerAddress;
    private Label lblPort;
    private Label lblPlayerName;

    public string ServerAddress => txtServerAddress.Text;
    public int Port => int.TryParse(txtPort.Text, out int port) ? port : 5000;
    public string PlayerName => txtPlayerName.Text;

    public LoginForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "����������� � ���� '���� ����'";
        this.Size = new Size(400, 250);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        // �����    
        lblServerAddress = new Label
        {
            Text = "����� �������:",
            Location = new Point(20, 20),
            Size = new Size(100, 23),
            TextAlign = ContentAlignment.MiddleLeft
        };

        lblPort = new Label
        {
            Text = "����:",
            Location = new Point(20, 60),
            Size = new Size(100, 23),
            TextAlign = ContentAlignment.MiddleLeft
        };

        lblPlayerName = new Label
        {
            Text = "��� ������:",
            Location = new Point(20, 100),
            Size = new Size(100, 23),
            TextAlign = ContentAlignment.MiddleLeft
        };

        // ���� �����
        txtServerAddress = new TextBox
        {
            Text = "localhost",
            Location = new Point(130, 20),
            Size = new Size(200, 23)
        };

        txtPort = new TextBox
        {
            Text = "5000",
            Location = new Point(130, 60),
            Size = new Size(200, 23)
        };

        txtPlayerName = new TextBox
        {
            Location = new Point(130, 100),
            Size = new Size(200, 23)
        };

        // ������ �����������
        btnConnect = new Button
        {
            Text = "������������",
            Location = new Point(130, 150),
            Size = new Size(120, 30),
            BackColor = Color.LightBlue
        };
        btnConnect.Click += BtnConnect_Click;

        // ���������� ��������� �� �����
        this.Controls.AddRange(new Control[] {
            lblServerAddress, lblPort, lblPlayerName,
            txtServerAddress, txtPort, txtPlayerName,
            btnConnect
        });
    }

    private async void BtnConnect_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtPlayerName.Text))
        {
            MessageBox.Show("������� ��� ������!", "������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        btnConnect.Enabled = false;
        btnConnect.Text = "�����������...";

        var client = new GameNetworkClient();
        bool connected = await client.ConnectAsync(ServerAddress, Port);

        if (connected)
        {
            // ���������� ��������� � �����
            await client.SendMessageAsync(new { Type = "Login", PlayerName = PlayerName });

            // ��������� ������� �����
            var gameForm = new GameForm(client, PlayerName);
            this.Hide();
            gameForm.ShowDialog();
            this.Close();
        }
        else
        {
            btnConnect.Enabled = true;
            btnConnect.Text = "������������";
        }
    }
}