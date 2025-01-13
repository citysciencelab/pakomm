using Normal.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

public class TouchResourceUX: MonoBehaviour
{

    [SerializeField] 
    private string[] path; 
    [SerializeField]
    private GameObject[] _resourcesObjects;
    [SerializeField]
    private List<GameObject> _menuObjects;
    [SerializeField]
    private GameObject Button;  
    [SerializeField]
    private GameObject RadialGroupHolder;
    [SerializeField]
    private Transform _categoryTransformPosition;
    

    public int count = 0; 


// Start is called before the first frame update
    void Awake()
    {
    
        //GameObject Category = Instantiate(_categoryItem, _categoryTransformPosition.parent);
        //Category.transform.position = _categoryTransformPosition.position; 
        //XR_Menu_Category _tempCat = Category.AddComponent<XR_Menu_Category>();
        //var _tintMenu = Category.AddComponent<XRTintInteractableVisual>();
        //_tintMenu.tintColor = Color.yellow;
        //Debug.Log("WOOOOOP " + Category.name);
        
        
        foreach (string _path in path)
        {
            _resourcesObjects = Resources.LoadAll(_path, typeof(GameObject))
                .Cast<GameObject>()
                .ToArray();

            for (int i = 0; i < _resourcesObjects.Length; i++)
            {
               
                Debug.Log("Name" + _resourcesObjects[i].name);
                GameObject _tempGO = Instantiate(Button, RadialGroupHolder.transform);
                IconForButton _icon = _resourcesObjects[i].GetComponent<IconForButton>();
                Image _image = _tempGO.GetComponentInChildren<Image>();
                _image.sprite = _icon.icon;
                //NameforButton _name = _resourcesObjects[i].GetComponent<NameforButton>();
                //Debug.Log(_name);
                //TextMeshProUGUI _textMesh = _tempGO.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                //Debug.Log(_textMesh);
                //_textMesh.text = _name.NameForButton;
                //Debug.Log(_textMesh.text);
                var screenTransGest = _tempGO.AddComponent<ScreenTransformGesture>();
                var uiPressed = _tempGO.AddComponent<uiPressed>();
                uiPressed.pressGesture = screenTransGest; 
                uiPressed.myPrefabToSpawn = _resourcesObjects[i];
                GameObject _visGo = Instantiate(_resourcesObjects[i], _tempGO.transform);
                count++;

            }
            


        }
    }

    void Start()
    {
        
        foreach (string _path in path)
        {
            for (int i = 0; i < _resourcesObjects.Length; i++)
            {
                Debug.Log("NEW STUFF" + _resourcesObjects[i].name);
            }
            


        }
    }
}


   

