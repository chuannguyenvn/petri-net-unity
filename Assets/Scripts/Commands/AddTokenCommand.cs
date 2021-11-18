using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddTokenCommand : Command
{
    private readonly State addingState;

    public AddTokenCommand(State addingState)
    {
        this.addingState = addingState;
    }

    public override void Execute()
    {
        Token newToken = ProgramManager.Instance.NewToken();
        newToken.transform.position =
            addingState.transform.position + Vector3.up * 100f * Screen.width / 1920f;
        newToken.prevPos = newToken.transform.position;
        newToken.destination = addingState;
    }

    public override void Unexecute()
    {
        addingState.ForceRemoveToken();
    }
}