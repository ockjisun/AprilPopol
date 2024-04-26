using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement2D : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpPower;
    [SerializeField] float groundRadius;
    [SerializeField] Vector3 groundOffset;

    Rigidbody2D rigid;
    bool isGrounded;
    int jumpCount;

    public bool IsGrounded => isGrounded;
    public Vector2 Veclocity => rigid.velocity;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        isGrounded = false;
        jumpCount = 0;
    }

    private void Update()
    {
        // LayerMask : 물리적으로 충돌체를 감지할때 특정 Layer만 검색하고 싶을 때 사용한다.
        LayerMask groundMask = 1 << LayerMask.NameToLayer("Ground");
        isGrounded = Physics2D.OverlapCircle(transform.position + groundOffset, groundRadius, groundMask);

        // 원에 groundMask 레이어를 가진 충돌체가 충돌했고 현재 속력 중 y값이 2이하일 경우 (=내려가는 중)
        if (isGrounded && rigid.velocity.y <= 2)
            jumpCount = 1;
    }
    public void Movement(Vector2 currentInput)
    {
        // 아래 방향키를 누르면 좌,우 움직임을 멈춰야한다. (=움직일 수 없다)
        if (isGrounded && currentInput.y == -1)
            currentInput.x = 0f;

        // 내 위치 값을 우측 벡터 기준 속도 * 방향 만큼 더해준다.
        //transform.position += Vector3.right * moveSpeed * hor * Time.deltaTime;        
        rigid.velocity = new Vector2(moveSpeed * currentInput.x, rigid.velocity.y);
    }
    public bool Jump()
    {
        if (jumpCount <= 0)
            return false;

        // ForceMode2D.Force : 지속적인 힘, 미는 거
        // ForceMode2D.Impulse : 응축된 힘, 때리는 거
        rigid.velocity = new Vector2(rigid.velocity.x, 0f);             // 현재 속력 중 y값을 0으로 변경.
        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);    // 위로 n의 힘만큼 힘을 가한다.
        jumpCount -= 1;                                                 // 점프 가능 횟수 1 감소.        
        return true;
    }
    public bool Throw(float power)
    {
        rigid.velocity = new Vector2(rigid.velocity.x, 0f);
        rigid.AddForce(Vector2.up * power, ForceMode2D.Impulse);
        return true;
    }
}
