using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerSetup : NetworkBehaviour
{
    [SyncVar (hook = "UpdateColor")]
    public Color playerColor;


    //public int playerNum = 1;

    [SyncVar(hook = "UpdateName")]
    public string baseName = "PLAYER";

    public Text playerNameText;

	void Start () {
        UpdateName(baseName);
        UpdateColor(playerColor);
	}
	
	void Update () {
		
	}

    public override void OnStartClient()
    {
        base.OnStartClient();
        playerNameText.enabled = false;
    }

    /*public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        CmdSetupPlayer();

        playerNameText.text = baseName + " " + playerNum;
    }*/

    [Command]
    public void CmdSetupPlayer()
    {
        GameManager.instance.AddPlayer(this);
    }

    private void UpdateColor(Color color)
    {
        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer m in meshes)
        {
            m.material.color = color;
        }
    }

    private void UpdateName(string nome)
    {
        playerNameText.enabled = true;
        playerNameText.text = nome;
    }
}
