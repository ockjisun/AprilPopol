using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.MPE;
using UnityEngine;

public abstract class Unit : RootBehaviour
{
    [SerializeField] Ability origin;    // �⺻ ��ġ

    // ���� �����Ƽ ��ġ
    private Ability ability;
    public float maxHp => ability.hp;
    public float power => ability.power;    
    public float speed => ability.speed;    
    public float cooltime => ability.cooltime;

    protected float hp;       // ü��
    protected int level;      // ����.
    protected int exp;        // ����ġ.
    protected int killCount;  // ų ī��Ʈ.

    protected Animator anim;
    public bool isAlive => hp > 0;

    protected override void OnPauseGame(bool isPause)
    {
        base.OnPauseGame(isPause);
        anim.speed = isPause ? 0f : 1f;
    }

    // ����ġ & ����
    protected virtual void AddExp(int amoun)
    {
        
    }

    // �ɷ�ġ ��� 
    public virtual void Setup()
    {
        anim = GetComponent<Animator>();
        hp = int.MaxValue;
        level = 1;
        exp = 0;
        killCount = 0;

        UpdateAbility();
    }

    // ���� ���� �������ͽ� ���
    protected virtual void UpdateAbility()
    {
        ability = origin + GetIncrease();
        hp = Mathf.Clamp(hp, 0, maxHp);
    }

    protected abstract Ability GetIncrease();

    // �ǰ�, ������ ���
    public virtual void TakeDamage(float power, float knockback = 0f)
    {
        if (!isAlive)
            return;

        if (Random.value < 0.15f)
        {
            power *= 2;
        }

        hp = Mathf.Clamp(hp - power, 0, maxHp);
        if (hp <= 0)
            Dead();
        else
            Hit(knockback);

    }

    protected virtual void Dead()
    {

    }
    protected virtual void Hit(float knockback)
    {

    }
}
