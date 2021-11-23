using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// The transitions inside a Petri net ///
public class Transition : Destination
{
    private bool isDragging = false;

    public Queue<FireCommand> fireCommands;

    public override void Start()
    {
        base.Start();

        // Getting a menu for the state
        if (!ProgramManager.Instance.isDisplaying)
        {
            menu = ProgramManager.Instance.NewTransitionMenu();
            ((TransitionMenu)menu).currentTransition = this;
        }

        ProgramManager.Instance.ticker.OnTick.AddListener(Fire);

        fireCommands = new Queue<FireCommand>();
    }

    void Update()
    {
        // If firing conditions are not satisfied, return
        if (tokens.Count != inDestinations.Count || tokens.Count == 0) return;
        // Else loop through all outStates and send 1 new token to each state
        foreach (State outState in outDestinations)
        {
            GameObject newToken = Instantiate(ProgramManager.Instance.tokenPrefab,
                ProgramManager.Instance.canvas.transform);
            newToken.transform.position = transform.position;
            newToken.GetComponent<Token>().MoveTo(outState);
            newToken.GetComponent<Token>().firingCommand = fireCommands.Peek();
            fireCommands.Peek().firingTokens.Add(newToken.GetComponent<Token>());
            Debug.Log(fireCommands.Peek().firingTokens.Count);
        }

        fireCommands.Dequeue();

        // And destroy all old tokens
        foreach (Token token in tokens)
        {
            Destroy(token.gameObject);
        }

        tokens.Clear();
    }

    // Function to request a token from each inDestinations to fire
    public void Fire()
    {
        // In case the transition is not fully implemented yet
        if (outDestinations.Count == 0) return;

        // Check if any inState doesn't have at least 1 token
        foreach (State inState in inDestinations)
        {
            if (inState.tokens.Count == 0) return;
        }

        // If all inDestinations have at least 1 token, fire them
        FireCommand fireCommand = new FireCommand(this, inDestinations, outDestinations);
        fireCommands.Enqueue(fireCommand);
        fireCommand.Execute();
    }

    // Manual firing is activated by left-clicking on this transition
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        // Some conditions to prevent unwanted interactions
        if (isDragging || eventData.button == PointerEventData.InputButton.Right ||
            mouse.currentArc != null) return;

        Fire();
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        isDragging = true;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        isDragging = false;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        transform.position += (Vector3)eventData.delta;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        transform.SetSiblingIndex(transform.parent.childCount - 1);

        // If the mouse is holding an arc
        if (mouse.currentArc != null)
        {
            // Try and get the transition that the arc is pointing from
            State arcOrigin = mouse.currentArc.origin.GetComponent<State>();

            // If can't find the state from the arc or the state is duplicated
            if (arcOrigin == null ||
                inDestinations.Find(x => x.identifier == arcOrigin.identifier) ||
                outDestinations.Find(x => x.identifier == arcOrigin.identifier))
            {
                Destroy(mouse.currentArc.gameObject);
                mouse.currentArc = null;
                return;
            }

            // Else configurate the arc
            new NewArcCommand(arcOrigin, this, mouse.currentArc).Execute();
            mouse.currentArc = null;
        }
        // If right-clicked, bring out a menu
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            background.onDeselectClick.Invoke();
            menu.Show(transform.position);
        }
    }
}