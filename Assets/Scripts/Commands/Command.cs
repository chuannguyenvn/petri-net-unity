using UnityEngine;

// Base class for the Command pattern //
public abstract class Command
{
    public Command()
    {
        ProgramManager.Instance.history.Push(this);
    }

    // Method for handling a specific operation
    public abstract void Execute();

    // Method for undo the last operation
    public abstract void Unexecute();
}