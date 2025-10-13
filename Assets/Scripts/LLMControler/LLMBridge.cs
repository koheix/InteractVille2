using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class LLMBridge
{
	IEnumerator GetGPTResponse(string message)
	{
		string url = "YOUR_URL"; //Azure Open AIのエンドポイントURL
		string privateApiKey = "YOUR_API_KEY"; //Azure Open AIのAPIキー

		// ChatBody chatBody = new ChatBody
		// {
		// 	model = gptSettings.model, messages = message, max_tokens = gptSettings.max_tokens,
		// 	temperature = gptSettings.temperature, top_p = gptSettings.top_p, frequency_penalty = gptSettings.frequency_penalty,
		// 	presence_penalty = gptSettings.presences_penalty
		// };
		yield return "tmp";
	}
}

// [Serializable]
// public class GPTSettings
// {
// 	public string model = "gpt-4";
//     //生成するトークンの最大数
// 	public int max_tokens = 2048;
//     //ランダム性をコントロールするパラメータ tempratureがゼロに近づくにつれてモデルは決定論的で繰り返しの多いものになる
// 	[Range(0.0f, 1.0f)]
// 	public float temperature = 0.2f;
//     //多様性をnucleusサンプリングを介して制御 
// 	[Range(0.0f, 1.0f)]
// 	public float top_p = 0.8f;
//     //APIがさらにトークンを生成しないようにする場所
// 	public string stop;
//     //新しいトークンを、これまでのテキストでの既存の頻度に基づいてどれだけペナルティを科すかを制御
//     //これにより、モデルが同じ行を言い換える可能性が低くなる
// 	[Range(0.0f, 2.0f)]
// 	public float frequency_penalty = 0;
//     //新しいトークンを、これまでのテキストでの出現に基づいてどれだけペナルティを科すかを制御
//     //これにより、モデルが新しいトピックについて話す可能性が高くなる
// 	[Range(0.0f, 2.0f),]
// 	public float presences_penalty = 0;
// }