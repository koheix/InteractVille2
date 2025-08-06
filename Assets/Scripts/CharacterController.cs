using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterController : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();  // Rigidbody2D を取得
        rb.gravityScale = 0;  // 重力を無効化
        rb.freezeRotation = true;  // 回転を固定
    }

    void Update()
    {
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
    }
}
