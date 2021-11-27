using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sub class used to make an arc //
public class NewArcCommand : Command
{
    public Destination origin; 
    public Destination target;
    public Arc arc;

    public NewArcCommand(Destination origin, Destination target, Arc arc)
    {
        this.origin = origin;
        this.target = target;
        this.arc = arc;
    }

    // Execute: Establish the connection between 2 states/transitions appropriately
    //          then configure the arc
    public override void Execute()
    {
        if (origin.GetType() == typeof(Transition))
        {
            ((Transition)origin).outDestinations.Add((State)target);
            ((State)target).inDestinations.Add((Transition)origin);
        }
        else
        {
            ((State)origin).outDestinations.Add((Transition)target);
            ((Transition)target).inDestinations.Add((State)origin);
        }

        arc.target = target.transform;
    }

    // Undo: Remove the established connection and the arc's target. The arc will destroy itself
    public override void Unexecute()
    {
        if (origin.GetType() == typeof(Transition))
        {
            ((Transition)origin).outDestinations.Remove((State)target);
            ((State)target).inDestinations.Remove((Transition)origin);
        }
        else
        {
            ((State)origin).outDestinations.Remove((Transition)target);
            ((Transition)target).inDestinations.Remove((State)origin);
        }

        arc.target = null;
    }
}