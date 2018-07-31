using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Prototype.NetworkLobby;

public class GameManager : NetworkBehaviour {

    public static GameManager instance;
    public Text messagText;
   
    public static List<PlayerControl> allPlayers = new List<PlayerControl>();
    public List<Text> nameText;
    public List<Text> playerScoreText;

    public int maxScore = 3;

    [SyncVar]
    public bool gameOver = false;

    private PlayerControl winner;

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
    }

    [Server]
    void Start () {
        StartCoroutine(GameLoopRoutine());
    }
	
	void Update () {
		
	}

    public IEnumerator GameLoopRoutine()
    {
        LobbyManager lManager = LobbyManager.s_Singleton;
        if(lManager != null)
        {
            while(allPlayers.Count < lManager._playerNumber)
            {
                yield return null;
            }
        }

        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(StartGame());
        yield return StartCoroutine(PlayGame());
        yield return StartCoroutine(EndGame());
        StartCoroutine(GameLoopRoutine());
    }

    [ClientRpc]
    public void RpcStartGame()
    {
        UpdateMessage("Lutem!");
        DisablePlayers();
    }
    
    private IEnumerator StartGame()
    {
        Reset();
        RpcStartGame();
        UpdateScore();
        yield return new WaitForSeconds(2f);
    }

    [ClientRpc]
    public void RpcPlayGame()
    {
        EnablePlayers();
        UpdateMessage("");
    }

    private IEnumerator PlayGame()
    {
        RpcPlayGame();

        while (!gameOver)
        {
            CheckScores();
            yield return null;
        }
    }

    [ClientRpc]
    public void RpcEndGame()
    {
        DisablePlayers();
    }

    private IEnumerator EndGame()
    {
        RpcEndGame();
        RpcUpdateMessage("Game Over \n" + winner.pSetup.baseName + " venceu!");
        yield return new WaitForSeconds(3f);
        Reset();
        LobbyManager.s_Singleton._playerNumber = 0;
        LobbyManager.s_Singleton.SendReturnToLobby(); ;
    }

    [ClientRpc]
    public void RpcUpdateMessage(string msg)
    {
        UpdateMessage(msg);
    } 

    public void UpdateMessage(string msg)
    {
        if(messagText == null)
        {
            return;
        }
        messagText.gameObject.SetActive(true);
        messagText.text = (msg);
    }

    public void CheckScores()
    {
        winner = GetWinner();
        if(winner != null)
        {
            gameOver = true;
        }
    }

    private void EnablePlayers()
    {
        for (int i = 0; i < allPlayers.Count; i++)
        {
            if(allPlayers[i] != null)
            {
                allPlayers[i].EnableControls();
            }
        }
    }
    private void DisablePlayers()
    {
        for (int i = 0; i < allPlayers.Count; i++)
        {
            if (allPlayers[i] != null)
            {
                allPlayers[i].DisableControls();
            }
        }
    }

    
    [ClientRpc]
    public void RpcUpdateScore(int[] playerScores, string[] playerNames)
    {
        for(int i = 0; i < allPlayers.Count; i++)
        {
            playerScoreText[i].text = playerScores[i].ToString();
            nameText[i].text = playerNames[i];
        }
    }

    public void UpdateScore()
    {
        if (isServer)
        {
            string[] pNames = new string[allPlayers.Count];
            int[] pScores = new int[allPlayers.Count];

            for (int i = 0; i < allPlayers.Count; i++)
            {
                if(allPlayers[i] != null)
                {
                    pNames[i] = allPlayers[i].GetComponent<PlayerSetup>().baseName;
                    pScores[i] = allPlayers[i].score;
                }
            }
            RpcUpdateScore(pScores, pNames);
        }
    }

    public PlayerControl GetWinner()
    {
        if (isServer)
        {
            for(int i = 0; i < allPlayers.Count; i++)
            {
                if(allPlayers[i].score >= maxScore)
                {
                    return allPlayers[i];
                }
            }
        }
        return null;
    }

    public void Reset()
    {
        for (int i = 0; i < allPlayers.Count; i++)
        {
            PlayerHealth pHealth = allPlayers[i].GetComponent<PlayerHealth>();
            pHealth.Reset();
            allPlayers[i].score = 0;
        }
        
    }
}
