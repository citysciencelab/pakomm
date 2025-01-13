using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Screen = UnityEngine.Screen;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class AnnotationManager : MonoBehaviour
{

    public static AnnotationManager AMG;

    public Canvas Canvas; 
    public GameObject InputModal;
    public GameObject ScrollAnnotationModal;
    public GameObject Scroll;
    public Vector3 offsetVR;
    public Vector3 worldUp; 
    private Vector3 pos, tempPosVec, finalPos;
    private float pivotX, pivotY; 
    public List<GameObject> itemsCreated; 

    private RectTransform _rect;
    private Transform _inputModalButton;
    private Transform _inputModalInput;
    private Transform _abort;
    private Transform _abortScroll; 
    private Button _but;
    private Button _abortButton; 
    private Button _abortButtonScroll; 
    private TMP_InputField _inputField;
    private GameObject _refGO;
    private bool vrMode;
    
    

    // Start is called before the first frame update
    private void Awake()
    {
        
        if (AMG != null)
        {
            GameObject.Destroy(AMG);
        }
        else
        {
            AMG = this;
        }

        
        _inputModalButton = InputModal.transform.Find("Button");
        _inputModalInput = InputModal.transform.Find("InputField (TMP)");
        _abort = InputModal.transform.Find("Abort");

        _rect = InputModal.GetComponent<RectTransform>();
        _but = _inputModalButton.GetComponent<Button>();
        _inputField = _inputModalInput.GetComponent<TMP_InputField>();
        _abortButton = _abort.GetComponent<Button>();

        _abortScroll = ScrollAnnotationModal.transform.Find("Abort");
        _abortButtonScroll = _abortScroll.GetComponent<Button>();
        
        _but.onClick.AddListener ( () => _commitInputModal());
        _abortButtonScroll.onClick.AddListener ( () => _abortRead());

    }
    
    void Start()
    {
        InputModal.SetActive(false);
        ScrollAnnotationModal.SetActive(false);
        
        vrMode = Manager.GameManager.VRMode; 
        if (vrMode)
        {
            Canvas.renderMode = RenderMode.WorldSpace;
            Canvas.worldCamera = Camera.main;
            Canvas.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        }
    }

    // Update is called once per frame
    void Update()
    {
       if (vrMode)
       {
           
               transform.forward = Camera.main.transform.forward;
          
           if (_refGO)
           {    
               tempPosVec = _refGO.transform.position; 
               finalPos = new Vector3(tempPosVec.x + offsetVR.x, tempPosVec.y + offsetVR.y, tempPosVec.z +offsetVR.z);
               ScrollAnnotationModal.transform.position = finalPos;
               //ScrollAnnotationModal.transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
               //ScrollAnnotationModal.transform.LookAt(Camera.main.transform, Vector3.up);
               ScrollAnnotationModal.transform.LookAt(ScrollAnnotationModal.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
           }
       }
    }

    public void SetAndShowInputModal(Vector2 pos, GameObject sender)
    {

        _refGO = sender; 
        InputModal.SetActive(true);
        Debug.Log("Sender" + sender.name);

        if (!vrMode)
        {
            float pivotX = pos.x / Screen.width; 
            float pivotY = pos.y / Screen.height;
            _rect.pivot = new Vector2(pivotX, pivotY);
            InputModal.transform.position = pos;
        }

        string _tmpName = Manager.GameManager.Annotation.name + "(Clone)"; 
        if (_tmpName.Equals(sender.name) && sender.tag == "AnnotationSphere") 
        {
            Debug.Log("World Annotation detected");
            _abortButton.onClick.AddListener ( () => _abortModalWithDelete());
        }
        else
        {
            _abortButton.onClick.AddListener ( () => _abortModal());
        }
    }

    public void ConstructAndShowScrollView(GameObject sender)
    {
       
        if (itemsCreated.Count > 0)
        {
            foreach (var item in itemsCreated)
            {
                Destroy(item);
            }
            
            itemsCreated.Clear();
        }

      
            Debug.Log("CLICKED Annotation Marker on GO " + sender.name);
            _refGO = sender;

            //DatabaseSyncAnnotation[] myResults = sender.GetComponentsInChildren<DatabaseSyncAnnotation>();
            
            //DatabaseSyncAnnotation[] myResults = sender.GetComponentsInChildren<DatabaseSyncAnnotation>();
            
            var objects = FindObjectsOfType<DatabaseSyncAnnotation>().Where(obj => obj.visualParentId == _refGO.GetComponent<DatabaseSyncNormal>().id);

            foreach (DatabaseSyncAnnotation data in objects)
            {
                
            GameObject Item = Instantiate(Manager.GameManager.AnnotationItem, Scroll.transform);
            
            
                if (vrMode)
                {
                    //Item.transform.position = new Vector3(0f, 0f, 0f);
                    //Item.transform.localScale = new Vector3(1f, 1f, 1f); 
                }
            
           
            
            TMP_Text content = Item.transform.Find("Content").GetComponent<TMP_Text>();
            content.text = data.content; 
            TMP_Text date = Item.transform.Find("DateText").GetComponent<TMP_Text>();
            date.text = data.visualCreatedTime.ToString(); 
            Debug.Log("ICH HAB WAS          "  + data.content);
            string newName = sender.name.Replace("(Clone)","").Trim();
            Debug.Log(newName);
            string path = "Thumbnails/" + newName; 
            Item.transform.Find("Picture").GetComponent<Image>().sprite = Resources.Load<Sprite>(path) as Sprite;
            Item.transform.Find("Button VotesUP").GetComponent<Button>().onClick.AddListener(() => upVote(data, Item));
            Item.transform.Find("Button VotesDOWN").GetComponent<Button>().onClick.AddListener(() => downVote(data, Item));
            Item.transform.Find("UPVOTES").GetComponent<TMP_Text>().text = data.upvotes.ToString(); 
            Item.transform.Find("DOWNVOTES").GetComponent<TMP_Text>().text = data.downvotes.ToString(); 
            itemsCreated.Add(Item);
          
            }
            
            if (!vrMode)
            {
            
                Vector3 screen = Camera.main.WorldToScreenPoint(_refGO.transform.position);
                float pivotX = screen.x / Screen.width;
                float pivotY = screen.y / Screen.height;
                ScrollAnnotationModal.GetComponent<RectTransform>().pivot = new Vector2(pivotX, pivotY);
                ScrollAnnotationModal.transform.position = screen;
            }
            else
            {
                if (_refGO)
                {    
                    tempPosVec = _refGO.transform.position; 
                    finalPos = new Vector3(tempPosVec.x + offsetVR.x, tempPosVec.y + offsetVR.y, tempPosVec.z +offsetVR.z);
                    ScrollAnnotationModal.transform.position = finalPos;
                    ScrollAnnotationModal.transform.LookAt(ScrollAnnotationModal.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
                }
            }
            
            ScrollAnnotationModal.SetActive(true);

    }

    
    private void upVote(DatabaseSyncAnnotation data, GameObject Item)
    {
        Debug.Log("Button VOTE UP ");
        int value = data.upVote();
        Item.transform.Find("UPVOTES").GetComponent<TMP_Text>().text = value.ToString();
        if (!Manager.GameManager.VRMode)
        {
            if (AnnotationList.AL != null)
            {
                Debug.Log("Refresh AL Reached");
                AnnotationList.AL.ConstructAndShowScrollView();
            }
            
        }
    }

    private void downVote(DatabaseSyncAnnotation data, GameObject Item)
    {
        Debug.Log( "Button VOTE DOWN ");
        int value = data.downVote();
        Item.transform.Find("DOWNVOTES").GetComponent<TMP_Text>().text = value.ToString();
        
        if (!Manager.GameManager.VRMode)
        {
            if (AnnotationList.AL != null)
            {
                Debug.Log("Refresh AL Reached");
                AnnotationList.AL.ConstructAndShowScrollView();
            }
            
        }

    }
    private void _commitInputModal()
    {
        Debug.Log("Commited");
        InputModal.SetActive(false);
        if (_refGO.tag == "Annotation")
        {
            DgraphQuery.DQ.addAnnotationDataBrush(_refGO, _refGO.GetOrAddComponent<DatabaseSyncNormal>().id, _inputField.text);
        }
        else
        {
            DgraphQuery.DQ.addAnnotationData(_refGO, _refGO.GetComponent<DatabaseSyncNormal>().id, _inputField.text);
        }
        _refGO.GetComponent<DatabaseSyncNormal>().SetMarker(true);
        
        //GameObject Marker = Instantiate(Manager.GameManager.AnnotationMarker, _refGO.transform.position, Quaternion.identity, _refGO.transform);
        _inputField.text = "";
        
      
    }
    
    private void _abortRead()
    {

        ScrollAnnotationModal.SetActive(false);
    }
    
    private void _abortModalWithDelete()
    {
        Debug.Log("Aborted With Delete");
        _refGO.GetComponent<DatabaseSyncNormal>().DeleteObject();
        InputModal.SetActive(false);
        _inputField.text = "";
    }
    
    private void _abortModal()
    {
        Debug.Log("Aborted");
        InputModal.SetActive(false);
        _inputField.text = "";
    }
}
