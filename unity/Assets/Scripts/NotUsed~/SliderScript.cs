using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//@author Anna Wolf, last changed: 27.07.2021

public class SliderScript : MonoBehaviour
{
    public GameObject obj;
    public Slider slider;
    private float previousSliderValue;
    

    public void changeRotation()
    {
        if(obj != null)
        {
            obj.transform.Rotate(0.0f, slider.value, 0.0f);
            previousSliderValue = slider.value;
        }
    }

    public void changeScale()
    {
        if (obj != null)
        {
            Vector3 currentScale = obj.transform.localScale;
            obj.transform.localScale += new Vector3 (slider.value-previousSliderValue, slider.value - previousSliderValue, slider.value - previousSliderValue);
            previousSliderValue = slider.value;
        }
    }

    //left and right - maybe better with raycast
    public void changePositionX()
    {
        if (obj != null)
        {
            obj.transform.position += new Vector3(slider.value - previousSliderValue, 0.0f, 0.0f);
            previousSliderValue = slider.value;
        }
    }

    //Height
    public void changePositionY()
    {
        if (obj != null)
        {
            obj.transform.position += new Vector3(0.0f, slider.value - previousSliderValue, 0.0f);
            previousSliderValue = slider.value;
        }
    }

    //depth, maybe better with raycast
    public void changePositionZ()
    {
        if (obj != null)
        {
            obj.transform.position += new Vector3(0.0f, 0.0f, slider.value - previousSliderValue);
            previousSliderValue = slider.value;
        }
    }
}
