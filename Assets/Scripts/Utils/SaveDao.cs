using UnityEngine;
using System.IO;
using System.Collections.Generic;

// アイテムとその個数を管理するクラス
[System.Serializable]
public class InventorySlot
{
    // public ItemData item;
    public string itemName;
    public int count;
}

//セーブデータ
[System.Serializable]
public class PlayerData
{
    public string name;
    public int hunger = 100;
    public int appleCount = 0;
    public float[] lastPosition= {4f, 1.3f};
    //インベントリデータ(各アイテムの個数もここで管理する)
    // public List<ItemData> inventoryItems = new List<ItemData>();
    // public Dictionary<ItemData, int> inventoryItems = new Dictionary<ItemData, int>();
    public List<InventorySlot> inventoryItems = new List<InventorySlot>();

    // 友ハムのデータ
    public int friendHamValence = 50;
    public int friendHamArousal = 50;
    public int friendHamHunger = 50;
    public int friendHamCloseness = 50;

    public string friendHamCurrentMood = "普通";
    public List<string> friendHamMemory = new List<string>();

    // 会話履歴
    public List<Message> conversationHistory = new List<Message>();

    // メタデータ
    public string lastPlayedDate = System.DateTime.Now.ToString(); // 要変更
}


public class SaveDao
{
    public static void SaveStructData(string userName, PlayerData data)
    {
        string SavePath = Application.persistentDataPath + "/" + userName + ".json";
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
