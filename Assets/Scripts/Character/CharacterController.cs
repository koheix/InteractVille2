using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*
characterの動きを制御するためのコード
歩行距離を計測する
*/
public class CharacterController : MonoBehaviour
{
    // 常にデフォルトの場所でスポーンするためのオプション
    [Header("テスト用")]
    [SerializeField] private bool SetPlayerPosition = false;   
    [SerializeField] private Vector2 PlayerPosition = new Vector2(4f, 1.3f);

    [Header("player speed")]
    [SerializeField] private float moveSpeed = 5f;
    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    // 歩行距離計測用の変数
    private Vector3 lastPosition;
    private float totalWalkDistance = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();  // Rigidbody2D を取得
        rb.gravityScale = 0;  // 重力を無効化
        rb.freezeRotation = true;  // 回転を固定

        // データから最後の位置を読み込んで設定
        Vector2 savedPosition = new Vector2();
        savedPosition.x = SaveDao.LoadData(PlayerPrefs.GetString("userName", default), data => data.lastPosition[0]);
        savedPosition.y = SaveDao.LoadData(PlayerPrefs.GetString("userName", default), data => data.lastPosition[1]);
        transform.position = savedPosition;

        // テスト用に特定の位置にセットするオプション
        if (SetPlayerPosition)
        {
            transform.position = PlayerPosition;
        }
        // --------------------------------------


        // 初期位置を記録
        lastPosition = transform.position;
    }

    void Update()
    {

        // 移動距離を計算
        float distanceThisFrame = Vector3.Distance(transform.position, lastPosition);
        totalWalkDistance += distanceThisFrame;
        lastPosition = transform.position;

        // Debug.Log($"総移動距離: {totalWalkDistance:F2}");

        // 入力処理（Raw を使うとキビキビした動きになる）
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();  // 斜め移動を速くしすぎないように正規化

        // アニメーション処理
        if (moveInput == Vector2.zero)
        {
            animator.SetInteger("WalkDirection", 0);
        }
        else if (moveInput.x > 0)
        {
            animator.SetInteger("WalkDirection", 4);
        }
        else if (moveInput.x < 0)
        {
            animator.SetInteger("WalkDirection", 2);
        }
        else if (moveInput.y > 0)
        {
            animator.SetInteger("WalkDirection", 3);
        }
        else if (moveInput.y < 0)
        {
            animator.SetInteger("WalkDirection", 1);
        }
    }

    void FixedUpdate()
    {
        // Rigidbody2D で移動する（transform.position ではなく velocity を使う）
        rb.linearVelocity = moveInput * moveSpeed;

        //体力が減っていたらスピードを遅くする
        int hunger = SaveDao.LoadData(PlayerPrefs.GetString("userName", default), data => data.hunger);
        if (hunger > 60)
        {
            moveSpeed = 5f;
        }
        else if (hunger > 30)
        {
            moveSpeed = 4f;
        }
        else
        {
            moveSpeed = 3f;
        }
    }

    // 外部から歩行距離を取得するメソッド
    public float GetTotalWalkDistance()
    {
        return totalWalkDistance;
    }

    // 歩行距離をリセットするメソッド
    public void ResetWalkDistance()
    {
        totalWalkDistance = 0f;
    }

    void OnApplicationQuit()
    {
        // アプリケーション終了時に現在の位置を保存
        SaveDao.UpdateData(PlayerPrefs.GetString("userName", default), data =>
        {
            data.lastPosition[0] = transform.position.x;
            data.lastPosition[1] = transform.position.y;
        });
    }
}
