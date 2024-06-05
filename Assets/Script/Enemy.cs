using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    [SerializeField] Ability grow;
    [SerializeField] Transform damagePivot;   // �������� ��µǴ� ����

    [SerializeField] Movement2D movement2D;
    [SerializeField] bool isLeft;

    SpriteRenderer spriteRenderer;
    CapsuleCollider2D collider2D;
    Rigidbody2D rigid;
    Player target;
    Animator anim;

    LayerMask groundMask;
    Vector3 rayHeight;  // ����ĳ��Ʈ ���� ��ġ ������
    Vector3 rayPoint;   // ����ĳ��Ʈ ���� ��ġ

    float delayTime;

    public float health = 100f;  // ���� ü��

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        groundMask = 1 << LayerMask.NameToLayer("Ground");  // "Ground" ���̾� ����ũ ����
        rayHeight = Vector3.up * 0.5f;
        collider2D = GetComponent<CapsuleCollider2D>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        movement2D = GetComponent<Movement2D>();
    }


    // �÷��̾ Ʈ���� ������ ������ �� ȣ��Ǵ� �޼���
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
            target = player;  // �÷��̾ Ÿ������ ����
    }

    // �÷��̾ Ʈ���� ������ ������ �� ȣ��Ǵ� �޼���
    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
            target = null;  // Ÿ�� ����
    }

    void Update()
    {
        Raycast();        // ����ĳ��Ʈ�� ����Ͽ� ��ֹ� �� ���� ����
        Movement();       // ���� �̵� ó��  
        AttackToPlayer(); // �÷��̾ ����
    }

    // ����ĳ��Ʈ�� ����Ͽ� ��ֹ� �� ���� ����
    void Raycast()
    {
        // ���� ������Ʈ üũ: ���̾ �浹�ϸ� ���� ��ȯ
        float dir = isLeft ? -1f : 1f;  // ����
        Vector3 point = transform.position + (Vector3.right * 0.5f * dir) + (Vector3.up * collider2D.size.y);  // ray�� ������
        if (Physics2D.Raycast(point, Vector3.down, collider2D.size.y * 0.9f, groundMask))                      // raycast (�浹 �� true)
        {
            isLeft = !isLeft;              // ���� ��ȯ
            spriteRenderer.flipX = isLeft; // ��������Ʈ ������
            dir = isLeft ? -1f : 1f;
        }

        // ���� üũ: �ٴ��� ������ ���� ��ȯ
        rayPoint = transform.position + (Vector3.right * 0.5f * dir) + rayHeight;
        if (!Physics2D.Raycast(rayPoint, Vector2.down, 1f, groundMask))
        {
            isLeft = !isLeft;
            spriteRenderer.flipX = isLeft;
        }
    }

    // ���� �̵� ó��
    void Movement()
    {
        Vector2 input = new Vector2(isLeft ? -1f : 1f, 0f); // ���⿡ ���� �Է� �� ����
        movement2D.Movement(input);                         // �̵� ó��
    }

    // �÷��̾ �����ϴ� �޼���
    private void AttackToPlayer()
    {
        if (delayTime > 0f)
        {
            delayTime = Mathf.Clamp(delayTime - Time.deltaTime, 0.0f, cooltime);  // ���� ������ ����
            return;
        }

        if (target != null)
        {
            // target.TakeDamage(power);  // �÷��̾�� ������ ������ (�ּ� ó����)
            delayTime = cooltime;
        }
    }

    // �������� �Ծ��� �� ȣ��Ǵ� �޼���
    public void Damage(float damageAmount)
    {
        health -= damageAmount;  // ü�� ����

        if (health <= 0)
        {
            Dead();  // ü���� 0 �����̸� ��� ó��
        }
    }

    // ������ �������� ���� �� ȣ��Ǵ� �޼��� (�������̵�)
    public override void TakeDamage(float power, float knockback = 0)
    {
        base.TakeDamage(power, knockback);
        DownUI.Instance.AppearDamage(damagePivot.position, power);  // ������ UI ���
        Hit(knockback);    // �������� ���� ������ Hit �޼��带 ȣ���Ͽ� knockback ����
    }


    // ���� �׾��� �� ȣ��Ǵ� �޼���
    public virtual void DeadForce()
    {
        hp = 0;  // ü���� 0���� ����
        Dead();
    }

    // ���� �������� ���� �� ȣ��Ǵ� �޼���
    public void Hit()
    {
        collider2D.enabled = false;
        enabled = false;
        GetComponent<Animator>().SetTrigger("onDead");
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;   // ������ٵ� Ÿ���� Static���� ����
    }

    // ��� ó�� �޼��� (�������̵�)
    protected override void Dead()
    {
        GetComponent<Collider2D>().enabled = false;
        rigid.velocity = Vector2.zero;
        anim.SetTrigger("onDead");
        StartCoroutine(IEDead());     // ��� �ڷ�ƾ ����
        gameObject.SetActive(false);
        // ExpObject exp = ExpObjectPool.Instance.GetRandomExpObject();
        // exp.transform.position = transform.position;   // ����ġ ������Ʈ ���� 

    }


    // ��� �� ���̵� �ƿ� ó���ϴ� �ڷ�ƾ
    private IEnumerator IEDead()
    {
        float fadeTime = 1.0f;
        Color color = spriteRenderer.color;
        while (fadeTime > 0.0f)
        {
            fadeTime = Mathf.Clamp(fadeTime - Time.deltaTime, 0.0f, 1.0f);
            color.a = fadeTime;
            spriteRenderer.color = color;

            yield return null;
        }

        Destroy(gameObject);   // ���� ������Ʈ ����
    }


    // ���� �Ͻ� ���� ó�� (�������̵�)
    protected override void OnPauseGame(bool isPause)
    {
        base.OnPauseGame(isPause);  // �⺻ �Ͻ� ���� ó��
        rigid.constraints = isPause ? RigidbodyConstraints2D.FreezeAll : RigidbodyConstraints2D.None;
    }

    protected override Ability GetIncrease()
    {
        return grow * GameManager.Instance.gameLevel;    // ���� �ɷ�ġ ��ȯ
    }
}
