/* ともハムのステータスを管理するスクリプト
 * 機嫌や満腹度の管理など
 * ステータスはLLMの応答生成、行動選択に影響を与えるかつLLMの出力で変化する
 * よって外からも読み書きできるようにする
 * 発話メソッドなども担う
 */

using UnityEngine;
using System.Collections;


public class FriendHamStatus : MonoBehaviour
{
    // LLMBridgeの参照
    [Header("LLM Bridge Reference")]
    public LLMBridge llmBridge;

    // valence(0 ~ 100で表現し、getterとsetterで制御する)
    private int valence = 50;
    public int Valence
    {
        get { return valence; }
        set { valence = Mathf.Clamp(value, 0, 100); }
    }
    // arousal(0 ~ 100で表現し、getterとsetterで制御する)
    private int arousal = 50;
    public int Arousal
    {
        get { return arousal; }
        set { arousal = Mathf.Clamp(value, 0, 100); }
    }
    // hunger(0 ~ 100で表現し、getterとsetterで制御する)
    private int hunger = 50;
    public int Hunger
    {
        get { return hunger; }
        set { hunger = Mathf.Clamp(value, 0, 100); }
    }

    // memory(ともハムの記憶を保存するための文字列リスト)
    public System.Collections.Generic.List<string> memory = new System.Collections.Generic.List<string>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ともハムの発話メソッド
    public IEnumerator Speak(string message, System.Action<string> callback)
    {
        // 発話処理
        // 記憶と相手の発話内容に基づいて返事をする
        Debug.Log("Sending request to Claude API...");
        
        IEnumerator responseCoroutine = llmBridge.GetLLMResponse(message, string.Join("\n", memory));
        yield return StartCoroutine(responseCoroutine);
        
        // レスポンスの取得
        string response = responseCoroutine.Current as string;
        // Debug.Log($"Claude Response: {response}");
        // string result = "APIの結果"; // 実際はレスポンスを入れる
        callback?.Invoke(response);
    }
}
