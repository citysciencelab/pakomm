using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class DisableCreation : MonoBehaviour
{
    public GameObject[] _srollPanels;

    public GameObject _TabButtons; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void disableCreation()
    {
        if (Manager.GameManager.locked)
        {
            _TabButtons.SetActive(false);
            foreach (GameObject go in _srollPanels)
            {
              go.SetActive(false);
            }
        }
        
    }
}
