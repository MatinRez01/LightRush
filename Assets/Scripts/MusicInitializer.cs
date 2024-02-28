using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicInitializer : MonoBehaviour
{
    [SerializeField] private GameObject musicPrefab;

    MusicManager musicManager;
    private void OnEnable()
    {
        musicManager = FindAnyObjectByType<MusicManager>();
        if(musicManager == null)
        {
           musicManager = Instantiate(musicPrefab).GetComponent<MusicManager>();
        }
        musicManager.Setup();
    }
}
