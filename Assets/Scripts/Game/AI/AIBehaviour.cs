using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading.Tasks;

public class AIBehaviour : MonoBehaviour
{
    //Game game;
    Move move = null;

    public int difficulty { set; get; } = 1;

    // Start is called before the first frame update
    void Start()
    {
        //GameObject.Find("Controller").GetComponent<Controller>().getGame();
    }

    public Move CalculateMove(Game game)
    {
        //Move move = null;
        AI ai = new AI(difficulty);
        move = ai.CalculateMove(game.Simplyfy());

        return move;
    }
}
