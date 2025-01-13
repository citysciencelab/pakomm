using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 * 
 * @author: Anna Wolf, 26.07.2022
 */


public class SessionLoader : MonoBehaviour
{
   
    public void LoadSessionOnClick()
    {
        int number = GetSessionNumber();
        Manager.GameManager.loadSceneFromDatabase(number, false);
    }

    public int GetSessionNumber()
    {
        int sessionNumber = 1;

        return sessionNumber;
    }
}
