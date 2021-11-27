using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sub class used to add a new token to a state //
public class AddTokenCommand : Command
{
    private State addingState;

    public AddTokenCommand(State addingState)
    {
        this.addingState = addingState;
    }

    // Execute: Instantiate a new token and configure it 
    public override void Execute()
    {
        Token newToken = ProgramManager.Instance.NewToken();
        newToken.transform.position =
            addingState.transform.position + Vector3.up * 100f * Screen.width / 1920f;
        newToken.destination = addingState;
    }

    // Undo: Simply remove it from this state
    public override void Unexecute()
    {
        addingState.ForceRemoveToken();
    }
}