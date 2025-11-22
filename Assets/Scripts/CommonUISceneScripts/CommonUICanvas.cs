/* CommonUICanvas.cs
 * 
 * シーンをまたいでタイトルへ戻るボタンを保持するためのクラス
 */

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CommonUICanvas : MonoBehaviour
{
    private Canvas rootCanvas;
    private Button return2TitleButton;

    private void Awake()
    {
        rootCanvas = GetComponent<Canvas>();
        return2TitleButton =  GetComponentInChildren<Button>();
        return2TitleButton.onClick.AddListener(() =>
        {
            QuitManager.Instance.RequestReturn2Title();
        });
        // rootCanvas.enabled = false;
        DontDestroyOnLoad(gameObject);

        // シーン変化を監視
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnSceneChanged(Scene pre, Scene next)
    {
        // タイトルシーン名に応じて切り替え
        if (next.name == "TitleScene") 
        {
            rootCanvas.enabled = false;  // UI 非表示
        }
        else
        {
            rootCanvas.enabled = true;   // UI 表示
        }
    }
}