using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class State : Destination
{
    /// The states inside a Petri net ///
    /// 
    public List<Transition> inTransitions; // Transitions that go to this state
    public List<Transition> outTransitions; // Transitions that go away from this state

    public InputField stateName;
    public Text tokenCount;
    private StateMenu stateMenu;

    public override void Start()
    {
        base.Start();

        // If no name is provided initially, get a default name of S[number]
        if (stateName.text == string.Empty) stateName.text = "S" + ProgramManager.Instance.StateCounter;

        // Generate a unique identifier for the state
        string randomStr = ProgramManager.Instance.RandomString(transform.position.magnitude);
        identifier = stateName.text + randomStr;

        // Getting a menu for the state
        stateMenu = ProgramManager.Instance.NewStateMenu();
        stateMenu.currentState = this;

        // Linking this state with the outTransitions by using arcs
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
        // Update token counter text
        tokenCount.text = tokens.Count.ToString();

        // All the code below control the visual representation of the tokens
        if (tokens.Count == 0) return;
        if (tokens.Count == 1)
        {
            tokens[0].transform.localPosition = Vector3.zero;
            return;
        }

        // If there are more than 1 token, spin them
        float tokenScale = ProgramManager.Instance.tokenScale;
        for (int i = 0; i < tokens.Count; i++)
        {
            if (tokens.Count > 4 && tokens.Count < 12)
            {
                tokens[i].transform.localScale = Vector3.one * tokenScale * (20f - tokens.Count) / 20;
                tokens[i].transform.localPosition =
                    Quaternion.Euler(0, 0, (float)i / tokens.Count * 360 + Time.time * 60) *
                    Vector3.up * 24;
            }
            else if (tokens.Count >= 12)
            {
                tokens[i].transform.localScale = Vector3.one * tokenScale * 0.35f;
                if (i % 3 == 0)
                {
                    tokens[i].transform.localPosition =
                        Quaternion.Euler(0, 0, (float)i / tokens.Count * 360 + Time.time * 60) *
                        Vector3.up * 18;
                }
                else
                {
                    tokens[i].transform.localPosition =
                        Quaternion.Euler(0, 0,
                            (i * 2 / 3) / (tokens.Count * 2f / 3) * 360 + Time.time * 30) *
                        Vector3.up * 30;
                }
            }
            else
            {
                tokens[i].transform.localScale = Vector3.one * tokenScale;
                tokens[i].transform.localPosition =
                    Quaternion.Euler(0, 0, (float)i / tokens.Count * 360 + Time.time * 60) *
                    Vector3.up * 24;
            }
        }
    }

    // Method to move a token from this state to the requesting transition
    public void Send(Transition nextTransition)
    {
        tokens[tokens.Count - 1].transform.localPosition = Vector3.zero;
        tokens[tokens.Count - 1].MoveTo(nextTransition);
        tokens.RemoveAt(tokens.Count - 1);
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
            Transition arcOrigin = mouse.currentArc.origin.GetComponent<Transition>();
            
            // If can't find the transition from the arc or the transition is duplicated
            if (arcOrigin == null || 
                inTransitions.Find(x => x.identifier == arcOrigin.identifier) ||
                outTransitions.Find(x => x.identifier == arcOrigin.identifier))
            {
                Destroy(mouse.currentArc.gameObject);
                mouse.currentArc = null;
                return;
            }

            // Else configurate the arc
            mouse.currentArc.target = transform;
            inTransitions.Add(arcOrigin);
            arcOrigin.outStates.Add(this);
            mouse.currentArc = null;
        }
        // If right-clicked, bring out a menu
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            background.onDeselectClick.Invoke();
            stateMenu.currentState = this;
            stateMenu.Show(transform.position);
        }
    }

    // When being destroyed, properly remove all relationships
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