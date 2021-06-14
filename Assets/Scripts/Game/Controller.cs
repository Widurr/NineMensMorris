using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public GameObject piece;
    public AIBehaviour opponent;
    [SerializeField]public GameObject endScreen;
    [SerializeField]public GameObject UIWhiteTurn;
    [SerializeField]public GameObject UIBlackTurn;
    [SerializeField] public Text endText;

    private Game game;
    public Game getGame() { return game; }

    public bool isPlayerWhite { get; private set; } = true;
    public int difficulty = 1;

    //[SerializeField] private int piecesPlaced = 0;
    public int piecesPlaced { get { return game.whitePiecesPlaced + game.blackPiecesPlaced; } }
    private bool gameOver = false;
    [SerializeField] private int movePlatesCount;
    CtrlFirstStage firstStage;

    public int ppw, ppb;

    public bool isWhiteTurn
    { 
        get { return game.isWhiteTurn; }
        set { game.isWhiteTurn = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (game == null)
        {
            game = new Game();
            game.gameState = Game.GameState.placing;
            game.isWhiteTurn = true;
        }
        SavingSystem.game.Add(game);
        //piecesPlaced = FindObjectsOfType<Piece>().Length;
       

        opponent = GameObject.FindGameObjectWithTag("AI").GetComponent<AIBehaviour>();
        opponent.difficulty = difficulty;

        firstStage = GetComponent<CtrlFirstStage>();
        StartCoroutine(PlacingStage());
        StartCoroutine(CountMovePlates());
    }

    private void Update()
    {
        ppw = game.whitePiecesPlaced;
        ppb = game.blackPiecesPlaced;

        ///*
        if(game.gameState == Game.GameState.moving && isWhiteTurn != isPlayerWhite)
        {
            AIBehaviour ai = opponent;
            Move move = ai.CalculateMove(game);
            game.ApplyMove(move);
            game.isWhiteTurn = !game.isWhiteTurn;
        }
        //*/
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
            DestroyAllPieces();
            endScreen.SetActive(true);
            if (WhoWon())
            {
                endScreen.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
                FindObjectOfType<AudioManager>().Play("WhiteWin");
            }
            else
            {
                endScreen.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
                FindObjectOfType<AudioManager>().Play("BlackWin");
            }
        }
        else
            UITurnChange();
    }
    
   
}
