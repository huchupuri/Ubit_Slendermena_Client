using GameClient.Forms;
using Ubit_Slendermena_Client;

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