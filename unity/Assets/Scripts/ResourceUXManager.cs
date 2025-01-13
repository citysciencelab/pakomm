using Normal.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class ResourceUXManager : MonoBehaviour
{

    [SerializeField] 
    private string[] path; 
    [SerializeField]
    private GameObject[] _resourcesObjects;
    [SerializeField]
    private List<GameObject> _menuObjects;
    [SerializeField]
    private float radius = 10f;
    [SerializeField]
    private GameObject _categoryItem;
    [SerializeField]
    private Transform _categoryTransformPosition;

    public Vector3 _goHolderScale; 
    

    public int count = 0; 


// Start is called before the first frame update
    void Awake()
    {
    
        GameObject Category = Instantiate(_categoryItem, _categoryTransformPosition.parent);
        Category.transform.position = _categoryTransformPosition.position;
        XR_Menu_Category _tempCat = Category.AddComponent<XR_Menu_Category>();
        var _tintMenu = Category.AddComponent<XRTintInteractableVisualPaKOMM>();
        _tintMenu.tintColor = Color.yellow;
        Debug.Log("WOOOOOP " + Category.name);

        _tempCat._resourceManager = this;
        
        foreach (string _path in path)
        {
            GameObject _goHolder = new GameObject(); 
            _goHolder.name = _path; 
            _goHolder.transform.parent = this.gameObject.transform;
            _goHolder.transform.localScale = _goHolderScale;
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
                //Debug.Log("Name" + _resourcesObjects[i].name);
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
            
            foreach (GameObject _catGO in _menuObjects)
            {
                _catGO.SetActive(false);
            }


        }
    }


    public void ToggelThrough()
    {
        Debug.Log("TOGGLEEEEEEEEEEEEEEEEE");
       
        if(count == 0)
        {
            _menuObjects[count].SetActive(true);
            count++; 

        }
        else if (count < _menuObjects.Count && count >=1)
        {
            _menuObjects[count-1].SetActive(false);
            _menuObjects[count].SetActive(true); 
            count++;    
        }
        else if (count == _menuObjects.Count)
        {
            _menuObjects[count-1].SetActive(false);
            count = 0; 
        }
        Manager.GameManager.LastSelectedCategory = this.gameObject; 

   }

    public void ResetSubs()
    {
        
        foreach (GameObject go in _menuObjects)
        {
            go.SetActive(false);
            Debug.Log("laufe ich? "); 
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
