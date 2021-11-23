using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public class ReachableMarkings : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI text;

   [Serializable]
   public class RawState
   {
      public string ID;
      public string name;
      public List<string> inTransitions;
      public List<string> outTransitions;
      public int tokenCount;

      public RawState(string ID, string name, int tokenCount)
      {
         this.ID = ID;
         this.name = name;
         inTransitions = new List<string>();
         outTransitions = new List<string>();
         this.tokenCount = tokenCount;
      }
   }

   [Serializable]
   public class RawTransition
   {
      public string ID;
      public string name;
      public List<string> inStates;
      public List<string> outStates;

      public RawTransition(string ID, string name)
      {
         this.ID = ID;
         this.name = name;
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
         foreach (string inState in checkingTransition.inStates)
         {
            if (states.Find(x => x.ID == inState).tokenCount == 0) return false;
         }

         return true;
      }

      public void Set()
      {
         foreach (RawState state in states)
         {
            ProgramManager.Instance.states.Find(x => x.identifier == state.ID)
               .ForceSetTokenCount(state.tokenCount);
         }
      }

      public override string ToString()
      {
         string printStr = "[";

         foreach (RawState state in states)
         {
            if (state.tokenCount == 0) printStr += state.name + ",";
            else printStr += state.tokenCount + "." + state.name + ",";
         }

         printStr = printStr.Substring(0, printStr.Length - 1) + "]";

         return printStr;
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
         sequence.Add(firingTransition.name);

         foreach (string inState in firingTransition.inStates)
         {
            currentMarking.states.Find(x => x.ID == inState).tokenCount--;
         }

         foreach (string outState in firingTransition.outStates)
         {
            currentMarking.states.Find(x => x.ID == outState).tokenCount++;
         }
      }

      public override string ToString()
      {
         string printStr = "[";

         if (sequence.Count > 0)
         {
            for (int i = 0; i < sequence.Count - 1; i++)
               printStr += sequence[i].Split('~')[0] + ", ";

            printStr += sequence[sequence.Count - 1].Split('~')[0];
         }

         printStr += "]";

         return printStr;
      }
   }

   [SerializeField] private Transform loadingBackground;
   [SerializeField] private AnimationCurve loadingCurve;
   [SerializeField] private TMP_InputField checkingMarkingField;

   private List<RawState> rawStates;
   private List<RawTransition> rawTransitions;
   private List<FiringSequence> firingSequences;
   private bool calculating = false;
   private int currentMarkingCount = 0;

   private void Start()
   {
      checkingMarkingField.onEndEdit.AddListener(OnEndEdit);
   }

   public void UpdateMarkings()
   {
      calculating = true;
      Init();
      StartCoroutine(Calculate_CO());
      StartCoroutine(FunniText_CO());
      StartCoroutine(RotateLoadingBackground_CO());
   }

   public void Init()
   {
      rawStates = new List<RawState>();
      rawTransitions = new List<RawTransition>();

      foreach (State state in ProgramManager.Instance.states)
      {
         RawState rawState =
            new RawState(state.identifier, state.inputField.text, state.tokens.Count);

         foreach (Destination inDestination in state.inDestinations)
            rawState.inTransitions.Add(inDestination.identifier);

         foreach (Destination outDestination in state.outDestinations)
            rawState.outTransitions.Add(outDestination.identifier);

         rawStates.Add(rawState);
      }

      foreach (Transition transition in ProgramManager.Instance.transitions)
      {
         RawTransition rawTransition =
            new RawTransition(transition.identifier, transition.inputField.text);

         foreach (Destination inDestination in transition.inDestinations)
            rawTransition.inStates.Add(inDestination.identifier);

         foreach (Destination outDestination in transition.outDestinations)
            rawTransition.outStates.Add(outDestination.identifier);

         rawTransitions.Add(rawTransition);
      }
   }

   public IEnumerator Calculate_CO()
   {
      firingSequences = new List<FiringSequence> { new FiringSequence(rawStates, rawTransitions) };

      yield return Recur(firingSequences[0]);

      IEnumerator Recur(FiringSequence currentSequence)
      {
         List<RawTransition> currentTransitions = currentSequence.currentMarking.transitions;

         for (int i = 0; i < currentTransitions.Count; i++)
         {
            if (!currentSequence.currentMarking.CheckEnabled(currentTransitions[i].ID)) continue;

            FiringSequence newSequence = DeepClone(currentSequence);
            newSequence.NewFire(i);
            firingSequences.Add(newSequence);
            yield return new WaitForEndOfFrame();
            yield return Recur(newSequence);
         }
      }

      string displayText = "Total: " + firingSequences.Count + "\n";

      StartCoroutine(PrintAnswer_CO());
      calculating = false;

      IEnumerator PrintAnswer_CO()
      {
         for (int i = 0; i < firingSequences.Count; i++)
         {
            displayText += i.ToString() + ". " + firingSequences[i].currentMarking.ToString() + "\n";
            if (i % 100 == 0) yield return new WaitForEndOfFrame();
         }

         text.text = displayText;

         loadingBackground.rotation = Quaternion.Euler(0, 0, 0);

         StopAllCoroutines();
      }
   }

   IEnumerator FunniText_CO()
   {
      List<string> funniTexts = new List<string>()
      {
         "Working hard",
         "Finding all markings",
         "Running as fast as possible",
         "Hunting reachable markings",
         "Getting your answers",
         "CPU is sweating",
         "Bullying CPU",
         "Frying your CPU",
         "Exploring possibilities",
         "Counting sheeps instead",
         "So many markings",
         "Hiring workers to help",
         "Counting pass ten fingers",
         "Running faster with legs",
         "Speeding things up",
         "That's a lot of markings",
         "Searching team dispatched",
         "Hide and seek with markings",
         "Recursion frenzy",
         "Recur(Recur(Recur(Recur(Re",
         "Should have used loop instead",
         "Stack is pretty busy",
         "Stack is not happy about this",
         "This gonna take a while",
         "Sit back and relax",
         "Starting time machine",
         "Might want to correct your posture"
      };

      int random = 0;
      int prevRandom = 0;
      float longWaitTime = 5f + Time.time;

      while (true)
      {
         while (random == prevRandom)
            random = Random.Range(0, funniTexts.Count);

         string randomText = funniTexts[random];
         string dots = "";

         for (int i = 0; i < 11; i++)
         {
            if (calculating) text.text = randomText + dots;
            else text.text = "Printing your answer" + dots;

            if (Time.time > longWaitTime)
               text.text += "\n\nLong waiting time? \nCounting up to "
                            + firingSequences.Count
                            + " reachable markings.";
            if (i % 3 == 2) dots += '.';
            yield return new WaitForSeconds(0.3f);
         }

         prevRandom = random;
      }
   }

   IEnumerator RotateLoadingBackground_CO()
   {
      float startTime = Time.time;
      while (true)
      {
         loadingBackground.rotation = Quaternion.Euler(0, 0,
            360 * loadingCurve.Evaluate(Mathf.Repeat(Time.time - startTime, 1f)));
         yield return new WaitForEndOfFrame();
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

   public void OnEndEdit(string strIndex)
   {
      int index = 0;
      int.TryParse(strIndex, out index);

      firingSequences[index].currentMarking.Set();
   }
}