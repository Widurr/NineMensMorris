using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading.Tasks;

public class AIBehaviour : MonoBehaviour
{
    Game game;
    Move move = null;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("Controller").GetComponent<Controller>().getGame();
    }


    private Move CalculateMove(int difficulty)
    {
        //Move move = null;
        AI ai = new AI(difficulty);
        move = ai.CalculateMove(game.Simplyfy());

        return move;
    }
}
