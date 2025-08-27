using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
characterのステータスを演算するコード
体力: 減ると歩く速度が遅くなる。ひまわりの種で回復する。
*/
public class CharacterStatus : MonoBehaviour
{
    //体力
    private int hunger;
    private CharacterController cc;

    void Start()
    {
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
        // Debug.Log(hunger);
        // Debug.Log("playerpref hunger: " + PlayerPrefs.GetInt("hunger"));
        //とりあえず単純に歩数が定数の閾値を超えたら体力を減らす
        // if (cc.GetTotalWalkDistance() > 10)
        // {
        //     hunger -= 10;
        //     PlayerPrefs.SetInt("hunger", hunger);
        //     cc.ResetWalkDistance();
        // }
    }

    void FixedUpdate()
    {

    }


}
