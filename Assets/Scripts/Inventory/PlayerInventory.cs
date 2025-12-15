using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class PlayerInventory : MonoBehaviour
{
    [Header("インベントリ")]
    [SerializeField] private List<ItemData> items = new List<ItemData>();
    [SerializeField] private Dictionary<ItemData, int> inventoryItems = new Dictionary<ItemData, int>();

    // アイテムボタンのプレハブ
    [SerializeField] private GameObject itemButtonPrefab;

    // インベントリのGridLayoutGroupにアイテムを表示するためのTransform
    [SerializeField] private Transform inventoryPanel;

    // singletonパターン
    public static PlayerInventory Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        // インベントリデータの読み込み
        LoadInventoryData();
    }

    void Start()
    {
        // // インベントリデータの読み込み
        // LoadInventoryData();
        // QuitManagerに保存タスクを登録
        QuitManager.Instance.AddQuitTask(SaveInventoryData());
    }

    // インベントリデータの読み込み
    private void LoadInventoryData()
    {
        Debug.Log("インベントリデータを読み込み中...");
        string userName = PlayerPrefs.GetString("userName", "default");
        // PlayerData data = SaveDao.LoadStructData(userName);
        // items = new List<ItemData>(data.inventoryItems);
        // インベントリの読み込みと変換
        List<InventorySlot> inventoryData = SaveDao.LoadData(userName, data => data.inventoryItems);
        // inventoryItems = inventoryData.ToDictionary(slot => slot.item, slot => slot.count);
        // nullは除外する
        inventoryItems = inventoryData.Where(slot => slot.item != null) .ToDictionary(slot => slot.item, slot => slot.count);
        //
        // inventoryItems = new Dictionary<ItemData, int>(data.inventoryItems);
        items = new List<ItemData>(inventoryItems.Keys);
        // デバッグ表示
        foreach (var item in items)
        {
            Debug.Log($"Loaded item: {item.itemName}");
        }

        // インベントリUIの更新
        // PopulateInventory(inventoryPanel, itemButtonPrefab, items.Count);
        PopulateInventory(inventoryPanel, itemButtonPrefab, inventoryItems.Count);

        Debug.Log("インベントリデータの読み込み完了");
    }
    
    public void AddItem(ItemData item, int value)
    {
        // items.Add(item);
        if (inventoryItems.ContainsKey(item))
        {
            inventoryItems[item] += value;
        }
        else
        {
            inventoryItems[item] = value;
        }
        Debug.Log($"{item.itemName}を取得しました！");
        
        // UI更新など
        // UpdateUI();
        // 子のGridLayoutGroupにアイテムを追加表示
        // PopulateInventory(inventoryPanel, itemButtonPrefab, items.Count);
        PopulateInventory(inventoryPanel, itemButtonPrefab, inventoryItems.Count);
    }
    
    // private void UpdateUI()
    // {
    //     // UI更新処理をここに
    // }

    void PopulateInventory(Transform panel, GameObject itemButtonPrefab, int itemCount)
    {
        // 既存のアイテム表示をクリア
        foreach (Transform child in panel)
        {
            Destroy(child.gameObject);
        }

        items = new List<ItemData>(inventoryItems.Keys);
        for (int i = 0; i < itemCount; i++)
        {
            GameObject itemButtonObj = Instantiate(itemButtonPrefab, panel);
            
            // テスト用
            var text = itemButtonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                // text.text = $"{items[i].itemName} x{inventoryItems[items[i]]}";
                text.text = $"{inventoryItems[items[i]]}";
            }
            // 画像を設定
            var iconImage = itemButtonObj.GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.sprite = items[i].icon;
            }
        }
    }

    // プレイヤーのインベントリデータを保存するコルーチン
    private System.Collections.IEnumerator SaveInventoryData()
    {
        Debug.Log("インベントリデータを保存中...");
        string userName = PlayerPrefs.GetString("userName", "default");
        // // SaveDao.UpdateData(userName, data => data.inventoryItems = new List<ItemData>(items));
        // SaveDao.UpdateData(userName, data => data.inventoryItems = new Dictionary<ItemData, int>(inventoryItems));
        var inventoryList = inventoryItems.Select(kvp => 
            new InventorySlot { item = kvp.Key, count = kvp.Value }
        ).ToList();
        SaveDao.UpdateData(userName, data => data.inventoryItems = inventoryList);

        yield return null;
        Debug.Log("インベントリデータの保存完了");
    }
}
