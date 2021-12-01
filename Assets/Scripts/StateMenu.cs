using UnityEngine;

// The menu that pops out when the user right-click a state //
public class StateMenu : Menu
{
    [SerializeField] private GameObject addToken; // On the left
    [SerializeField] private GameObject removeToken; // On the right
    [SerializeField] private GameObject newArc; // On top
    [SerializeField] private GameObject remove; // On bottom

    public State currentState; // The corresponding state

    public override void Start()
    {
        base.Start();

        destination = currentState;
    }

    // Add a new token to currentState
    public void AddToken()
    {
        new AddTokenCommand(currentState).Execute();
    }

    // Remove a token to currentState
    public void RemoveToken()
    {
        if (currentState.tokens.Count > 0)
            new RemoveTokenCommand(currentState).Execute();
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
        new RemoveDestinationCommand(currentState).Execute();
        ForceHide();
    }

    // Show the menu, with appropriate selection positions and animations
    public override void Show(Vector2 position)
    {        
        isShowing = true;
        StopAllCoroutines();
        transform.SetSiblingIndex(0);

        transform.position = position;
        StartCoroutine(MoveUIObject_CO(addToken, Vector2.up * 100, false));
        StartCoroutine(MoveUIObject_CO(removeToken, Vector2.down * 100, false));
        StartCoroutine(MoveUIObject_CO(newArc, Vector2.left * 100, false));
        StartCoroutine(MoveUIObject_CO(remove, Vector2.right * 100, false));
        StartCoroutine(MoveUIObject_CO(currentState.inputField.gameObject,
            Vector2.one.normalized * -100 + Vector2.up * 10, false));
        StartCoroutine(MoveUIObject_CO(currentState.tokenCount.gameObject,
            Vector2.one.normalized * 70, false));
    }

    // Hide the menu, with animations
    public override void Hide()
    {
        StopAllCoroutines();
        transform.SetSiblingIndex(0);

        StartCoroutine(MoveUIObject_CO(addToken, Vector2.zero, true));
        StartCoroutine(MoveUIObject_CO(removeToken, Vector2.zero, true));
        StartCoroutine(MoveUIObject_CO(newArc, Vector2.zero, true));
        StartCoroutine(MoveUIObject_CO(remove, Vector2.zero, true));
        StartCoroutine(MoveUIObject_CO(currentState.inputField.gameObject, Vector2.down * 70, true));
        StartCoroutine(MoveUIObject_CO(currentState.tokenCount.gameObject, Vector2.up * 70, true));
    }

    // Quickly hide the menu without animations
    public override void ForceHide()
    {
        StopAllCoroutines();
        transform.SetSiblingIndex(0);
        
        addToken.transform.localPosition = Vector2.zero;
        removeToken.transform.localPosition = Vector2.zero;
        newArc.transform.localPosition = Vector2.zero;
        remove.transform.localPosition = Vector2.zero;
        StartCoroutine(MoveUIObject_CO(currentState.inputField.gameObject, Vector2.down * 70, false));
        StartCoroutine(MoveUIObject_CO(currentState.tokenCount.gameObject, Vector2.up * 70, false));
    }
}