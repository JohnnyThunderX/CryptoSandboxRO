using System.Diagnostics;
using System.Runtime.InteropServices;
using Spectre.Console;

namespace CryptoSandbox.Courses
{
    public class Utils
    {
        public static void Pause()
        {
            AnsiConsole.Markup("[bold green]APASĂ ORICE PENTRU A CONTINUA[/]");
            Console.ReadKey();
            AnsiConsole.Cursor.MoveUp(1);
            AnsiConsole.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
        }

        public static void CopyToClipboard(string text)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // for Windows
                var powershell = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = $"-Command \"Set-Clipboard -Value '{text}'\"",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                };
                Process.Start(powershell);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // for MacOS
                var process = Process.Start(
                    new ProcessStartInfo("pbcopy")
                    {
                        RedirectStandardInput = true,
                        UseShellExecute = false,
                    }
                );
                process.StandardInput.Write(text);
                process.StandardInput.Close();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // for Linux try xclip (needs to be installed)
                var process = Process.Start(
                    new ProcessStartInfo("xclip", "-selection clipboard")
                    {
                        RedirectStandardInput = true,
                        UseShellExecute = false,
                    }
                );
                process.StandardInput.Write(text);
                process.StandardInput.Close();
            }
        }
    }
}
