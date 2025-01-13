using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    // Start is called before the first frame update

    public List<TabButton> _tabButtons;
    public Color _tabIdleColor;
    public Color _tabHoverColor;
    public Color _tabActiveColor;
    public TabButton _selectedTab;
    public List<GameObject> _scrollListSwap; 


    public void Subscribe(TabButton button)
    {
        if (_tabButtons == null)
        {
            _tabButtons = new List<TabButton>(); 
        }

        _tabButtons.Add(button);
    }

    public void OnTabEnter(TabButton button)
    {
        ResetTabs();
        if (_selectedTab == null || button != _selectedTab)
        {
            button._background.color = _tabHoverColor;
        }

    }
    
    
    public void OnTabExit(TabButton button)
    {
        ResetTabs();

    }

    public void OnTabSelected(TabButton button)
    {
        _selectedTab = button;
        ResetTabs();
        button._background.color = _tabActiveColor;

        if (!Manager.GameManager.locked)
        {
            int index = button.transform.GetSiblingIndex();
            for (int i = 0; i < _scrollListSwap.Count; i++)
            {

                if (i == index)
                {
                    _scrollListSwap[i].SetActive(true);
                }
                else
                {
                    _scrollListSwap[i].SetActive(false);

                }
            }
        }
     

       
        

    }



    public void ResetTabs()
    {
        foreach (TabButton button in _tabButtons)
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
