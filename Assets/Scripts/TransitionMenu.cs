using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionMenu : Menu
{
    [SerializeField] private GameObject remove;
    [SerializeField] private GameObject newArc;

    public Transition currentTransition;

    public void Remove()
    {
        StopAllCoroutines();
        Destroy(currentTransition.gameObject);
        Hide();
        Destroy(gameObject);
    }

    public void NewArc()
    {
        Arc newArc = ProgramManager.Instance.NewArrow();
        newArc.origin = currentTransition.transform;
        newArc.target = ProgramManager.Instance.mouse.transform;
        ProgramManager.Instance.mouse.currentArc = newArc;
        ProgramManager.Instance.mouse.origin = currentTransition;
        Hide();
    }

    public override void Show(Vector2 position)
    {
        transform.SetSiblingIndex(0);
        transform.position = position;
        StartCoroutine(MoveUIObject_CO(remove, Vector2.right * 100, false));
        StartCoroutine(MoveUIObject_CO(newArc, Vector2.left * 100, false));
    }

    public override void Hide()
    {
        transform.SetSiblingIndex(0);

        StartCoroutine(MoveUIObject_CO(remove, Vector2.zero, true));
        StartCoroutine(MoveUIObject_CO(newArc, Vector2.zero, true));
    }

    public override void ForceHide()
    {
        transform.SetSiblingIndex(0);

        remove.transform.position = Vector2.zero;
        newArc.transform.position = Vector2.zero;
    }
}