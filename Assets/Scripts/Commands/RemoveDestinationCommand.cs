using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sub class used to remove a state/transition //
public class RemoveDestinationCommand : Command
{
    private Destination destination;
    private Vector2 position;
    private List<Destination> inDestinations;
    private List<Destination> outDestinations;
    private string name;

    public RemoveDestinationCommand(Destination destination)
    {
        this.destination = destination;
        position = destination.transform.position;
        inDestinations = destination.inDestinations;
        outDestinations = destination.outDestinations;
        name = destination.inputField.text;
    }

    // Execute: Deactivate the state/transition and remove all connection to and from this.
    public override void Execute()
    {
        destination.gameObject.SetActive(false);
        
        foreach (Destination destination in inDestinations)
        {
            int index = destination.outDestinations.FindIndex(x => x.identifier == this.destination.identifier);
            if (index != -1)
            {
                destination.outDestinations.RemoveAt(index);
                
            }
        }

        foreach (Destination destination in outDestinations)
        {
            int index = destination.inDestinations.FindIndex(x => x.identifier == this.destination.identifier);
            if (index != -1) destination.inDestinations.RemoveAt(index);
        }
    }

    // Execute: Reactivate the state/transition and restore all connections and arcs.
    public override void Unexecute()
    {
        destination.gameObject.SetActive(true);

        destination.transform.position = position;
        destination.inDestinations = inDestinations;
        destination.outDestinations = outDestinations;
        
        foreach (Destination destination in inDestinations)
        {
            destination.outDestinations.Add(this.destination);
        }

        foreach (Destination destination in outDestinations)
        {
            destination.inDestinations.Add(this.destination);
        }
        
        foreach (Destination inDestination in inDestinations)
        {
            Arc arc = GameObject.Instantiate(ProgramManager.Instance.arrowPrefab,
                ProgramManager.Instance.canvas.transform).GetComponent<Arc>();

            arc.origin = inDestination.transform;
            arc.target = destination.transform;
        }

        foreach (Destination outDestination in outDestinations)
        {
            Arc arc = GameObject.Instantiate(ProgramManager.Instance.arrowPrefab,
                ProgramManager.Instance.canvas.transform).GetComponent<Arc>();

            arc.origin = destination.transform;
            arc.target = outDestination.transform;
        }

        destination.inputField.text = name;
    }
}