using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    private bool isGameStarted = false;
    private bool giveInputToUser = false;
    public bool GameStarted { get { return isGameStarted; } set { isGameStarted = value; } }
    public bool GiveInputToUser { get { return giveInputToUser; } set { giveInputToUser = value; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Application.targetFrameRate = 60;
    }
}
