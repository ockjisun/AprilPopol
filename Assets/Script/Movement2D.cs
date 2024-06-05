using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] float jumpPower;
    [SerializeField] float groundRadius;   // 지면 감지 반경
    [SerializeField] Vector3 groundOffset; // 지면 감지 오프셋

    Rigidbody2D rigid;
    bool isGrounded;
    int jumpCount;
    SpriteRenderer spriteRenderer;

    public bool IsGrounded => isGrounded;

    public float MoveSpeed => moveSpeed;   // 이동 속도를 가져오는 속성

    public Vector2 Velocity => rigid.velocity;   // 리지드바디의 현재 속도를 반환

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // groundOffset을 플레이어의 중심으로 설정
        groundOffset = new Vector3(0f, -0.5f, 0f);

        jumpCount = 1;       // 초기값 설정 (최초에는 한 번의 점프 가능) 

        /* 
         jumpCount를 Start 메서드에서 초기화하였으며, 점프 후에는 jumpCount를 감소시킵니다.
         지면에 닿으면 jumpCount를 1로 다시 초기화하여 다시 점프할 수 있도록 합니다.
         groundOffset과 groundRadius를 사용하여 지면 감지를 수행합니다.
        */
    }

    private void Update()
    {
        // LayerMask : 물리적으로 충돌체를 감지할 때 특정 Layer만 검색하고 싶을 때 사용
        LayerMask groundMask = 1 << LayerMask.NameToLayer("Ground");

        // 원형 감지 오버랩 함수를 사용하여 지면 감지
        RaycastHit2D hit = Physics2D.CircleCast(transform.position + groundOffset, groundRadius, Vector2.down, groundRadius, groundMask);

        // 점프 가능 조건 설정 (플레이어가 지면에 닿아 있고, y 속도가 0 이하일 때)
        if (hit.collider != null && rigid.velocity.y <= 0)
            jumpCount = 1;  // 점프 가능 횟수 초기화
    }

    public void Movement(Vector2 currentInput)
    {
        // 아래 방향키를 누르면 좌, 우 움직임을 멈춰야한다. (=움직일 수 없다)
        if (isGrounded && currentInput.y == -1)
            currentInput.x = 0f;

        // 스프라이트의 방향을 이동 방향에 맞게 설정
        if (currentInput.x != 0)
            spriteRenderer.flipX = currentInput.x < 0;

        // 리지드바디의 속도 설정 (좌, 우 방향 이동)
        rigid.velocity = new Vector2(moveSpeed * currentInput.x, rigid.velocity.y);
    }

    public bool Jump()
    {
        if (jumpCount <= 0)
            return false;

        // ForceMode2D.Force : 지속적인 힘, 미는 거
        // ForceMode2D.Impulse : 응축된 힘, 때리는 거
        rigid.velocity = new Vector2(rigid.velocity.x, 0f);             // 현재 속력 중 y값을 0으로 변경.
        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);    // 위로 jumpPower 만큼의 힘을 가한다.
        jumpCount -= 1;                                                 // 점프 가능 횟수 1 감소.        
        return true;
    }

    public bool Throw(float power)
    {
        rigid.velocity = new Vector2(rigid.velocity.x, 0f);      // 현재 속도 중 y값을 0으로 설정
        rigid.AddForce(Vector2.up * power, ForceMode2D.Impulse); // 위로 power 만큼의 힘을 가한다.
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.DrawWireDisc(transform.position + groundOffset, Vector3.back, groundRadius);
    }
}
