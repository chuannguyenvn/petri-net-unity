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
    public List<State> inStates; // States that go to this transition
    public List<State> outStates; // States that go away from this transition


    public InputField transitionName;
    private TransitionMenu transitionMenu;
    private bool isDragging = false;

    public override void Start()
    {
        base.Start();

        // If no name is provided initially, get a default name of T[number]
        if (transitionName.text == string.Empty)
            transitionName.text = "T" + ProgramManager.Instance.TransitionCounter;

        // Generate a unique identifier for the transition
        string randomStr = ProgramManager.Instance.RandomString(transform.position.magnitude);
        identifier = transitionName.text + randomStr;

        // Getting a menu for the transition
        transitionMenu = ProgramManager.Instance.NewTransitionMenu();
        transitionMenu.currentTransition = this;

        // Linking this transition with the outStates by using arcs
        foreach (State state in outStates)
        {
            Arc arc = Instantiate(ProgramManager.Instance.arrowPrefab,
                ProgramManager.Instance.canvas.transform).GetComponent<Arc>();

            arc.origin = transform;
            arc.target = state.transform;
        }

        ProgramManager.Instance.ticker.OnTick.AddListener(Fire);
    }

    void Update()
    {
        // If firing conditions are not satisfied, return
        if (tokens.Count != inStates.Count || tokens.Count == 0) return;

        // Else loop through all outStates and send 1 new token to each state
        foreach (State outState in outStates)
        {
            GameObject newToken = Instantiate(ProgramManager.Instance.tokenPrefab,
                ProgramManager.Instance.canvas.transform);
            newToken.transform.position = transform.position;
            newToken.GetComponent<Token>().MoveTo(outState);
        }

        // And destroy all old tokens
        foreach (Token token in tokens)
        {
            Destroy(token.gameObject);
        }

        ClearTokens();
    }

    // Function to request a token from each inStates to fire
    public void Fire()
    {
        // In case the transition is not fully implemented yet
        if (outStates.Count == 0) return;
        
        // Check if any inState doesn't have at least 1 token
        foreach (State inState in inStates)
        {
            if (inState.tokens.Count == 0) return;
        }

        // If all inStates have at least 1 token, fire them
        foreach (State inState in inStates)
        {
            inState.Send(this);
        }
    }

    // Manual firing is activated by left-clicking on this transition
    public override void OnPointerUp(PointerEventData eventData)
    {
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
        isDragging = false;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        transform.position += (Vector3)eventData.delta;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        // If the mouse is holding an arc
        if (mouse.currentArc != null)
        {
            // Try and get the transition that the arc is pointing from
            State arcOrigin = mouse.currentArc.origin.GetComponent<State>();

            // If can't find the state from the arc or the state is duplicated
            if (arcOrigin == null ||
                inStates.Find(x => x.identifier == arcOrigin.identifier) ||
                outStates.Find(x => x.identifier == arcOrigin.identifier))
            {
                Destroy(mouse.currentArc.gameObject);
                mouse.currentArc = null;
                return;
            }

            // Else configurate the arc
            mouse.currentArc.target = transform;
            inStates.Add(arcOrigin);
            arcOrigin.outTransitions.Add(this);
            mouse.currentArc = null;
        }
        // If right-clicked, bring out a menu
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            background.onDeselectClick.Invoke();
            transitionMenu.currentTransition = this;
            transitionMenu.Show(transform.position);
        }
    }

    // When being destroyed, properly remove all relationships
    private void OnDestroy()
    {
        foreach (State state in inStates)
        {
            state.outTransitions.RemoveAt(
                state.outTransitions.FindIndex(x => x.identifier == identifier));
        }

        foreach (State state in outStates)
        {
            state.inTransitions.RemoveAt(
                state.inTransitions.FindIndex(x => x.identifier == identifier));
        }
    }
}