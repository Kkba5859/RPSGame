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
