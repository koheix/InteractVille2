using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopperDialogueSystem : DialogueSystem
{
    [Header("Shopper UI References")]
    public Button yesButton;
    public Button noButton;

    [Header("Shop Manager Reference")]
    public ShopManager shopManager;

    [Header("Shopper FSM Reference")]
    private ShopperFSM shopperFSM = new ShopperFSM();

    [Header("UI References")]
    public GameObject buyBox;
    public GameObject itemListPanel;
    public TextMeshProUGUI buyBoxCharacterNameText;
    public TextMeshProUGUI buyBoxDialogueText;
    public Button buyBoxYesButton;
    public Button buyBoxNoButton;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();

        // 買い物用のUIは初期状態で非表示
        buyBox.SetActive(false);

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
            // 基本UIBOXを非表示にして買い物UIを表示
            dialogueBox.SetActive(false);
            buyBox.SetActive(true);
        }
        currentLineIndex = 0;
        DisplayLine();
    }

    // buyboxのはいかいいえボタンがクリックされたときに呼び出される
    void OnClickBuyBoxYorNButton(Button clickedButton)
    {
        Debug.Log("Clicked BuyBox Button: " + clickedButton.name);
        bool isYes = (clickedButton == buyBoxYesButton);
        // 商品の購入処理をここに追加
        if (isYes)
        {
            Debug.Log("Player chose to buy the item.");
            // ここでshopManagerのBuyItemメソッドを呼び出すなどの処理を追加
            // 例: shopManager.BuyItem(selectedItem);
        }
        else
        {
            Debug.Log("Player chose not to buy the item.");
        }
        dialogueLines = shopperFSM.changeState(isYes);
        currentLineIndex = 0;
        DisplayLine();
    }
}
