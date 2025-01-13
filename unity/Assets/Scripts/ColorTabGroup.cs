using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTabGroup : MonoBehaviour
{
    // Start is called before the first frame update

    public List<ColorTabButton> _tabButtons;
    public Color _tabIdleColor;
    public Color _tabHoverColor;
    public Color _tabActiveColor;
    public ColorTabButton _selectedTab;
  


    public void Subscribe(ColorTabButton button)
    {
        if (_tabButtons == null)
        {
            _tabButtons = new List<ColorTabButton>(); 
        }

        _tabButtons.Add(button);
    }

    public void OnTabEnter(ColorTabButton button)
    {
        ResetTabs();
        if (_selectedTab == null || button != _selectedTab)
        {
            button._background.color = _tabHoverColor;
        }

    }
    
    
    public void OnTabExit(ColorTabButton button)
    {
        ResetTabs();

    }

    public void OnTabSelected(ColorTabButton button)
    {
        _selectedTab = button;
        ResetTabs();
        button._background.color = _tabActiveColor;
        

     

        int index = button.transform.GetSiblingIndex();
        
        

    }



    public void ResetTabs()
    {
        foreach (ColorTabButton button in _tabButtons)
        {
            if (_selectedTab != null && button == _selectedTab)
            {
           
                continue;
            }
            else
            {
                button._background.color = _tabIdleColor;

            }

        }
    }
}
