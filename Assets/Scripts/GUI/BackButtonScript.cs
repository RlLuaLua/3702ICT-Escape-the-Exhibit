using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButtonScript : MonoBehaviour
{
    public void ShowMainMenu()
    {
        GameObject.Find("GUIManager").GetComponent<GUIManager>().ShowMainMenu();
    }
}
