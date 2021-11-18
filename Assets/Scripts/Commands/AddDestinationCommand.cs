using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public override void Execute()
    {
        if (isState) destination = ProgramManager.Instance.NewState();
        else destination = ProgramManager.Instance.NewTransition();

        destination.transform.position = position;
        destination.inputField.ActivateInputField();
    }

    public override void Unexecute()
    {
        GameObject.Destroy(destination.gameObject);
    }
}