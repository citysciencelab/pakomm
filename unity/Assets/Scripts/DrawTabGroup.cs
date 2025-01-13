using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTabGroup : MonoBehaviour
{
    // Start is called before the first frame update

    public List<DrawTabButton> _tabButtons;
    public Color _tabIdleColor;
    public Color _tabHoverColor;
    public Color _tabActiveColor;
    public DrawTabButton _selectedTab;
  


    public void Subscribe(DrawTabButton button)
    {
        if (_tabButtons == null)
        {
            _tabButtons = new List<DrawTabButton>(); 
        }

        _tabButtons.Add(button);
    }

    public void OnTabEnter(DrawTabButton button)
    {
        ResetTabs();
        if (_selectedTab == null || button != _selectedTab)
        {
            button._background.color = _tabHoverColor;
        }

    }
    
    
    public void OnTabExit(DrawTabButton button)
    {
        ResetTabs();

    }

    public void OnTabSelected(DrawTabButton button)
    {

            if (_selectedTab != null && button == _selectedTab)
            {
                button._background.color = _tabIdleColor;
                _selectedTab = null;
                
            }
            
            else if( _selectedTab == null || button != _selectedTab)
            {
                button._background.color = _tabActiveColor;
                _selectedTab = button;
                ResetTabs();
            }
        
        
       

     

        

    }



    public void ResetTabs()
    {
        foreach (DrawTabButton button in _tabButtons)
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
