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


        //formatter.Serialize(stream, );
        stream.Close();
    }




}
