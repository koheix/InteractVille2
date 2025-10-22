using UnityEngine;
using System.IO;

//セーブデータ
[System.Serializable]
public class PlayerData
{
    public string name;
    public int hunger = 100;
    public int appleCount = 0;
    public float[] lastPosition= {4f, 1.3f};
}


public class SaveDao
{

    public static void SaveStructData(string userName, PlayerData data)
    {
        string SavePath = Application.persistentDataPath + "/" + userName + ".json";
        Debug.Log(SavePath);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public static PlayerData LoadStructData(string userName)
    {
        string SavePath = Application.persistentDataPath + "/" + userName + ".json";
        if(File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);

            return JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            // データがなければ作成する
            PlayerData data = new PlayerData();

            data.name = userName;
            //保存
            SaveStructData(userName, data);

            return data;
        }
    }

    // 汎用更新メソッド
    public static void UpdateData(string userName, System.Action<PlayerData> updateAction)
    {
        PlayerData data = LoadStructData(userName);
        updateAction(data);  // 任意の更新処理を実行
        SaveStructData(userName, data);
    }
    
    // 汎用ロードメソッド
    public static T LoadData<T>(string userName, System.Func<PlayerData, T> selector)
    {
        PlayerData data = LoadStructData(userName);
        return selector(data);
    }

}
