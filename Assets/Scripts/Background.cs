using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Background : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent onDeselectClick;
    
    public bool isPerforming = false;
    public bool isShowing = false;
    private DefaultMenu defaultMenu;

    void Start()
    {
        defaultMenu = ProgramManager.Instance.defaultMenu;
    }

    void Update()
    {
        if (defaultMenu.transform.position.magnitude > 5000) isPerforming = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isPerforming) return;

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (isShowing) defaultMenu.ForceHide();
            
            isPerforming = true;
            isShowing = true;
            defaultMenu.Show(eventData.position);
        }
        else
        {
            onDeselectClick.Invoke();
            isPerforming = true;
            isShowing = false;
        }
    }
    
    
}