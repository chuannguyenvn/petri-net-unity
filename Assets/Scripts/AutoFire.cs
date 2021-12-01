using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Handles auto-firing // 
public class AutoFire : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnTick;

    [SerializeField] public float tickPeriod = 1f;
    [SerializeField] private float currentTick = 1f;
    [FormerlySerializedAs("isTicking")] [SerializeField] bool isFiring = false;

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
        if (currentTick <= 0f && isFiring)
        {
            OnTick.Invoke();
            currentTick = tickPeriod;
        }

        if (ProgramManager.Instance.isDisplaying) return;

        if (isFiring)
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
        isFiring = !isFiring;
    }
}