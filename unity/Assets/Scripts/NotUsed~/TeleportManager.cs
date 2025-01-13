using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://www.youtube.com/watch?v=4vrggeTSZL8 
//Teleport for handtracking

public class TeleportManager : MonoBehaviour
{
    public GameObject player;
    public GameObject destination;


    public bool isAiming = false;
    private GameObject currentDestination;



    // Start is called before the first frame update
    void Start()
    {
        currentDestination = Instantiate(destination, transform.position, Quaternion.identity); 
    }

    // Update is called once per frame
    void Update()
    {
        if(isAiming)
        {
            CheckForDestination(); 
        }
    }

    public void SwitchAiming()
    {
        isAiming = !isAiming;
        if (!isAiming)
        {
            currentDestination.SetActive(false);
        }
          
    }

    public void CheckForDestination()
    {
        Ray ray = new Ray(transform.position, transform.rotation * Vector3.up);
        RaycastHit hit;
        bool isHitting = Physics.Raycast(ray, out hit, 10, 1 << 9); //hit nothing but layer 9

        if(isHitting)
        {
            currentDestination.transform.position = hit.point;
            currentDestination.SetActive(true);
            Debug.Log("Checkfordestination - ishitting");
        }

    }

    public void Teleport()
    {
        if (isAiming && currentDestination.activeSelf)
        {
            Debug.Log("Teleport: " + currentDestination.transform.position);
            player.transform.position = currentDestination.transform.position;
            currentDestination.SetActive(false);
        }
    }

}
