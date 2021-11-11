using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMenu : Menu
{
    [SerializeField] private GameObject addToken;
    [SerializeField] private GameObject removeToken;
    [SerializeField] private GameObject newArc;
    [SerializeField] private GameObject remove;

    public State currentState;

    public void AddToken()
    {
        Token newToken = ProgramManager.Instance.NewToken();
        newToken.transform.position = transform.position + Vector3.up * 100f * Screen.width/1920f;
        newToken.destination = currentState;
    }

    public void RemoveToken()
    {
        if (currentState.tokens.Count > 0)
        {
            Token toBeRemoved = currentState.tokens[0];
            currentState.tokens.RemoveAt(0);
            Destroy(toBeRemoved.gameObject);
        }
    }

    public void NewArc()
    {
        Arc newArc = ProgramManager.Instance.NewArrow();
        newArc.origin = currentState.transform;
        newArc.target = ProgramManager.Instance.mouse.transform;
        ProgramManager.Instance.mouse.currentArc = newArc;
        ProgramManager.Instance.mouse.origin = currentState;
        Hide();
    }

    public void Remove()
    {
        StopAllCoroutines();
        Destroy(currentState.gameObject);
        Hide();
        Destroy(gameObject);
    }


    public override void Show(Vector2 position)
    {
        transform.SetSiblingIndex(0);
        transform.position = position;
        StartCoroutine(MoveUIObject_CO(addToken, Vector2.up * 100,false));
        StartCoroutine(MoveUIObject_CO(removeToken, Vector2.down * 100,false));
        StartCoroutine(MoveUIObject_CO(newArc, Vector2.left * 100,false));
        StartCoroutine(MoveUIObject_CO(remove, Vector2.right * 100,false));
        StartCoroutine(MoveUIObject_CO(currentState.name.gameObject, Vector2.one.normalized * -70,false));
        StartCoroutine(MoveUIObject_CO(currentState.tokenCount.gameObject, Vector2.one.normalized *70,false));
        isHiding = false;
    }

    public override void Hide()
    {
        transform.SetSiblingIndex(0);
        //StopAllCoroutines();
        StartCoroutine(MoveUIObject_CO(addToken, Vector2.zero, true));
        StartCoroutine(MoveUIObject_CO(removeToken, Vector2.zero, true));
        StartCoroutine(MoveUIObject_CO(newArc, Vector2.zero, true));
        StartCoroutine(MoveUIObject_CO(remove, Vector2.zero, true));
        StartCoroutine(MoveUIObject_CO(currentState.name.gameObject, Vector2.down * 70, false));
        StartCoroutine(MoveUIObject_CO(currentState.tokenCount.gameObject, Vector2.up * 70, false));
        isHiding = true;
    }

    public override void ForceHide()
    {
        transform.SetSiblingIndex(0);
        //StopAllCoroutines();
        addToken.transform.position = Vector2.zero;
        removeToken.transform.position = Vector2.zero;
        newArc.transform.position = Vector2.zero;
        remove.transform.position = Vector2.zero;
        StartCoroutine(MoveUIObject_CO(currentState.name.gameObject, Vector2.down * 70, false));
        StartCoroutine(MoveUIObject_CO(currentState.tokenCount.gameObject, Vector2.up * 70, false));
        isHiding = true;
    }
}