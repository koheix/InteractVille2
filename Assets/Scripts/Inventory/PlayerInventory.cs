using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    [Header("インベントリ")]
    [SerializeField] private int itemCount = 0;
    [SerializeField] private List<string> items = new List<string>();
    
    public void AddItem(string itemName, int value)
    {
        // if (itemName.Contains("りんご") || itemName.Contains("Apple"))
        // {
        //     itemCount += value;
        //     Debug.Log($"りんごを{value}個ゲット！ 合計: {itemCount}個");
        // }
        // else
        // {
            items.Add(itemName);
            Debug.Log($"{itemName}を取得しました！");
        // }
        
        // UI更新など
        UpdateUI();
    }
    
    private void UpdateUI()
    {
        // UI更新処理をここに
    }
    
    public int GetitemCount() => itemCount;
    public List<string> GetItems() => new List<string>(items);
}
