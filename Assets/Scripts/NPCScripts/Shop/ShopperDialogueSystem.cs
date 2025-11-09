using UnityEngine;
using UnityEngine.UI;

public class ShopperDialogueSystem : DialogueSystem
{
    [Header("Shopper UI References")]
    public Button yesButton;
    public Button noButton;

    [Header("Shopper FSM Reference")]
    private ShopperFSM shopperFSM = new ShopperFSM();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        // Next buttonにクリックイベントを追加
        if (yesButton != null)
        {
            // yesButton.onClick.AddListener(NextLine);
            yesButton.onClick.AddListener(() => OnClickYorNButton(yesButton));
        }
        if (noButton != null)
        {
            // noButton.onClick.AddListener(NextLine);
            noButton.onClick.AddListener(() => OnClickYorNButton(noButton));
        }
    }

    public override void StartDialogue()
    {
        dialogueLines = shopperFSM.changeState(true); // 初期状態へ遷移
        nextButton.gameObject.SetActive(false); // Nextボタンは非表示にしておく
        base.StartDialogue();
    }

    // はいかいいえボタンがクリックされたときに呼び出される
    void OnClickYorNButton(Button clickedButton)
    {
        Debug.Log("Clicked Button: " + clickedButton.name);
        bool isYes = (clickedButton == yesButton);
        dialogueLines = shopperFSM.changeState(isYes);
        // 買い物状態の場合は商品を表示するなどの処理を追加
        if (shopperFSM.CurrentState == ShopState.BuyMenu)
        {
            // 商品リスト表示などの処理をここに追加
            Debug.Log("Displaying Buy Menu...");
        }
        currentLineIndex = 0;
        DisplayLine();
    }
}
