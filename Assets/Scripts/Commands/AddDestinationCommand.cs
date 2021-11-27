using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sub class used to add a new state/transition //
public class AddDestinationCommand : Command
{
    private Destination destination;
    private bool isState;
    private Vector2 position;

    public AddDestinationCommand(Vector2 position, bool isState)
    {
        this.position = position;
        this.isState = isState;
    }

    // Execute: Instantiate a new state/transition
    public override void Execute()
    {
        if (isState) destination = ProgramManager.Instance.NewState();
        else destination = ProgramManager.Instance.NewTransition();
        destination.transform.position = position;
        destination.inputField.ActivateInputField();
    }

    // Undo: Destroy that state/transition
    public override void Unexecute()
    {
        if (isState) ProgramManager.Instance.states.RemoveAt(
            ProgramManager.Instance.states.FindIndex(x => x.identifier == destination.identifier));
        else  ProgramManager.Instance.transitions.RemoveAt(
            ProgramManager.Instance.transitions.FindIndex(x => x.identifier == destination.identifier));

        GameObject.Destroy(destination.gameObject);
    }
}