using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrlFirstStage : MonoBehaviour
{

    public GameObject movePlate;
    [SerializeField] private Controller controllerScript;

    // Start is called before the first frame update
    void Start()
    {
     // controllerScript = GetComponent<Controller>();
    }


    

    public void CreateMovePlates(Game game)
    {
        
        // Creating a piece that would be put on the board
        GameObject obj = Instantiate(controllerScript.piece, new Vector3(-100f, 0f, -1f), Quaternion.identity);
        Piece p = obj.GetComponent<Piece>();
        p.isWhite = game.isWhiteTurn;
        p.Activate();
        p.xBoard = -100;
        p.SetCoords();
        FindObjectOfType<AudioManager>().Play("PieceMenu");
        

        if (controllerScript.difficulty == 0 || game.isWhiteTurn == controllerScript.isPlayerWhite)    // No AI
        {
            // Creating the move plates
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (game.PositionOnBoard(i, j))
                        MovePlateSpawn(i, j, obj);
                }

            }
        }
        else // AI
        {
            AIBehaviour ai = controllerScript.opponent;
            Move move = ai.CalculateMove(game);
            game.pieceReference = obj;
            game.ApplyMove(move);
            game.isWhiteTurn = !game.isWhiteTurn;
        }
    }

    private void MovePlateSpawn(int x, int y, GameObject piece)
    {
        GameObject p = controllerScript.GetPosition(x, y);

        if (p == null)
        {
            float worldX = x - 3;
            float worldY = 3 - y;

            GameObject mp = Instantiate(movePlate, new Vector3(worldX, worldY, -3.0f), Quaternion.identity);
            MovePlate mpScript = mp.GetComponent<MovePlate>();
            mpScript.SetReference(piece);
            mpScript.SetCoords(x, y);

        }
    }
}
