using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Normal;
using Normal.Realtime;
using Unity.Mathematics;
using Unity.VisualScripting;


public class NetworkManagerCopySession : MonoBehaviour
{

    [SerializeField] private GameObject _brushStrokePrefab = null;

    public static NetworkManagerCopySession NetM; //Singleton
    public byte maxPlayersInRoom = 50;
    public int roomId = 0;
    public Realtime _RealtimeSource;
    public Realtime _RealtimeTarget;
    public int source;
    public int target;
    public bool isRunning = false;
    public bool sourceConnected = false;
    public bool targetConnected = false;
    public bool annotationsFound = false; 
    public Realtime.InstantiateOptions _InstantiateOptions;

    
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

      
        _RealtimeSource.didConnectToRoom += RealtimeOndidConnectToRoomSource;
        _RealtimeSource.didDisconnectFromRoom += RealtimeOndidDisconnectFromRoomSource;


        _RealtimeTarget.didConnectToRoom += RealtimeOndidConnectToRoomTarget;
        _RealtimeTarget.didDisconnectFromRoom += RealtimeOndidDisconnectFromRoomTarget;

    }

    private void Start()
    {
        _InstantiateOptions = new Realtime.InstantiateOptions()
        {
            useInstance = _RealtimeTarget,
            ownedByClient = true,
            destroyWhenOwnerLeaves = false,
            destroyWhenLastClientLeaves = false,
            preventOwnershipTakeover = false,

        };
    }

    private void Fertig(Room sender)
    {
        Debug.Log("FERTIG? !=");
    }


    public void connectToRooms(int roomSource, int roomTarget)
    {
        
        source = roomSource;
        target = roomTarget;
        _RealtimeSource.Connect(roomSource.ToString());
    }

    private void RealtimeOndidDisconnectFromRoomSource(Realtime realtime)
    {
        Debug.Log("Disconnected From Room From Copy Manager Room ON SOURCE REALTIME  " + realtime.room);
        sourceConnected = false; 
    }

    private void RealtimeOndidDisconnectFromRoomTarget(Realtime realtime)
    {
        Debug.Log("Disconnected From Room From Copy Manager Room ON TARGET REALTIME  " + realtime.room);
        //Manager.GameManager.restartGame();
        targetConnected = true; 

    }

    private void RealtimeOndidConnectToRoomSource(Realtime realtime)
    {
        Debug.Log("CONNECTED TO ROOOOOOOOOOOOOOOOM    " + realtime.room.name  + "  On  Realtime SOURCE");
        
        _RealtimeTarget.Connect(target.ToString());
        Debug.Log(_RealtimeSource.clientID);
        sourceConnected = true; 
    }
    
    
    
    private async void RealtimeOndidConnectToRoomTarget(Realtime realtime)
    {
        Debug.Log("CONNECTED TO ROOOOOOOOOOOOOOOOM    " + realtime.room.name  + "  On  Realtime TARGET ");

        StartCoroutine(CopyStuff()); 
        Debug.Log(_RealtimeTarget.clientID);
        targetConnected = true; 


    }


    public IEnumerator strategist(List<int> idsForMerger, int targetroom)
    {
        foreach (int id in idsForMerger)
        {
            while (sourceConnected && targetConnected)
                yield return null;
            
            yield return new WaitForSeconds(1f);
            Debug.Log("Stratgist");
            target = targetroom;
            source = id;
            _RealtimeSource.Connect(source.ToString());
            while (!sourceConnected && !targetConnected)
                yield return null;
            Debug.Log("ICH BIN WIRKLICH CONNECTED MIT BEIDEN");
            yield return StartCoroutine("FindAnnotations");
            while (!annotationsFound)
                yield return null;

            yield return StartCoroutine("CopyStuff"); 

        }
       
        
    }
    
    IEnumerator FindAnnotations()
    {
        var annotations = FindObjectsOfType<BrushStroke>().Where(obj => obj.refreshed == true);
        if (annotations.Count() <= 0)
        {
            annotationsFound = false; 
        }
        else
        {
            annotationsFound = true; 
        }

        yield return new WaitForSeconds(1f);
    }
    
    IEnumerator CopyStuff()
    {
        yield return StartCoroutine("LongCopyStuff");
        Debug.Log("Fertig mit dem Kopieren?");
        yield return new WaitForSeconds(1f);
        StartCoroutine(DisconnectRoutine());
    }
   
    IEnumerator LongCopyStuff()
    {
        var annotations = FindObjectsOfType<BrushStroke>().Where(obj => obj.refreshed == true);
        if (annotations.Count() != 0)
        {
            Debug.Log(annotations.Count());

            foreach (BrushStroke data in annotations)
            {
                
                var brushStrokeGO = Realtime.Instantiate(_brushStrokePrefab.name, _InstantiateOptions);
                brushStrokeGO.GetComponent<RealtimeView>().RequestOwnership();
                brushStrokeGO.GetComponent<RealtimeTransform>().RequestOwnership();
                brushStrokeGO.transform.position = data.gameObject.transform.position;
                brushStrokeGO.GetComponent<BrushStroke>()._brushWidth = data.GetModelBrushWidth();
                brushStrokeGO.GetComponent<BrushStroke>().SetBrushStrokeWidth(data.GetModelBrushWidth());
                brushStrokeGO.GetComponent<BrushStroke>()._color = data.GetModelColor();
                brushStrokeGO.GetComponent<BrushStroke>().SetModelColor(data.GetModelColor());
                brushStrokeGO.GetComponent<Renderer>().material.color = data.GetModelColor();
                
               
                int ribbonCount = data.GetRibbonsPointsCount();
                Debug.Log(ribbonCount);
                for (int i=0 ; i < ribbonCount; i++)
                {
                    Vector3 pos = data.GetRibbonsPointPosition(i);
                    Quaternion quat = data.GetRibbonsPointRotation(i); 
                    Debug.Log(pos.ToString());
                    Debug.Log(quat.ToString());
                    brushStrokeGO.GetComponent<BrushStroke>().AddRibbonPoint(pos, quat);
                    Debug.Log("Added RibbonPoint");

                }
                
                brushStrokeGO.GetComponent<BrushStroke>().SetModelFinalized();


                //foreach (RibbonPointModel ribbon in data.GetComponent<B>())

            }
            
            Debug.Log("habe die schleife durchlaufen");
            
        }

        yield return new WaitForSeconds(1f);
    }

    private void checkForBrushDatabase()
    {
       /* Realtime.InstantiateOptions options = new Realtime.InstantiateOptions();
        options.ownedByClient = true;
        options.preventOwnershipTakeover = false;
        options.useInstance = ;
        options.destroyWhenOwnerLeaves = false;
        options.destroyWhenLastClientLeaves = true;
        
        GameObject obj = Realtime.Instantiate("Brushstroke", new Vector3(1, 1, 1), Quaternion.identity, options);
        obj.GetComponent<BrushStroke>().AddRibbonPoint(new Vector3(3f, 3f, 3f),quaternion.identity);
        obj.GetComponent<BrushStroke>().AddRibbonPoint(new Vector3(4f, 4f, 4f),quaternion.identity);
        obj.GetComponent<BrushStroke>().AddRibbonPoint(new Vector3(5f, 5f, 5f),quaternion.identity);
        obj.GetComponent<BrushStroke>().AddRibbonPoint(new Vector3(5f, 5f, 5f),quaternion.identity);
        obj.GetComponent<BrushStroke>().AddRibbonPoint(new Vector3(5f, 5f, 5f),quaternion.identity);
        */
    }

    public void OnDisable()
    {   
        Debug.Log("Disabled NetworkManager Copy Session" );
        _RealtimeSource.didConnectToRoom -= RealtimeOndidConnectToRoomSource; 
        _RealtimeTarget.didConnectToRoom -= RealtimeOndidConnectToRoomTarget; 
        
        _RealtimeSource.didDisconnectFromRoom -= RealtimeOndidDisconnectFromRoomSource; 
        _RealtimeTarget.didDisconnectFromRoom -= RealtimeOndidDisconnectFromRoomTarget; 
        
        
    }
    
    private void OnDestroy()
    {
        Debug.Log("Destroyed NetworkManager Copy Session" );
        _RealtimeSource.didConnectToRoom -= RealtimeOndidConnectToRoomSource;
        _RealtimeTarget.didConnectToRoom -= RealtimeOndidConnectToRoomTarget;
        
        _RealtimeSource.didDisconnectFromRoom -= RealtimeOndidDisconnectFromRoomSource; 
        _RealtimeTarget.didDisconnectFromRoom -= RealtimeOndidDisconnectFromRoomTarget;
        
    }

    IEnumerator DisconnectRoutine()
    {
        _RealtimeSource.Disconnect();
        _RealtimeTarget.Disconnect();
        DgraphQuery.DQ.createEventSessionLoader();
        yield return new WaitForSeconds(0.1f);
    }

  
  
}

