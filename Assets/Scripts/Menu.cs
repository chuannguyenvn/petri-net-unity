using System.Collections;
using UnityEngine;

/// Base class for the child menu classes ///
public abstract class Menu : MonoBehaviour
{
    private Background background;
    private float speed = 20;

    void Start()
    {
        // Required to perform correctly at different screen size
        speed *= Screen.width / 1920f; 
        
        // Get the background, which has an event that invoked whenever user click on blank space
        background = ProgramManager.Instance.canvas.GetComponent<Background>();
        // Add the appropriate subscriber to that event
        background.onDeselectClick.AddListener(Hide);
        
        // Initially move the menu to the back of the screen
        transform.SetSiblingIndex(0);
    }

    // Base methods
    public abstract void Show(Vector2 position);
    public abstract void Hide();
    public abstract void ForceHide();

    // Coroutine to move a button at certain speed, to certain location
    protected IEnumerator MoveUIObject_CO(GameObject UIobject, Vector2 targetPos, bool buttonHiding)
    {
        // Based on whether the menu is showing, move them to the top or the bottom of the screen
        if (buttonHiding) transform.SetSiblingIndex(0);
        else transform.SetSiblingIndex(transform.parent.childCount - 1);

        // Move the button while the distance is too large
        while (Vector2.Distance(UIobject.transform.localPosition, targetPos) > 1f)
        {
            // The movement is based on the distance between 2 objects,
            // thus achieving the ramping up and slowing down effect
            Vector2 weightedDirection = targetPos - (Vector2)UIobject.transform.localPosition;
            UIobject.transform.Translate(weightedDirection * Time.deltaTime * speed);
            yield return new WaitForEndOfFrame();
        }

        // If the target position is (0, 0) (relatively to the menu), the button is hiding.
        // Move it to a location far away from the screen to "hide" it
        if (targetPos == Vector2.zero) transform.position = Vector3.up * 10000;
        UIobject.transform.localPosition = targetPos;
        background.isPerforming = false;
    }
}