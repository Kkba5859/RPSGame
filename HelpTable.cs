using Spectre.Console;

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