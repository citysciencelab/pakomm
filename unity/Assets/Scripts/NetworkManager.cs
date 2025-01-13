using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Normal;
using Normal.Realtime;


public class NetworkManager : MonoBehaviour
{

    public static NetworkManager NetM; //Singleton
    public byte maxPlayersInRoom = 50;
    public int roomId = 0;
    public Realtime _Realtime; 
    
    
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
        
        
        //DontDestroyOnLoad(this);

        _Realtime = GetComponent<Realtime>();
       
        _Realtime.didConnectToRoom += RealtimeOndidConnectToRoom;
        _Realtime.didDisconnectFromRoom += RealtimeOndidDisconnectFromRoom;
    }

    private void RealtimeOndidDisconnectFromRoom(Realtime realtime)
    {
        Debug.Log("Disconnected From Room ");
        Manager.GameManager.restartGame();
    }

    private void RealtimeOndidConnectToRoom(Realtime realtime)
    {
        Debug.Log("CONNECTED TO ROOOOOOOOOOOOOOOOM");
        Debug.Log(NetworkManager.NetM._Realtime.clientID);
        
        var pics = FindObjectsOfType<DatabaseSyncNormal>().Where(obj => obj.id != null );
        if(pics.Count() == 0){
            
            DgraphQuery.DQ.loadSessionNormcore(roomId);

            if (realtime.clientID == 0)
            {
                Manager.GameManager.generateCams();
            }
        }
        
        Manager.GameManager.RoomConnected = true; 

        if (Manager.GameManager.TUIOMode)
        {
            //Instantiate(Manager.GameManager.Cursors); 
        }

        if (!Manager.GameManager.VRMode)
        {
            Debug.Log("Ich bin auf nem Touch Device am Start");
            Manager.GameManager.TouchIOSMenu.SetActive(true);
            Manager.GameManager.AnnotationsList.SetActive(true); 
        }

        if (Manager.GameManager.VRMode)
        {
          
        }
    }

    public void OnDisable()
    {   
        Debug.Log("Disabled NetworkManager" );
        _Realtime.didConnectToRoom -= RealtimeOndidConnectToRoom; 
        _Realtime.didDisconnectFromRoom -= RealtimeOndidDisconnectFromRoom; 
    }

    private void OnDestroy()
    {
        _Realtime.didConnectToRoom -= RealtimeOndidConnectToRoom;
        _Realtime.didDisconnectFromRoom -= RealtimeOndidDisconnectFromRoom;
        Debug.Log(" DESTROYED");
        
    }

    /*
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
        //DgraphQuery.DQ.loadSession(roomId);
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
        //base.OnLeftRoom();
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
    }*/
}

