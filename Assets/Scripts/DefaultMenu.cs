using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultMenu : Menu
{
    [SerializeField] private GameObject newState;
    [SerializeField] private GameObject newTransition;

    private bool isPerforming = false;
    public void NewState()
    {
        ProgramManager.Instance.NewState().transform.position = transform.position;
        Hide();
    }

    public void NewTransition()
    {
        ProgramManager.Instance.NewTransition().transform.position = transform.position;
        Hide();
    }

    public override void Show(Vector2 position)
    {
        transform.position = position;
        newState.transform.position = position;
        newTransition.transform.position = position;
        StartCoroutine(MoveUIObject_CO(newState, Vector2.left * 100, false));
        StartCoroutine(MoveUIObject_CO(newTransition, Vector2.right * 100, false));
    }

    public override void Hide()
    {
        StartCoroutine(MoveUIObject_CO(newState, Vector2.zero, true));
        StartCoroutine(MoveUIObject_CO(newTransition, Vector2.zero, true));
    }

    public override void ForceHide()
    {
        newState.transform.position = Vector2.zero;
        newTransition.transform.position = Vector2.zero;
    }
    
}