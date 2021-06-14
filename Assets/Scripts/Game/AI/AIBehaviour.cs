using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading.Tasks;

public class AIBehaviour : MonoBehaviour
{
    //Game game;
    Move move = null;

    private TranspositionTable transpositionTable;
    private int tstSize = 999999;

    public int difficulty { set; get; } = 1;

    // Start is called before the first frame update
    void Start()
    {
        //GameObject.Find("Controller").GetComponent<Controller>().getGame();
        transpositionTable = new TranspositionTable(tstSize);
    }

    public Move CalculateMove(Game game)
    {
        //Move move = null;
        AI ai = new AI(difficulty, transpositionTable);
        move = ai.CalculateMove(game.Simplyfy());

        return move;
    }
}
