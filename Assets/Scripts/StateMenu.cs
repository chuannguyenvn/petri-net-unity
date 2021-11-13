using UnityEngine;

/// The menu that pops out when the user right-click a state ///
public class StateMenu : Menu 
{
    [SerializeField] private GameObject addToken; // On the left
    [SerializeField] private GameObject removeToken; // On the right
    [SerializeField] private GameObject newArc; // On top
    [SerializeField] private GameObject remove; // On bottom

    public State currentState; // The corresponding state

    // Add a new token to currentState
    public void AddToken()
    {
        Token newToken = ProgramManager.Instance.NewToken();
        newToken.transform.position = transform.position + Vector3.up * 100f * Screen.width / 1920f;
        newToken.prevPos = newToken.transform.position;
        newToken.destination = currentState;
    }
    
    // Remove a token to currentState
    public void RemoveToken()
    {
        if (currentState.tokens.Count > 0)
        {
            Token toBeRemoved = currentState.tokens[0];
            currentState.tokens.RemoveAt(0);
            Destroy(toBeRemoved.gameObject);
        }
    }

    // Construct an arc from this state
    public void NewArc()
    {
        Arc newArc = ProgramManager.Instance.NewArrow();
        newArc.origin = currentState.transform;
        newArc.target = ProgramManager.Instance.mouse.transform;
        ProgramManager.Instance.mouse.currentArc = newArc;
        ProgramManager.Instance.mouse.origin = currentState;
        Hide();
    }

    // Remove this state. Several methods will be called in order to handle the removal properly
    public void Remove()
    {
        StopAllCoroutines();
        Destroy(currentState.gameObject);
        Hide();
        Destroy(gameObject);
    }
    
    // Show the menu, with appropriate selection positions and animations
    public override void Show(Vector2 position)
    {
        transform.SetSiblingIndex(0);
        transform.position = position;
        StartCoroutine(MoveUIObject_CO(addToken, Vector2.up * 100,false));
        StartCoroutine(MoveUIObject_CO(removeToken, Vector2.down * 100,false));
        StartCoroutine(MoveUIObject_CO(newArc, Vector2.left * 100,false));
        StartCoroutine(MoveUIObject_CO(remove, Vector2.right * 100,false));
        StartCoroutine(MoveUIObject_CO(currentState.stateName.gameObject, Vector2.one.normalized * -70,false));
        StartCoroutine(MoveUIObject_CO(currentState.tokenCount.gameObject, Vector2.one.normalized *70,false));
    }
    
    // Hide the menu, with animations
    public override void Hide()
    {
        transform.SetSiblingIndex(0);
        StartCoroutine(MoveUIObject_CO(addToken, Vector2.zero, true));
        StartCoroutine(MoveUIObject_CO(removeToken, Vector2.zero, true));
        StartCoroutine(MoveUIObject_CO(newArc, Vector2.zero, true));
        StartCoroutine(MoveUIObject_CO(remove, Vector2.zero, true));
        StartCoroutine(MoveUIObject_CO(currentState.stateName.gameObject, Vector2.down * 70, true));
        StartCoroutine(MoveUIObject_CO(currentState.tokenCount.gameObject, Vector2.up * 70, true));
    }

    // Quickly hide the menu without animations
    public override void ForceHide()
    {
        transform.SetSiblingIndex(0);
        addToken.transform.position = Vector2.zero;
        removeToken.transform.position = Vector2.zero;
        newArc.transform.position = Vector2.zero;
        remove.transform.position = Vector2.zero;
        StartCoroutine(MoveUIObject_CO(currentState.stateName.gameObject, Vector2.down * 70, false));
        StartCoroutine(MoveUIObject_CO(currentState.tokenCount.gameObject, Vector2.up * 70, false));
    }
}