using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Represent a single token in a Petri net ///
public class Token : MonoBehaviour
{
    public Destination destination; // The state/transition that this token belongs to
    private bool isMoving = false;
    public Vector2 prevPos;
    public FireCommand firingCommand;
    void Start()
    {
        transform.localScale = Vector3.one * ProgramManager.Instance.tokenScale;
        Debug.Log(prevPos);
        // Tokens should always stay at the top of the screen
        transform.SetSiblingIndex(transform.parent.childCount - 1);
    }

    void Update()
    {
        // In case the destination is null, show error message
        if (destination == null) Debug.LogError("Token missing its destination");

        // The difference between this token's current position and its destination's position
        Vector2 diff = destination.transform.position - transform.position;
        
        // The distance between them is the magnitude of the diff vector
        float distance = diff.magnitude;

        
        if (distance > 5f && transform.parent != destination.transform)
        {
            isMoving = true;
            float speed = 1000f * Screen.width / 1920;
            if (distance < 20f) speed -= distance * 4;
            transform.Translate(diff.normalized * Time.deltaTime * speed);
            transform.localScale = Vector3.one * ProgramManager.Instance.tokenScale;
        }
        else if (isMoving)
        {
            Debug.Log("Done Moving");
            isMoving = false;
            destination.tokens.Add(this);
            transform.SetParent(destination.transform);
            transform.localPosition = Vector3.zero;
            prevPos = transform.position;

            firingCommand?.firingTokens.Remove(this);
        }
    }

    // Method to change this token's destination. Update() will handle all the moving stuff
    public void MoveTo(Destination targetDestination)
    {
        destination = targetDestination;
    }

    public void Destroy()
    {
        destination.tokens.Remove(this);
        Destroy(gameObject);
    }
}