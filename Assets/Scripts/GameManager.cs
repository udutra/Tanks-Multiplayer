using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour {

    public static GameManager instance;
    public Text messagText;
    public int minPlayers = 2;
    public int maxPlayers = 4;

    [SyncVar]
    public int playerCount = 0;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start () {
        StartCoroutine(GameLoopRoutine());
    }
	
	void Update () {
		
	}

    public IEnumerator GameLoopRoutine()
    {
        yield return StartCoroutine(EnterLoby());
        yield return StartCoroutine(PlayGame());
        yield return StartCoroutine(EndGame());
    }
    private IEnumerator EnterLoby()
    {
        messagText.gameObject.SetActive(true);
        messagText.text = "Esperando jogadores...";
        while (playerCount < minPlayers)
        {
            DisablePlayers();
            yield return null;
        }
    }

    private IEnumerator PlayGame()
    {
        EnablePlayers();
        messagText.gameObject.SetActive(false);
        yield return null;
    }

    private IEnumerator EndGame()
    {
        yield return null;
    }

    private void SetPlayerState(bool state)
    {
        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();
        foreach (PlayerController p in allPlayers)
        {
            p.enabled = state;
        }
    }

    private void EnablePlayers()
    {
        SetPlayerState(true);
    }
    private void DisablePlayers()
    {
        SetPlayerState(false);
    }

    public void AddPlayer()
    {
        if(playerCount < maxPlayers)
        {
            playerCount++;
        }
    }
}
