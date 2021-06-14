using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    public GameObject controller;

    [SerializeField] GameObject reference = null;

    // board, not world positions
    int boardX, boardY;

    public bool isKillerPlate = false;

    public void OnMouseUp()
    {
            controller = GameObject.FindGameObjectWithTag("GameController");
            Controller script = controller.GetComponent<Controller>();
            Piece refPiece = reference.GetComponent<Piece>();
        if (!isKillerPlate)
        {

            script.SetPositionEmpty(refPiece.xBoard, refPiece.yBoard);

            refPiece.xBoard = boardX;
            refPiece.yBoard = boardY;
            refPiece.SetCoords();

            script.SetPosition(reference);
            var game = script.getGame();
            if(game.gameState == Game.GameState.placing)
            {
                if (game.isWhiteTurn)
                    game.whitePiecesPlaced++;
                else
                    game.blackPiecesPlaced++;
            }

            refPiece.DestroyMovePlates();
            if(script.IsInMill(reference))
                KillerPlateSpawn(!script.isWhiteTurn);
            script.isWhiteTurn = !script.isWhiteTurn;
            FindObjectOfType<AudioManager>().Play("PiecePut");


            // AI
            /*
            var game = script.getGame();
            if (game.gameState == Game.GameState.moving && script.difficulty > 0 && script.isWhiteTurn != script.isPlayerWhite)
            {
                AIBehaviour ai = script.opponent;
                
                Move move = ai.CalculateMove(game);
                game.ApplyMove(move);
                script.isWhiteTurn = !script.isWhiteTurn;
            }
            */
        }
        else
        {
            script.SetPositionEmpty(refPiece.xBoard, refPiece.yBoard);
            refPiece.DestroyMovePlates();
            FindObjectOfType<AudioManager>().Play("PieceDestroyed");
            Destroy(reference);
        }
      script.DeclareWinner();
    }

    public void SetCoords(int x, int y)
    {
        boardX = x;
        boardY = y;
    }

    public void SetReference(GameObject obj)
    {
        reference = obj;
    }

    private void MakeKiller(GameObject obj)
    {
        isKillerPlate = true;
        reference = obj;
        gameObject.GetComponent<SpriteRenderer>().color = new Vector4(1f, 0.0f, 0.0f, 1f);
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        gameObject.GetComponent<Collider2D>().enabled = true;
    }

    public GameObject GetReference() { return reference; }

    private void KillerPlateSpawn(bool createOnWhite, bool destroyOnesInMills = false)
    {
        var controllerScript = controller.GetComponent<Controller>();
        GameObject piece;
        Piece pieceScript;
        bool flag = false;
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if(controllerScript.PositionOnBoard(i, j))
                {
                    piece = controllerScript.GetPosition(i, j);
                    if (piece != null)
                    {
                        pieceScript = piece.GetComponent<Piece>();
                        if(pieceScript.isWhite == createOnWhite && (destroyOnesInMills || !pieceScript.IsInMill()))
                        {
                            flag = true;
                            float worldX = i - 3;
                            float worldY = 3 - j;

                            GameObject mp = Instantiate(gameObject, new Vector3(worldX, worldY, -3.0f), Quaternion.identity);
                            MovePlate mpScript = mp.GetComponent<MovePlate>();
                            mpScript.MakeKiller(piece);
                            mpScript.SetCoords(i, j);
                        }
                    }
                }
            }
        }
        // if all of enemies pieces are in mills, then you can destroy them too
        if (!flag)
            KillerPlateSpawn(createOnWhite, true);
    }
}
