using UnityEngine;
using UnityEngine.UI;

// The menu that pops out when the user right-click a transition //
public class TransitionMenu : Menu
{
    [SerializeField] private GameObject remove; // On the right
    [SerializeField] private GameObject newArc; // On the left

    public Transition currentTransition; // The corresponding transition
    
    public override void Start()
    {
        base.Start();

        destination = currentTransition;
    }
    
    // Remove this transition. Several methods will be called in order to handle the removal properly
    public void Remove()
    {
        StopAllCoroutines();
        new RemoveDestinationCommand(currentTransition).Execute();
        ForceHide();
        //Destroy(gameObject);
    }

    // Construct an arc from this transition
    public void NewArc()
    {
        Arc newArc = ProgramManager.Instance.NewArrow();
        newArc.origin = currentTransition.transform;
        newArc.target = ProgramManager.Instance.mouse.transform;
        ProgramManager.Instance.mouse.currentArc = newArc;
        ProgramManager.Instance.mouse.origin = currentTransition;
        Hide();
    }

    // Show the menu, with appropriate selection positions and animations
    public override void Show(Vector2 position)
    {        
        isShowing = true;
        StopAllCoroutines();
        transform.SetSiblingIndex(0);
        
        transform.position = position;
        StartCoroutine(MoveUIObject_CO(remove, Vector2.right * 100, false));
        StartCoroutine(MoveUIObject_CO(newArc, Vector2.left * 100, false));
    }

    // Hide the menu, with animations
    public override void Hide()
    {
        StopAllCoroutines();
        transform.SetSiblingIndex(0);

        StartCoroutine(MoveUIObject_CO(remove, Vector2.zero, true));
        StartCoroutine(MoveUIObject_CO(newArc, Vector2.zero, true));
    }

    // Quickly hide the menu without animations
    public override void ForceHide()
    {
        StopAllCoroutines();
        transform.SetSiblingIndex(0);

        remove.transform.localPosition = Vector2.zero;
        newArc.transform.localPosition = Vector2.zero;
    }
}