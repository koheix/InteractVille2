using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FriendHamDialogueSystem : DialogueSystem
{
    [Header("UI References")]
    public GameObject chattingBox;
    // public GameObject presentBox;
    public GameObject pettingBox;
    // ともハムとのインタラクション全体を終了するためのボタン
    public Button quitButton;
    public Button presentButton;
    public Button chatButton;
    public Button petButton;




    [Header("Chatting Box UI References")]
    public Button sendButton;
    public TMP_InputField chatInputField;
    public TextMeshProUGUI chattingCharacterNameText;
    public TextMeshProUGUI chattingText;
    // メニューに戻るためのボタン
    public Button returnButton;

    private FriendHamFSM friendHamFSM = new FriendHamFSM();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        if (chattingBox != null)
        {
            chattingBox.SetActive(false);
        }
        // if (pettingBox != null)
        // {
        //     pettingBox.SetActive(false);
        // }
        // presentBoxは未実装
        // if (presentBox != null)
        // {
        //     presentBox.SetActive(false); 
        // }
        // quitButtonにクリックイベントを追加, 非表示にしておく
        if (quitButton != null)
        {
            quitButton.gameObject.SetActive(false);
            quitButton.onClick.AddListener(EndDialogue);
        }
        // if (presentButton != null)
        // {
        //     presentButton.onClick.AddListener(OpenPresentBox);
        // }
        if (chatButton != null)
        {
            chatButton.onClick.AddListener(OpenChattingBox);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void StartDialogue()
    {
        // friendHamFSM.changeState(true); // 初期状態へ遷移
        dialogueLines = friendHamFSM.EnterState(FriendHamState.Greeting);
        quitButton.gameObject.SetActive(true); // やめるボタンを表示
        base.StartDialogue();
    }


    // インタラクション全体を終了する
    protected override void EndDialogue()
    {
        base.EndDialogue();
        // pettingBox.SetActive(false);
        // presentBox.SetActive(false);
        chattingBox.SetActive(false);
        quitButton.gameObject.SetActive(false); // やめるボタンを非表示
    }

    // 雑談ボックスを開く
    void OpenChattingBox()
    {
        chattingBox.SetActive(true);
        dialogueBox.SetActive(false);
        // pettingBox.SetActive(false);
        // presentBox.SetActive(false);
    }

    // // プレゼントボックスを開く（未実装）
    // void OpenPresentBox()
    // {
    //     // presentBox.SetActive(true);
    //     dialogueBox.SetActive(false);
    //     chattingBox.SetActive(false);
    //     pettingBox.SetActive(false);
    // }

}
