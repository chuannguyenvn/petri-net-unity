using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Background : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent onDeselectClick; // Fire when the user clicking the background
    
    public bool isPerforming = false;
    private DefaultMenu defaultMenu;

    void Start()
    {
        // Get the only default menu from ProgramManager
        defaultMenu = ProgramManager.Instance.defaultMenu;
    }

    void Update()
    {
        if (defaultMenu.transform.position.magnitude > 5000) isPerforming = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isPerforming) return;
        

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            onDeselectClick.Invoke();
            //defaultMenu.ForceHide();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            isPerforming = true;
            onDeselectClick.Invoke();
            defaultMenu.ForceHide();
            defaultMenu.Show(eventData.position);
        }
    }
}