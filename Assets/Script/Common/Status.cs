using UnityEditor.Playables;
using static UnityEngine.GraphicsBuffer;

[System.Serializable] public class Ability
{
    public float hp;         // 체력
    public float power;      // 공격력
    public float speed;      // 이동 속도
    public float cooltime;   // 공격 속도

    public static Ability operator +(Ability origin, Ability target)
    {
        Ability newAbility = new Ability();
        newAbility.hp = origin.hp + target.hp;
        newAbility.power = origin.power + target.power;
        newAbility.speed = origin.speed + target.speed;
        newAbility.cooltime = origin.cooltime + target.cooltime;
        return newAbility;
    }
    public static Ability operator *(Ability origin, float ratio)
    {
        Ability newAbility = new Ability();
        newAbility.hp = origin.hp * ratio;
        newAbility.power = origin.power * ratio;
        newAbility.speed = origin.speed * ratio;
        newAbility.cooltime = origin.cooltime * ratio;
        return newAbility;
    }
}

[System.Serializable] public class WeaponStatus
{
    public float power;                 // 공격력.
    public float projectileCount;       // 투사체 개수.
    public float continueTime;          // 지속시간.
    public float cooltime;              // 쿨타임.
    public float knockback;             // 넉백 수치.
    public float penetrate;             // 관통력.

    public static WeaponStatus operator *(WeaponStatus origin, Ability target)
    {
        WeaponStatus newStatus = new WeaponStatus();
        newStatus.power = origin.power * (1 + target.power / 100f);
        newStatus.cooltime = origin.cooltime * (1 + target.cooltime / 100f);
        newStatus.continueTime = origin.continueTime;
        newStatus.projectileCount = origin.projectileCount;
        newStatus.knockback = origin.knockback;
        newStatus.penetrate = origin.penetrate;

        return newStatus;
    }
    public static WeaponStatus operator *(WeaponStatus origin, int value)
    {
        WeaponStatus newStatus = new WeaponStatus();
        newStatus.power = origin.power * value;
        newStatus.projectileCount = origin.projectileCount * value;
        newStatus.cooltime = origin.cooltime * value;
        newStatus.continueTime = origin.continueTime * value;
        newStatus.knockback = origin.knockback * value;
        newStatus.penetrate = origin.penetrate * value;
        return newStatus;
    }
    public static WeaponStatus operator +(WeaponStatus origin, WeaponStatus target)
    {
        WeaponStatus newStatus = new WeaponStatus();
        newStatus.power = origin.power + target.power;
        newStatus.projectileCount = origin.projectileCount + target.projectileCount;
        newStatus.cooltime = origin.cooltime + target.cooltime;
        newStatus.continueTime = origin.continueTime + target.continueTime;
        newStatus.knockback = origin.knockback + target.knockback;
        newStatus.penetrate = origin.penetrate + target.penetrate;
        return newStatus;
    }
}
