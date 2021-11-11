using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Menu : MonoBehaviour
{
    protected Background background;
    protected float speed = 20;
    protected bool isHiding = true;
    
    void Start()
    {
        speed *= Screen.width / 1920f;
        background = ProgramManager.Instance.canvas.GetComponent<Background>();
        background.onDeselectClick.AddListener(Hide);
        transform.SetSiblingIndex(0);
    }

    public abstract void Show(Vector2 position);
    public abstract void Hide();
    public abstract void ForceHide();

    protected IEnumerator MoveUIObject_CO(GameObject UIobject, Vector2 targetPos, bool buttonHiding)
    {
        if (buttonHiding) transform.SetSiblingIndex(0);
        else
        {
            Debug.Log("Wrong");
            transform.SetSiblingIndex(transform.parent.childCount - 1);
        }
        
        while (Vector2.Distance(UIobject.transform.localPosition, targetPos) > 1f)
        {
            UIobject.transform.Translate((targetPos - (Vector2)UIobject.transform.localPosition) *
                                       Time.deltaTime * speed);
            yield return new WaitForEndOfFrame();
        }

        if (targetPos == Vector2.zero) transform.position = Vector3.up * 10000;
        UIobject.transform.localPosition = targetPos;
        background.isPerforming = false;
    }
}