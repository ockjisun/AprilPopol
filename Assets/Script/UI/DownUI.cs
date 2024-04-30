using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DownUI : Singleton<DownUI>
{
    [SerializeField] Image expFill;
    [SerializeField] Text LevelText;

    public void UpdateExp(float current, float max)
    {
        if (max <= 0)
            expFill.fillAmount = 0f;
        else
            expFill.fillAmount = current / max;
    }

    public void UpdateLevel(int amount)
    {
        LevelText.text = $"Lv.{amount}";
    }
    public void AppearDamage(Vector3 hitPoint, float amount)
    {
        // DamageText newText = Instantiate(damagePrefab, transform);
        // newText.Setup(hitPoint, Mathf.RoundToInt(amount));
    }

}
