using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameManager gameManager;
    [Space]
    [SerializeField] private Wave[] waves;
    [SerializeField] private int highestScore = 500;

    private int currentWaveIndex = -1;
    bool lastWave;
    int score { 
        get 
        {
            return gameManager.CurrentScore;
        } 
    }


    void Update()
    {
        if (lastWave) return;
        if (GetNextWave().scoreRequirement < score)
        {
            SwitchToNextWave();
        }
        
    }
    private void SwitchToNextWave()
    {
        GetNextWave().onWaveActive.Invoke();
        currentWaveIndex += 1;
        if (CurrentWaveLastWave())
        {
            lastWave = true;
        }
        else
        {
            lastWave = false;
        }
    }
    private bool CurrentWaveLastWave()
    {
        if (currentWaveIndex == waves.Length - 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private Wave GetNextWave()
    {
        int nextWaveIndex = currentWaveIndex + 1;

        return waves[nextWaveIndex];
    }
}
[Serializable]
public class Wave
{
    [SerializeField] public int scoreRequirement;
    [SerializeField] public UnityEvent onWaveActive;
}