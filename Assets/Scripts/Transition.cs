using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Transition : Destination, IPointerClickHandler ,IPointerUpHandler, IBeginDragHandler, IEndDragHandler,
    IDragHandler
{
    public List<State> inStates;
    public List<State> outStates;

    private bool isDragging = false;

    [SerializeField] private InputField name;
    private TransitionMenu transitionMenu;

    public override void  Start()
    {
        base.Start();
        if (name.text == string.Empty) name.text = "T" + ProgramManager.Instance.TransitionCounter;
        identifier = name.text + ProgramManager.Instance.RandomString(transform.position.magnitude);
        transitionMenu = ProgramManager.Instance.NewTransitionMenu();
        transitionMenu.currentTransition = this;

        foreach (State state in outStates)
        {
            Arc arc = Instantiate(ProgramManager.Instance.arrowPrefab,
                ProgramManager.Instance.canvas.transform).GetComponent<Arc>();

            arc.origin = transform;
            arc.target = state.transform;
        }
    }

    void Update()
    {
        if (tokens.Count == inStates.Count && tokens.Count != 0)
        {
            foreach (State outState in outStates)
            {
                GameObject newToken = Instantiate(ProgramManager.Instance.tokenPrefab,
                    ProgramManager.Instance.canvas.transform);
                newToken.transform.position = transform.position;
                newToken.GetComponent<Token>().MoveTo(outState);
            }

            foreach (Token token in tokens)
            {
                Destroy(token.gameObject);
            }
            
            ClearTokens();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging || eventData.button == PointerEventData.InputButton.Right || mouse.currentArc != null || outStates.Count == 0) return;
        foreach (State inState in inStates)
        {
            if (inState.tokens.Count == 0) return;
        }
        
        foreach (State inState in inStates)
        {
            inState.Fire(this);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position += (Vector3)eventData.delta;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (mouse.currentArc != null)
        {
            State arrowTarget = mouse.currentArc.origin.GetComponent<State>();
            if (arrowTarget == null || arrowTarget.GetComponent<Transition>() != null ||
                inStates.Find(x => x.identifier == arrowTarget.identifier) || 
                outStates.Find(x => x.identifier == arrowTarget.identifier))
            {
                Debug.Log("Wrong");
                Destroy(mouse.currentArc.gameObject);
                mouse.currentArc = null;
                return;
            }
            mouse.currentArc.target = transform;
            inStates.Add(arrowTarget);
            arrowTarget.outTransitions.Add(this);
            mouse.currentArc = null;
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            background.onDeselectClick.Invoke();
            transitionMenu.currentTransition = this;
            transitionMenu.Show(transform.position);
        }
    }
    
    private void OnDestroy()
    {
        foreach (State state in inStates)
        {
            state.outTransitions.RemoveAt(state.outTransitions.FindIndex(x => x.identifier == identifier));
        }
        
        foreach (State state in outStates)
        {
            state.inTransitions.RemoveAt(state.inTransitions.FindIndex(x => x.identifier == identifier));
        }
    }
}