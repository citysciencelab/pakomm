using Normal.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class threedTouchObjectMenu: MonoBehaviour
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
            GameObject _goHolder = new GameObject();
            _goHolder.name = _path;
            _goHolder.transform.parent = this.gameObject.transform;
            _menuObjects.Add(_goHolder);

            _resourcesObjects = Resources.LoadAll(_path, typeof(GameObject))
                .Cast<GameObject>()
                .ToArray();

            for (int i = 0; i < _resourcesObjects.Length; i++)
            {
                float angle = i * Mathf.PI / _resourcesObjects.Length;
                float x = Mathf.Cos(angle);
                float z = Mathf.Sin(angle);
                Vector3 pos = transform.position + new Vector3(x, 0, z);
                float angleDegrees = -angle * Mathf.Rad2Deg;
                Quaternion rot = Quaternion.Euler(0, angleDegrees, 0);
                Debug.Log("Name" + _resourcesObjects[i].name);
                GameObject _tempGO = Instantiate(_resourcesObjects[i], _goHolder.transform);
                _tempGO.transform.SetPositionAndRotation(pos, rot);


                //Geht nur bei Prefabs mit gleichem Material
                //GameObject _aff =  Instantiate(Resources.Load(("Interaction Affordance")) as GameObject, _tempGO.transform); 
                //_tempGO.AddComponent<XRGrabInteractable>();
                var _tempInfinte = _tempGO.AddComponent<XRMenuItem>();
                var _tint = _tempGO.AddComponent<XRTintInteractableVisualPaKOMM>();
                _tint.tintColor = Color.cyan;
                _tempInfinte._prefab = _resourcesObjects[i];
                //_tempGO.GetComponent<Rigidbody>().useGravity = false; 
                // _tempGO.transform.parent = this.gameObject.transform;


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


   

