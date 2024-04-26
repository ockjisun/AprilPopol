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
        // LayerMask : ���������� �浹ü�� �����Ҷ� Ư�� Layer�� �˻��ϰ� ���� �� ����Ѵ�.
        LayerMask groundMask = 1 << LayerMask.NameToLayer("Ground");
        isGrounded = Physics2D.OverlapCircle(transform.position + groundOffset, groundRadius, groundMask);

        // ���� groundMask ���̾ ���� �浹ü�� �浹�߰� ���� �ӷ� �� y���� 2������ ��� (=�������� ��)
        if (isGrounded && rigid.velocity.y <= 2)
            jumpCount = 1;
    }
    public void Movement(Vector2 currentInput)
    {
        // �Ʒ� ����Ű�� ������ ��,�� �������� ������Ѵ�. (=������ �� ����)
        if (isGrounded && currentInput.y == -1)
            currentInput.x = 0f;

        // �� ��ġ ���� ���� ���� ���� �ӵ� * ���� ��ŭ �����ش�.
        //transform.position += Vector3.right * moveSpeed * hor * Time.deltaTime;        
        rigid.velocity = new Vector2(moveSpeed * currentInput.x, rigid.velocity.y);
    }
    public bool Jump()
    {
        if (jumpCount <= 0)
            return false;

        // ForceMode2D.Force : �������� ��, �̴� ��
        // ForceMode2D.Impulse : ����� ��, ������ ��
        rigid.velocity = new Vector2(rigid.velocity.x, 0f);             // ���� �ӷ� �� y���� 0���� ����.
        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);    // ���� n�� ����ŭ ���� ���Ѵ�.
        jumpCount -= 1;                                                 // ���� ���� Ƚ�� 1 ����.        
        return true;
    }
    public bool Throw(float power)
    {
        rigid.velocity = new Vector2(rigid.velocity.x, 0f);
        rigid.AddForce(Vector2.up * power, ForceMode2D.Impulse);
        return true;
    }
}
