using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ticker : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnTick;

    [SerializeField] public float tickPeriod = 1f;
    [SerializeField] private float currentTick = 1f;

    private bool isTicking = false;

    void Start()
    {
    }

    void Update()
    {
        if (isTicking)
        {
            transform.rotation = Quaternion.Euler(0, 0, Time.time * 100);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        currentTick -= Time.deltaTime;
        if (currentTick <= 0f && isTicking)
        {
            OnTick.Invoke();
            currentTick = tickPeriod;
        }
    }

    public void ToggleTicking()
    {
        isTicking = !isTicking;
    }
}