using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Ticker : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnTick;

    [SerializeField] public float tickPeriod = 1f;
    [SerializeField] private float currentTick = 1f;


    [SerializeField] bool isTicking = false;
    [SerializeField] private Transform pauseBackground;
    [SerializeField] private Transform pauseFace;
    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        currentTick -= Time.deltaTime;
        if (currentTick <= 0f && isTicking)
        {
            OnTick.Invoke();
            currentTick = tickPeriod;
        }

        if (ProgramManager.Instance.isDisplaying) return;
        
        if (isTicking)
        {
            pauseFace.gameObject.SetActive(true);
            pauseBackground.gameObject.SetActive(true);
            image.enabled = false;
            pauseBackground.rotation = Quaternion.Euler(0, 0, Time.time * 300);
        }
        else
        {
            pauseFace.gameObject.SetActive(false);
            pauseBackground.gameObject.SetActive(false);
            image.enabled = true;
            pauseBackground.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void ToggleTicking()
    {
        isTicking = !isTicking;
    }
}