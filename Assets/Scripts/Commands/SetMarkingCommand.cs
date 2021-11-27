using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMarkingCommand : Command
{
    private Dictionary<string, int> tokenCountByStates;
    private List<ReachableMarkings.RawState> rawStates;
    
    public SetMarkingCommand(ReachableMarkings.Marking marking)
    {
        tokenCountByStates = new Dictionary<string, int>();
        foreach (State state in ProgramManager.Instance.states)
        {
            rawStates = marking.states;
            tokenCountByStates.Add(state.identifier, state.tokens.Count);
        }
    }
    
    public override void Execute()
    {
        foreach (ReachableMarkings.RawState state in rawStates)
        {
            ProgramManager.Instance.states.Find(x => x.identifier == state.ID)
                .ForceSetTokenCount(state.tokenCount);
        }
    }
    
    public override void Unexecute()
    {
        foreach (State state in ProgramManager.Instance.states)
        {
            state.ForceSetTokenCount(tokenCountByStates[state.identifier]);
        }
    }
}
