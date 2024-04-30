using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] FollowCamera followCam;

    public float gameTime;
    public int gameLevel;

    bool isGameClear;
    bool isPauseGame;

    private void Update()
    {
        if (isPauseGame)
            return;

        gameTime += Time.deltaTime;
        gameLevel = (int)(gameTime / MAX_GAME_TIME / MAX_LEVEL);
        if (!isGameClear && gameTime >= MAX_GAME_TIME)
            isGameClear = true;
    }



    public event Action<bool> onPauseGame;     // 일시정지 이벤트
    public const float MAX_GAME_TIME = 15f;
    public const int MAX_LEVEL = 30;
}
