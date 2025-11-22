using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;


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
        // ロールを演じさせるためのシステムメッセージ
        public string system = "あなたは友達のハムスターです。";
        public Message[] messages;
        public bool stream = false; // ストリーミングオプション
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
    // public IEnumerator GetLLMResponse(string message, string context = "")
    // {
    //     // リクエストボディの作成
    //     ClaudeRequest requestData = new ClaudeRequest
    //     {
    //         messages = new Message[]
    //         {
    //             new Message
    //             {
    //                 role = "user",
    //                 content = message
    //             }
    //         }
    //     };

    //     string jsonData = JsonUtility.ToJson(requestData);
    //     byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

    //     // UnityWebRequestの作成
    //     using (UnityWebRequest request = new UnityWebRequest(CLAUDE_API_URL, "POST"))
    //     {
    //         request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    //         request.downloadHandler = new DownloadHandlerBuffer();

    //         // ヘッダーの設定
    //         request.SetRequestHeader("Content-Type", "application/json");
    //         request.SetRequestHeader("x-api-key", PlayerPrefs.GetString("APIKey"));
    //         request.SetRequestHeader("anthropic-version", CLAUDE_VERSION);

    //         // リクエスト送信
    //         yield return request.SendWebRequest();

    //         // エラーハンドリング
    //         if (request.result != UnityWebRequest.Result.Success)
    //         {
    //             Debug.LogError($"Claude API Error: {request.error}");
    //             Debug.LogError($"Response Code: {request.responseCode}");
    //             Debug.LogError($"Response: {request.downloadHandler.text}");
    //             yield return $"Error: {request.error}";
    //             yield break;
    //         }

    //         // レスポンスのパース
    //         string responseText = request.downloadHandler.text;
    //         ClaudeResponse response = null;

    //         try
    //         {
    //             response = JsonUtility.FromJson<ClaudeResponse>(responseText);
    //         }
    //         catch (Exception e)
    //         {
    //             Debug.LogError($"Failed to parse Claude API response: {e.Message}");
    //             Debug.LogError($"Raw response: {responseText}");
    //             // yield return $"Parse Error: {e.Message}";
    //             // yield break;
    //         }

    //         // パース成功後の処理
    //         if (response.content != null && response.content.Length > 0)
    //         {
    //             yield return response.content[0].text;
    //         }
    //         else
    //         {
    //             Debug.LogWarning("Claude API returned empty content");
    //             yield return "No response content";
    //         }
    //     }
    // }

    // ストリーミング対応版
    public IEnumerator GetLLMResponse(string systemMessage, Message[] messages, System.Action<string> onPartialResponse = null, bool stream = true)
    {
        ClaudeRequest requestData = new ClaudeRequest
        {
            system = systemMessage,
            messages = messages,  // 履歴全体を送信
            stream = true
        };


        string jsonData = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        // リクエストボディの表示（デバッグ用）
        Debug.Log($"Request Body: {jsonData}");

        using (UnityWebRequest request = new UnityWebRequest(CLAUDE_API_URL, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new StreamingDownloadHandler(onPartialResponse);

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("x-api-key", PlayerPrefs.GetString("APIKey"));
            request.SetRequestHeader("anthropic-version", CLAUDE_VERSION);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Claude API Error: {request.error}");
                yield return $"Error: {request.error}";
                yield break;
            }

            yield return ((StreamingDownloadHandler)request.downloadHandler).GetFullText();
        }
    }

    // カスタムDownloadHandler
    public class StreamingDownloadHandler : DownloadHandlerScript
    {
        private System.Action<string> onPartialResponse;
        private string fullText = "";

        public StreamingDownloadHandler(System.Action<string> callback) : base()
        {
            onPartialResponse = callback;
        }

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (data == null || dataLength == 0) return false;

            string chunk = Encoding.UTF8.GetString(data, 0, dataLength);

            // Server-Sent Events (SSE)形式をパース
            string[] lines = chunk.Split('\n');
            foreach (string line in lines)
            {
                if (line.StartsWith("data: "))
                {
                    string jsonData = line.Substring(6);
                    if (jsonData == "[DONE]") continue;

                    try
                    {
                        // Claude streaming responseのパース
                        var streamEvent = JsonUtility.FromJson<ClaudeStreamEvent>(jsonData);

                        if (streamEvent.type == "content_block_delta" &&
                            streamEvent.delta != null &&
                            !string.IsNullOrEmpty(streamEvent.delta.text))
                        {
                            fullText += streamEvent.delta.text;
                            onPartialResponse?.Invoke(fullText);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"Parse error: {e.Message}");
                    }
                }
            }

            return true;
        }

        public string GetFullText()
        {
            return fullText;
        }
    }

    // Streaming用のデータクラス
    [System.Serializable]
    public class ClaudeStreamEvent
    {
        public string type;
        public StreamDelta delta;
    }

    [System.Serializable]
    public class StreamDelta
    {
        public string type;
        public string text;
    }

    // テスト
    // private void Start()
    // {
    //     // StartCoroutine(ExampleUsage());
    // }

    // メッセージ履歴を管理するクラス
    [System.Serializable]
    public class ConversationHistory
    {
        public List<Message> messages = new List<Message>();

        public void AddUserMessage(string content)
        {
            messages.Add(new Message { role = "user", content = content });
        }

        public void AddAssistantMessage(string content)
        {
            messages.Add(new Message { role = "assistant", content = content });
        }

        public Message[] ToArray()
        {
            return messages.ToArray();
        }

        public void Clear()
        {
            messages.Clear();
        }
    }

    // private IEnumerator ExampleUsage()
    // {
    //     Debug.Log("Sending request to Claude API...");

    //     IEnumerator responseCoroutine = GetLLMResponse("元気ですか？");
    //     yield return StartCoroutine(responseCoroutine);

    //     // レスポンスの取得
    //     string response = responseCoroutine.Current as string;
    //     Debug.Log($"Claude Response: {response}");
    // }
}