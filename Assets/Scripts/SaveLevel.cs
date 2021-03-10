using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveLevel

{
    public static void SaveLevels(Level level)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string filepath = Application.persistentDataPath + "/level.save";

        FileStream stream = new FileStream(filepath, FileMode.Create);

        LevelData data = new LevelData(level);

        formatter.Serialize(stream, data);

        stream.Close();
    }

    public static LevelData LoadLevels()
    {
        string filepath = Application.persistentDataPath + "/level.save";
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
 