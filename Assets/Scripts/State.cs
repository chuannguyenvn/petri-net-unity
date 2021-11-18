using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class State : Destination
{
    /// The states inside a Petri net ///
    /// 
    //public List<Transition> inTransitions; // Transitions that go to this state
    //public List<Transition> outTransitions; // Transitions that go away from this state

    public Text tokenCount;
    //public Queue<AddTokenCommand> addTokenCommands;

    public override void Start()
    {
        base.Start();
        
        // Getting a menu for the state
        menu = ProgramManager.Instance.NewStateMenu();
        ((StateMenu)menu).currentState = this;
        
       

        //addTokenCommands = new Queue<AddTokenCommand>();
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

    public void ForceAddToken(int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            Token newToken = ProgramManager.Instance.NewToken();
            newToken.transform.position = transform.position;
            newToken.prevPos = newToken.transform.position;
            newToken.destination = this;
            newToken.transform.SetParent(transform);
            tokens.Add(newToken);
        }
    }
    
    public void ForceRemoveToken()
    {
        Token toBeDeleted = tokens[tokens.Count - 1];
        tokens.RemoveAt(tokens.Count - 1);
        toBeDeleted.Destroy();
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
            Transition arcOrigin = mouse.currentArc.origin.GetComponent<Transition>();
            
            // If can't find the transition from the arc or the transition is duplicated
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