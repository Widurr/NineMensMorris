using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SavingSystem : MonoBehaviour
{

    [SerializeField]
    Piece piecePrefab;

    public static List<Piece> pieces = new List<Piece>();
    public static List<Game> game = new List<Game>();

    const string PIECE_SUB = "/piece";
    const string PIECE_COUNT_SUB = "/piece.count";


    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void Awake()
    {
        LoadGame();
       
    }

    public static void SaveGame()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string gamePath = Application.persistentDataPath + "game.bin";
        string path = Application.persistentDataPath + PIECE_SUB;
        string countPath = Application.persistentDataPath + PIECE_COUNT_SUB;

       FileStream gameStream = new FileStream(gamePath, FileMode.Create);
        for (int i = 0; i < game.Count; i++)
        {
            GameData gameData = new GameData(game[i]);
            formatter.Serialize(gameStream, gameData);
            gameStream.Close();
        }

        FileStream countStream = new FileStream(countPath, FileMode.Create);
        formatter.Serialize(countStream, pieces.Count);
        countStream.Close();

        for (int i = 0; i < pieces.Count; i++)
        {
            FileStream stream = new FileStream(path + i, FileMode.Create);
            PieceData data = new PieceData(pieces[i]);
            formatter.Serialize(stream, data);
            stream.Close();
        }

    }

    public void LoadGame()
    {

        BinaryFormatter formatter = new BinaryFormatter();
        string gamePath = Application.persistentDataPath + "game.bin";
        string path = Application.persistentDataPath + PIECE_SUB;
        string countPath = Application.persistentDataPath + PIECE_COUNT_SUB;
        int pieceCount = 0;
        if (File.Exists(gamePath))
        {
            FileStream gameStream = new FileStream(gamePath, FileMode.Open);
            GameData gameData = formatter.Deserialize(gameStream) as GameData;
            gameStream.Close();
            Game game = FindObjectOfType<Controller>().getGame();
            game.isWhiteTurn = gameData.isWhiteTurn;
           // game.positions = gameData.positions;
          //  game.positionsMask = gameData.positionsMask;
            game.gameState = (Game.GameState)gameData.gameState;
        }
        else
        {
            Debug.LogError("Game data File not found");
        }

        if (File.Exists(countPath))
        {
            FileStream countStream = new FileStream(countPath, FileMode.Open);

            pieceCount = (int)formatter.Deserialize(countStream);
            countStream.Close();
        }
        else
        {
            Debug.LogError("Count File not found");
        }
        for (int i = 0; i < pieceCount; i++)
        {
            if (File.Exists(path + i))
            {
                FileStream stream = new FileStream(path + i, FileMode.Open);
                PieceData data = formatter.Deserialize(stream) as PieceData;

                stream.Close();

                Vector3 position = new Vector3(data.position[0], data.position[1], data.position[2]);

                Piece piece = Instantiate(piecePrefab, position, Quaternion.identity);
                piece.isWhite = data.isWhite;
                if (piece.isWhite)
                    piece.GetComponent<SpriteRenderer>().color = new Vector4(0.9f, 0.9f, 0.9f, 1f);
                else
                    piece.GetComponent<SpriteRenderer>().color = new Vector4(0.2f, 0.2f, 0.2f, 1f);
            }
            else
            {
                Debug.LogError("Piece File not found");
            }
        }
    }



}
