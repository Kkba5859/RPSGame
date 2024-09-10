interface IGameRules
{
    string DetermineWinner(int userMove, int computerMove);
}

// Class for circular game rules
class CircularGameRules : IGameRules
{
    private readonly string[] _moves;

    /// <summary>
    /// Initializes a new instance of the <see cref="CircularGameRules"/> class.
    /// </summary>
    /// <param name="moves">An array of strings representing the game moves.</param>
    public CircularGameRules(string[] moves)
    {
        _moves = moves;
    }

    /// <summary>
    /// Determines the winner of a game based on the user's move and the computer's move.
    /// </summary>
    /// <param name="userMove">The user's move, represented as an integer.</param>
    /// <param name="computerMove">The computer's move, represented as an integer.</param>
    /// <returns>A string indicating the result of the game, either "Draw", "You win!", or "You lose!"</returns>
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
