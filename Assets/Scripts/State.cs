using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class State : Destination, IPointerClickHandler, IPointerUpHandler, IBeginDragHandler,
    IEndDragHandler,
    IDragHandler
{
    public List<Transition> inTransitions;
    public List<Transition> outTransitions;

    public InputField name;
    public Text tokenCount;
    private StateMenu stateMenu;

    public override void Start()
    {
        base.Start();
        
        if (name.text == string.Empty) name.text = "S" + ProgramManager.Instance.StateCounter;
        identifier = name.text + ProgramManager.Instance.RandomString(transform.position.magnitude);
        stateMenu = ProgramManager.Instance.NewStateMenu();
        stateMenu.currentState = this;

        foreach (Transition transition in outTransitions)
        {
            Arc arc = Instantiate(ProgramManager.Instance.arrowPrefab,
                ProgramManager.Instance.canvas.transform).GetComponent<Arc>();

            arc.origin = transform;
            arc.target = transition.transform;
        }
    }

    void Update()
    {
        tokenCount.text = tokens.Count.ToString();
        if (tokens.Count == 0) return;
        else if (tokens.Count == 1) tokens[0].transform.localPosition = Vector3.zero;
        else
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                
                if (tokens.Count > 4 && tokens.Count < 12)
                {
                    tokens[i].transform.localScale = Vector3.one *
                        ProgramManager.Instance.tokenScale * (20f - tokens.Count) / 20;
                    tokens[i].transform.localPosition =
                        Quaternion.Euler(0, 0, (float)i / tokens.Count * 360 + Time.time * 60) *
                        Vector3.up * 24;
                }
                else if (tokens.Count >= 12)
                {
                    tokens[i].transform.localScale = Vector3.one *
                        ProgramManager.Instance.tokenScale * 0.35f;
                    if (i % 3 == 0)
                    {
                        tokens[i].transform.localPosition =
                            Quaternion.Euler(0, 0, (float) i / tokens.Count * 360 + Time.time * 60) *
                            Vector3.up * 18;
                    }
                    else
                    {
                        tokens[i].transform.localPosition =
                            Quaternion.Euler(0, 0, (i * 2 / 3) / (tokens.Count * 2f /3) * 360 + Time.time * 30) *
                            Vector3.up * 30;
                    }
                }
                else
                {
                    tokens[i].transform.localScale = Vector3.one * ProgramManager.Instance.tokenScale;
                    tokens[i].transform.localPosition =
                        Quaternion.Euler(0, 0, (float)i / tokens.Count * 360 + Time.time * 60) *
                        Vector3.up * 24;
                }
                
                
            }
        }
    }

    public void Fire(Transition nextTransition)
    {
        tokens[tokens.Count - 1].transform.localPosition = Vector3.zero;
        tokens[tokens.Count - 1].MoveTo(nextTransition);
        tokens.RemoveAt(tokens.Count - 1);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position += (Vector3)eventData.delta;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (mouse.currentArc != null)
        {
            Transition arrowTarget = mouse.currentArc.origin.GetComponent<Transition>();
            if (arrowTarget == null || arrowTarget.GetComponent<State>() != null ||
                inTransitions.Find(x => x.identifier == arrowTarget.identifier) ||
                outTransitions.Find(x => x.identifier == arrowTarget.identifier))
            {
                Debug.Log("Wrong");
                Destroy(mouse.currentArc.gameObject);
                mouse.currentArc = null;
                return;
            }

            mouse.currentArc.target = transform;
            inTransitions.Add(arrowTarget);
            arrowTarget.outStates.Add(this);
            mouse.currentArc = null;
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            background.onDeselectClick.Invoke();
            stateMenu.currentState = this;
            stateMenu.Show(transform.position);
        }
    }

    private void OnDestroy()
    {
        foreach (Transition transition in inTransitions)
        {
            transition.outStates.RemoveAt(
                transition.outStates.FindIndex(x => x.identifier == identifier));
        }

        foreach (Transition transition in outTransitions)
        {
            transition.inStates.RemoveAt(
                transition.inStates.FindIndex(x => x.identifier == identifier));
        }
    }
}