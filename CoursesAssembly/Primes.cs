using System.Diagnostics;
using System.Numerics;
using CryptoSandbox.Engine;
using Spectre.Console;
using static CryptoSandbox.Courses.Utils;

namespace CryptoSandbox.Courses
{
    public class Primes
    {
        private static void introduction() =>
            AnsiConsole.MarkupLine(
                "[cyan bold]Explicație[/]:"
                    + "\n\t[blue]• Ce avem[/]: [italic magenta]p[/] și [italic magenta]q[/]"
                    + " 2 numere [magenta]prime[/]"
                    + "\n\t[blue]• Ce facem[/]:"
                    + " calculăm produsul lor, [magenta italic]N[/]"
                    + "\n\t[red]• Problema[/]:"
                    + " pentru a afla numerele [italic magenta]p[/] și [italic magenta]q[/] trebuie să încercăm toate combinațiile de numere prime până la [magenta]√N[/]"
            );

        private static void keypoints() =>
            AnsiConsole.MarkupLine(
                "[cyan bold]Puncte cheie[/]:"
                    + "\n\t[blue]•[/] [white bold]Factorizarea numerelor prime[/]"
                    + "\n\t[blue]•[/] [white bold]Ghicirea brute-force a numerelor prime inițiale[/]"
                    + "\n\t[blue]•[/] [white bold]Dificultatea „spargerii” numerelor de 100+ cifre[/]"
                    + "\n\t[blue]•[/] [white bold]Exemple de numere folosite în agoritmii din prezent[/]"
            );

