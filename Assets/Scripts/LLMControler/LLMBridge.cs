using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class LLMBridge
{
	public static async UniTask<string> GetGPTResponse(ChatMessage[] message, GPTSettings gptSettings, CancellationToken token)
	{
		try
		{
			var url = "YOUR_URL"; //Azure Open AIのエンドポイントURL
			var privateApiKey = "YOUR_API_KEY"; //Azure Open AIのAPIキー

			ChatBody chatBody = new ChatBody
			{
				model = gptSettings.model, messages = message, max_tokens = gptSettings.max_tokens,
				temperature = gptSettings.temperature, top_p = gptSettings.top_p, frequency_penalty = gptSettings.frequency_penalty,
				presence_penalty = gptSettings.presences_penalty
			};
			string myJson = JsonUtility.ToJson(chatBody);
			byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(myJson);

			using UnityWebRequest www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
			www.uploadHandler = new UploadHandlerRaw(jsonToSend);
			www.downloadHandler = new DownloadHandlerBuffer();
			www.SetRequestHeader("api-key", privateApiKey);
			www.SetRequestHeader("Content-Type", "application/json");

			await www.SendWebRequest().ToUniTask(cancellationToken: token);
			var response = www.downloadHandler.text;
			//JSON形式のresponseをBodyクラスに変換
			var responseJson = JsonUtility.FromJson<ChatResponse>(response);
			//Bodyクラスの中のmessagesの中のcontentを取得
			var output = responseJson.choices[0].message.content;
			return output;
		}
		catch (Exception e)
		{
			throw new Exception($"エラー:{e}");
		}
	}
}

[Serializable]
public class GPTSettings
{
	public string model = "gpt-4";
    //生成するトークンの最大数
	public int max_tokens = 2048;
    //ランダム性をコントロールするパラメータ tempratureがゼロに近づくにつれてモデルは決定論的で繰り返しの多いものになる
	[Range(0.0f, 1.0f)]
	public float temperature = 0.2f;
    //多様性をnucleusサンプリングを介して制御 
	[Range(0.0f, 1.0f)]
	public float top_p = 0.8f;
    //APIがさらにトークンを生成しないようにする場所
	public string stop;
    //新しいトークンを、これまでのテキストでの既存の頻度に基づいてどれだけペナルティを科すかを制御
    //これにより、モデルが同じ行を言い換える可能性が低くなる
	[Range(0.0f, 2.0f)]
	public float frequency_penalty = 0;
    //新しいトークンを、これまでのテキストでの出現に基づいてどれだけペナルティを科すかを制御
    //これにより、モデルが新しいトピックについて話す可能性が高くなる
	[Range(0.0f, 2.0f),]
	public float presences_penalty = 0;
}