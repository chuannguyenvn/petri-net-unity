using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Token : MonoBehaviour
{
    public Destination destination;
    private bool isMoving = false;

    void Start()
    {
        transform.localScale = Vector3.one * ProgramManager.Instance.tokenScale;
        transform.SetSiblingIndex(transform.parent.childCount - 1);
    }

    void Update()
    {
        if (destination == null) return;

        float distance = Vector2.Distance(transform.position, destination.transform.position);
       
        if (distance > 5f && transform.parent != destination.transform)
        {
            isMoving = true;
            float speed = 1000f * Screen.width / 1920;
            if (distance < 20f) speed -= distance * 4;
            transform.Translate((destination.transform.position - transform.position).normalized *
                Time.deltaTime * speed);
            transform.localScale = Vector3.one * ProgramManager.Instance.tokenScale;

        }
        else if (isMoving)
        {
            destination.tokens.Add(this);
            isMoving = false;
            transform.SetParent(destination.transform);
        }
    }

    public void MoveTo(Destination targetDestination)
    {
        destination = targetDestination;
    }
}