using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerializationManager 
{
    public static bool SaveJSON(string saveName, object saveData)
    {

        if (!Directory.Exists(Application.persistentDataPath + "/saves"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves");
        }

        string path = Application.persistentDataPath + "/saves/" + saveName + ".save";

        var serializedData = JsonConvert.SerializeObject(saveData, Formatting.Indented);

        File.WriteAllText(path, serializedData);
        Debug.Log(path);

        return true;

    }

    public static object LoadJSON(string path)
    {
        path = Application.persistentDataPath + "/saves/" + path + ".save";

        if (!File.Exists(path))
        {
            return null;
        }


        try
        {
            string json = File.ReadAllText(path);
            object save = JsonConvert.DeserializeObject(json,typeof(SaveData));
            return save;
        }
        catch
        {
            Debug.LogErrorFormat("Failed to load file at {0}", path);
            return null;
        }

    }
}
