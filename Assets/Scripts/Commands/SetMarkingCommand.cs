using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sub class used to view a marking //
public class SetMarkingCommand : Command
{
    private Dictionary<string, int> tokenCountByStates; 
    private List<ReachableMarkings.RawState> rawStates;

    public SetMarkingCommand(ReachableMarkings.Marking marking)
    {
        rawStates = marking.states;
        
        tokenCountByStates = new Dictionary<string, int>();
        foreach (State state in ProgramManager.Instance.states)
            tokenCountByStates.Add(state.identifier, state.tokens.Count);
    }

    // Execute: Force all states to set their token count with this marking's counts
    public override void Execute()
    {
        foreach (ReachableMarkings.RawState state in rawStates)
        {
            ProgramManager.Instance.states.Find(x => x.identifier == state.ID)
                .ForceSetTokenCount(state.tokenCount);
        }
    }

    // Undo: Restore all states' token counts using a premade dictionary
    public override void Unexecute()
    {
        foreach (State state in ProgramManager.Instance.states)
            state.ForceSetTokenCount(tokenCountByStates[state.identifier]);
    }
}