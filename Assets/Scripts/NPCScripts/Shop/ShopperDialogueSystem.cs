using UnityEngine;
using UnityEngine.UI;

public class ShopperDialogueSystem : DialogueSystem
{
    [Header("Shopper UI References")]
    public Button yesButton;
    public Button noButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // override void Start()
    // {
    //     base.Start();
    // }

    // // Update is called once per frame
    // override void Update()
    // {
    //     base.Update();
    // }

    public override void StartDialogue()
    {
        base.StartDialogue();
        // ここにShopperDialogueSystem固有の処理を追加
    }
}