        private static void FirstTest()
        {
            AnsiConsole.Clear();
            int fails = 0,
                maxfails = 5;
            Markup contents = new Markup(
                "1) Vei primi un număr de tip [magenta italic]N[/] format prin înmulțire."
                    + "\n2) Va trebui să ghicești din [red bold]5 încercări[/] perechea de numere prime [italic magenta] p[/] și [italic magenta]q[/] ce au dus la [magenta italic]N[/]."
                    + "\n3) Numerele prime alese vor fi între [magenta bold]5 și 50[/]."
                    + "\n[cyan bold]SUCCES![/]"
            );
            var panel = new Panel(contents)
                .Header("TESTUL 1 - Hands-on Experiment")
                .RoundedBorder()
                .BorderColor(Color.Aquamarine1)
                .Expand();
            AnsiConsole.Write(panel);
            Pause();
            long p = PrimeMathEngine.GetRandomPrime(5, 50),
                q = PrimeMathEngine.GetRandomPrime(5, 50),
                n = p * q;
            AnsiConsole.MarkupLine($"[yellow]Numărul N:[/] [magenta bold]{n}[/]");
            AnsiConsole.MarkupLine("[yellow]Ce numere crezi că au fost înmulțite?[/]");
            bool correct = false;
            while (fails < 5 && !correct)
            {
                string input = AnsiConsole.Ask<string>(
                    $"[bold white]Încercarea {fails + 1}/{maxfails}:[/] "
                );
                string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (
                    parts.Length == 2
                    && long.TryParse(parts[0], out long userP)
                    && long.TryParse(parts[1], out long userQ)
                )
                {
                    if (userP * userQ == n)
                    {
                        AnsiConsole.MarkupLine("[bold green]Corect![/]");
                        correct = true;
                        break;
                    }
                    else
                    {
                        fails++;
                        AnsiConsole.MarkupLine(
                            "[red]Greșit![/]" + $" {userP} × {userQ} = {userP * userQ}"
                        );
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine(
                        "[red]Format invalid![/] Te rog scrie două numere separate prin spațiu."
                    );
                }
            }
            if (correct)
                AnsiConsole.MarkupLine(
                    $"[bold green]Bravo! Ai găsit numerele prime corecte din[/] {fails + 1} [bold green]încercări.[/]"
                );
            else
                AnsiConsole.MarkupLine(
                    $"[bold red]Ups! Se pare că nu ai găsit numerele în limita încercărilor. Răspunsul corect era:[/] [magenta]{p}[/] × [magenta]{q}[/] = [blue]{n}[/]"
                );
        }

        private static void SecondTest()
        {
            int samples = 25; // if higher it might hit long limit

            AnsiConsole.Clear();
            Markup contents = new Markup(
                $"0) Dacă la primul test ai văzut cât de dificil poate fi pentru un om să rezolve factorizarea numerelor prime,\n   acum [magenta]vei asista[/] PC-ul tău la rezolvarea a {samples} de astfel de probleme."
                    + $"\n1) Calculatorul generează {samples} numere [italic magenta]N = p × q[/] din ce în ce mai mari."
                    + "\n2) [bold yellow]Brute-force automat:[/] Procesorul caută divizorii până la [magenta]√N[/]."
                    + "\n3) Urmărește coloana de [cyan]Timp[/] pentru a vedea creșterea dificultății."
            );

            var panel = new Panel(contents)
                .Header("TESTUL 2 - Analiza Performanței")
                .RoundedBorder()
                .BorderColor(Color.Aquamarine1)
                .Expand();

            AnsiConsole.Write(panel);
            Pause();

            List<double> times = new List<double>();

            var table = new Table()
                .Border(TableBorder.Rounded)
                .Expand()
                .AddColumn("[yellow]Nr.[/]")
                .AddColumn("[magenta]N (Produs)[/]")
                .AddColumn("[white]Factorul[/] [green italic bold]p[/]")
                .AddColumn("[white]Factorul[/] [green italic bold]q[/]")
                .AddColumn("[cyan]Milisecunde[/]")
                .AddColumn("[blue]Secunde[/]");
            ;
            long lastP = 0,
                lastQ = 0;
            AnsiConsole
                .Live(table)
                .Start(ctx =>
                {
                    for (int c = 0; c < samples; c++)
                    {
                        // c = 0  => max ~ 1.000
                        // c = 29 => max ~ 3.000.000.000 (3 mld)
                        // N will be at most 9 * 10^18 (long limit)
                        long min = (long)(100 * Math.Pow(1.7, c));
                        long max = (long)(1000 * Math.Pow(1.8, c));

                        long p = PrimeMathEngine.GetRandomPrime(min, max);
                        long q = PrimeMathEngine.GetRandomPrime(min, max);
                        if (p > 0 && q > long.MaxValue / p)
                        {
                            // if it overflows, force it to something a little bit smaller
                            q = long.MaxValue / p;
                        }
                        long n = p * q;

                        // --- MEASUREMENT START ---
                        Stopwatch sw = Stopwatch.StartNew();

                        long foundP = -1;
                        for (long i = 2; i <= Math.Sqrt(n); i++)
                        {
                            if (n % i == 0)
                            {
                                foundP = i;
                                break;
                            }
                        }
                        sw.Stop();
                        // --- MEASUREMENT STOP ---

                        // computing the values for display
                        double ms = sw.Elapsed.TotalMilliseconds,
                            s = sw.Elapsed.TotalSeconds;
                        times.Add(ms);
                        lastP = foundP;
                        lastQ = n / foundP;
                        table.AddRow(
                            "[yellow]" + (c + 1).ToString() + "[/]",
                            n.ToString("N0"),
                            foundP.ToString("N0"),
                            (n / foundP).ToString("N0"),
                            $"[cyan bold]{ms:F4} ms[/]",
                            $"[blue bold]{s:F4} s[/]"
                        );

                        ctx.Refresh();
                    }
                });
            AnsiConsole.WriteLine();
            Pause();
            var chart = new BarChart()
                .Width(60)
                .Label("[green bold]Evoluția Timpului de Calcul (ms)[/]")
                .CenterLabel();

            for (int i = 0; i < times.Count; i++)
            {
                // generate bars
                if (i < 15 && i % 3 == 0) // skip most possible small values -> green
                    chart.AddItem($"Test {i + 1}:", Math.Round(times[i], 2), Color.Green);
                else if (i < 20 && i >= 15) // show all medium values -> yellow
                    chart.AddItem($"Test {i + 1}:", Math.Round(times[i], 2), Color.Yellow);
                else if (i >= 20) // show all high values -> red
                    chart.AddItem($"Test {i + 1}:", Math.Round(times[i], 2), Color.Red);
            }

            AnsiConsole.Write(chart);
            Pause();
            Console.WriteLine();
            AnsiConsole.Write(
                new Rule("[yellow]Proiecție Teoretică (Brute-Force)[/]").RuleStyle("grey")
            );
            Console.WriteLine();
            void PrintProjection(int digits)
            {
                // compute the number of digits for last tested N
                double currentDigits = Math.Floor(Math.Log10(lastP * lastQ) + 1);
                double extraDigits = digits - currentDigits;

                // Brute-Force difficulty incrcease rate: ~O(sqrt(N))
                // N gains 1 more digit (~10 times the previous N), sqrt(N) increases sqrt(10) times (~3.16)
                double lastTimeMs = times.Last();

                // estimated time in years
                double years =
                    (lastTimeMs / 1000.0)
                    * Math.Pow(Math.Sqrt(10), extraDigits)
                    / (3600 * 24 * 365.25);

                // text formatting
                string timeStr;
                if (years > 1e12)
                    timeStr = "Trilioane de ani (Peste vârsta Universului)";
                else if (years > 1e9)
                    timeStr = $"{years / 1e9:F1} Miliarde de ani";
                else if (years > 1e6)
                    timeStr = $"{years / 1e6:F1} Milioane de ani";
                else if (years >= 1)
                    timeStr = $"{years:N0} ani";
                else
                    timeStr = "Sub un an (necesită numere mai mari pentru siguranță)";

                AnsiConsole.MarkupLine(
                    $"[blue]•[/] Pentru [magenta bold]{digits} cifre[/]: [red]{timeStr}[/]"
                );
            }

            PrintProjection(10);
            PrintProjection(30);
            PrintProjection(50);
            PrintProjection(55);
            PrintProjection(100);
            PrintProjection(512);
            PrintProjection(1024);
        }

        private static void ThirdTest()
        {
            AnsiConsole.Clear();
            Markup contents = new Markup(
                $"0) La testul anterior ai văzut cât de mult crește durata de „spargere” a unui număr cu cât are\n   mai multe cifre. Ai mai observat probabil că de la circa 15 cifre totul durează brusc\n   mult mai mult decât numărul anterior."
                    + $"\n1) Această demonstrație este despre [magenta]RSA[/]: acum vei vedea cum arată de fapt [magenta]numerele folosite în criptografia adevărată[/]."
                    + "\n2) Vei vedea, de asemenea, ce numere folosesc algoritmii cei mai cunoscuți."
                    + "\n3) Anterior am ajuns la concluzia că un număr de [magenta]~15-21 de cifre[/] poate fi spart în [red]sub 10 secunde[/], dar chiar și vizual, între acesta și [magenta]RSA-2048 (~617 cifre)[/] este o diferență [red bold]colosală[/]."
            );

            var panel = new Panel(contents)
                .Header("TESTUL 3 - Giganții RSA")
                .RoundedBorder()
                .BorderColor(Color.Aquamarine1)
                .Expand();

            AnsiConsole.Write(panel);
            Pause();

            int[] bitSizes = { 512, 1024, 2048, 4096 };

            foreach (int bits in bitSizes)
            {
                AnsiConsole
                    .Status()
                    .Spinner(Spinner.Known.Dots)
                    .Start(
                        $"Generăm un număr prim de [bold cyan]{bits} biți[/]...",
                        ctx =>
                        {
                            Stopwatch sw = new();
                            sw.Start();
                            BigInteger bigPrime = PrimeMathEngine.GetRandomBigInt(bits);
                            sw.Stop();
                            string rawNumber = bigPrime.ToString();
                            int digits = rawNumber.Length;
                            // display inside the panel
                            var content = new Panel(
                                new Markup(
                                    rawNumber
                                        + $"\n   [blue bold]Generată în: {sw.Elapsed.TotalMilliseconds} ms[/]"
                                )
                            )
                                .Header($"Cheie de {bits} biți (~{digits} cifre)")
                                .RoundedBorder()
                                .BorderColor(Color.Magenta)
                                .Expand();

                            AnsiConsole.Write(content);
                        }
                    );
                Pause();
            }
            AnsiConsole.MarkupLine(
                "[white bold]Ai observat că generarea unor numere atât de mari este aproape instantanee?\n\tȘi totuși este imposibilă rezolvarea acestor chei în prezent...[/]"
            );
            AnsiConsole.MarkupLine(
                "\n[green underline]AICI SE ÎNCHEIE PRIMUL SET DE DEMONSTRAȚII[/]"
            );
            AnsiConsole.MarkupLine("[blue underline]Ce ai experimentat:[/]");
            AnsiConsole.MarkupLine(
                "[blue bold]•[/] [white bold]Cum funcționează factorizarea numerelor prime[/]."
                    + "\n[blue bold]•[/] [white bold]Cât de mult durează să „spargi” un astfel de număr față de cât durează să îl generezi[/]."
                    + "\n[blue bold]•[/] [white bold]De ce contează numărul de cifre în criptografie[/]"
            );
        }

        public static void Run()
        {
            introduction();
            keypoints();
            bool back = false;
            while (!back)
            {
                Console.WriteLine();
                var test = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow bold]Alege un test dintre cele de mai jos:[/]")
                        .PageSize(4)
                        .HighlightStyle(Color.Magenta)
                        .MoreChoicesText(
                            "[grey](Folosește tastele-săgeți pentru a naviga în meniu)[/]"
                        )
                        .AddChoices(
                            "Factorizarea - experiment",
                            "Spargerea numerelor",
                            "RSA",
                            "Înapoi"
                        )
                        .AddCancelResult("none")
                );
                switch (test)
                {
                    case "Factorizarea - experiment":
                        FirstTest();
                        break;
                    case "Spargerea numerelor":
                        SecondTest();
                        break;
                    case "RSA":
                        ThirdTest();
                        break;
                    case "Înapoi":
                        back = true;
                        break;
                    default:
                        break;
                }
            }
            return;
        }
    }
}
