using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    public static void SaveGame(GameData gameData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath+"/game.bin";
        FileStream stream = new FileStream(path, FileMode.Create);

        GameData data = GetGameData();
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static GameData LoadGame()
    {
        string path = Application.persistentDataPath + "/game.bin";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("File not found");
            return null;
        }
    }




    public static PieceData[] GetPieces()
    {
        var objects = GameObject.FindGameObjectsWithTag("Piece");



        PieceData[] pieces = new PieceData[objects.Length];
        int i = 0;
        foreach (GameObject piece in objects)
        {
            pieces[i] = new PieceData(piece);
            i++;
        }
        return pieces;
    }
    public static GameData GetGameData()
    {
        var controller = GameObject.FindGameObjectWithTag("GameController");
        bool turnInfo = controller.GetComponent<Controller>().isWhiteTurn;
        GameData gameData = new GameData(turnInfo, GetPieces());
        return gameData;
    }



}
