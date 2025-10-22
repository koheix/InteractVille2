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

    // 停止時のhunger回復用
    [SerializeField] private float hungerRecoveryInterval = 0.1f; // 0.1秒ごと
    [SerializeField] private int hungerRecoveryAmount = 1; // 回復量
    private float idleTimer = 0f; // 止まっている時間をためて記録していく
    private float previousWalkDistance = 0f; // 前の移動距離

    void Start()
    {

        // appleCount = PlayerPrefs.GetInt("appleCount", 0);
        appleCount = SaveDao.LoadData(PlayerPrefs.GetString("userName", "default"), data => data.appleCount);

        cc = GetComponent<CharacterController>();
        // //体力のロード
        // 前のバージョン
        // if (PlayerPrefs.HasKey("hunger"))
        // {
        //     // キーが存在する場合の処理
        //     hunger = PlayerPrefs.GetInt("hunger");
        // }
        // else
        // {
        //     // キーが存在しない場合の処理（初回起動など）
        //     PlayerPrefs.SetInt("hunger", 100);
        //     hunger = 100;
        // }
        hunger = SaveDao.LoadData(PlayerPrefs.GetString("userName", "default"), data => data.hunger);
        Debug.Log("hungerrrr" + hunger);

        //最初の歩行距離
        previousWalkDistance = cc.GetTotalWalkDistance();
    }

    void Update()
    {
        Debug.Log(hunger);
        Debug.Log("playerpref hunger: " + SaveDao.LoadData(PlayerPrefs.GetString("userName", "default"), data => data.hunger));
        
        //移動距離の取得
        float currentWalkDistance = cc.GetTotalWalkDistance();

        // 単純に歩数が定数の閾値を超えたら体力を減らす
        if (currentWalkDistance > 1)
        {
            //体力を減らす
            hunger = Mathf.Clamp(--hunger, 0, 100);
            // PlayerPrefs.SetInt("hunger", hunger);
            SaveDao.UpdateData(PlayerPrefs.GetString("userName", "default"), data => data.hunger = hunger);
            cc.ResetWalkDistance();
            // statusのUIに反映
            UpdateHungerUI();

            // 歩いているのでタイマーをリセット
            idleTimer = 0f;
        }
        else
        {
            // プレイヤーが止まっている
            if(currentWalkDistance == previousWalkDistance)
            {
                // タイマー加算
                idleTimer += Time.deltaTime;

                Debug.Log("ifに入ってる");
                Debug.Log(idleTimer);
                // 1秒ごとにhungerを回復
                if(idleTimer >= hungerRecoveryInterval)
                {
                    // hungerの反映
                    hunger = Mathf.Clamp(hunger + hungerRecoveryAmount, 0, 100);
                    // PlayerPrefs.SetInt("hunger", hunger);
                    SaveDao.UpdateData(PlayerPrefs.GetString("userName", "default"), data => data.hunger = hunger);
                    UpdateHungerUI();

                    // タイマーをリセット
                    idleTimer = 0f;

                }
            }
            else
            {
                // 移動しているならタイマーをリセット
                idleTimer = 0f;
            }
        }

        previousWalkDistance = currentWalkDistance;

        //リンゴの数をUIに反映
        // appleCount = PlayerPrefs.GetInt("appleCount", 0);
        appleCount = SaveDao.LoadData(PlayerPrefs.GetString("userName", "default"), data => data.appleCount);
        
        appleCountUI.text = appleCount.ToString();
    }

    // hungerのUIを更新するメソッド
    private void UpdateHungerUI()
    {
        hungerGauge.fillAmount = hunger / 100f;
        if (hunger > 60)
        {
            hungerGauge.color = Color.green;
        }
        else if (hunger > 30)
        {
            hungerGauge.color = Color.yellow;
        }
        else
        {
            hungerGauge.color = Color.red;
        }
    }

    void FixedUpdate()
    {

    }


}