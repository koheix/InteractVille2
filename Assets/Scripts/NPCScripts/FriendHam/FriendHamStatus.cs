/* ともハムのステータスを管理するスクリプト
 * 機嫌や満腹度の管理など
 * ステータスはLLMの応答生成、行動選択に影響を与えるかつLLMの出力で変化する
 * よって外からも読み書きできるようにする
 * 発話メソッドなども担う
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


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
    private int closeness = 50;
    public int Closeness
    {
        get { return closeness; }
        set { closeness = Mathf.Clamp(value, 0, 100); }
    }

    // memory(ともハムの記憶を保存するための文字列リスト)
    // ゲームが終了するときに保存する(SaveDaoを使う)
    // public List<string> memory = new List<string>();
    // memory = SaveDao.LoadData(PlayerPrefs.GetString("userName", "default"), data => data.friendHamMemory);
    public List<string> memory;
    private const int MaxMemorySize = 20; // 最大メモリ数
    private LLMBridge.ConversationHistory conversationHistory = new LLMBridge.ConversationHistory();

    //singleton化
    // public static FriendHamStatus Instance { get; private set; }
    // private void Awake()
    // {
    //     DontDestroyOnLoad(gameObject);
    // }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        memory = SaveDao.LoadData(PlayerPrefs.GetString("userName", "default"), data => data.friendHamMemory);
        valence = SaveDao.LoadData(PlayerPrefs.GetString("userName", "default"), data => data.friendHamValence);
        arousal = SaveDao.LoadData(PlayerPrefs.GetString("userName", "default"), data => data.friendHamArousal);
        hunger = SaveDao.LoadData(PlayerPrefs.GetString("userName", "default"), data => data.friendHamHunger);
        closeness = SaveDao.LoadData(PlayerPrefs.GetString("userName", "default"), data => data.friendHamCloseness);

        // Debug.Log($"FriendHamStatus: Loaded memory count = {memory.Count}, Valence={Valence}, Arousal={Arousal}, Hunger={Hunger}");
    }
    void OnEnable()
    {
        Debug.Log("FriendHamStatus: Registering SaveMemory task to QuitManager");
        QuitManager.Instance.AddReturn2TitleTask(SaveMemory());
        QuitManager.Instance.AddReturn2TitleTask(SaveValence());
        QuitManager.Instance.AddReturn2TitleTask(SaveArousal());
        QuitManager.Instance.AddReturn2TitleTask(SaveCloseness());
    }

    void OnDisable()
    {
        // 会話履歴を保存しておく
        Debug.Log("FriendHamStatus: 会話履歴の保存");
        List<Message> conversationLog = new List<Message>();
        conversationLog = SaveDao.LoadData(PlayerPrefs.GetString("userName", "default"), data => data.conversationHistory);
        if (conversationHistory.messages.Count > 1)
        {
            conversationHistory.messages.RemoveAt(conversationHistory.messages.Count - 1);
            conversationLog.AddRange(conversationHistory.messages);
        }
        SaveDao.UpdateData(PlayerPrefs.GetString("userName", "default"), data => data.conversationHistory = conversationLog);
        conversationHistory.Clear();
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    // ともハムの発話メソッド
    // public IEnumerator Speak(string message, System.Action<string> callback)
    // {
    //     // 発話処理
    //     // 記憶と相手の発話内容に基づいて返事をする
    //     Debug.Log("[Friend Ham]Sending request to Claude API...");

    //     IEnumerator responseCoroutine = llmBridge.GetLLMResponse(message, string.Join("\n", memory));
    //     yield return StartCoroutine(responseCoroutine);

    //     // レスポンスの取得
    //     string response = responseCoroutine.Current as string;
    //     // Debug.Log($"Claude Response: {response}");
    //     // string result = "APIの結果"; // 実際はレスポンスを入れる
    //     callback?.Invoke(response);
    // }
    // public IEnumerator Speak(string message, System.Action<string> onUpdate, System.Action<string> onComplete = null)
    // {
    //     Debug.Log("[Friend Ham]Sending request to Claude API...");

    //     string finalResponse = "";

    //     IEnumerator responseCoroutine = llmBridge.GetLLMResponse(
    //         message,
    //         string.Join("\n", memory),
    //         (partialText) =>
    //         {
    //             finalResponse = partialText;
    //             onUpdate?.Invoke(partialText);  // リアルタイム更新
    //         }
    //     );

    //     yield return StartCoroutine(responseCoroutine);

    //     onComplete?.Invoke(finalResponse);  // 完了時のコールバック
    // }

    public IEnumerator Speak(string message, System.Action<string> onUpdate, System.Action<string> onComplete = null)
    {
        Debug.Log("[Friend Ham]Sending request to Claude API...");

        // ユーザーメッセージを履歴に追加
        conversationHistory.AddUserMessage(message);

        string finalResponse = "";

        IEnumerator responseCoroutine = llmBridge.GetLLMResponse(
            "あなたは親しみやすい友達のハムスターです。\n" +
            "ただし、メッセージは1から3文程度の短い文章で答えてください。\n" +
            "また、メッセージのみで、描写は含めないでください。\n" + 
            "現在時刻: " + TimeUtil.GetCurrentTimeString() + "\n" +
            "以下はこのユーザーとの会話でのあなたの記憶です。" + 
            string.Join("\n", memory) +
            "この情報を元に、以下のユーザーメッセージに返答してください。",  // システムメッセージ
            conversationHistory.ToArray(),  // 履歴全体を送信
            (partialText) =>
            {
                finalResponse = partialText;
                onUpdate?.Invoke(partialText);
            }
        );

        yield return StartCoroutine(responseCoroutine);

        // アシスタントの返答を履歴に追加
        conversationHistory.AddAssistantMessage(finalResponse);

        // // メモリにも保存
        // memory.Add($"User: {message}");
        // memory.Add($"Assistant: {finalResponse}");
        DebugPrintConversation();

        onComplete?.Invoke(finalResponse);
    }

    // 会話履歴をクリア
    public void ClearConversation()
    {
        conversationHistory.Clear();
        // memory.Clear();
    }

    public void DebugPrintConversation()
    {
        Debug.Log("=== Conversation History ===");
        foreach (var msg in conversationHistory.messages)
        {
            Debug.Log($"{msg.role}: {msg.content}");
        }
    }

    // ゲーム終了時に履歴をLLMに渡してメモリを保存する
    public IEnumerator SaveMemory()
    {
        // LLMに履歴を渡してメモリを生成する
        Debug.Log("[Friend Ham]メモリを生成中...");

        string finalResponse = "";
        
        // 要約支持を履歴に追加
        string message = "これまでの会話履歴から、あなたとの重要な思い出や情報を3つ程度要約してメモリとして保存してください。" +
                         "それぞれは短い文章で表現してください。" +
                         "また、箇条書き形式で、その内容だけを出力してください。";
        conversationHistory.AddUserMessage(message);

        IEnumerator responseCoroutine = llmBridge.GetLLMResponse(
            "最近の会話履歴から重要な情報を整理し、要約してください。",  // システムメッセージ
            conversationHistory.ToArray(),  // 履歴全体を送信
            (partialText) =>
            {
                finalResponse = partialText;
            }
            // stream: false  // ストリーミングは不要
        );
        // StartCoroutine(responseCoroutine);
        yield return StartCoroutine(responseCoroutine);
        Debug.Log($"[Friend Ham]生成されたメモリ: {finalResponse}");
        // memoryに保存
        memory.Add(finalResponse + $" (この記憶の時間: {TimeUtil.GetCurrentTimeString()})");
        // MaxMemorySize 個を超えたら古いものから削除
        if (memory.Count > MaxMemorySize)
        {
            memory.RemoveAt(0);
        }

        // ここで履歴を保存する処理を追加
        // SaveDaoを使って保存
        SaveDao.UpdateData(PlayerPrefs.GetString("userName", "default"), data => data.friendHamMemory = memory);
    }

    // ゲーム終了時にValenceを保存する
    public IEnumerator SaveValence()
    {
        // 友ハムの各種ステータスも更新して保存する
        Debug.Log("[Friend Ham]Valenceステータスを更新中...");
        // ValenceをLLMに計算させる
        yield return StartCoroutine(
        llmBridge.GetLLMStructuredOutputResponse(
            name: "return_calculation",
            description: "Returns the result of a calculation",
            "以下の会話データから、ハムスターの感情価(Valence)を算出してください。(0~100の範囲で数値を返してください）\n" +
            "会話データ:" +
            conversationHistory.MessagesToString() +
            "\n会話前のValence:" + Valence.ToString(),
            onComplete: result => Valence = Mathf.Clamp((int)result, 0, 100),
            onError: error => Debug.LogError(error)
        ));
        Debug.Log($"[Friend Ham]ステータス更新完了: Valence={Valence}");
        // SaveDaoを使って保存
        SaveDao.UpdateData(PlayerPrefs.GetString("userName", "default"), data => data.friendHamValence = Valence);
    }

    // ゲーム終了時にArousalを保存する
    public IEnumerator SaveArousal()
    {
        // 友ハムの各種ステータスも更新して保存する
        Debug.Log("[Friend Ham]Arousalステータスを更新中...");
        // ArousalをLLMに計算させる
        yield return StartCoroutine(
        llmBridge.GetLLMStructuredOutputResponse(
            name: "return_calculation",
            description: "Returns the result of a calculation",
            "以下の会話データから、ハムスターの覚醒度(Arousal)を算出してください。(0~100の範囲で数値を返してください）\n" +
            "会話データ:" +
            conversationHistory.MessagesToString() +
            "\n会話前のArousal:" + Arousal.ToString(),
            onComplete: result => Arousal = Mathf.Clamp((int)result, 0, 100),
            onError: error => Debug.LogError(error)
        ));
        Debug.Log($"[Friend Ham]ステータス更新完了: Arousal={Arousal}");
        // SaveDaoを使って保存
        SaveDao.UpdateData(PlayerPrefs.GetString("userName", "default"), data => data.friendHamArousal = Arousal);
    }
    // ゲーム終了時にClosenessを保存する
    public IEnumerator SaveCloseness()
    {
        // 友ハムの各種ステータスも更新して保存する
        Debug.Log("[Friend Ham]Closenessステータスを更新中...");
        // ClosenessをLLMに計算させる
        yield return StartCoroutine(
        llmBridge.GetLLMStructuredOutputResponse(
            name: "return_calculation",
            description: "Returns the result of a calculation",
            "以下の会話データから、ハムスターの親密度(Closeness)を算出してください。(0~100の範囲で数値を返してください）\n" +
            "会話データ:" +
            conversationHistory.MessagesToString() +
            "\n会話前のCloseness:" + Closeness.ToString(),
            onComplete: result => Closeness = Mathf.Clamp((int)result, 0, 100),
            onError: error => Debug.LogError(error)
        ));
        Debug.Log($"[Friend Ham]ステータス更新完了: Closeness={Closeness}");
        // SaveDaoを使って保存
        SaveDao.UpdateData(PlayerPrefs.GetString("userName", "default"), data => data.friendHamCloseness = Closeness);
    }
}
