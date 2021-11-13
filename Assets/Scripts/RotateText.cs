using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateText : MonoBehaviour
{
    private void Update()
    {
        transform.rotation  =Quaternion.Euler(0,0, Mathf.Sin(Time.time * 3) * 25);
    }
}
