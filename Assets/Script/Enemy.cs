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
    Vector3 rayHeight;
    Vector3 rayPoint;

    float delayTime;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        groundMask = 1 << LayerMask.NameToLayer("Ground");
        rayHeight = Vector3.up * 0.5f;
        collider2D = GetComponent<CapsuleCollider2D>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
            target = player;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
            target = null;
    }


    void Update()
    {
        Raycast();
        Movement();
        AttackToPlayer();
    }

    void Raycast()
    {
        // ���� ������Ʈ üũ : ���̾ �浹�ϸ� ���� ������
        float dir = isLeft ? -1f : 1f;  // ����
        Vector3 point = transform.position + (Vector3.right * 0.5f * dir) + (Vector3.up * collider2D.size.y);  // ray�� ������
        if (Physics2D.Raycast(point, Vector3.down, collider2D.size.y * 0.9f, groundMask))                       // raycast (�浹 �� true)
        {
            isLeft = !isLeft;
            spriteRenderer.flipX = isLeft;
            dir = isLeft ? -1f : 1f;
        }

        // ���� üũ : �ٴ��� ������ �����̶�� �Ǵ��ϰ� ���� �ٲٱ�
        rayPoint = transform.position + (Vector3.right * 0.5f * dir) + rayHeight;
        if (!Physics2D.Raycast(rayPoint, Vector2.down, 1f, groundMask))
        {
            isLeft = !isLeft;
            spriteRenderer.flipX = isLeft;
        }
    }

    void Movement()
    {
        // ���⿡ ���� �Է� ��
        Vector2 input = new Vector2(isLeft ? -1f : 1f, 0f);
        movement2D.Movement(input);
    }

    private void AttackToPlayer()
    {
        if (delayTime > 0f)
        {
            delayTime = Mathf.Clamp(delayTime - Time.deltaTime, 0.0f, cooltime);
            return;
        }

        if (target != null)
        {
            // target.TakeDamage(power);
            delayTime = cooltime;
        }
    }
    public override void TakeDamage(float power, float knockback = 0)
    {
        base.TakeDamage(power, knockback);
        DownUI.Instance.AppearDamage(damagePivot.position, power);
    }
    public virtual void DeadForce()
    {
        hp = 0;
        Dead();
    }
    protected override void Hit(float knockback)
    {
        if (anim != null)
            anim.SetTrigger("onHit");

        // �ڷ� �б�.
        if (knockback > 0f)
        {
            Vector3 dir = (transform.position - Player.Instance.transform.position).normalized;
            rigid.velocity = dir * knockback;
        }
    }
    protected override void Dead()
    {
        GetComponent<Collider2D>().enabled = false;
        rigid.velocity = Vector2.zero;
        anim.SetTrigger("onDead");
        StartCoroutine(IEDead());

        // ExpObject exp = ExpObjectPool.Instance.GetRandomExpObject();
        // exp.transform.position = transform.position;

    }
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

        Destroy(gameObject);
    }

    protected override void OnPauseGame(bool isPause)
    {
        base.OnPauseGame(isPause);
        rigid.constraints = isPause ? RigidbodyConstraints2D.FreezeAll : RigidbodyConstraints2D.None;
    }

    protected override Ability GetIncrease()
    {
        return grow * GameManager.Instance.gameLevel;
    }
}
