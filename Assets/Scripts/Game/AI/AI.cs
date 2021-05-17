using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Diagnostics;

public class AI
{
    int depth;
    private Random random = new Random();

    private Stopwatch stopwatch = new Stopwatch();
    private int timeLimit = 5000;

    public AI(int difficulty)
    {
        switch (difficulty)
        {
            case 1:
                depth = 2;
                break;
            case 2:
                depth = 3;
                break;
            case 3:
                depth = 4;
                break;
            default:
                depth = 2;
                break;
        }
    }

    public Move CalculateMove(BoardSimple board)
    {
        var locker = new object();

        var nextStates = board.NextGameStates.OrderBy(x => random.Next()).ToArray();
        if (nextStates.Length == 0)
            return null;

        var bestResult = float.NegativeInfinity;
        var bestMove = nextStates[0].LastMove;

        stopwatch.Start();

        for (int lookAhead = 3; lookAhead <= 6; lookAhead++)
        {
            Parallel.For(0, nextStates.Length, (i) =>
            {
                var result = AlphaBeta(nextStates[i], lookAhead, float.NegativeInfinity, float.PositiveInfinity, board.CurrentPlayer);

                lock (locker)
                {
                    if (result >= bestResult && stopwatch.ElapsedMilliseconds < timeLimit)
                    {
                        bestResult = result;
                        bestMove = nextStates[i].LastMove;
                    }
                }
            });
        }

        return bestMove;
    }

    private float AlphaBeta(BoardSimple board, int depth, float alpha, float beta, int maximizingPlayer)
    {
        if (depth == 0 || board.IsGameOver || stopwatch.ElapsedMilliseconds >= timeLimit)
            return CalculateRating(board, maximizingPlayer);

        if (board.CurrentPlayer == maximizingPlayer)
        {
            var value = float.NegativeInfinity;
            foreach (var child in board.NextGameStates.OrderBy(x => RandomInt()))
            {
                value = Math.Max(value, AlphaBeta(child, depth - 1, alpha, beta, maximizingPlayer));
                alpha = Math.Max(alpha, value);
                if (alpha >= beta)
                    break;
            }
            return value;
        }
        else
        {
            var value = float.PositiveInfinity;
            foreach (var child in board.NextGameStates.OrderBy(x => RandomInt()))
            {
                value = Math.Min(value, AlphaBeta(child, depth - 1, alpha, beta, maximizingPlayer));
                beta = Math.Min(beta, value);
                if (alpha >= beta)
                    break;
            }
            return value;
        }
    }

    private int RandomInt()
    {
        lock (random)
        {
            return random.Next();
        }
    }

    private float CalculateRating(BoardSimple board, int player)
    {
        int otherPlayer = player == 2 ? 1 : 2;

        var playerStones = board.AvailableStones[player - 1] + board.StonesOnBoard[player - 1];
        var otherPlayerStones = board.AvailableStones[otherPlayer - 1] + board.StonesOnBoard[otherPlayer - 1];
        return playerStones - otherPlayerStones;
    }
}
