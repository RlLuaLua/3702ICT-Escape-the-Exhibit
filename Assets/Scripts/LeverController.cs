using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverController : Interactable
{
    public GameObject handle;
    public bool isOn;

    void Start()
    {
        isOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        handle.transform.localRotation = Quaternion.Euler(0, 0, isOn ? -45 : 45);
    }

    public override void SpinInteract()
    {
        isOn = !isOn;
    }
}
