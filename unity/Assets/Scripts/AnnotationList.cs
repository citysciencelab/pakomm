using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnnotationList : MonoBehaviour
{
    
    public static AnnotationList AL;
    
    public GameObject ScrollPanel;
    public List<GameObject> itemsCreated;
    private bool vrMode;
    public Vector3 moveToPosition;
    private bool moving;
    public Transform PivotPos;
    


    
    private void Awake()
    {
        
        if (AL != null)
        {
            GameObject.Destroy(AL);
        }
        else
        {
            AL = this;
        }

        

    }
    
    public void ConstructAndShowScrollView()
    {
       
        if (itemsCreated.Count > 0)
        {
            foreach (var item in itemsCreated)
            {
                Destroy(item);
            }
            
            itemsCreated.Clear();
        }

      
           var objects = FindObjectsOfType<DatabaseSyncAnnotation>();

            foreach (DatabaseSyncAnnotation data in objects)
            {
                
                var pics = FindObjectsOfType<DatabaseSyncNormal>().Where(obj => obj.id == data.visualParentId);

                if (pics.Count() != 0)
                {
                    string newName = pics.First().gameObject.name.Replace("(Clone)","").Trim();
                    Debug.Log(newName);

                    GameObject Item = Instantiate(Manager.GameManager.AnnotationItem, ScrollPanel.transform);

                    TMP_Text content = Item.transform.Find("Content").GetComponent<TMP_Text>();
                    content.text = data.content;
                    string path = "Thumbnails/" + newName; 
                    Item.transform.Find("Picture").GetComponent<Image>().sprite = Resources.Load<Sprite>(path) as Sprite;
                    Item.transform.Find("Picture").GetComponent<Button>().onClick.AddListener(() =>  lerpToPosition(pics.First().gameObject));
                    Item.transform.Find("UPVOTES").GetComponent<TMP_Text>().text = data.upvotes.ToString(); 
                    Item.transform.Find("DOWNVOTES").GetComponent<TMP_Text>().text = data.downvotes.ToString(); 
                    Item.transform.Find("Button VotesUP").GetComponent<Button>().onClick.AddListener(() => upVote(data, Item));
                    Item.transform.Find("Button VotesDOWN").GetComponent<Button>().onClick.AddListener(() => downVote(data, Item));
                    TMP_Text date = Item.transform.Find("DateText").GetComponent<TMP_Text>();
                    date.text = data.visualCreatedTime.ToString(); 
                    Debug.Log("ICH HAB WAS          "  + data.content);
                    itemsCreated.Add(Item);
                }
          
            }
            
    

    }

    private void upVote(DatabaseSyncAnnotation data, GameObject Item)
    {
        Debug.Log("Button VOTE UP ");
        int value = data.upVote();
        Item.transform.Find("UPVOTES").GetComponent<TMP_Text>().text = value.ToString();
    }

    private void downVote(DatabaseSyncAnnotation data, GameObject Item)
    {
        Debug.Log( "Button VOTE DOWN ");
        int value = data.downVote();
        Item.transform.Find("DOWNVOTES").GetComponent<TMP_Text>().text = value.ToString(); 
    }

    

    public void lerpToPosition(GameObject go)
    {
        
        if (Manager.GameManager.ARModeIOS)
        {
            return; 
        }
        else
        {

            moveToPosition = new Vector3(go.transform.position.x, -166f, go.transform.position.z);
            moving = true;
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        vrMode = Manager.GameManager.VRMode;
        PivotPos = Manager.GameManager.Pivot.transform; 

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (!moving)
        {
            return;
        }
        if (moving)
        {
            Debug.Log("Lerping to Position " + moveToPosition);
            Manager.GameManager.Pivot.transform.position = Vector3.Lerp(Manager.GameManager.Pivot.transform.position,
                moveToPosition, Time.deltaTime * 10f);
            //Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 10f, Time.deltaTime * 10f); 

            if(Vector3.Distance(PivotPos.position , moveToPosition) < 0.1f) {
                Debug.Log("Target Reached!!!");
                moving = false; 
            }
        }
        
       
    }
}
