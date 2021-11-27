using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sub class used to remove a token //
public class RemoveTokenCommand : Command
{
    private State removingState;

    public RemoveTokenCommand(State removingState)
    {
        this.removingState = removingState;
    }

    // Execute: Destroy a token of this state
    public override void Execute()
    {
        Token toBeRemoved = removingState.tokens[0];
        removingState.tokens.RemoveAt(0);
        GameObject.Destroy(toBeRemoved.gameObject);
    }

    // Execute: Force the state to add a token to itself
    public override void Unexecute()
    {
        removingState.ForceAddToken();
    }
}
