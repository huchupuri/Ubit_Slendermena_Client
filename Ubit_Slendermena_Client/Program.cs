using Ubit_Slendermena_Client.Forms;

namespace Ubit_Slendermena_Client;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new LoginForm());
    }
}