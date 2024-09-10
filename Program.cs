class Game
{
    private readonly string[] _moves;
    private readonly IGameRules _rules;
    private readonly IHmacGenerator _hmacGenerator;
    private readonly IHelpTable _helpTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        /// <param name="moves">An array of strings representing the game moves.</param>
        /// <param name="rules">An instance of the <see cref="IGameRules"/> interface representing the game rules.</param>
        /// <param name="hmacGenerator">An instance of the <see cref="IHmacGenerator"/> interface representing the HMAC generator.</param>
        /// <param name="helpTable">An instance of the <see cref="IHelpTable"/> interface representing the help table.</param>
    public Game(string[] moves, IGameRules rules, IHmacGenerator hmacGenerator, IHelpTable helpTable)
    {
        _moves = moves;
        _rules = rules;
        _hmacGenerator = hmacGenerator;
        _helpTable = helpTable;
    }

    // Starts the game by generating a HMAC key, making a computer move, and 
    // displaying the HMAC. Then, it enters a loop where it continuously prompts 
    // the user for input until a valid move is made or the game is exited.
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

    /// <summary>
    /// Displays the game menu to the user, showing available moves and options to exit or view help.
    /// </summary>
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

    // The main entry point of the Rock Paper Scissors game application.
    // It takes an array of string arguments representing the game moves.
    // The function initializes the game rules, HMAC generator, and help table based on the input arguments.
    // It then starts the game using the initialized game object.
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