using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Spectre.Console;

// Interface for generating HMAC
interface IHmacGenerator
{
    string GenerateKey();
    string ComputeHmac(string key, string message);
}

// Class for generating HMAC using SHA256
class Sha256HmacGenerator : IHmacGenerator
{

    public string GenerateKey()
    {
        var key = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(key);
        }
        return BitConverter.ToString(key).Replace("-", "").ToLower();
    }

    public string ComputeHmac(string key, string message)
    {
        var keyBytes = Enumerable.Range(0, key.Length / 2)
            .Select(x => Convert.ToByte(key.Substring(x * 2, 2), 16))
            .ToArray();
        using (var hmac = new HMACSHA256(keyBytes))
        {
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}

// Interface for game rules
interface IGameRules
{
    string DetermineWinner(int userMove, int computerMove);
}

// Class for circular game rules
class CircularGameRules : IGameRules
{
    private readonly string[] _moves;

    public CircularGameRules(string[] moves)
    {
        _moves = moves;
    }

    public string DetermineWinner(int userMove, int computerMove)
    {
        if (userMove == computerMove) return "Draw";

        int n = _moves.Length;
        int half = n / 2;

        if ((computerMove > userMove && computerMove <= userMove + half) ||
            (computerMove < userMove && computerMove + n <= userMove + half))
        {
            return "You lose!";
        }
        else
        {
            return "You win!";
        }
    }
}

// Interface for displaying help table
interface IHelpTable
{
    void Display();
}

// Class for displaying help table
class GameHelpTable : IHelpTable
{
    private readonly string[] _moves;
    private readonly IGameRules _rules;
    private const int PageSize = 10; 

    public GameHelpTable(string[] moves, IGameRules rules)
    {
        _moves = moves;
        _rules = rules;
    }

    public void Display()
    {
        int n = _moves.Length;
        int totalColumnPages = (int)Math.Ceiling((double)n / PageSize);
        int currentColumnPage = 1;

        while (true)
        {
            var table = new Table
            {
                Border = TableBorder.Rounded,
                Title = new TableTitle($"Help Table - Columns Page {currentColumnPage}/{totalColumnPages}")
            };

            table.AddColumn(new TableColumn("PC \\ User").Centered());
            int startColumn = (currentColumnPage - 1) * PageSize;
            int endColumn = Math.Min(startColumn + PageSize, n);
            for (int i = startColumn; i < endColumn; i++)
            {
                table.AddColumn(new TableColumn(_moves[i]).Centered());
            }

            for (int i = 0; i < n; i++)
            {
                var row = new List<Markup> { new Markup(_moves[i]) };
                for (int j = startColumn; j < endColumn; j++)
                {
                    string result;
                    if (i == j)
                    {
                        result = "[yellow]Draw[/]";
                    }
                    else
                    {
                        result = _rules.DetermineWinner(i, j) == "You win!" ? "[green]Win[/]" : "[red]Lose[/]";
                    }
                    row.Add(new Markup(result));
                }
                table.AddRow(row.ToArray());
            }

            AnsiConsole.Clear();
            AnsiConsole.Write(table);

            Console.WriteLine("\nCommands: [n]ext, [p]revious, [e]xit");
            var input = Console.ReadLine()?.Trim().ToLower();

            switch (input)
            {
                case "n":
                    if (currentColumnPage < totalColumnPages) currentColumnPage++;
                    break;
                case "p":
                    if (currentColumnPage > 1) currentColumnPage--;
                    break;
                case "e":
                    return;
                default:
                    Console.WriteLine("Invalid command. Use [n]ext, [p]revious, or [e]xit.");
                    break;
            }
        }
    }
}
// Class for game
class Game
{
    private readonly string[] _moves;
    private readonly IGameRules _rules;
    private readonly IHmacGenerator _hmacGenerator;
    private readonly IHelpTable _helpTable;

    public Game(string[] moves, IGameRules rules, IHmacGenerator hmacGenerator, IHelpTable helpTable)
    {
        _moves = moves;
        _rules = rules;
        _hmacGenerator = hmacGenerator;
        _helpTable = helpTable;
    }

    public void Start()
    {
        string key = _hmacGenerator.GenerateKey();
        int computerMove = new Random().Next(_moves.Length);
        string hmac = _hmacGenerator.ComputeHmac(key, _moves[computerMove]);

        Console.WriteLine("HMAC: " + hmac);
        DisplayMenu();

    

        string input;
        while (true)
        {
            input = Console.ReadLine()?.Trim() ?? "";
            if (input == "0")
            {
                Console.WriteLine("Exiting...");
                return;
            }
            else if (input == "?")
            {
                _helpTable.Display();
                DisplayMenu();
                continue;
            }

            if (int.TryParse(input, out int userMove) && userMove > 0 && userMove <= _moves.Length)
            {
                userMove -= 1;
                Console.WriteLine($"Your move: {_moves[userMove]}");
                Console.WriteLine($"Computer move: {_moves[computerMove]}");
                Console.WriteLine(_rules.DetermineWinner(userMove, computerMove));
                Console.WriteLine("HMAC key: " + key);
                Console.WriteLine("\nYou can verify the SHA256 hash using this online calculator:");
                Console.WriteLine("https://www.xorbin.com/tools/sha256-hash-calculator");
                break;
            }
            else
            {
                Console.WriteLine("Invalid input. Please try again.");
                DisplayMenu();
            }
        }
    }

    private void DisplayMenu()
    {
        Console.WriteLine("Available moves:");
        for (int i = 0; i < _moves.Length; i++)
        {
            Console.WriteLine($"{i + 1} - {_moves[i]}");
        }
        Console.WriteLine("0 - exit");
        Console.WriteLine("? - help");
        Console.Write("Enter your move: ");
    }

    static void Main(string[] args)
    {
        if (args.Length < 3 || args.Length % 2 == 0 || args.Distinct().Count() != args.Length)
        {
            Console.WriteLine("Error: Invalid arguments. You must provide an odd number (>= 3) of unique moves.");
            Console.WriteLine("Example: dotnet run Rock Paper Scissors");
            return;
        }

        var rules = new CircularGameRules(args);
        var hmacGenerator = new Sha256HmacGenerator();
        var helpTable = new GameHelpTable(args, rules);
        var game = new Game(args, rules, hmacGenerator, helpTable);

        game.Start();
    }
}