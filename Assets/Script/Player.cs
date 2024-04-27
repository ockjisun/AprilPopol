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
    LayerMask enemyMask;                // ���ʹ� ���̾� ����ũ

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

        // ���� �Է�Ű�� ���� Jump�� ȣ���Ѵ�.
        // ���� ���� ���δ� Movement2D�� �Ǵ��Ѵ�.
        if (Input.GetKeyDown(KeyCode.W) && movement2D.Jump())
            anim.SetTrigger("onJump");

        // �߷� �� ����

    }

    void LateUpdate()
    {
        // �ִϸ������� �Ķ���� ����.
        anim.SetBool("isRun", currentInput.x != 0);
        anim.SetBool("isGround", movement2D.IsGrounded);
        anim.SetFloat("velocityY", Mathf.Round(movement2D.Veclocity.y));

        currentInput = Vector2.zero;
    }
}
