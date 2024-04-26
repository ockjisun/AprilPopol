using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Movement2D movement2D;
    [SerializeField] float attackRadius;

    Animator anim;                      // �ִϸ�����.
    Vector2 currentInput;               // ���� �Է� ��.
    SpriteRenderer spriteRenderer;      // ��������Ʈ ������.

    void Start()
    {
        anim = GetComponent<Animator>();
        currentInput = Vector2.zero;
        spriteRenderer = GetComponent<SpriteRenderer>();    
    }

    private void Update()
    {
        currentInput.x = Input.GetAxisRaw("Horizontal");
        currentInput.y = Input.GetAxisRaw("Vertical");
        movement2D.Movement(currentInput);

        if (currentInput.x != 0f)
            spriteRenderer.flipX = currentInput.x < 0;

        // ���� �Է�Ű�� ���� Jump�� ȣ���Ѵ�.
        // ���� ���� ���δ� Movement2D�� �Ǵ��Ѵ�.
        if (Input.GetKeyDown(KeyCode.Z) && movement2D.Jump())
            anim.SetTrigger("onJump");

        /*
        // �� �ؿ� ���� �浹�� ��� �������� �ش�.
        Collider2D enemyCollider = Physics2D.OverlapCircle(transform.position, attackRadius, enemyMask);
        if (enemyCollider != null && movement2D.Veclocity.y < 0)
        {
            enemyCollider.GetComponent<Enemy>().Hit();
            movement2D.Throw(7f);
        }
        */
    }

    void LateUpdate()
    {
        // �ִϸ������� �Ķ���� ����.
        anim.SetBool("isRun", currentInput.x != 0);
        anim.SetBool("isCrouch", currentInput.y == -1);
        anim.SetBool("isGround", movement2D.IsGrounded);
        anim.SetFloat("velocityY", Mathf.Round(movement2D.Veclocity.y));

        currentInput = Vector2.zero;
    }
}
