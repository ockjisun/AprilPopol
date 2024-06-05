using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    public static Player Instance { get; private set; }    // // Player Ŭ������ �̱��� �ν��Ͻ�

    [SerializeField] Ability grow;  // Player Ŭ������ �̱��� �ν��Ͻ�

    Movement2D movement2D; // Player Ŭ������ �̱��� �ν��Ͻ�
    [SerializeField] float attackRadius;    // �÷��̾ ���� ������ �� �ִ� �ݰ�

    Animator anim;                      // �ִϸ�����.
    Vector2 currentInput;               // ���� �Է� ��.
    SpriteRenderer spriteRenderer;      // ��������Ʈ ������.
    LayerMask enemyMask;                // ���ʹ� ���̾� ����ũ

    [SerializeField] float groundRadius;    // ���� Ž�� �ݰ�
    [SerializeField] Vector3 groundOffset;  // ���� Ž�� ������

    bool isGrounded;  // �÷��̾ ���鿡 �ִ��� ����
    Rigidbody2D rigid;
    float moveSpeed;

    public bool IsGrounded => isGrounded;  // �÷��̾ ���鿡 �ִ��� Ȯ���ϴ� �Ӽ�

    void Start()
    {
        anim = GetComponent<Animator>();  // �ִϸ����� ������Ʈ ��������
        currentInput = Vector2.zero;      // ���� �Է��� 0���� �ʱ�ȭ
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyMask = 1 << LayerMask.NameToLayer("Enemy"); // "Enemy" ���̾ �ش��ϴ� ��Ʈ ����ũ�� �����Ͽ� enemyMask ������ �����ϴ� ����
        rigid = GetComponent<Rigidbody2D>();
        isGrounded = false;   // �ʱ�ȭ: ���鿡 ���� ����

        movement2D = GetComponent<Movement2D>();  // movement2D ������ Movement2D ������Ʈ �Ҵ�

        moveSpeed = movement2D.MoveSpeed;  // moveSpeed ������ Movement2D�� moveSpeed �� �Ҵ�
    }

    private void Update()
    {
        currentInput.x = Input.GetAxisRaw("Horizontal");
        currentInput.y = Input.GetAxisRaw("Vertical");

        

        // ���� �Է� ó��
        if (Input.GetKeyDown(KeyCode.Space) && movement2D.Jump())
            anim.SetTrigger("onJump");

        // �̵�
        movement2D.Movement(currentInput);

        // ����
        Attack();

        // �÷��̾ ���鿡 �ִ��� Ȯ��
        isGrounded = Physics2D.OverlapCircle(transform.position + groundOffset, groundRadius, LayerMask.GetMask("Ground"));
    }    

    private void Attack()
    {
        // �÷��̾ �ٶ󺸴� ���� ����
        bool facingRight = spriteRenderer.flipX == false;                               // �÷��̾ �������� �ٶ󺸰� �ִ��� ���θ� Ȯ��
        Vector2 attackDirection = facingRight ? Vector2.right : Vector2.left;           // ����ĳ��Ʈ�� �� ������ ����
        Debug.DrawRay(transform.position, attackDirection * attackRadius, Color.red);   // ���� �����ϴ� ������ ��Ÿ���� ��

        // ���� �ݰ� ���� �� Ž��
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, attackRadius, attackDirection, attackRadius, enemyMask);   // attackRadius ���� ���� ���� ã��

        // ������ ������ ������ ������
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
    private void Movement(Vector2 currentInput)
    {
        // �Ʒ� ����Ű�� ������ ��, �� �������� ������Ѵ�. (=������ �� ����)
        if (isGrounded && currentInput.y == -1)
            currentInput.x = 0f;

        // ������ٵ��� �ӵ� ���� (��, �� ���� �̵�)
        rigid.velocity = new Vector2(moveSpeed * currentInput.x, rigid.velocity.y);

        // �̵� ���⿡ ���� ��������Ʈ ������
        if (currentInput.x != 0f)
            spriteRenderer.flipX = currentInput.x < 0;
    }

    public void Hit()
    {
        if(movement2D.Throw(8f))
        {
            anim.SetTrigger("onHurt");   // ���� �ִϸ��̼� Ʈ����
            StartCoroutine(IEGodMode()); // ���� ���� �ڷ�ƾ ����
        }
    }
    IEnumerator IEGodMode()
    {
        float godModeTime = 2.0f;   // ���� ���� ���� �ð�
        float offset = 0.05f;       // �����̴� ����
        float time = offset;        // ��¦�̴� �ð�
        int prevLayer = gameObject.layer;  // ���� ���̾� ����

        spriteRenderer.ChangeAlpha(0.8f);  // ���� ���¸� ��Ÿ���� ���� ���� �� ����

        while ((godModeTime -= Time.deltaTime) >= 0.0f)
        {
            if((time -= Time.deltaTime) <= 0.0f)
            {
                time = offset;
                spriteRenderer.enabled = !spriteRenderer.enabled;
            }

            yield return null;
        }

        spriteRenderer.enabled = true;   // ������ Ȱ��ȭ
        spriteRenderer.ChangeAlpha(1f);  // ���� ���� 1�� ����
        gameObject.layer = prevLayer;    // ���� �±׷� ������

        Collider2D collider = GetComponent<Collider2D>();
        collider.enabled = false; // �ݶ��̴� ��Ȱ��ȭ
        yield return null;
        collider.enabled = true;  // �ݶ��̴� Ȱ��ȭ
    }

  
    void LateUpdate()
    {
        // �ִϸ����� �Ķ���� ����.
        anim.SetBool("isRun", currentInput.x != 0);
        anim.SetBool("isGround", movement2D.IsGrounded);
        anim.SetFloat("velocityY", Mathf.Round(movement2D.Velocity.y));

        currentInput = Vector2.zero; // �Է� ���� ó�� �� �ʱ�ȭ
    }

    // ���� ������ ���� ������ �ɷ�ġ�� ��ȯ�ϴ� �޼���
    protected override Ability GetIncrease()
    {
        // ���ݷ�(power)�� �������� ������Ű�� �ڵ�
        float increasedPower = grow.power * GameManager.Instance.gameLevel;
        float increasedSpeed = grow.speed * GameManager.Instance.gameLevel;
        float increasedHp = grow.hp * GameManager.Instance.gameLevel;

        // ������ ������ ���ο� Ability ��ü�� �����Ͽ� ��ȯ
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
        // �����Ϳ��� ���� �ݰ��� ��Ÿ���� �� �׸���
        Gizmos.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, attackRadius);
    }
}


// ��������Ʈ�� ���� ���� �����ϴ� Ȯ�� �޼���
public static class Method
{
    public static void ChangeAlpha(this SpriteRenderer target, float alpha)
    {
        Color color = target.color;
        color.a = alpha;
        target.color = color;
    }
}
