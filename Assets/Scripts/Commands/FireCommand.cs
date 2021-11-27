using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Sub class used to fire an enabled transition //
public class FireCommand : Command
{
    public Transition firingTransition;
    public List<Destination> inStates;
    public List<Destination> outStates;
    public List<Token> firingTokens;

    public FireCommand(Transition firingTransition, List<Destination> inStates,
        List<Destination> outStates)
    {
        this.firingTransition = firingTransition;
        this.inStates = inStates;
        this.outStates = outStates;
        firingTokens = new List<Token>();
    }

    // Execute: Call a method for each inState. That method will handle the rest
    public override void Execute()
    {
        foreach (State inState in inStates)
            inState.Send(firingTransition);
    }

    // Undo: Restore 1 token in each inState,
    //       then destroy or remove tokens depending on whether the tokens have arrived or not
    public override void Unexecute()
    {
        foreach (State inState in inStates)
            inState.ForceAddToken();

        if (firingTokens.Count == 0)
        {
            foreach (State outState in outStates)
                outState.ForceRemoveToken();
        }
        else
        {
            foreach (Token firingToken in firingTokens)
                firingToken.Destroy();
        }
    }
}