using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class InputTest : MonoBehaviour
{
    
    public InputActionProperty _buttonPressPrimary;
    public InputActionProperty _buttonPressSecondary;

    public InputActionProperty _buttonThumbStick; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_buttonPressPrimary.action.triggered)
        {
            if (gameObject.name.Contains("Left"))
            {
                Debug.Log("Ich habe den Button Primary auf dem "+ gameObject.name +" Gedrückt");
                

            }
            if (gameObject.name.Contains("Right"))
            {
                Debug.Log("Ich habe den Button Primary auf dem "+ gameObject.name +" Gedrückt");
                //Manager.GameManager.ShowVRMenuToggle();

            }
        }   
        if (_buttonPressSecondary.action.triggered)
        {
         
            if (gameObject.name.Contains("Left"))
            {
                Debug.Log("Ich habe den Button Secondary auf dem "+ gameObject.name +" Gedrückt");
              

            }
            
            if (gameObject.name.Contains("Right"))
            {
                Manager.GameManager.DrawingModeToggle();
                Debug.Log("Ich habe den Button Secondary auf dem "+ gameObject.name +" Gedrückt");

            }
            
        }

        if (_buttonThumbStick.action.triggered)
        {
            Debug.Log("Thumbstick Clicked from InputTest");
        }
        
    }
    
  
}
