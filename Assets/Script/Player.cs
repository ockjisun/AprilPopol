using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    public static Player Instance { get; private set; }

    [SerializeField] Ability grow;

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
        movement2D = GetComponent<Movement2D>();
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
        if (Input.GetKeyDown(KeyCode.Q) && movement2D.Jump())
            anim.SetTrigger("onJump");

        // �߷� �� ����
        LayerMask mask = 1 << LayerMask.NameToLayer("Enemy");
        Collider2D check = Physics2D.OverlapCircle(transform.position, attackRadius, mask);
        if (check != null)
        {
            check.GetComponent<Enemy>().Hit();
            movement2D.Throw(6f);
        }
    }
    private void Attack()
    {
        
        bool facingRight = spriteRenderer.flipX == false;                               // �÷��̾ �������� �ٶ󺸰� �ִ��� ���θ� Ȯ��
        Vector2 attackDirection = facingRight ? Vector2.right : Vector2.left;           // ����ĳ��Ʈ�� �� ������ ����
        Debug.DrawRay(transform.position, attackDirection * attackRadius, Color.red);   // ���� �����ϴ� ������ ��Ÿ���� ��

        
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, attackRadius, attackDirection, attackRadius, enemyMask);   // attackRadius ���� ���� ���� ã��

        // ã�� ���鿡�� �������� ���մϴ�.
        foreach (RaycastHit2D hit in hits)
        {
            // ������ �������� ���ϴ� �ڵ� �ۼ�
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                // ������ �������� ���ϴ� �Լ� ȣ�� (���÷� Damage �Լ��� ȣ��)
                enemy.Damage(grow.power);
            }
        }
    }

    void LateUpdate()
    {
        // �ִϸ������� �Ķ���� ����.
        anim.SetBool("isRun", currentInput.x != 0);
        anim.SetBool("isGround", movement2D.IsGrounded);
        anim.SetFloat("velocityY", Mathf.Round(movement2D.Veclocity.y));

        currentInput = Vector2.zero;
    }
    protected override Ability GetIncrease()
    {
        // ���ݷ�(power)�� �������� ������Ű�� �ڵ�
        float increasedPower = grow.power * GameManager.Instance.gameLevel;
        float increasedSpeed = grow.speed * GameManager.Instance.gameLevel;
        float increasedHp = grow.hp * GameManager.Instance.gameLevel;

        // ������ ������ ���ο� Ability ��ü�� �����Ͽ� ��ȯ�մϴ�.
        return new Ability
        {
            hp = increasedHp,
            power = increasedPower,
            speed = increasedSpeed,
            cooltime = grow.cooltime
        };
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, attackRadius);
    }
}

