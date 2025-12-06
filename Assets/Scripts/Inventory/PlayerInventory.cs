using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [Header("インベントリ")]
    [SerializeField] private List<ItemData> items = new List<ItemData>();

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
    }
    
    public void AddItem(ItemData item, int value)
    {
        // if (itemName.Contains("りんご") || itemName.Contains("Apple"))
        // {
        //     itemCount += value;
        //     Debug.Log($"りんごを{value}個ゲット！ 合計: {itemCount}個");
        // }
        // else
        // {
            items.Add(item);
            Debug.Log($"{item.itemName}を取得しました！");
        // }
        
        // UI更新など
        // UpdateUI();
        // 子のGridLayoutGroupにアイテムを追加表示
        PopulateInventory(inventoryPanel, itemButtonPrefab, items.Count);
    }
    
    // private void UpdateUI()
    // {
    //     // UI更新処理をここに
    // }
    
    // public int GetitemCount() => itemCount;
    // public List<string> GetItems() => new List<string>(items);

    // アイテムリストをインベントリに表示
    void PopulateInventory(Transform panel, GameObject itemButtonPrefab, int itemCount)
    {
        for (int i = 0; i < itemCount; i++)
        {
            GameObject itemButtonObj = Instantiate(itemButtonPrefab, panel);
            
            // テスト用
            var text = itemButtonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = items[i].itemName;
            }
            // 画像を設定
            var iconImage = itemButtonObj.GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.sprite = items[i].icon;
            }
            // // ボタンをクリックしたときのリスナーを設定
            // var button = itemButtonObj.GetComponent<Button>();
            // if (button != null)
            // {
            //     int index = i; // ローカル変数にキャプチャ
            //     button.onClick.AddListener(() => {
            //         buyBoxDialogueText.text = $"{shopItems[index].itemName}は{shopItems[index].price}りんごでかえますよ！\nかいますか？";
            //         selectedItem = shopItems[index];
            //         Debug.Log($"選択されたアイテム: {selectedItem.itemName}");
            //     });
            // }
        }
    }
}
