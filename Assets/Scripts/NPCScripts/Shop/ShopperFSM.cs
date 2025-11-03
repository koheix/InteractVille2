/*
    shopper の状態を管理する FSMクラス
*/

using UnityEngine;

// ショップ会話の状態
public enum ShopState
{
    Idle,       // 待機中
    Greeting,   // 挨拶
    BuyMenu,    // 購入メニュー
    // SellMenu,   // 売却メニュー
    End         // 終了
}

public class ShopNPC : MonoBehaviour
{
    // 現在の状態を保持
    private ShopState currentState = ShopState.Idle;

    [Header("DialogueSystem Reference")]
    public DialogueSystem dialogueSystem;

    void Start()
    {
        // 待機状態から始める
        currentState = ShopState.Idle;
    }

    void Update()
    {
        // // 簡易的にスペースキーで会話開始
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     if (currentState == ShopState.Idle)
        //     {
        //         EnterState(ShopState.Greeting);
        //     }
        // }
    }

    // 現在の状態とyes, noボタンの選択に基づいて状態を変更
    // ボタンを押すときに呼び出す
    void changeState(bool isYes)
    {
        if(currentState == ShopState.Idle)
        {
            EnterState(ShopState.Greeting);
        }
        else if (currentState == ShopState.Greeting)
        {
            if (isYes)
            {
                EnterState(ShopState.BuyMenu);
            }
            else
            {
                EnterState(ShopState.End);
            }
        }
        else if (currentState == ShopState.BuyMenu)
        {
            EnterState(ShopState.End);
        }
        else if (currentState == ShopState.End)
        {
            EnterState(ShopState.Idle);
        }
    }

    void EnterState(ShopState newState)
    {
        currentState = newState;
        switch (newState)
        {
            case ShopState.Greeting:
                Debug.Log("NPC：いらっしゃい！今日は何をお探し？");
                ShowMainMenu();
                break;

            case ShopState.BuyMenu:
                Debug.Log("NPC：こちらが商品一覧です🛒");
                // 商品リスト表示処理を書く
                break;

            // case ShopState.SellMenu:
            //     Debug.Log("NPC：売りたいもんあるん？見せて💰");
            //     // インベントリの売却処理を書く
            //     break;

            case ShopState.End:
                Debug.Log("NPC：まいどあり〜！また来てな！");
                currentState = ShopState.Idle;
                break;
        }
    }

    void ShowMainMenu()
    {
        Debug.Log("1. 買う\n2. 売る\n3. やめる");

        // // 仮：キー入力でメニュー選択
        // StartCoroutine(WaitForMenuInput());
    }

    // System.Collections.IEnumerator WaitForMenuInput()
    // {
    //     bool selected = false;

    //     while (!selected)
    //     {
    //         if (Input.GetKeyDown(KeyCode.Alpha1))
    //         {
    //             EnterState(ShopState.BuyMenu);
    //             selected = true;
    //         }
    //         else if (Input.GetKeyDown(KeyCode.Alpha2))
    //         {
    //             EnterState(ShopState.SellMenu);
    //             selected = true;
    //         }
    //         else if (Input.GetKeyDown(KeyCode.Alpha3))
    //         {
    //             EnterState(ShopState.End);
    //             selected = true;
    //         }
    //         yield return null;
    //     }
    // }
}