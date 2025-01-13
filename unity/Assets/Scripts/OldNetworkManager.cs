//TODO: CHANGE TO NORMCORE


/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using RoomOptions = Photon.Realtime.RoomOptions;

// Tutorial by Valem : https://www.youtube.com/watch?v=KHWuTBmT1oI 
// adapted by Anna Wolf 15.07.2021; 12.07.2022

public class OldNetworkManager : MonoBehaviourPunCallbacks
{

    public static NetworkManager NetM; //Singleton
    public byte maxPlayersInRoom = 50;
    public int roomId = 0;
    
    
    private void Awake()
    {
        if (NetM != null)
        {
            GameObject.Destroy(NetM);
        }
        else
        {
            NetM = this;
        }

        DontDestroyOnLoad(this);

        //TODO: Remove the line below when integrating into big project
        //ConnectToServer();
    }
    

    public void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("[PaKOMM] Try Connect To Server..."); 
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("[PaKOMM] Connected To Server.");
        
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayersInRoom;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        roomOptions.CleanupCacheOnLeave = false;
        
        PhotonNetwork.JoinOrCreateRoom(roomId.ToString(), roomOptions,TypedLobby.Default);
    }


    public override void OnJoinedRoom()
    {
        Debug.Log("[PaKOMM] You joined the room with id " + roomId);
        base.OnJoinedRoom();
        DgraphQuery.DQ.loadSession(roomId);
        //SceneContentActivation.sceneContentManager.ActivateContent();   //added by aw 12.07.2022 to ensure, that server connection is enabled before content activation
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("[PaKOMM] A new user entered the room.");
        base.OnPlayerEnteredRoom(newPlayer);
    }

    
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("[PaKOMM] Disconnect from the server APPLICATION QUIT" + cause);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnLeftRoom();
        Debug.Log("[PaKOMM] Player should have Quit" + otherPlayer.ActorNumber);
        GameObject[] cleanNetPlayer = GameObject.FindGameObjectsWithTag("NP");
        foreach (GameObject player in cleanNetPlayer)
        {
            if (player.GetComponent<PhotonView>().CreatorActorNr == otherPlayer.ActorNumber)
            {
                PhotonNetwork.Destroy(player);
                Debug.Log("[PaKOMM] Player entfernt");
            }
         }
    }
}
*/
