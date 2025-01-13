using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trackCameraPosition : MonoBehaviour
{

    public Vector3 _offsetVector;

    public GameObject _xrOrigin; 
    // Start is called before the first frame update
    void Start()
    {
        if (!Manager.GameManager.VRMode)
        {
            Debug.Log("Disabled CameraTracked and Menu for VR Menu");
            this.gameObject.SetActive(false); 
           
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y,
            Camera.main.transform.position.z);
        transform.position = transform.position + _offsetVector;

        transform.rotation = _xrOrigin.transform.rotation; 


    }
}
