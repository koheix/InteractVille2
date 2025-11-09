using UnityEngine;

/**
 * プレイヤーがNPCに近づいたときにダイアログを開始するトリガースクリプト
 * NPCにアタッチして使用
 */
public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public DialogueSystem dialogueSystem;
    private bool isPlayerInRange = false;

    [Header("UI Prompt")]
    public GameObject interactionPrompt; // "Eキーで話す"などの表示用

    void Start()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    void Update()
    {
        // プレイヤーが範囲内にいる時のみspaceキーでダイアログ開始
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Space) && !dialogueSystem.IsDialogueActive)
        {
            if (dialogueSystem != null)
            {
                dialogueSystem.StartDialogue();

                // プロンプトを非表示
                if (interactionPrompt != null)
                {
                    interactionPrompt.SetActive(false);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;

            // インタラクションプロンプトを表示
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;

            // プロンプトを非表示
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }

            // ダイアログが進行中の場合は強制終了
            if (dialogueSystem != null)
            {
                dialogueSystem.ForceEndDialogue();
            }
        }
    }
}