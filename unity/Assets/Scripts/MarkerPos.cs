using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class MarkerPos : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 offset;
    public Vector3 offsetVR; 
    private Vector3 tempPosVec;
    private Vector3 finalPos;
    private float height; 
    public Canvas CanvasParent; 
    public Button btn;
    public bool vrMode; 
    
    void Start()
    {
        height = transform.parent.GetComponent<Collider>().bounds.size.y;
        Debug.Log(height); 
        vrMode = Manager.GameManager.VRMode; 
        if (vrMode)
        {
            CanvasParent.renderMode = RenderMode.WorldSpace;
            CanvasParent.worldCamera = Camera.main;
            transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        }
        btn.onClick.AddListener ( ()=>  AnnotationManager.AMG.ConstructAndShowScrollView(transform.parent.gameObject));
    }

    // Update is called once per frame
    void Update()
    {
        if (!vrMode)
        {
            tempPosVec = Camera.main.WorldToScreenPoint(transform.parent.position);
            finalPos = tempPosVec + offset;
            transform.position = finalPos;
            btn.transform.position = finalPos;
        }
        else
        {
            tempPosVec = transform.parent.transform.position; 
            finalPos = new Vector3(transform.parent.position.x +offsetVR.x, transform.parent.position.y + height + offsetVR.y, transform.parent.position.z+offsetVR.x);
            btn.transform.position = finalPos;
            btn.transform.LookAt(Camera.main.transform);
        }
       

    }

    private void SendAnnotationsToScroll()
    {
        AnnotationManager.AMG.ConstructAndShowScrollView(transform.parent.gameObject);
    }
}
