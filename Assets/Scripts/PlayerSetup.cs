using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerSetup : NetworkBehaviour
{
    [SyncVar (hook = "UpdateColor")]
    public Color playerColor;

    [SyncVar(hook = "UpdateName")]
    public int playerNum = 1;
    
    public string baseName = "PLAYER";
    public Text playerNameText;

	void Start () {
        if (!isLocalPlayer)
        {
            UpdateName(playerNum);
            UpdateColor(playerColor);
        }
	}
	
	void Update () {
		
	}

    public override void OnStartClient()
    {
        base.OnStartClient();
        playerNameText.enabled = false;
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        CmdSetupPlayer();

        playerNameText.text = baseName + " " + playerNum;
    }

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

    private void UpdateName(int pNum)
    {
        playerNameText.enabled = true;
        playerNameText.text = baseName + " " + pNum;
    }
}
