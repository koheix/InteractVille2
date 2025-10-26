using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class LLMBridge : MonoBehaviour
{
    private const string CLAUDE_API_URL = "https://api.anthropic.com/v1/messages";
    private const string CLAUDE_VERSION = "2023-06-01";
    
    // レスポンスのJSONデータ構造
    [System.Serializable]
    private class ClaudeRequest
    {
        public string model = "claude-sonnet-4-20250514";
        public int max_tokens = 1024;
        public Message[] messages;
    }
    
    [System.Serializable]
    private class Message
    {
        public string role;
        public string content;
    }
    
    [System.Serializable]
    private class ClaudeResponse
    {
        public string id;
        public string type;
        public string role;
        public Content[] content;
        public string model;
        public string stop_reason;
    }
    
    [System.Serializable]
    private class Content
    {
        public string type;
        public string text;
    }
    
    /// <summary>
    /// Claude APIからレスポンスを取得する
    /// </summary>
    /// <param name="message">送信するメッセージ</param>
    /// <param name="APIKey">Claude APIキー</param>
    /// <returns>Claude APIからのレスポンステキスト</returns>
    public IEnumerator GetLLMResponse(string message, string context = "")
    {
        // リクエストボディの作成
        ClaudeRequest requestData = new ClaudeRequest
        {
            messages = new Message[]
            {
                new Message
                {
                    role = "user",
                    content = message
                }
            }
        };
        
        string jsonData = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        
        // UnityWebRequestの作成
        using (UnityWebRequest request = new UnityWebRequest(CLAUDE_API_URL, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            
            // ヘッダーの設定
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("x-api-key", PlayerPrefs.GetString("APIKey"));
            request.SetRequestHeader("anthropic-version", CLAUDE_VERSION);
            
            // リクエスト送信
            yield return request.SendWebRequest();
            
            // エラーハンドリング
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Claude API Error: {request.error}");
                Debug.LogError($"Response Code: {request.responseCode}");
                Debug.LogError($"Response: {request.downloadHandler.text}");
                yield return $"Error: {request.error}";
                yield break;
            }
            
            // レスポンスのパース
            string responseText = request.downloadHandler.text;
            ClaudeResponse response = null;
            
            try
            {
                response = JsonUtility.FromJson<ClaudeResponse>(responseText);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse Claude API response: {e.Message}");
                Debug.LogError($"Raw response: {responseText}");
                // yield return $"Parse Error: {e.Message}";
                // yield break;
            }
            
            // パース成功後の処理
            if (response.content != null && response.content.Length > 0)
            {
                yield return response.content[0].text;
            }
            else
            {
                Debug.LogWarning("Claude API returned empty content");
                yield return "No response content";
            }
        }
    }

	// テスト
	private void Start()
    {
        // StartCoroutine(ExampleUsage());
    }
    
    private IEnumerator ExampleUsage()
    {
        Debug.Log("Sending request to Claude API...");
        
        IEnumerator responseCoroutine = GetLLMResponse("元気ですか？");
        yield return StartCoroutine(responseCoroutine);
        
        // レスポンスの取得
        string response = responseCoroutine.Current as string;
        Debug.Log($"Claude Response: {response}");
    }
}