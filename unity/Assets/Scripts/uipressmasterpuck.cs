using Normal.Realtime;
using TouchScript.Gestures;
using UnityEngine;
using TouchScript.Gestures.TransformGestures;
using TouchScript.Hit;
using TouchScript.Pointers;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class uipressmasterpuck : MonoBehaviour
{
    public PressGesture pressGesture;
    public bool _isDraw;
    public bool _isEraser;
    public bool _isColor;
    public bool _isWidth;
    public int _color = 0;
    public int _width = 0;
    public bool _is12;
    public bool _is28;
    public bool _is112;
    public bool _isZero;

    public GameObject _lastSelected; 




    private void Start()
    {
        pressGesture = gameObject.GetComponent<PressGesture>();
        pressGesture.MinPointers = 1;
        pressGesture.MaxPointers = 1; 
        pressGesture.Pressed += startedTransform;

    }

    private void OnDestroy()
    {
        pressGesture.Pressed -= startedTransform;

    }
    
    
    
   

    private void startedTransform(object sender, System.EventArgs e)
    {

        if (_isColor)
        {
            Brush.BrushManager.SetBrushColor(_color);

        }

        if (_isWidth)
        {
            Brush.BrushManager.SetBrushWitdh(_width);

        }

        if (_isDraw)
        {
           
                Manager.GameManager.DrawingModeToggle();
                _lastSelected = this.gameObject; 
        }
           
            
         
        

        if (_isEraser)
        {
            
                Manager.GameManager.EraserModeToggle();
               

        }
          
          
        

        if (_is28)
        {
           
            Manager.GameManager.Schnack28();
          
        }

        if (_is12)
        {
        
            Manager.GameManager.Schnack12();
       
        }

        if (_is112)
        {
          
            Manager.GameManager.Schnack112();
           
        }

        if (_isZero)
        {
           
            Manager.GameManager.backToZero();
           
        }
         


    }

}
