using GameClient.Forms;

namespace GameClient;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new ConnectionForm());
    }
}