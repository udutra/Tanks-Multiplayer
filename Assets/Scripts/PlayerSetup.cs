using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerSetup : NetworkBehaviour
{

    public Color playerColor;
    public string baseName = "PLAYER";
    public int playerNum = 1;
    public Text playerNameText;

	void Start () {
		
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

        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer m in meshes)
        {
            m.material.color = playerColor;
        }
        playerNameText.enabled = true;
        playerNameText.text = baseName + " " + playerNum;
    }

    [Command]
    public void CmdSetupPlayer()
    {
        GameManager.instance.AddPlayer(this);
    }
}
