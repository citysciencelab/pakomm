using UnityEngine;

public class UIMenuToggle : MonoBehaviour
{
    //[SerializeField]
    //private GameObject buttonLayoutHelpAnnotationObj;

    [SerializeField]
    private GameObject menuRootObj = null;

    [SerializeField]
    private float toogleCooldown = 0.2f;

    private float m_toggleTimeStamp = 0f;

    // Update is called once per frame
    void Update()
    {
        //if (OVRInput.Get(OVRInput.Button.Start))
        {
            if(m_toggleTimeStamp + toogleCooldown <= Time.time)
            {
                ToogleMenuRootObjActive();
                m_toggleTimeStamp = Time.time;
            }
        }
    }

    private void ToogleMenuRootObjActive()
    {
        menuRootObj.SetActive(!menuRootObj.activeSelf);
    }
}
