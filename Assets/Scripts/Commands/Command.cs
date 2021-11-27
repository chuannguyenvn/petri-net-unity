using UnityEngine;

public abstract class Command
{
    public Command()
    {
        ProgramManager.Instance.history.Push(this);
    }
    
    public abstract void Execute();
    public abstract void Unexecute();
}
