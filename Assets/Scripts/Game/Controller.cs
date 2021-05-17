using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public GameObject piece;
    [SerializeField]public GameObject endScreen;
    [SerializeField]public GameObject UIWhiteTurn;
    [SerializeField]public GameObject UIBlackTurn;
    [SerializeField] public Text endText;

    private Game game;
    public Game getGame() { return game; }

    private bool isPlayerWhite = true;

    //[SerializeField] private int piecesPlaced = 0;
    public int piecesPlaced { get { return game.whitePiecesPlaced + game.blackPiecesPlaced; } }
    private bool gameOver = false;
    [SerializeField] private int movePlatesCount;
    CtrlFirstStage firstStage;

    public bool isWhiteTurn
    { 
        get { return game.isWhiteTurn; }
        set { game.isWhiteTurn = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        game = new Game();
        game.gameState = Game.GameState.placing;
        game.isWhiteTurn = true;
        

        firstStage = GetComponent<CtrlFirstStage>();
        StartCoroutine(PlacingStage());
        StartCoroutine(CountMovePlates());
    }

    IEnumerator PlacingStage()
    {
        while (piecesPlaced < 18)
            yield return StartCoroutine(AddPlates());
        yield return new WaitUntil(() => movePlatesCount == 0);
        game.gameState = Game.GameState.moving;
    }

    IEnumerator AddPlates()
    {
        {
            while (movePlatesCount < 2)
            {
                firstStage.CreateMovePlates(game);
                yield return new WaitForSeconds(0f);
                //piecesPlaced++;
                if (isWhiteTurn)
                    game.whitePiecesPlaced++;
                else
                    game.blackPiecesPlaced++;
            }
        }
    }

    IEnumerator CountMovePlates()
    {
        while (game.gameState != Game.GameState.moving)
        {
            movePlatesCount = GameObject.FindGameObjectsWithTag("MovePlate").Length;
            yield return new WaitForSeconds(0f);
        }
    }

    public GameObject CreatePiece(bool isWhite, int x, int y)
    {
        GameObject obj = Instantiate(piece, new Vector3(0f, 0f, -1f), Quaternion.identity);
        Piece p = obj.GetComponent<Piece>();
        p.isWhite = isWhite;
        p.xBoard = x;
        p.yBoard = y;

        p.Activate();
        return obj;
    }

    public void SetPosition(GameObject obj)
    {
        game.SetPosition(obj);
    }
    public void SetPositionEmpty(int x, int y)
    {
        game.SetPositionEmpty(x, y);
    }
    public GameObject GetPosition(int x, int y)
    {
        return game.GetPosition(x, y);
    }
    public bool PositionOnBoard(int x, int y)
    {
        return game.PositionOnBoard(x, y);
    }

    public bool IsInMill(GameObject piece)
    {
        return game.IsInMill(piece);
    }

    public int piecesCount(bool isWhite)
    {
        return game.piecesCount(isWhite);
    }

    public void UITurnChange()
    {
        if(game.isWhiteTurn)
        {
            UIWhiteTurn.SetActive(true);
            UIBlackTurn.SetActive(false);
        }
        else
        {
            UIWhiteTurn.SetActive(false);
            UIBlackTurn.SetActive(true);
        }
    }
    private void DestroyAllPieces()
    {
        GameObject[] allPieces = GameObject.FindGameObjectsWithTag("Piece");
        for (int i = 0; i < allPieces.Length; i++)
        {
            Destroy(allPieces[i]);
        }
    }
    private bool WhoWon()
    {
        return !game.isWhiteTurn;
    }
    public void DeclareWinner()
    {
        if (game.IsWinner())
        {
            //DestroyAllPieces();
            endScreen.SetActive(true);
            if (WhoWon())
            {
                endText.text = "White won!";
            }
            else
                endText.text = "Black won!";
        }
        else
            UITurnChange();
    }
   
}
