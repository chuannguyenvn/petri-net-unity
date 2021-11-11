using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ticker : MonoBehaviour
{
    public static Ticker Instance;

    [HideInInspector] public UnityEvent OnTick;

    [SerializeField] public float tickPeriod = 1f;
    [SerializeField] private float currentTick = 1f;

    public GameObject arrowPrefab;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        OnTick.AddListener(TickLog);
    }

    void Update()
    {
        currentTick -= Time.deltaTime;
        if (currentTick <= 0f)
        {
            OnTick.Invoke();
            currentTick = tickPeriod;
        }
    }

    private void TickLog()
    {
        Debug.Log("Ticked!");
    }
}