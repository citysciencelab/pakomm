using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(Image))]
public class ColorTabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    // Start is called before the first frame update

    public ColorTabGroup _tabGroup;
    
    public Image _background; 
    void Start()
    {
        _background = GetComponent<Image>();
        _tabGroup.Subscribe(this);
    }
    

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("OnPointerEnter", gameObject);
        _tabGroup.OnTabEnter(this);
       
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick" , gameObject);
        _tabGroup.OnTabSelected(this);
   

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("OnPointerExit", gameObject);
        _tabGroup.OnTabExit(this);
    }
}
