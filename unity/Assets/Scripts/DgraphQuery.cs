using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Normal.Realtime;
using Normal.Realtime.Serialization;
using SimpleGraphQL;
using TMPro;
using TouchScript.Behaviors;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class DgraphQuery : MonoBehaviour
{
    // Start is called before the first frame update

    public static DgraphQuery DQ;
    public GraphQLConfig config;
    public string ActiveSessionName;
    public int newestSessionNumber;
    public int activeSessionNumber;
    private GameObject tempObject;
    private GameObject activeParent;
    public bool isInitialized = false;
    public GameObject DatabaseObjects;
    public bool check = false;
    public List<RibbonPointRef> robs = new List<RibbonPointRef>();
    public bool fileFound = false;

    private void Awake()
    {
        if (DQ != null)
        {
            GameObject.Destroy(DQ);
        }
        else
        {
            DQ = this;
        }

        //DontDestroyOnLoad(this);
        DatabaseObjects = new GameObject();
    }

    void Start()
    {
        Debug.Log("robs " + robs.Count);
    }


    public async void addLogOperation(int evt, string objectId, float px, float py, float pz, float rx, float ry, float rz, float sx, float sy, float sz, string content)
    {

        int number;
        var graphQLNewest = new GraphQLClient(config);

        // You can search by file name, operation name, or operation type
        // or... mix and match between all three
        Query getNewest = graphQLNewest.FindQuery("getLogOperationNumber");

        string resultsNewest = await graphQLNewest.Send(
            getNewest.ToRequest(new Dictionary<string, int>
            {
            }),
            null,
            null,
            null

        );

        Debug.Log(resultsNewest);

        LoadDatabaseSession deserialEventSessionNewest = JsonConvert.DeserializeObject<LoadDatabaseSession>(resultsNewest);
        if (deserialEventSessionNewest.Data.QueryLogOperation.Count > 0)
        {
            number = deserialEventSessionNewest.Data.QueryLogOperation.First().Number;
            number = number + 1;
        }
        else
        {

            number = 1;
        }

        var graphQL = new GraphQLClient(config);

        // You can search by file name, operation name, or operation type
        // or... mix and match between all three
        Query query = graphQL.FindQuery("addLogOperation");

        string results = await graphQL.Send(
            query.ToRequest(new Dictionary<string, object>
            {
                { "objectid", objectId },
                { "number", number },
                { "eventSession", evt },
                { "content", content },
                { "pX", px },
                { "pY", py },
                { "pZ", pz },
                { "rX", rx },
                { "rY", ry },
                { "rZ", rz },
                { "sX", sx },
                { "sY", sy },
                { "sZ", sz },
                { "created_at", ToRfc3339String(DateTime.Now)},


            })
        );

        Debug.Log(results);
        LoadDatabaseSession deserialEventSession = JsonConvert.DeserializeObject<LoadDatabaseSession>(results);

        Debug.Log(" Ich habe eine LogOperation gespeichert ");
    }

    //Set to Locked after Leaving
    public async void lockUnlockSessionByNumber(int number, bool locked)
    {
        var graphQL = new GraphQLClient(config);

        //Take Ownership

        // You can search by file name, operation name, or operation type
        // or... mix and match between all three
        Query sessionFive = graphQL.FindQuery("lockUnlockSessionByNumber");

        string results = await graphQL.Send(
            sessionFive.ToRequest(new Dictionary<string, object>
            {
                { "number", number },
                { "locked", locked },

            })

        );


        Debug.Log(results);
        Debug.Log("Database Session " + number + " LOCKED / UNLOCKED : " + locked);

    }


    public async void UpdateEventSessionScreenshotPath(int sessionNumber, string relativePath)
    {
        var graphQL = new GraphQLClient(config);

        //Take Ownership

        // You can search by file name, operation name, or operation type
        // or... mix and match between all three
        Query query = graphQL.FindQuery("UpdateEventSessionScreenshotPath");

        string results = await graphQL.Send(
            query.ToRequest(new Dictionary<string, object>
            {
                { "number", sessionNumber },
                { "screenshotPath", relativePath },
            })
        );

        Debug.Log(results);
        Debug.Log($"DgraphHelper: Screenshot path of session {sessionNumber} was updated with {relativePath}");
    }


    public async void deleteSessionByNumber(int number)
    {
        var graphQL = new GraphQLClient(config);

        //Take Ownership

        // You can search by file name, operation name, or operation type
        // or... mix and match between all three
        Query sessionFive = graphQL.FindQuery("deleteSessionByNumber");

        string results = await graphQL.Send(
            sessionFive.ToRequest(new Dictionary<string, int>
            {
                { "number", number }
            })

        );


        Debug.Log(results);
        Debug.Log("Database Session " + number + " gelöscht");


    }

    //normal Session Creation
    public async void createNewSession(string name)
    {
        var graphQL = new GraphQLClient(config);

        // You can search by file name, operation name, or operation type
        // or... mix and match between all three
        Query getNewest = graphQL.FindQuery("getHighestSessionNr");

        string results = await graphQL.Send(
            getNewest.ToRequest(new Dictionary<string, int>
            {
            }),
            null,
            null,
            null

        );

        Debug.Log(results);

        LoadDatabaseSession deserialEventSession = JsonConvert.DeserializeObject<LoadDatabaseSession>(results);

        if (deserialEventSession.Data.QueryEventSession.Count >= 1)
        {
            newestSessionNumber = deserialEventSession.Data.QueryEventSession.First().Number;
            Debug.Log("HöchsteEventNumber " + newestSessionNumber);
            activeSessionNumber = newestSessionNumber + 1;
            DatabaseObjects.name = activeSessionNumber.ToString();
            NetworkManager.NetM.roomId = activeSessionNumber;
            NetworkManager.NetM._Realtime.Connect(activeSessionNumber.ToString());
        }
        else
        {
            activeSessionNumber = 1;
            DatabaseObjects.name = activeSessionNumber.ToString();
            NetworkManager.NetM.roomId = activeSessionNumber;
            NetworkManager.NetM._Realtime.Connect(activeSessionNumber.ToString());
        }

        var graphQLtwo = new GraphQLClient(config);
        Query query = graphQLtwo.FindQuery("addNewSession");

        string resultCreate = await graphQLtwo.Send(
            query.ToRequest(new Dictionary<string, object>
            {
                { "number", activeSessionNumber },
                { "name", name },
                { "created_at", ToRfc3339String(DateTime.Now) },
                { "deleted", false },
                { "locked", false },

            })
        );

        Debug.Log(DateTime.Now);
        LoadDatabaseSession deserialEventSessionCreate =
            JsonConvert.DeserializeObject<LoadDatabaseSession>(resultCreate);
        Debug.Log(resultCreate);
    }

    //COPYSESSION 
    public async void copySession(string name, int copyId)
    {
        var graphQL = new GraphQLClient(config);

        // You can search by file name, operation name, or operation type
        // or... mix and match between all three
        Query getNewest = graphQL.FindQuery("getHighestSessionNr");

        string results = await graphQL.Send(
            getNewest.ToRequest(new Dictionary<string, int>
            {
            }),
            null,
            null,
            null

        );

        Debug.Log(results);

        LoadDatabaseSession deserialEventSession = JsonConvert.DeserializeObject<LoadDatabaseSession>(results);

        if (deserialEventSession.Data.QueryEventSession.Count >= 1)
        {
            newestSessionNumber = deserialEventSession.Data.QueryEventSession.First().Number;
            Debug.Log("HöchsteEventNumber " + newestSessionNumber);
            activeSessionNumber = newestSessionNumber + 1;
            DatabaseObjects.name = activeSessionNumber.ToString();
        }

        var graphQLtwo = new GraphQLClient(config);

        Query addNew = graphQLtwo.FindQuery("addNewSession");

        string resultCreate = await graphQLtwo.Send(
            addNew.ToRequest(new Dictionary<string, object>
            {
                { "number", activeSessionNumber },
                { "name", name },
                { "created_at", ToRfc3339String(DateTime.Now) },
                { "deleted", false },
                { "locked", false },


            })
        );

        Debug.Log(DateTime.Now);
        LoadDatabaseSession deserialEventSessionCreate =
            JsonConvert.DeserializeObject<LoadDatabaseSession>(resultCreate);
        Debug.Log(resultCreate);

        /////////////////// Load Objects

        var graphQLthree = new GraphQLClient(config);

        // You can search by file name, operation name, or operation type
        // or... mix and match between all three
        Query sessionGetObjects = graphQLthree.FindQuery("getObjectsFromSession");

        string resultsthree = await graphQLthree.Send(
            sessionGetObjects.ToRequest(new Dictionary<string, int>
            {
                { "eventNr", copyId }

            })


        );

        Debug.Log(resultsthree);

        ////////////////// Load Objects END

        LoadDatabaseSession loadedObjects = JsonConvert.DeserializeObject<LoadDatabaseSession>(resultsthree);
        // Debug.Log("EVENTNumber  " + deserialEventSession.Data.QueryEventSession[0].Number);


        if (loadedObjects.Data.QueryEventSession != null)
        {
            if (loadedObjects.Data.QueryEventSession.FirstOrDefault() != null)
            {

                foreach (Object objects in loadedObjects.Data.QueryEventSession.FirstOrDefault().Objects)
                {

                    var graphQLadd = new GraphQLClient(config);

                    Query queryAdd = graphQLadd.FindQuery("addPlacedObject");

                    string resultNext = await graphQLadd.Send(
                        queryAdd.ToRequest(new Dictionary<string, object>
                        {
                            { "eventSession", activeSessionNumber },
                            { "pX", objects.PX },
                            { "pY", objects.PY },
                            { "pZ", objects.PZ },
                            { "rX", objects.RX },
                            { "rY", objects.RY },
                            { "rZ", objects.RZ },
                            { "sX", objects.SX },
                            { "sY", objects.SY },
                            { "sZ", objects.SZ },
                            { "prefabName", objects.PrefabName },
                            { "created_at", ToRfc3339String(DateTime.Now) },


                        })
                    );

                    Debug.Log(resultNext);

                    LoadDatabaseSession resultAnno = JsonConvert.DeserializeObject<LoadDatabaseSession>(resultNext);

                    foreach (Annotation annos in objects.annotation)
                    {

                        var graphQLAnno = new GraphQLClient(config);

                        Query queryAnno = graphQLAnno.FindQuery("addAnnotationData");

                        string AddAnnoResults = await graphQLAnno.Send(
                            queryAnno.ToRequest(new Dictionary<string, object>
                            {
                                { "id", resultAnno.Data.AddPlacedObject.PlacedObject.First().Id },
                                { "content", annos.content },
                                { "voteUp", annos.upvotes },
                                { "voteDown", annos.downvotes },
                                { "created_at", ToRfc3339String(DateTime.Now) },

                            })
                        );

                        Debug.Log(AddAnnoResults);

                    }



                }

            }


        }

        //COPY BRUSHES FROM DATABASE 
        var graphQLBrush = new GraphQLClient(config);

        // You can search by file name, operation name, or operation type
        // or... mix and match between all three
        Query sessionFiveBrush = graphQLBrush.FindQuery("getBrushAnnotationsFromSession");

        string resultsBrush = await graphQLBrush.Send(
            sessionFiveBrush.ToRequest(new Dictionary<string, int>
            {
                { "eventNr", copyId }

            })


        );

        Debug.Log(resultsBrush);

        LoadDatabaseSession deserialEventSessionBrush =
            JsonConvert.DeserializeObject<LoadDatabaseSession>(resultsBrush);
        // Debug.Log("EVENTNumber  " + deserialEventSession.Data.QueryEventSession[0].Number);

        if (deserialEventSessionBrush.Data != null)
        {
            if (deserialEventSessionBrush.Data.QueryEventSession != null)
            {
                //int numb = deserialEventSession.Data.QueryEventSession.First().Number;


                foreach (BrushAnnotation brush in deserialEventSessionBrush.Data.QueryEventSession.First()
                             .brushAnnotation)
                {
                    var graphQLBrushAdd = new GraphQLClient(config);

                    Query queryBrushAdd = graphQLBrushAdd.FindQuery("addBrushAnnotation");

                    string resultsBrushAdd = await graphQLBrushAdd.Send(
                        queryBrushAdd.ToRequest(new Dictionary<string, object>
                        {
                            { "eventSession", activeSessionNumber },
                            { "pX", brush.PX },
                            { "pY", brush.PY },
                            { "pZ", brush.PZ },
                            { "rX", brush.RX },
                            { "rY", brush.RY },
                            { "rZ", brush.RZ },
                            { "sX", brush.SX },
                            { "sY", brush.SY },
                            { "sZ", brush.SZ },
                            { "r", brush.brushColor.r },
                            { "g", brush.brushColor.g },
                            { "b", brush.brushColor.b },
                            { "a", brush.brushColor.a },
                            { "brushStrokeWidth", brush.brushStrokeWidth },
                            { "prefabName", brush.PrefabName },
                            { "created_at", ToRfc3339String(DateTime.Now) },
                            { "ribbons", brush.ribbons }


                        })
                    );

                    Debug.Log(resultsBrushAdd);
                    LoadDatabaseSession deserialEventSessionBrushAdd =
                        JsonConvert.DeserializeObject<LoadDatabaseSession>(resultsBrushAdd);


                    foreach (Annotation annos in brush.annotation)
                    {

                        var graphQLAnnoBrush = new GraphQLClient(config);

                        Query queryAnnoBrush = graphQLAnnoBrush.FindQuery("addAnnotationDataBrush");

                        string AddAnnoResults = await graphQLAnnoBrush.Send(
                            queryAnnoBrush.ToRequest(new Dictionary<string, object>
                            {
                                { "id", deserialEventSessionBrushAdd.Data.addBrushAnnotation.BrushAnnotation.First().id },
                                { "content", annos.content },
                                { "voteUp", annos.upvotes },
                                { "voteDown", annos.downvotes },
                                { "created_at", ToRfc3339String(DateTime.Now) },

                            })
                        );

                        Debug.Log(AddAnnoResults);

                    }






                }



            }
        }
        //NetworkManagerCopySession.NetM.connectToRooms(copyId, activeSessionNumber);
        DgraphQuery.DQ.createEventSessionLoader();

    }


    //MERGE SESSIONS 
    public async void mergeSession(string name)
    {
        if (Manager.GameManager.idsForMerger.Count <= 0)
        {
            Debug.Log("Ich bin leer, also nix zu tun");
            return;
        }

        var graphQL = new GraphQLClient(config);

        // You can search by file name, operation name, or operation type
        // or... mix and match between all three
        Query getNewest = graphQL.FindQuery("getHighestSessionNr");

        string results = await graphQL.Send(
            getNewest.ToRequest(new Dictionary<string, int>
            {
            }),
            null,
            null,
            null

        );

        Debug.Log(results);

        LoadDatabaseSession deserialEventSession = JsonConvert.DeserializeObject<LoadDatabaseSession>(results);

        if (deserialEventSession.Data.QueryEventSession.Count >= 1)
        {
            newestSessionNumber = deserialEventSession.Data.QueryEventSession.First().Number;
            Debug.Log("HöchsteEventNumber " + newestSessionNumber);
            activeSessionNumber = newestSessionNumber + 1;
            DatabaseObjects.name = activeSessionNumber.ToString();
        }

        var graphQLtwo = new GraphQLClient(config);

        Query addNew = graphQLtwo.FindQuery("addNewSession");

        string resultCreate = await graphQLtwo.Send(
            addNew.ToRequest(new Dictionary<string, object>
            {
                { "number", activeSessionNumber },
                { "name", name },
                { "created_at", ToRfc3339String(DateTime.Now) },
                { "deleted", false },
                { "locked", false},

            })
        );

        Debug.Log(DateTime.Now);
        LoadDatabaseSession deserialEventSessionCreate = JsonConvert.DeserializeObject<LoadDatabaseSession>(resultCreate);
        Debug.Log(resultCreate);

        //////   MERGER ///////////// Load Objects
        foreach (int id in Manager.GameManager.idsForMerger)
        {

            var graphQLthree = new GraphQLClient(config);

            // You can search by file name, operation name, or operation type
            // or... mix and match between all three
            Query sessionGetObjects = graphQLthree.FindQuery("getObjectsFromSession");

            string resultsthree = await graphQLthree.Send(
                sessionGetObjects.ToRequest(new Dictionary<string, int>
                {
                    { "eventNr", id }

                })


            );

            Debug.Log(resultsthree);

            ////////////////// Load Objects END

            LoadDatabaseSession loadedObjects = JsonConvert.DeserializeObject<LoadDatabaseSession>(resultsthree);
            // Debug.Log("EVENTNumber  " + deserialEventSession.Data.QueryEventSession[0].Number);

            if (loadedObjects.Data.QueryEventSession != null)
            {
                if (loadedObjects.Data.QueryEventSession.FirstOrDefault() != null)
                {

                    foreach (Object objects in loadedObjects.Data.QueryEventSession.FirstOrDefault().Objects)
                    {

                        var graphQLadd = new GraphQLClient(config);

                        Query queryAdd = graphQLadd.FindQuery("addPlacedObject");

                        string resultNext = await graphQLadd.Send(
                            queryAdd.ToRequest(new Dictionary<string, object>
                            {
                            { "eventSession", activeSessionNumber },
                            { "pX", objects.PX },
                            { "pY", objects.PY },
                            { "pZ", objects.PZ },
                            { "rX", objects.RX },
                            { "rY", objects.RY },
                            { "rZ", objects.RZ },
                            { "sX", objects.SX },
                            { "sY", objects.SY },
                            { "sZ", objects.SZ },
                            { "prefabName", objects.PrefabName },
                            { "created_at", ToRfc3339String(DateTime.Now) },


                            })
                        );

                        Debug.Log(resultNext);

                        LoadDatabaseSession resultAnno = JsonConvert.DeserializeObject<LoadDatabaseSession>(resultNext);

                        foreach (Annotation annos in objects.annotation)
                        {

                            var graphQLAnno = new GraphQLClient(config);

                            Query queryAnno = graphQLAnno.FindQuery("addAnnotationData");

                            string AddAnnoResults = await graphQLAnno.Send(
                                queryAnno.ToRequest(new Dictionary<string, object>
                                {
                                        { "id", resultAnno.Data.AddPlacedObject.PlacedObject.First().Id },
                                        { "content", annos.content },
                                        { "voteUp", annos.upvotes },
                                        { "voteDown", annos.downvotes },
                                        { "created_at", ToRfc3339String(DateTime.Now)},

                                })
                            );

                            Debug.Log(AddAnnoResults);

                        }



                    }

                }


            }
            //COPY BRUSHES FROM DATABASE 
            var graphQLBrush = new GraphQLClient(config);

            // You can search by file name, operation name, or operation type
            // or... mix and match between all three
            Query sessionFiveBrush = graphQLBrush.FindQuery("getBrushAnnotationsFromSession");

            string resultsBrush = await graphQLBrush.Send(
                sessionFiveBrush.ToRequest(new Dictionary<string, int>
                {
                { "eventNr", id }

                })


            );

            Debug.Log(resultsBrush);

            LoadDatabaseSession deserialEventSessionBrush =
                JsonConvert.DeserializeObject<LoadDatabaseSession>(resultsBrush);
            // Debug.Log("EVENTNumber  " + deserialEventSession.Data.QueryEventSession[0].Number);

            if (deserialEventSessionBrush.Data != null)
            {
                if (deserialEventSessionBrush.Data.QueryEventSession != null)
                {
                    //int numb = deserialEventSession.Data.QueryEventSession.First().Number;


                    foreach (BrushAnnotation brush in deserialEventSessionBrush.Data.QueryEventSession.First()
                                 .brushAnnotation)
                    {
                        var graphQLBrushAdd = new GraphQLClient(config);

                        Query queryBrushAdd = graphQLBrushAdd.FindQuery("addBrushAnnotation");

                        string resultsBrushAdd = await graphQLBrushAdd.Send(
                            queryBrushAdd.ToRequest(new Dictionary<string, object>
                            {
                            { "eventSession", activeSessionNumber },
                            { "pX", brush.PX },
                            { "pY", brush.PY },
                            { "pZ", brush.PZ },
                            { "rX", brush.RX },
                            { "rY", brush.RY },
                            { "rZ", brush.RZ },
                            { "sX", brush.SX },
                            { "sY", brush.SY },
                            { "sZ", brush.SZ },
                            { "r", brush.brushColor.r },
                            { "g", brush.brushColor.g },
                            { "b", brush.brushColor.b },
                            { "a", brush.brushColor.a },
                            { "brushStrokeWidth", brush.brushStrokeWidth },
                            { "prefabName", brush.PrefabName },
                            { "created_at", ToRfc3339String(DateTime.Now) },
                            { "ribbons", brush.ribbons }


                            })
                        );

                        Debug.Log(resultsBrushAdd);
                        LoadDatabaseSession deserialEventSessionBrushAdd =
                            JsonConvert.DeserializeObject<LoadDatabaseSession>(resultsBrushAdd);


                        foreach (Annotation annos in brush.annotation)
                        {

                            var graphQLAnnoBrush = new GraphQLClient(config);

                            Query queryAnnoBrush = graphQLAnnoBrush.FindQuery("addAnnotationDataBrush");

                            string AddAnnoResults = await graphQLAnnoBrush.Send(
                                queryAnnoBrush.ToRequest(new Dictionary<string, object>
                                {
                                { "id", deserialEventSessionBrushAdd.Data.addBrushAnnotation.BrushAnnotation.First().id },
                                { "content", annos.content },
                                { "voteUp", annos.upvotes },
                                { "voteDown", annos.downvotes },
                                { "created_at", ToRfc3339String(DateTime.Now) },

                                })
                            );

                            Debug.Log(AddAnnoResults);

                        }






                    }



                }
            }
        }
        DgraphQuery.DQ.createEventSessionLoader();

    }


    public async void createEventSessionLoader()
    {

        var graphQL = new GraphQLClient(config);

        // You can search by file name, operation name, or operation type
        // or... mix and match between all three
        Query getNewest = graphQL.FindQuery("queryAllEventSessions");

        string results = await graphQL.Send(
            getNewest.ToRequest(new Dictionary<string, int>
            {
            }),
            null,
            null,
            null

        );
        Debug.Log(results);
        LoadDatabaseSession deserialEventSession = JsonConvert.DeserializeObject<LoadDatabaseSession>(results);

        if (deserialEventSession.Data.QueryEventSession != null)
        {
            foreach (QueryEventSession every in deserialEventSession.Data.QueryEventSession)
            {
                if (every != null)
                {
                    if (every.deleted == false)
                    {
                        Debug.Log(" ICH BIN " + every.Name + "    " + every.Number + "   Created_at " + every.created_at);
                        GameObject tmp = Instantiate(Manager.GameManager.Card, Manager.GameManager.eventSessionLoaderContent.transform);
                        Transform header = tmp.transform.Find("Header Text");
                        header.GetComponent<Text>().text = every.Name;

                        Transform created = tmp.transform.Find("CreatedAt");
                        created.GetComponent<Text>().text = every.created_at.ToString();
                        Transform tmpTrans = tmp.transform.Find("TextButton");
                        tmpTrans.GetComponentInChildren<Text>().text = " Load Session " + every.Number;
                        Transform del = tmp.transform.Find("Delete");
                        Button delbut = del.GetComponent<Button>();
                        Transform Copy = tmp.transform.Find("CopyButton");
                        Button copbut = Copy.GetComponent<Button>();
                        Transform Toggl = tmp.transform.Find("Toggle");
                        Toggl.name = every.Number.ToString();
                        Transform Locked = tmp.transform.Find("Locked");
                        Locked.GetComponent<Toggle>().isOn = every.locked;
                        Locked.name = every.Number.ToString();
                        //TextMeshProUGUI tmpTitle = tmpTrans.GetComponent<TextMeshProUGUI>();
                        //Debug.Log(tmpTitle);
                        //tmpTitle.text = every.Number.ToString();
                        //Transform tmpButt = tmp.transform.Find("Trigger");
                        Button but = tmpTrans.GetComponent<Button>();

                        but.onClick.AddListener(() => Manager.GameManager.loadSceneFromDatabase(every.Number, every.locked));
                        delbut.onClick.AddListener(() => Manager.GameManager.DeleteSessionWithNumber(every.Number, tmp));
                        copbut.onClick.AddListener(() => Manager.GameManager.copySession(every.Number));

                        //String pathToImage = every.Number.ToString(); 
                        string pathToImage = every.screenshotPath;

                        Transform Image = tmp.transform.Find("Image");

                        //Image.GetComponent<Image>().sprite = GetPhoto(pathToImage);
                        var image = Image.GetComponent<Image>();

                        //var texture = new Texture2D(73, 73);
                        StartCoroutine(ScreenshotHelper.shared.LoadThumbnailCoroutine(pathToImage, image));
                    }
                }


                LayoutRebuilder.ForceRebuildLayoutImmediate(Manager.GameManager.EventSessionLoader
                    .GetComponent<RectTransform>());
            }

        }

        else
        {

        }

        Manager.GameManager.EventSessionLoader.SetActive(true);

    }

    //Search Session  by Term or Number
    public async void createEventSessionLoaderBySearch(string term)
    {
        var graphQL = new GraphQLClient(config);
        Query getNewest;
        string results;

        int value;
        if (int.TryParse(term, out value))
        {
            //TODO: Query by SessionNumber
            Debug.Log("ICH BIN EINE ZAHL : " + value);

            getNewest = graphQL.FindQuery("searchEventSessionByNumber");
            results = await graphQL.Send(
                getNewest.ToRequest(new Dictionary<string, object>
                {
                    { "value", value },
                }),
                null,
                null,
                null

            );
        }

        else
        {
            getNewest = graphQL.FindQuery("searchEventSessionByTerm");
            results = await graphQL.Send(
                getNewest.ToRequest(new Dictionary<string, object>
                {
                    { "term", term },
                }),
                null,
                null,
                null

            );

        }



        // You can search by file name, operation name, or operation type
        // or... mix and match between all three

        Debug.Log(results);
        LoadDatabaseSession deserialEventSession = JsonConvert.DeserializeObject<LoadDatabaseSession>(results);

        if (deserialEventSession.Data.QueryEventSession != null)
        {
            foreach (QueryEventSession every in deserialEventSession.Data.QueryEventSession)
            {
                if (every != null)
                {
                    if (every.deleted == false)
                    {
                        Debug.Log(" ICH BIN " + every.Name + "    " + every.Number + "   Created_at " + every.created_at);
                        GameObject tmp = Instantiate(Manager.GameManager.Card, Manager.GameManager.eventSessionLoaderContent.transform);
                        Transform header = tmp.transform.Find("Header Text");
                        header.GetComponent<Text>().text = every.Name;
                        String pathToImage = every.Number.ToString();
                        Transform Image = tmp.transform.Find("Image");
                        //Image.GetComponent<Image>().sprite = GetPhoto(pathToImage);
                        var image = Image.GetComponent<Image>();

                        //var texture = new Texture2D(73, 73);
                        StartCoroutine(ScreenshotHelper.shared.LoadThumbnailCoroutine(pathToImage, image));
                        //Image.GetComponent<Image>().sprite = GetPhoto(pathToImage);
                        Transform created = tmp.transform.Find("CreatedAt");
                        created.GetComponent<Text>().text = every.created_at.ToString();
                        Transform tmpTrans = tmp.transform.Find("TextButton");
                        tmpTrans.GetComponentInChildren<Text>().text = " Load Session " + every.Number;
                        Transform del = tmp.transform.Find("Delete");
                        Button delbut = del.GetComponent<Button>();
                        Transform Copy = tmp.transform.Find("CopyButton");
                        Button copbut = Copy.GetComponent<Button>();
                        Transform Toggl = tmp.transform.Find("Toggle");
                        Toggl.name = every.Number.ToString();
                        //TextMeshProUGUI tmpTitle = tmpTrans.GetComponent<TextMeshProUGUI>();
                        //Debug.Log(tmpTitle);
                        //tmpTitle.text = every.Number.ToString();
                        //Transform tmpButt = tmp.transform.Find("Trigger");
                        Button but = tmpTrans.GetComponent<Button>();

                        but.onClick.AddListener(() => Manager.GameManager.loadSceneFromDatabase(every.Number, every.locked));
                        delbut.onClick.AddListener(() => Manager.GameManager.DeleteSessionWithNumber(every.Number, tmp));
                        copbut.onClick.AddListener(() => Manager.GameManager.copySession(every.Number));
                    }
                }


                LayoutRebuilder.ForceRebuildLayoutImmediate(Manager.GameManager.EventSessionLoader
                    .GetComponent<RectTransform>());
            }

        }

        Manager.GameManager.EventSessionLoader.SetActive(true);

    }



//    public IEnumerator delayedShare(string path)
//    {
//        Debug.Log("DelayShare Reached ");
//#if !UNITY_EDITOR
//		    string url = Application.dataPath + "/" + path + ".png";
//#endif
//#if UNITY_EDITOR
//        string url = Path.GetDirectoryName(Application.dataPath) + "/" + path + ".png";
//#endif
//#if !UNITY_IOS || !UNITY_ANDROID

//        while (IsFileUnavailable(url))
//        {
//            Debug.Log("file locked");
//            yield return new WaitForSeconds(.05f);
//        }
//        CopyPhoto(path);

//#endif


//    }

    protected virtual bool IsFileUnavailable(string path)
    {
        // if file doesn't exist, return true
        if (!File.Exists(path))
            return true;

        FileInfo file = new FileInfo(path);
        FileStream stream = null;

        try
        {
            stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
        }
        catch (IOException)
        {
            //the file is unavailable because it is:
            //still being written to
            //or being processed by another thread
            //or does not exist (has already been processed)
            return true;
        }
        finally
        {
            if (stream != null)
                stream.Close();
        }

        //file is not locked
        return false;
    }


//    public void CopyPhoto(string path)
//    {

//#if !UNITY_EDITOR
//		string url = Application.dataPath + "/" + path + ".png";
//#endif
//#if UNITY_EDITOR
//        string url = Path.GetDirectoryName(Application.dataPath) + "/" + path + ".png";
//#endif
//        if (File.Exists(url))
//        {
//            var bytes = File.ReadAllBytes(url);
//            File.WriteAllBytes(Application.persistentDataPath + "/" + path + ".png", bytes);
//            Debug.Log("DATEI KOPIERT");
//            File.Delete(url);
//            StartCoroutine(Upload(path));

//        }
//        else
//        {

//            Debug.Log("Datei nicht gefunden");

//        }
//    }


    //public Sprite GetPhoto(String path)
    //{

    //    string url = Application.persistentDataPath + "/" + path + ".png";
    //    if (File.Exists(url))
    //    {

    //        var bytes = File.ReadAllBytes(url);
    //        Texture2D texture = new Texture2D(73, 73);
    //        texture.LoadImage(bytes);
    //        Sprite spri = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
    //            new Vector2(0.5f, 0.5f));
    //        return spri;

    //    }

    //    else if (!File.Exists(url))
    //    {
    //        StartCoroutine(Upload(path));
    //        if (fileFound)
    //        {
    //            Debug.Log("file war NICHT DA UND wurde heruntergalden");
    //            var bytes = File.ReadAllBytes(url);
    //            Texture2D texture = new Texture2D(73, 73);
    //            texture.LoadImage(bytes);
    //            Sprite spri = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
    //                new Vector2(0.5f, 0.5f));
    //            return spri;
    //        }
    //        else
    //        {
    //            return null;
    //        }

    //    }

    //    return null;
    //}


    //public IEnumerator Upload(string path)
    //{

    //    var cloud = new ES3Cloud("https://pakomm.madrobot.de/ES3Cloud.php", "21c82bc7a059");
    //    yield return StartCoroutine(cloud.Sync(path + ".png"));
    //    if (cloud.isDone)
    //    {
    //        Debug.Log(" FILE UPLOADED / DOWNLOADED   " + path);


    //    }
    //    if (cloud.isError)
    //    {
    //        Debug.LogError(cloud.error);
    //        fileFound = false;

    //    }

    //}

    public async void deleteObjectById(GameObject go, string id)
    {


        if (id != null || id != "")
        {
            var graphQL = new GraphQLClient(config);

            //Take Ownership
            go.GetComponent<RealtimeView>().RequestOwnership();
            go.GetComponent<RealtimeTransform>().RequestOwnership();


            var annotations = FindObjectsOfType<DatabaseSyncAnnotation>().Where(obj => obj.visualParentId == id);
            if (annotations != null)
            {
                foreach (DatabaseSyncAnnotation annos in annotations)
                {
                    Realtime.Destroy(annos.gameObject);

                }



            }

            // You can search by file name, operation name, or operation type
            // or... mix and match between all three
            if (go.tag == "Annotation")
            {
                Query sessionFive = graphQL.FindQuery("deleteBrushAnnotationByID");
                string results = await graphQL.Send(
                    sessionFive.ToRequest(new Dictionary<string, string>
                    {
                        { "id", id }
                    })

                );
                Debug.Log(results);
                //LOGOPERATION 
                addLogOperation(activeSessionNumber, id, 0, 0, 0, 0, 0, 0, 0, 0, 0, "Deleted Annotation Object");
            }
            else
            {

                Query sessionFive = graphQL.FindQuery("deleteObjectByID");
                string results = await graphQL.Send(
                    sessionFive.ToRequest(new Dictionary<string, string>
                    {
                        { "id", id }
                    })

                );
                Debug.Log(results);


                //LOGOPERATION 
                addLogOperation(activeSessionNumber, id, 0, 0, 0, 0, 0, 0, 0, 0, 0, "Deleted Placed Object");

            }

            Debug.Log("Database Objekt gelöscht");



            //DELETE OBJECT
            Debug.Log("Realtime gelöscht");
            Realtime.Destroy(go);

            if (!Manager.GameManager.VRMode)
            {
                AnnotationList.AL.ConstructAndShowScrollView();
            }


        }
    }

    public async void loadSession(int sessionNumber)
    {
        NetworkManager.NetM.roomId = sessionNumber;
        NetworkManager.NetM._Realtime.Connect(sessionNumber.ToString());
        activeSessionNumber = sessionNumber;
        DatabaseObjects.name = "DatabaseObjects " + activeSessionNumber.ToString();
        isInitialized = true;
    }

    public async void loadSessionNormcore(int sessionNumber)
    {
        var graphQL = new GraphQLClient(config);

        // You can search by file name, operation name, or operation type
        // or... mix and match between all three
        Query sessionFive = graphQL.FindQuery("getObjectsFromSession");

        string results = await graphQL.Send(
            sessionFive.ToRequest(new Dictionary<string, int>
            {
                { "eventNr", sessionNumber }

            })


        );

        Debug.Log(results);

        LoadDatabaseSession deserialEventSession = JsonConvert.DeserializeObject<LoadDatabaseSession>(results);
        // Debug.Log("EVENTNumber  " + deserialEventSession.Data.QueryEventSession[0].Number);


        if (deserialEventSession.Data.QueryEventSession != null)
        {
            //int numb = deserialEventSession.Data.QueryEventSession.First().Number;
            DatabaseObjects.name = sessionNumber.ToString();

            if (deserialEventSession.Data.QueryEventSession.FirstOrDefault() != null)
            {
                Manager.GameManager.locked = deserialEventSession.Data.QueryEventSession.First().locked;

                foreach (Object objects in deserialEventSession.Data.QueryEventSession.First().Objects)
                {
                    Debug.Log(objects.Id + "    " + objects.PrefabName + "    " + objects.PX);


                    if (objects.PrefabName != null && Resources.Load(objects.PrefabName))
                    {
                        Realtime.InstantiateOptions options = new Realtime.InstantiateOptions();
                        options.ownedByClient = false;
                        options.preventOwnershipTakeover = false;
                        options.useInstance = NetworkManager.NetM._Realtime;
                        options.destroyWhenOwnerLeaves = false;
                        options.destroyWhenLastClientLeaves = true;

                        tempObject = Realtime.Instantiate(objects.PrefabName,
                            new Vector3(objects.PX, objects.PY, objects.PZ),
                            Quaternion.Euler(objects.RX, objects.RY, objects.RZ), options);

                        if (tempObject != null)
                        {
                            tempObject.GetComponent<RealtimeTransform>().RequestOwnership();
                            tempObject.transform.localScale = new Vector3(objects.SX, objects.SY, objects.SZ);
                        }

                        var tmpDataBaseId = tempObject.GetOrAddComponent<DatabaseSyncNormal>();
                        tmpDataBaseId.SetId(objects.Id);
                        tempObject.GetComponent<Collider>().enabled = true;

                    }

                    if (objects.annotation != null)
                    {
                        foreach (Annotation annos in objects.annotation)
                        {
                            GameObject _tmpAnno = Manager.GameManager.AnnotationData;
                            Realtime.InstantiateOptions options = new Realtime.InstantiateOptions();
                            options.ownedByClient = true;
                            options.preventOwnershipTakeover = false;
                            options.useInstance = NetworkManager.NetM._Realtime;
                            options.destroyWhenOwnerLeaves = false;
                            options.destroyWhenLastClientLeaves = true;
                            GameObject obj = Realtime.Instantiate(prefabName: Manager.GameManager.AnnotationData.name,
                                options);
                            obj.GetComponent<RealtimeView>().RequestOwnership();
                            //obj.transform.SetParent(tempObject.transform);
                            tempObject.GetComponent<DatabaseSyncNormal>().SetMarker(true);

                            obj.GetOrAddComponent<DatabaseSyncAnnotation>().SetAnnotation(annos.id, annos.content,
                                annos.upvotes, annos.downvotes, annos.created_at, objects.Id);

                        }
                    }

                }
            }


        }


        if (!Manager.GameManager.VRMode)
        {
            AnnotationList.AL.ConstructAndShowScrollView();
        }

        if (Manager.GameManager.locked)
        {
            Manager.GameManager.TouchIOSMenu.GetComponent<DisableCreation>().disableCreation();
        }

        var graphQLBrush = new GraphQLClient(config);

        // You can search by file name, operation name, or operation type
        // or... mix and match between all three
        Query sessionFiveBrush = graphQLBrush.FindQuery("getBrushAnnotationsFromSession");

        string resultsBrush = await graphQLBrush.Send(
            sessionFiveBrush.ToRequest(new Dictionary<string, int>
            {
                { "eventNr", sessionNumber }

            })


        );

        Debug.Log(resultsBrush);

        LoadDatabaseSession deserialEventSessionBrush = JsonConvert.DeserializeObject<LoadDatabaseSession>(resultsBrush);
        // Debug.Log("EVENTNumber  " + deserialEventSession.Data.QueryEventSession[0].Number);

        if (deserialEventSessionBrush.Data != null)
        {
            if (deserialEventSessionBrush.Data.QueryEventSession != null)
            {
                //int numb = deserialEventSession.Data.QueryEventSession.First().Number;


                foreach (BrushAnnotation brush in deserialEventSessionBrush.Data.QueryEventSession.First()
                             .brushAnnotation)
                {
                    Debug.Log(deserialEventSessionBrush.Data.QueryEventSession.First().brushAnnotation.Count);
                    Debug.Log(brush.id + "    " + brush.PrefabName + "    " + brush.PX);


                    if (brush.PrefabName != null && Resources.Load(brush.PrefabName))
                    {
                        Realtime.InstantiateOptions options = new Realtime.InstantiateOptions();
                        options.ownedByClient = false;
                        options.preventOwnershipTakeover = false;
                        options.useInstance = NetworkManager.NetM._Realtime;
                        options.destroyWhenOwnerLeaves = false;
                        options.destroyWhenLastClientLeaves = true;

                        tempObject = Realtime.Instantiate(brush.PrefabName,
                            new Vector3(brush.PX, brush.PY, brush.PZ),
                            Quaternion.Euler(brush.RX, brush.RY, brush.RZ), options);

                        if (tempObject != null)
                        {
                            tempObject.GetComponent<RealtimeTransform>().RequestOwnership();
                            tempObject.GetComponent<RealtimeView>().RequestOwnership();
                            tempObject.transform.localScale = new Vector3(brush.SX, brush.SY, brush.SZ);
                        }

                        BrushStroke _brushStroke = tempObject.GetComponent<BrushStroke>();
                        _brushStroke._brushWidth = brush.brushStrokeWidth;
                        _brushStroke._color = new Color(brush.brushColor.r,
                            brush.brushColor.g, brush.brushColor.b, brush.brushColor.a);

                        tempObject.GetComponent<Renderer>().material.color = _brushStroke._color;
                        tempObject.GetComponent<BrushStrokeMesh>()._brushStrokeWidth =
                            brush.brushStrokeWidth;


                        var tmpDataBaseId = tempObject.GetOrAddComponent<DatabaseSyncNormal>();
                        tmpDataBaseId.SetId(brush.id);
                        tempObject.GetComponent<Collider>().enabled = true;


                        foreach (RibbonPointRef rib in brush.ribbons)
                        {

                            if (rib.number == 0)
                            {
                                tempObject.GetComponent<BrushStroke>().myNewAddRibbonPoint(
                                    new Vector3(rib.pos.x, rib.pos.y, rib.pos.z),
                                    new Quaternion(rib.rot.x, rib.rot.y, rib.rot.z, rib.rot.w));
                                Debug.Log("ERSTER RIBBON " + rib.pos.x + "    " + rib.number);
                            }
                            else if (rib.number == brush.ribbons.Count - 1)
                            {
                                tempObject.GetComponent<BrushStroke>().myNewAddRibbonPoint(
                                    new Vector3(rib.pos.x, rib.pos.y, rib.pos.z),
                                    new Quaternion(rib.rot.x, rib.rot.y, rib.rot.z, rib.rot.w));
                                Debug.Log("LETZTER RIBBON " + rib.pos.x + "    " + rib.number);

                            }
                            else
                            {
                                tempObject.GetComponent<BrushStroke>().myNewAddRibbonPoint(
                                    new Vector3(rib.pos.x, rib.pos.y, rib.pos.z),
                                    new Quaternion(rib.rot.x, rib.rot.y, rib.rot.z, rib.rot.w));
                            }

                            Debug.Log("DEBUG RIBBON           " + rib.pos.x + "    " + rib.pos.y +
                                      "    " + rib.pos.z + "    " + rib.number);

                        }

                        foreach (RibbonPointRef rib in brush.ribbons)
                        {

                            //Debug.Log("DEBUG RIBBON           " + rib.pos.x + "    " + rib.pos.y + "    " + rib.pos.z + "    " + rib.number);
                            //tempObject.GetComponent<BrushStrokeMesh>().InsertRibbonPoint(new Vector3(rib.position.x, rib.position.y, rib.position.z), new Quaternion(rib.rotation.x, rib.rotation.y, rib.rotation.z, rib.rotation.w));
                        }

                        tempObject.GetComponent<BrushStroke>().ResetCollidersAndSetFinalized();


                        if (brush.annotation != null)
                        {
                            foreach (Annotation annos in brush.annotation)
                            {
                                GameObject _tmpAnno = Manager.GameManager.AnnotationData;

                                GameObject obj = Realtime.Instantiate(
                                    prefabName: Manager.GameManager.AnnotationData.name,
                                    options);
                                obj.GetComponent<RealtimeView>().RequestOwnership();
                                //obj.transform.SetParent(tempObject.transform);
                                tempObject.GetComponent<DatabaseSyncNormal>().SetMarker(true);

                                obj.GetOrAddComponent<DatabaseSyncAnnotation>().SetAnnotation(annos.id,
                                    annos.content,
                                    annos.upvotes, annos.downvotes, annos.created_at, brush.id);

                            }
                        }





                    }



                }
            }
        }
    }


    public async void addPlacedObjects(GameObject tmpT, int evt, float px, float py, float pz, float rx, float ry, float rz, float sx, float sy, float sz, string prefabName)
    {

        var graphQL = new GraphQLClient(config);
        //TODO: Change to Normcore
        Realtime.InstantiateOptions options = new Realtime.InstantiateOptions();
        options.ownedByClient = true;
        options.preventOwnershipTakeover = false;
        options.useInstance = NetworkManager.NetM._Realtime;
        options.destroyWhenOwnerLeaves = false;
        options.destroyWhenLastClientLeaves = true;
        GameObject obj = Realtime.Instantiate(prefabName: prefabName, new Vector3(px, py, pz), tmpT.transform.rotation, options);
        obj.transform.localScale = new Vector3(sx, sy, sz);

        Debug.Log("ADDED OJBECT - " + obj.GetComponent<RealtimeView>().isSceneView);

        //obj.transform.SetParent(DatabaseObjects.transform);

        tmpT.GetComponent<RealtimeView>().RequestOwnership();
        tmpT.GetComponent<RealtimeTransform>().RequestOwnership();
        Realtime.Destroy(tmpT);
        Debug.LogWarning("OBJECT NAME " + tmpT.name);


#if UNITY_ANDROID

        //Manager.GameManager.ScaleManipulationPanel.SetManipulationPanel(obj.GetComponent<Placeable>());
        //obj.GetComponent<Placeable>().InitPlaceableSequence();

#endif
        // You can search by file name, operation name, or operation type
        // or... mix and match between all three
        Query query = graphQL.FindQuery("addPlacedObject");

        string results = await graphQL.Send(
            query.ToRequest(new Dictionary<string, object>
            {
                { "eventSession", evt },
                { "pX", px },
                { "pY", py },
                { "pZ", pz },
                { "rX", rx },
                { "rY", ry },
                { "rZ", rz },
                { "sX", sx },
                { "sY", sy },
                { "sZ", sz },
                { "prefabName", prefabName },
                { "created_at", ToRfc3339String(DateTime.Now)},


            })
        );

        Debug.Log(results);
        LoadDatabaseSession deserialEventSession = JsonConvert.DeserializeObject<LoadDatabaseSession>(results);
        Debug.Log(deserialEventSession.Data.AddPlacedObject.PlacedObject.First().Id + "    HABE HIER JETZT DIE ID ");


        //TODO: CHANGE TO NORMCORE
        obj.GetOrAddComponent<DatabaseSyncNormal>().SetId(deserialEventSession.Data.AddPlacedObject.PlacedObject.First().Id);
        obj.GetComponent<Collider>().enabled = true;
        check = true;

        //LOGOPERATION 
        addLogOperation(evt, deserialEventSession.Data.AddPlacedObject.PlacedObject.First().Id, px, py, pz, rx, ry, rz, sx, sy, sz, "Added Placed Object " + "Initial Pos / Scale / RX ");


    }

    public async void addAnnotationObject(int evt, float px, float py, float pz, Vector2 inputPos)
    {

        var graphQL = new GraphQLClient(config);
        GameObject _tmpAnno = Manager.GameManager.Annotation;

        Realtime.InstantiateOptions options = new Realtime.InstantiateOptions();
        options.ownedByClient = true;
        options.preventOwnershipTakeover = false;
        options.useInstance = NetworkManager.NetM._Realtime;
        options.destroyWhenOwnerLeaves = false;
        options.destroyWhenLastClientLeaves = true;
        GameObject obj = Realtime.Instantiate(prefabName: Manager.GameManager.Annotation.name, new Vector3(px, py, pz), _tmpAnno.transform.rotation, options);

        Debug.Log("ADDED OJBECT - " + obj.GetComponent<RealtimeView>().isSceneView);

        Query query = graphQL.FindQuery("addPlacedObject");

        string results = await graphQL.Send(
            query.ToRequest(new Dictionary<string, object>
            {
                { "eventSession", evt },
                { "pX", px },
                { "pY", py },
                { "pZ", pz },
                { "rX", _tmpAnno.transform.rotation.eulerAngles.x },
                { "rY", _tmpAnno.transform.rotation.eulerAngles.y },
                { "rZ", _tmpAnno.transform.rotation.eulerAngles.z },
                { "sX", _tmpAnno.transform.localScale.x },
                { "sY", _tmpAnno.transform.localScale.y },
                { "sZ", _tmpAnno.transform.localScale.z },
                { "prefabName", _tmpAnno.name },
                { "created_at", ToRfc3339String(DateTime.Now)},


            })
        );

        Debug.Log(results);
        LoadDatabaseSession deserialEventSession = JsonConvert.DeserializeObject<LoadDatabaseSession>(results);
        Debug.Log(deserialEventSession.Data.AddPlacedObject.PlacedObject.First().Id + "    HABE HIER JETZT DIE ID ");


        //TODO: CHANGE TO NORMCORE
        obj.GetOrAddComponent<DatabaseSyncNormal>().SetId(deserialEventSession.Data.AddPlacedObject.PlacedObject.First().Id);
        obj.GetComponent<Collider>().enabled = true;
        AnnotationManager.AMG.SetAndShowInputModal(inputPos, obj);
        check = true;

        //LOGOPERATION 
        addLogOperation(evt, deserialEventSession.Data.AddPlacedObject.PlacedObject.First().Id, px, py, pz, _tmpAnno.transform.rotation.eulerAngles.x, _tmpAnno.transform.rotation.eulerAngles.y, _tmpAnno.transform.rotation.eulerAngles.x, _tmpAnno.transform.localScale.x, _tmpAnno.transform.localScale.y, _tmpAnno.transform.localScale.z, "Add Annotation Object  " + "Initial Pos / Scale / RX ");

    }

    //ADD ANNOTATION DATA 
    public async void addAnnotationData(GameObject sender, string ObjectId, string content)
    {

        var graphQL = new GraphQLClient(config);
        GameObject _tmpAnno = Manager.GameManager.AnnotationData;

        Realtime.InstantiateOptions options = new Realtime.InstantiateOptions();
        options.ownedByClient = false;
        options.preventOwnershipTakeover = false;
        options.useInstance = NetworkManager.NetM._Realtime;
        options.destroyWhenOwnerLeaves = false;
        options.destroyWhenLastClientLeaves = true;
        GameObject obj = Realtime.Instantiate(prefabName: Manager.GameManager.AnnotationData.name, options);
        //obj.GetComponent<RealtimeView>().RequestOwnership();
        //obj.transform.SetParent(sender.transform);
        Debug.Log("ADDED OJBECT - " + obj.GetComponent<RealtimeView>().isSceneView);

        Query query = graphQL.FindQuery("addAnnotationData");

        string results = await graphQL.Send(
            query.ToRequest(new Dictionary<string, object>
            {
                { "id", ObjectId },
                { "content", content },
                { "voteUp", 0 },
                { "voteDown", 0 },
                { "created_at", ToRfc3339String(DateTime.Now)},

            })
        );

        Debug.Log(results);

        LoadDatabaseSession deserialEventSession = JsonConvert.DeserializeObject<LoadDatabaseSession>(results);
        //Debug.Log(deserialEventSession.Data.Annotations.Length + "    HABE HIER JETZT DIE ID für ANNOTATION DATA " );

        obj.GetOrAddComponent<DatabaseSyncAnnotation>().SetAnnotation(deserialEventSession.Data.updatePlacedObject.placedObject.First().annotation.Last().id, deserialEventSession.Data.updatePlacedObject
            .placedObject.First().annotation.Last().content, deserialEventSession.Data.updatePlacedObject
            .placedObject.First().annotation.Last().upvotes, deserialEventSession.Data.updatePlacedObject
            .placedObject.First().annotation.Last().downvotes, deserialEventSession.Data.updatePlacedObject
            .placedObject.First().annotation.Last().created_at, ObjectId);

        check = true;

    }


    public async void addAnnotationDataBrush(GameObject sender, string ObjectId, string content)
    {

        var graphQL = new GraphQLClient(config);
        GameObject _tmpAnno = Manager.GameManager.AnnotationData;

        Realtime.InstantiateOptions options = new Realtime.InstantiateOptions();
        options.ownedByClient = false;
        options.preventOwnershipTakeover = false;
        options.useInstance = NetworkManager.NetM._Realtime;
        options.destroyWhenOwnerLeaves = false;
        options.destroyWhenLastClientLeaves = true;
        GameObject obj = Realtime.Instantiate(prefabName: Manager.GameManager.AnnotationData.name, options);
        //obj.GetComponent<RealtimeView>().RequestOwnership();
        //obj.transform.SetParent(sender.transform);
        Debug.Log("ADDED OJBECT - " + obj.GetComponent<RealtimeView>().isSceneView);

        Query query = graphQL.FindQuery("addAnnotationDataBrush");

        string results = await graphQL.Send(
            query.ToRequest(new Dictionary<string, object>
            {
                { "id", ObjectId },
                { "content", content },
                { "voteUp", 0 },
                { "voteDown", 0 },
                { "created_at", ToRfc3339String(DateTime.Now)},

            })
        );

        Debug.Log(results);
        LoadDatabaseSession deserialEventSession = JsonConvert.DeserializeObject<LoadDatabaseSession>(results);
        //Debug.Log(deserialEventSession.Data.Annotations.Length + "    HABE HIER JETZT DIE ID für ANNOTATION DATA " );

        obj.GetOrAddComponent<DatabaseSyncAnnotation>().SetAnnotation(deserialEventSession.Data.updateBrushAnnotation.brushAnnotation.First().annotation.Last().id, deserialEventSession.Data.updateBrushAnnotation.brushAnnotation.First().annotation.Last().content, deserialEventSession.Data.updateBrushAnnotation.brushAnnotation.First().annotation.Last().upvotes,
            deserialEventSession.Data.updateBrushAnnotation.brushAnnotation.First().annotation.Last().downvotes, deserialEventSession.Data.updateBrushAnnotation.brushAnnotation.First().annotation.Last().created_at, ObjectId);

        check = true;

    }

    public async void addBrushAnnotation(GameObject obj, int evt, float px, float py, float pz, float rx, float ry, float rz, float sx, float sy, float sz, float r, float g, float b, float a, float brushWidth, string prefabName)
    {

        robs = new List<RibbonPointRef>();
        RealtimeArray<RibbonPointModel> ribbons = obj.GetComponent<BrushStroke>().GetRibbon();
        RibbonPointRef[] array = new RibbonPointRef[ribbons.Count];

        Debug.Log(ribbons.Count);
        Debug.Log(ribbons[0].position.x + "       " + ribbons[0].position.y + "        " + ribbons[0].position.z);
        //Create Ribbons Array
        //RibbonPointRef[] ribs = new RibbonPointRef[ribbons.Count];
        Debug.Log("Robs count " + robs.Count);


        int index = 0;


        for (int i = 0; i < ribbons.Count; i++)
        {
            //Debug.Log(ribbons[i].position.x + "       " + ribbons[i].position.y + "        "+ ribbons[i].position.z );
            array[i] = new RibbonPointRef(i, new PositionFormat(ribbons[i].position.x, ribbons[i].position.y, ribbons[i].position.z), new RotationFormat(ribbons[i].rotation.x, ribbons[i].rotation.y, ribbons[i].rotation.z, ribbons[i].rotation.w));
            robs.Add(new RibbonPointRef(i, new PositionFormat(ribbons[i].position.x, ribbons[i].position.y, ribbons[i].position.z), new RotationFormat(ribbons[i].rotation.x, ribbons[i].rotation.y, ribbons[i].rotation.z, ribbons[i].rotation.w)));
        }



        Debug.Log("ROBS COUNT" + robs.Count);

        Debug.Log("ADDED BRUSH ANNOTATION TO DATABASE?");
        //Debug.Log(formattedForRib);
        var graphQL = new GraphQLClient(config);

        Query query = graphQL.FindQuery("addBrushAnnotation");

        string results = await graphQL.Send(
            query.ToRequest(new Dictionary<string, object>
            {
                { "eventSession", evt },
                { "pX", px },
                { "pY", py },
                { "pZ", pz },
                { "rX", rx },
                { "rY", ry },
                { "rZ", rz },
                { "sX", sx },
                { "sY", sy },
                { "sZ", sz },
                { "r", r },
                { "g", g },
                { "b", b },
                { "a", a },
                { "brushStrokeWidth", brushWidth },
                { "prefabName", prefabName },
                { "created_at", ToRfc3339String(DateTime.Now)},
                { "ribbons", array.ToList()}


            })
        );

        Debug.Log(results);
        LoadDatabaseSession deserialEventSession = JsonConvert.DeserializeObject<LoadDatabaseSession>(results);
        Debug.Log(deserialEventSession.Data.addBrushAnnotation.BrushAnnotation.First().id + "    HABE HIER JETZT DIE ID ");


        obj.GetOrAddComponent<DatabaseSyncNormal>().SetId(deserialEventSession.Data.addBrushAnnotation.BrushAnnotation.First().id);
        //obj.GetComponent<Collider>().enabled = true;

        robs.Clear();

        //LOGOPERATION 
        addLogOperation(evt, deserialEventSession.Data.addBrushAnnotation.BrushAnnotation.First().id, px, py, pz, rx, ry, rz, sx, sy, sz, "Added Brush Annotation to DB " + "Initial Pos / Scale / RX ");

    }

    public async void addInitialObjects(Transform tmpT, int evt, float px, float py, float pz, float rx, float ry, float rz, float sx, float sy, float sz, string prefabName)
    {

        var graphQL = new GraphQLClient(config);
        //GameObject obj = PhotonNetwork.InstantiateRoomObject(prefabName, tmpT.position, Quaternion.identity);

        // You can search by file name, operation name, or operation type
        // or... mix and match between all three
        Query query = graphQL.FindQuery("addPlacedObject");



        string results = await graphQL.Send(
            query.ToRequest(new Dictionary<string, object>
            {
                { "eventSession", evt },
                { "pX", px },
                { "pY", py },
                { "pZ", pz },
                { "rX", rx },
                { "rY", ry },
                { "rZ", rz },
                { "sX", sx },
                { "sY", sy },
                { "sZ", sz },
                { "prefabName", prefabName },


            })
        );

        Debug.Log(results);
        LoadDatabaseSession deserialEventSession = JsonConvert.DeserializeObject<LoadDatabaseSession>(results);
        Debug.Log(deserialEventSession.Data.AddPlacedObject.PlacedObject.First().Id + "    HABE HIER JETZT DIE ID ");
        tmpT.gameObject.GetOrAddComponent<DatabaseID>().id = deserialEventSession.Data.AddPlacedObject.PlacedObject.First().Id;

    }

    public async void updatePlacedObjects(string id, float px, float py, float pz, float rx, float ry, float rz, float sx, float sy, float sz)
    {
        var graphQL = new GraphQLClient(config);

        // You can search by file name, operation name, or operation type
        // or... mix and match between all three
        Query query = graphQL.FindQuery("updatePlacedObject");

        string results = await graphQL.Send(
            query.ToRequest(new Dictionary<string, object>
            {
                { "id", id},
                { "pX", px },
                { "pY", py },
                { "pZ", pz },
                { "rX", rx },
                { "rY", ry },
                { "rZ", rz },
                { "sX", sx },
                { "sY", sy },
                { "sZ", sz },


            })
        );

        Debug.Log(results);

        //LOGOPERATION 
        addLogOperation(activeSessionNumber, id, px, py, pz, rx, ry, rz, sx, sy, sz, "PlacedObject " + "Update Pos / Scale / RX ");

    }
    public async void updateBrushAnnotation(string id, float px, float py, float pz, float rx, float ry, float rz, float sx, float sy, float sz)
    {
        var graphQL = new GraphQLClient(config);

        // You can search by file name, operation name, or operation type
        // or... mix and match between all three
        Query query = graphQL.FindQuery("updateBrushAnnotation");

        string results = await graphQL.Send(
            query.ToRequest(new Dictionary<string, object>
            {
                { "id", id},
                { "pX", px },
                { "pY", py },
                { "pZ", pz },
                { "rX", rx },
                { "rY", ry },
                { "rZ", rz },
                { "sX", sx },
                { "sY", sy },
                { "sZ", sz },


            })
        );

        Debug.Log(results);
        //LOGOPERATION 
        addLogOperation(activeSessionNumber, id, px, py, pz, rx, ry, rz, sx, sy, sz, "BrushStroke" + "UpdateBrushAnnotation Pos / Scale / RX ");
    }


    public async void updateAnnotation(string id, int downVotes, int upVotes)
    {
        var graphQL = new GraphQLClient(config);

        // You can search by file name, operation name, or operation type
        // or... mix and match between all three

        Query query = graphQL.FindQuery("updateAnnotation");

        string results = await graphQL.Send(
            query.ToRequest(new Dictionary<string, object>
            {
                { "id", id},
                { "upvotes", upVotes },
                { "downvotes", downVotes },


            })
        );

        Debug.Log(results);
    }

    public async void Subscribe()
    {
        var graphQL = new GraphQLClient(config);
        Query query = graphQL.FindQuery("subscriptionSession");

        Debug.Log("SESSION NUMBER : " + activeSessionNumber);
        bool success = await graphQL.Subscribe(
            query.ToRequest(new Dictionary<string, float>
            {
                { "number", 46},
            })
        );

        Debug.Log(success ? "Subscribed!" : "Subscribe failed!");
        graphQL.RegisterListener(OnSubscriptionUpdated);
    }

    public async void Unsubscribe()
    {
        var graphQL = new GraphQLClient(config);
        Query query = graphQL.FindQuery("subscriptionSession");

        await graphQL.Unsubscribe(query.ToRequest());
        graphQL.UnregisterListener(OnSubscriptionUpdated);
        Debug.Log("Unsubscribed!");
    }

    public void OnSubscriptionUpdated(string payload)
    {
        Debug.Log("Subscription updated: " + payload);
        //LoadDatabaseSession deserialEventSession = JsonConvert.DeserializeObject<LoadDatabaseSession>(payload);
        //Debug.Log(deserialEventSession.Data.QueryEventSession.First().Objects.First().annotation.First().content);
    }



    public class LoadDatabaseSession
    {
        public Data Data { get; set; }
    }

    public class Data
    {
        public List<QueryEventSession> QueryEventSession { get; set; }
        public List<QueryLogOperation> QueryLogOperation { get; set; }
        public AddPlacedObject AddPlacedObject { get; set; }
        public AddLogOperation AddLogOperation { get; set; }
        public UpdatePlacedObject updatePlacedObject { get; set; }
        public AddBrushAnnotation addBrushAnnotation { get; set; }
        public UpdateBrushAnnotation updateBrushAnnotation { get; set; }
        public AddRibbonPointInput AddRibbonPointInput { get; set; }
    }

    public partial class AddBrushAnnotation
    {
        [JsonProperty("brushAnnotation")] public BrushAnnotation[] BrushAnnotation { get; set; }
    }

    public partial class AddPlacedObject
    {
        [JsonProperty("placedObject")] public PlacedObject[] PlacedObject { get; set; }
    }

    public partial class PlacedObject
    {
        public string Id { get; set; }
        public string prefabName { get; set; }
        public List<Annotation> annotation { get; set; }
    }

    public partial class AddLogOperation
    {
        [JsonProperty("logOperation")] public LogOperation[] LogOperations { get; set; }
    }

    public class LogOperation
    {
        public string id { get; set; }
        public int Number { get; set; }
        public DateTime created_at { get; set; }
        public string content { get; set; }
        public string object_id { get; set; }
        public float PX { get; set; }
        public float PY { get; set; }
        public float PZ { get; set; }
        public float RX { get; set; }
        public float RY { get; set; }
        public float RZ { get; set; }
        public float SX { get; set; }
        public float SY { get; set; }
        public float SZ { get; set; }

    }

    public class QueryLogOperation
    {
        public string id { get; set; }
        public int Number { get; set; }
        public DateTime created_at { get; set; }
        public string content { get; set; }
        public string object_id { get; set; }
        public float PX { get; set; }
        public float PY { get; set; }
        public float PZ { get; set; }
        public float RX { get; set; }
        public float RY { get; set; }
        public float RZ { get; set; }
        public float SX { get; set; }
        public float SY { get; set; }
        public float SZ { get; set; }
    }



    public class QueryEventSession
    {
        public int Number { get; set; }
        public string Name { get; set; }

        public bool deleted { get; set; }
        public bool locked { get; set; }

        public DateTime created_at { get; set; }
        public List<Object> Objects { get; set; }
        public List<BrushAnnotation> brushAnnotation { get; set; }
        public string screenshotPath { get; set; }
    }


    public class Object
    {
        public string Id { get; set; }
        public float PX { get; set; }
        public float PY { get; set; }
        public float PZ { get; set; }
        public float RX { get; set; }
        public float RY { get; set; }
        public float RZ { get; set; }
        public float SX { get; set; }
        public float SY { get; set; }
        public float SZ { get; set; }
        public string PrefabName { get; set; }
        public List<Annotation> annotation { get; set; }
    }

    public class BrushAnnotation
    {
        public string id { get; set; }
        public DateTime created_at { get; set; }
        public BrushColor brushColor { get; set; }
        public float PX { get; set; }
        public float PY { get; set; }
        public float PZ { get; set; }
        public float RX { get; set; }
        public float RY { get; set; }
        public float RZ { get; set; }
        public float SX { get; set; }
        public float SY { get; set; }
        public float SZ { get; set; }
        public float brushStrokeWidth { get; set; }
        public string PrefabName { get; set; }
        public DateTime modified_at { get; set; }
        [JsonProperty("ribbons")] public List<RibbonPointRef> ribbons = new List<RibbonPointRef>();
        public List<Annotation> annotation { get; set; }

    }


    public string ToRfc3339String(DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz", DateTimeFormatInfo.InvariantInfo);
    }


    public class Annotation
    {
        public string content { get; set; }
        public DateTime created_at { get; set; }
        public string id { get; set; }
        public int upvotes { get; set; }
        public int downvotes { get; set; }
    }


    public class RibbonPoint
    {
        public Vector3 position { get; set; }
        public Quaternion rotation { get; set; }
    }

    public class UpdatePlacedObject
    {
        public List<PlacedObject> placedObject { get; set; }
    }

    public class UpdateBrushAnnotation
    {
        public List<BrushAnnotation> brushAnnotation { get; set; }
    }

    public class AddRibbonPointInput
    {

        //[JsonProperty("brushAnnotation")] public BrushAnnotationInput brushAnnotation { get; set; }
        public List<RibbonPointRef> ribbonPoint { get; set; }
    }


    [Serializable]
    public class RibbonPointRef
    {
        public RibbonPointRef(int number, PositionFormat position, RotationFormat rotation)
        {
            this.number = number;
            this.pos = position;
            this.rot = rotation;
        }
        public int number { get; set; }
        public PositionFormat pos { get; set; }
        public RotationFormat rot { get; set; }
    }


    [Serializable]
    public class PositionFormat
    {
        public PositionFormat(float px, float py, float pz)
        {
            this.x = px;
            this.y = py;
            this.z = pz;
        }

        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }

    [Serializable]
    public class RotationFormat
    {
        public RotationFormat(float rx, float ry, float rz, float rw)
        {
            this.x = rx;
            this.y = ry;
            this.z = rz;
            this.w = rw;
        }

        public float w { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }


    public class BrushColor
    {
        public float r { get; set; }
        public float g { get; set; }
        public float b { get; set; }
        public float a { get; set; }
    }

}




