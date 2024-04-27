using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Movement2D movement2D;
    [SerializeField] float attackRadius;

    Animator anim;                      // 애니메이터.
    Vector2 currentInput;               // 현재 입력 값.
    SpriteRenderer spriteRenderer;      // 스프라이트 렌더러.
    LayerMask enemyMask;                // 에너미 레이어 마스크

    void Start()
    {
        anim = GetComponent<Animator>();
        currentInput = Vector2.zero;
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyMask = 1 << LayerMask.NameToLayer("Enemy");
    }

    private void Update()
    {
        currentInput.x = Input.GetAxisRaw("Horizontal");
        currentInput.y = Input.GetAxisRaw("Vertical");
        movement2D.Movement(currentInput);

        if (currentInput.x != 0f)
            spriteRenderer.flipX = currentInput.x < 0;

        // 점프 입력키를 눌러 Jump를 호출한다.
        // 점프 가능 여부는 Movement2D가 판단한다.
        if (Input.GetKeyDown(KeyCode.W) && movement2D.Jump())
            anim.SetTrigger("onJump");

        // 발로 적 공격

    }

    void LateUpdate()
    {
        // 애니메이터의 파라미터 갱신.
        anim.SetBool("isRun", currentInput.x != 0);
        anim.SetBool("isGround", movement2D.IsGrounded);
        anim.SetFloat("velocityY", Mathf.Round(movement2D.Veclocity.y));

        currentInput = Vector2.zero;
    }
}
