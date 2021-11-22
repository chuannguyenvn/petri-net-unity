using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEditor.Build;
using UnityEditor.VersionControl;
using UnityEditorInternal.VersionControl;
using UnityEngine;

[Serializable]
public class ReachableMarkings : MonoBehaviour
{
   [Serializable]
   public class RawState
   {
      public string ID;
      public List<string> inTransitions;
      public List<string> outTransitions;
      public int tokenCount;

      public RawState(string ID, int tokenCount)
      {
         this.ID = ID;
         inTransitions = new List<string>();
         outTransitions = new List<string>();
         this.tokenCount = tokenCount;
      }
   }

   [Serializable]
   public class RawTransition
   {
      public string ID;
      public List<string> inStates;
      public List<string> outStates;

      public RawTransition(string ID)
      {
         this.ID = ID;
         inStates = new List<string>();
         outStates = new List<string>();
      }
   }

   [Serializable]
   public class Marking
   {
      public List<RawState> states;
      public List<RawTransition> transitions;

      public Marking(List<RawState> states, List<RawTransition> transitions)
      {
         this.states = DeepClone(states);
         this.transitions = DeepClone(transitions);
      }

      public bool CheckEnabled(string transition)
      {
         RawTransition checkingTransition = transitions.Find(x => x.ID == transition);
         Debug.Log("Checking " + transition + " with " + checkingTransition.inStates.Count + " states.");
         foreach (string inState in checkingTransition.inStates)
         {
            Debug.Log("Token count: " + states.Find(x => x.ID == inState).tokenCount);
            if (states.Find(x => x.ID == inState).tokenCount == 0) return false;
         }

         return true;
      }
   }

   [Serializable]
   public class FiringSequence
   {
      public List<string> sequence;
      public Marking currentMarking;

      public FiringSequence(List<RawState> states, List<RawTransition> transitions)
      {
         sequence = new List<string>();
         currentMarking = new Marking(states, transitions);
      }

      public void NewFire(int index)
      {
         RawTransition firingTransition = currentMarking.transitions[index];
         sequence.Add(firingTransition.ID);

         foreach (string inState in firingTransition.inStates)
         {
            currentMarking.states.Find(x => x.ID == inState).tokenCount--;
         }

         foreach (string outState in firingTransition.outStates)
         {
            currentMarking.states.Find(x => x.ID == outState).tokenCount++;
         }
      }

      public void Print()
      {
         string printStr = "[";
         foreach (string transition in sequence) printStr += transition + ", ";
         printStr += "]";
         
         Debug.Log(printStr);
      }
   }

   public List<RawState> rawStates;
   public List<RawTransition> rawTransitions;

   private void Start()
   {
      InvokeRepeating(nameof(UpdateMarkings), 0, 1);
   }

   public void UpdateMarkings()
   {
      Init();
      Calculate();
   }
   
   public void Init()
   {
      rawStates = new List<RawState>();
      rawTransitions = new List<RawTransition>();

      foreach (State state in ProgramManager.Instance.states)
      {
         RawState rawState = new RawState(state.identifier, state.tokens.Count);
         
         foreach (Destination inDestination in state.inDestinations)
            rawState.inTransitions.Add(inDestination.identifier);

         foreach (Destination outDestination in state.outDestinations)
            rawState.outTransitions.Add(outDestination.identifier);

         rawStates.Add(rawState);
      }

      foreach (Transition transition in ProgramManager.Instance.transitions)
      {
         RawTransition rawTransition = new RawTransition(transition.identifier);

         foreach (Destination inDestination in transition.inDestinations)
            rawTransition.inStates.Add(inDestination.identifier);

         foreach (Destination outDestination in transition.outDestinations)
            rawTransition.outStates.Add(outDestination.identifier);

         rawTransitions.Add(rawTransition);
      }

      // foreach (RawState state in rawStates)
      // {
      //    string printStr = state.ID + ": \nIn: ";
      //
      //    foreach (string inTransition in state.inTransitions)
      //       printStr += inTransition;
      //
      //    printStr += "\nOut: ";
      //
      //    foreach (string outTransition in state.outTransitions)
      //       printStr += outTransition;
      //
      //    Debug.Log(printStr);
      // }
   }
   
   public void Calculate()
   {
      List<FiringSequence> firingSequences = new List<FiringSequence>();
      
      firingSequences.Add(new FiringSequence(rawStates, rawTransitions));

      Recur(firingSequences[0]);

      void Recur(FiringSequence currentSequence)
      {
         List<RawTransition> currentTransitions = currentSequence.currentMarking.transitions;
         Debug.Log(currentTransitions.Count);

         for (int i = 0; i < currentTransitions.Count; i++)
         {
            if (currentSequence.currentMarking.CheckEnabled(currentTransitions[i].ID))
            {
               FiringSequence newSequence = DeepClone(currentSequence);
               newSequence.NewFire(i);
               firingSequences.Add(newSequence);
               Recur(newSequence);
            }
         }
      }

      foreach (FiringSequence sequence in firingSequences)
      {
         sequence.Print();
      }
   }

   public static T DeepClone<T>(T obj)
   {
      using (var ms = new MemoryStream())
      {
         var formatter = new BinaryFormatter();
         formatter.Serialize(ms, obj);
         ms.Position = 0;

         return (T)formatter.Deserialize(ms);
      }
   }
}