using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//@author: Anna Wolf, last changed 27.07.2021; Markus Kühner last changed 29.12.2021

public class ButtonFunctionalities : MonoBehaviour
{
    [SerializeField]
    private GameObject _prefabToInstantiate;
    [SerializeField]
    private GameObject _currentCanvasCollection;
    [SerializeField]
    private GameObject _nextCanvasCollection;

    //private LaserPointerCustom _laserPointerCustom;
    //private CubeManipulation _cubeManipulation;

    //Network dependencies
    public bool networkMode = true;

    //TODO: delete unneccessary variable
    private string prefabName;

   /* private void Awake()
    {
        //_cubeManipulation = CubeManipulation.Instance;
        //_laserPointerCustom = LaserPointerCustom.Instance;

        /*if (_cubeManipulation == null)
            Debug.LogError("The CubeManipulation instance isn`t existing yet or will never");
        */
       /*if (_laserPointerCustom == null)
            Debug.LogError("The LaserPointerCustom instance isn`t existing yet or will never");
    }*/

    /// <summary>
    /// deactivates the current pressed canvas and activates the next category one, 
    /// if some prefab is spawned - it should start from the beginning canvas categories again
    /// </summary>
    public void ChangeToNextCategoryCanvas()
    {
        _currentCanvasCollection.SetActive(false);

        if (_nextCanvasCollection)
            _nextCanvasCollection.SetActive(true);
        else
        {
            Debug.Log(this.gameObject.name + ": is not instantiating via Button, but directing to the next category layer");
        }
    }

    //instatiate prefabs; in networkMode from ressources folder to be synchronized over photon PUN (multiusermodus)
    //otherwise from the linked prefab without using photon  (single user modus)
    public void SpawnPrefabButton()
    {
        GameObject obj; 

        if (networkMode)
        {
            //to spawn networked objects, use PhotonNetwork.Instantiate(), 
            //put Prefabs in Ressources Folder and add their name
            //PhotonNetwor.InstatiateRoomObject - object wird auf Server instantiiert
           // obj = PhotonNetwork.Instantiate(_prefabToInstantiate.name, _laserPointerCustom.transform.position, Quaternion.identity);
        }
        else
        {
           // obj = Instantiate(_prefabToInstantiate, _laserPointerCustom.transform.position, Quaternion.identity);
        }
        
        //TODO: why should this not be necessary if in Network Mode?
        //obj.GetComponent<Placeable>().InitPlaceableSequence();
        //TODO: logic should work without this here
        //obj.SetActive(true);
    }
}