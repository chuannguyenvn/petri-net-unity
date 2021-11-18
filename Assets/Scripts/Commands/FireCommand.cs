using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FireCommand : Command
{
    public Transition firingTransition;
    public List<Destination> inStates;
    public List<Destination> outStates;
    public List<Token> firingTokens;
    public bool done = false;
    
    public FireCommand(Transition firingTransition, List<Destination> inStates, List<Destination> outStates)
    {
        this.firingTransition = firingTransition;
        this.inStates = inStates;
        this.outStates = outStates;
        firingTokens = new List<Token>();
    }
    
    public override void Execute()
    {
        foreach (State inState in inStates)
        {
            inState.Send(firingTransition);
        }
    }

    public override void Unexecute()
    {
        foreach (State inState in inStates)
        {
            inState.ForceAddToken();
        }
        
        if (firingTokens.Count == 0)
        {
            foreach (State outState in outStates)
            {
                Debug.Log("Removed after done firing");
                outState.ForceRemoveToken();
            }
        }
        else
        {
            foreach (Token firingToken in firingTokens)
            {
                Debug.Log("Destroyed on the run");
                firingToken.Destroy();
            }
        }
    }
}
