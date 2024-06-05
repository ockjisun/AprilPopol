using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] float jumpPower;
    [SerializeField] float groundRadius;   // ���� ���� �ݰ�
    [SerializeField] Vector3 groundOffset; // ���� ���� ������

    Rigidbody2D rigid;
    bool isGrounded;
    int jumpCount;
    SpriteRenderer spriteRenderer;

    public bool IsGrounded => isGrounded;

    public float MoveSpeed => moveSpeed;   // �̵� �ӵ��� �������� �Ӽ�

    public Vector2 Velocity => rigid.velocity;   // ������ٵ��� ���� �ӵ��� ��ȯ

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // groundOffset�� �÷��̾��� �߽����� ����
        groundOffset = new Vector3(0f, -0.5f, 0f);

        jumpCount = 1;       // �ʱⰪ ���� (���ʿ��� �� ���� ���� ����) 

        /* 
         jumpCount�� Start �޼��忡�� �ʱ�ȭ�Ͽ�����, ���� �Ŀ��� jumpCount�� ���ҽ�ŵ�ϴ�.
         ���鿡 ������ jumpCount�� 1�� �ٽ� �ʱ�ȭ�Ͽ� �ٽ� ������ �� �ֵ��� �մϴ�.
         groundOffset�� groundRadius�� ����Ͽ� ���� ������ �����մϴ�.
        */
    }

    private void Update()
    {
        // LayerMask : ���������� �浹ü�� ������ �� Ư�� Layer�� �˻��ϰ� ���� �� ���
        LayerMask groundMask = 1 << LayerMask.NameToLayer("Ground");

        // ���� ���� ������ �Լ��� ����Ͽ� ���� ����
        RaycastHit2D hit = Physics2D.CircleCast(transform.position + groundOffset, groundRadius, Vector2.down, groundRadius, groundMask);

        // ���� ���� ���� ���� (�÷��̾ ���鿡 ��� �ְ�, y �ӵ��� 0 ������ ��)
        if (hit.collider != null && rigid.velocity.y <= 0)
            jumpCount = 1;  // ���� ���� Ƚ�� �ʱ�ȭ
    }

    public void Movement(Vector2 currentInput)
    {
        // �Ʒ� ����Ű�� ������ ��, �� �������� ������Ѵ�. (=������ �� ����)
        if (isGrounded && currentInput.y == -1)
            currentInput.x = 0f;

        // ��������Ʈ�� ������ �̵� ���⿡ �°� ����
        if (currentInput.x != 0)
            spriteRenderer.flipX = currentInput.x < 0;

        // ������ٵ��� �ӵ� ���� (��, �� ���� �̵�)
        rigid.velocity = new Vector2(moveSpeed * currentInput.x, rigid.velocity.y);
    }

    public bool Jump()
    {
        if (jumpCount <= 0)
            return false;

        // ForceMode2D.Force : �������� ��, �̴� ��
        // ForceMode2D.Impulse : ����� ��, ������ ��
        rigid.velocity = new Vector2(rigid.velocity.x, 0f);             // ���� �ӷ� �� y���� 0���� ����.
        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);    // ���� jumpPower ��ŭ�� ���� ���Ѵ�.
        jumpCount -= 1;                                                 // ���� ���� Ƚ�� 1 ����.        
        return true;
    }

    public bool Throw(float power)
    {
        rigid.velocity = new Vector2(rigid.velocity.x, 0f);      // ���� �ӵ� �� y���� 0���� ����
        rigid.AddForce(Vector2.up * power, ForceMode2D.Impulse); // ���� power ��ŭ�� ���� ���Ѵ�.
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.DrawWireDisc(transform.position + groundOffset, Vector3.back, groundRadius);
    }
}
