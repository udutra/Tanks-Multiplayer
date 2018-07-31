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


    public List<PlayerController> allPlayers;
    public List<Text> nameText;
    public List<Text> playerScoreText;

    public Color[] playerColors;

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
        UpdateScore();
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

    public void AddPlayer(PlayerSetup pS)
    {
        if(playerCount < maxPlayers)
        {
            allPlayers.Add(pS.GetComponent<PlayerController>());
            pS.playerColor = playerColors[playerCount];
            pS.playerNum = playerCount + 1;
            playerCount++;
        }
    }

    [ClientRpc]
    public void RpcUpdateScore(int[] playerScores, string[] playerNames)
    {
        for(int i = 0; i < playerCount; i++)
        {
            playerScoreText[i].text = playerScores[i].ToString();
            nameText[i].text = playerNames[i];
        }
    }

    public void UpdateScore()
    {
        if (isServer)
        {
            int[] scores = new int[playerCount];
            string[] names = new string[playerCount];

            for (int i = 0; i < playerCount; i++)
            {
                scores[i] = allPlayers[i].score;
                names[i] = allPlayers[i].GetComponent<PlayerSetup>().playerNameText.text;
            }
            RpcUpdateScore(scores, names);
        }
    }
}
