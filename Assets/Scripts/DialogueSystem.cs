using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class DialogueLine
{
    public string characterName;
    [TextArea(3, 5)]
    public string text;
}

public class DialogueSystem : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialogueBox;
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI dialogueText;
    public Button nextButton;
    
    [Header("Dialogue Data")]
    public DialogueLine[] dialogueLines;
    
    private int currentLineIndex = 0;
    private bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    
    [Header("Typing Animation")]
    public float typeSpeed = 0.05f;
    
    void Start()
    {
        // 初期状態でダイアログボックスを非表示
        dialogueBox.SetActive(false);
        
        // Next buttonにクリックイベントを追加
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(NextLine);
        }
    }
    
    void Update()
    {
        // ダイアログが表示されている時のみ入力を受け付ける
        if (isDialogueActive)
        {
            // スペースキー、エンターキー、マウスクリックで次の行へ
            if (Input.GetKeyDown(KeyCode.Space) || 
                Input.GetKeyDown(KeyCode.Return) || 
                Input.GetMouseButtonDown(0))
            {
                NextLine();
            }
        }
    }
    
    public void StartDialogue()
    {
        if (dialogueLines.Length == 0) return;
        
        isDialogueActive = true;
        currentLineIndex = 0;
        dialogueBox.SetActive(true);
        
        DisplayLine();
    }
    
    void DisplayLine()
    {
        if (currentLineIndex < dialogueLines.Length)
        {
            DialogueLine currentLine = dialogueLines[currentLineIndex];
            
            // キャラクター名を設定
            if (characterNameText != null)
            {
                characterNameText.text = currentLine.characterName;
            }
            
            // タイピングエフェクトでテキストを表示
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(TypeText(currentLine.text));
        }
        else
        {
            EndDialogue();
        }
    }
    
    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";
        
        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
        
        isTyping = false;
    }
    
    public void NextLine()
    {
        // タイピング中の場合は即座に全文表示
        if (isTyping)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            dialogueText.text = dialogueLines[currentLineIndex].text;
            isTyping = false;
            return;
        }
        
        currentLineIndex++;
        DisplayLine();
    }
    
    void EndDialogue()
    {
        isDialogueActive = false;
        dialogueBox.SetActive(false);
        currentLineIndex = 0;
    }
    
    // 外部からダイアログを終了させるメソッド
    public void ForceEndDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        EndDialogue();
    }
}