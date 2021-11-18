using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveTokenCommand : Command
{
    private readonly State removingState;

    public RemoveTokenCommand(State removingState)
    {
        this.removingState = removingState;
    }

    public override void Execute()
    {
        Token toBeRemoved = removingState.tokens[0];
        removingState.tokens.RemoveAt(0);
        GameObject.Destroy(toBeRemoved.gameObject);
    }

    public override void Unexecute()
    {
        removingState.ForceAddToken();
    }
}
