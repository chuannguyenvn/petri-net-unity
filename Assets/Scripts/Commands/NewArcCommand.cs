using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewArcCommand : Command
{
    public Destination origin; // Holds the transform of the "tail" of the arc
    public Destination target;

    public Arc arc;
    public NewArcCommand(Destination origin, Destination target, Arc arc)
    {
        this.origin = origin;
        this.target = target;
        this.arc = arc;
    }
    
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
