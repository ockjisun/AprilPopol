using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DownUI : Singleton<DownUI>
{
    [SerializeField] Image expFill;

    public void UpdateExp(float current, float max)
    {
        if (max <= 0)
            expFill.fillAmount = 0f;
        else
            expFill.fillAmount = current / max;
    }
}
