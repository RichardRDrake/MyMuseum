using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveLevel


    // Script for reading/creating the binary save files 
{
    // Function creates/overwrites a save file with the given name/level data.
    public static void CreateLevel(Level level, string name)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string filepath = Application.persistentDataPath + "/" + name;

        FileStream stream = new FileStream(filepath, FileMode.Create);

        LevelData data = new LevelData(level);
        
        formatter.Serialize(stream, data);

        stream.Close();
    }

    // Function deserialises (reads) the specific save file and returns the data held
    public static LevelData LoadLevel(string name)
    {
        string filepath = Application.persistentDataPath + "/" + name;
        if (File.Exists(filepath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(filepath, FileMode.Open);

            LevelData data = formatter.Deserialize(stream) as LevelData;
            stream.Close();

            return data;
        } else
        {
            Debug.LogError("Save file not found");
            return null;
        }

    }
}
 