using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    [Header("ショップに並ぶアイテム")]
    public List<ItemData> shopItems;
    private int appleCount = 0; // プレイヤーの所持リンゴ

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // プレイヤーのリンゴを取得
        appleCount = SaveDao.LoadData(PlayerPrefs.GetString("userName", default), data => data.appleCount);

        // デバッグ用にショップアイテムを表示
        DisplayShopItems();
    }

    // Update is called once per frame
    void Update()
    {

    }
    // ショップアイテムを表示（デバッグ用）
    void DisplayShopItems()
    {
        Debug.Log("=== ショップアイテム一覧 ===");
        foreach (var item in shopItems)
        {
            Debug.Log($"{item.itemName} - {item.price}円");
        }
    }
    
    // アイテムを購入
    public bool BuyItem(ItemData item)
    {
        if (appleCount >= item.price)
        {
            appleCount -= item.price;
            
            Debug.Log($"{item.itemName}を購入しました！ 残金: {appleCount}円");
            
            // ここでインベントリに追加する処理を呼ぶ
            AddToInventory(item);
            
            return true;
        }
        else
        {
            Debug.Log("所持リンゴ数が足りません！");
            return false;
        }
    }
    
    // インベントリに追加（後で実装）
    void AddToInventory(ItemData item)
    {
        // TODO: インベントリシステムと連携
        Debug.Log($"{item.itemName}をインベントリに追加");
    }

    public int GetAppleCount()
    {
        return appleCount;
    }
    
    void onApplicationQuit()
    {
        // アプリケーション終了時に所持リンゴを保存
        SaveDao.UpdateData(PlayerPrefs.GetString("userName", default), data => data.appleCount = appleCount);
    }

}
