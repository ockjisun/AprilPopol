using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RootBehaviour : MonoBehaviour
{
    protected bool isPauseObject;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.onPauseGame += OnPauseGame;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.onPauseGame -= OnPauseGame;
    }

    protected virtual void OnPauseGame(bool isPause)
    {
        isPauseObject = isPause;
    }
}
