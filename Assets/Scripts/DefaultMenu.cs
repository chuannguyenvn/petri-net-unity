using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// The default menu when right-clicking the background ///
public class DefaultMenu : Menu // Menu provides base methods 
{
    [SerializeField] private GameObject newState; // On the left
    [SerializeField] private GameObject newTransition; // On the right

    // Generate a new state, set its position appropriately, then hide the menu 
    public void NewState()
    {
        ProgramManager.Instance.NewState().transform.position = transform.position;
        Hide();
    }

    // Generate a new transition, set its position appropriately, then hide the menu 
    public void NewTransition()
    {
        ProgramManager.Instance.NewTransition().transform.position = transform.position;
        Hide();
    }

    // Show the menu, with appropriate selection positions and animations
    public override void Show(Vector2 position)
    {
        transform.position = position;
        newState.transform.position = position;
        newTransition.transform.position = position;
        StartCoroutine(MoveUIObject_CO(newState, Vector2.left * 100, false));
        StartCoroutine(MoveUIObject_CO(newTransition, Vector2.right * 100, false));
    }

    // Hide the menu, with animations
    public override void Hide()
    {
        StartCoroutine(MoveUIObject_CO(newState, Vector2.zero, true));
        StartCoroutine(MoveUIObject_CO(newTransition, Vector2.zero, true));
    }

    // Quickly hide the menu without animations
    public override void ForceHide()
    {
        StopAllCoroutines();
        newState.transform.position = Vector2.zero;
        newTransition.transform.position = Vector2.zero;
    }
}