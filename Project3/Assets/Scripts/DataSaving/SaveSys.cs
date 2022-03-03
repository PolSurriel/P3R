using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSys
{

    // Saves data in a bin file
    public static void SaveMenuData(GameInfo gameInfo)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/saves.bin";    // path where its saved
        FileStream stream = new FileStream(path, FileMode.Create);

        MenuData data = new MenuData(gameInfo);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    // Load data from a bin file
    public static MenuData LoadMenuData()
    {
        string path = Application.persistentDataPath + "/saves.bin";    // path from where its loaded
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            MenuData data = formatter.Deserialize(stream) as MenuData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
