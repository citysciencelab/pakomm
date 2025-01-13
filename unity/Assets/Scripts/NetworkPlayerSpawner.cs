//TODO: CHANGE TO NORMCORE



/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkPlayerSpawner : MonoBehaviourPunCallbacks
{
    private GameObject spawnedPlayerPrefab;


    public override void OnJoinedRoom()
    {
        #if UNITY_ANDROID
            base.OnJoinedRoom();
            spawnedPlayerPrefab =  PhotonNetwork.Instantiate("NetworkPlayer", transform.position, transform.rotation);
        #endif
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.Destroy(spawnedPlayerPrefab);
        Debug.Log("Player Left Room");
        
    }
}
*/
