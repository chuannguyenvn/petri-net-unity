using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    public Arc currentArc;
    public Destination origin;

    private void Start()
    {
        ProgramManager.Instance.canvas.GetComponent<Background>().onDeselectClick
            .AddListener(Deselect);
    }

    void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void SetTarget(Destination destination)
    {
        currentArc.target = destination.transform;
    }

    private void Deselect()
    {
        if (currentArc != null) Destroy(currentArc.gameObject);
    }
}