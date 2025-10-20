using UnityEngine;
using System.IO;

//セーブデータ
[System.Serializable]
public class PlayerData
{
    public string name;
    public int hunger;
    public int applecounter;
    public float[] lastPosition;
}


public class SaveDao
{

    public static void SaveData(string userName, PlayerData data)
    {
        string SavePath = Application.persistentDataPath + "/" + userName + ".json";
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public static PlayerData LoadData(string userName)
    {
        string SavePath = Application.persistentDataPath + "/" + userName + ".json";
        if(File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);

            return JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            PlayerData data = new PlayerData();

            data.name = userName;

            return data;
        }
    }

    // 汎用更新メソッド
    public static void UpdateData(string userName, System.Action<PlayerData> updateAction)
    {
        string SavePath = Application.persistentDataPath + "/" + userName + ".json";
        PlayerData data = LoadData(SavePath);
        updateAction(data);  // 任意の更新処理を実行
        SaveData(userName, data);    
    }

}
