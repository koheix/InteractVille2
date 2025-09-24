using UnityEngine;
// using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI;
using TMPro;

/*
characterのステータスを演算するコード
体力: 減ると歩く速度が遅くなる。ひまわりの種で回復する。
*/
public class CharacterStatus : MonoBehaviour
{
    //体力
    private int hunger;
    private CharacterController cc;

    [Header("体力のUI")]
    [SerializeField] private Image hungerGauge;

    [Header("りんごのUI")]
    [SerializeField] private TextMeshProUGUI appleCountUI;
    //リンゴの数
    private int appleCount;

    void Start()
    {
        // for test
        PlayerPrefs.SetInt("hunger", 100);

        appleCount = PlayerPrefs.GetInt("appleCount", 0);

        cc = GetComponent<CharacterController>();
        //体力のロード
        if (PlayerPrefs.HasKey("hunger"))
        {
            // キーが存在する場合の処理
            hunger = PlayerPrefs.GetInt("hunger");
        }
        else
        {
            // キーが存在しない場合の処理（初回起動など）
            PlayerPrefs.SetInt("hunger", 100);
            hunger = 100;
        }
    }

    void Update()
    {
        Debug.Log(hunger);
        Debug.Log("playerpref hunger: " + PlayerPrefs.GetInt("hunger"));
        
        // 単純に歩数が定数の閾値を超えたら体力を減らす
        if (cc.GetTotalWalkDistance() > 1)
        {
            //体力を減らす
            hunger = Mathf.Clamp(--hunger, 0, 100);
            PlayerPrefs.SetInt("hunger", hunger);
            cc.ResetWalkDistance();
            // statusのUIに反映
            hungerGauge.fillAmount = hunger / 100f;
            if (hunger > 60)
            {
                hungerGauge.color = Color.green;
            }
            else if (hunger > 30) {
                hungerGauge.color = Color.yellow;
            }
            else {
                hungerGauge.color = Color.red;
            }
        }

        //リンゴの数をUIに反映
        appleCount = PlayerPrefs.GetInt("appleCount", 0);
        appleCountUI.text = appleCount.ToString();
    }

    void FixedUpdate()
    {

    }


}
