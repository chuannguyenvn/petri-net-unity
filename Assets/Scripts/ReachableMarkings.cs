using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

// Class used to find and manage operation related to reachable markings //
[Serializable]
public class ReachableMarkings : MonoBehaviour
{
    // Raw classes are a simplified versions of the State and Transition classes
    // Working with real State and Transition classes are complex and requires heavy operations
    // This approach is much more flexible and fast, at the cost of memory
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

    // Represent a Petri net marking
    [Serializable]
    public class Marking
    {
        public List<RawState> states;
        public List<RawTransition> transitions;

        // Deep-copy every RawStates and RawTransitions
        public Marking(List<RawState> states, List<RawTransition> transitions)
        {
            this.states = DeepClone(states);
            this.transitions = DeepClone(transitions);
        }

        // Simple method to check whether this transition is enabled, at this marking
        public bool CheckEnabled(string transition)
        {
            RawTransition checkingTransition = transitions.Find(x => x.ID == transition);
            foreach (string inState in checkingTransition.inStates)
            {
                if (states.Find(x => x.ID == inState).tokenCount == 0) return false;
            }

            return true;
        }

        // Overriding ToString() to accurately represent this marking
        public override string ToString()
        {
            string printStr = "[";

            foreach (RawState state in states)
                printStr += state.tokenCount + "." + state.name + ",";
            
            printStr = (printStr.Length > 1 ? printStr.Substring(0, printStr.Length - 1) : "[") + "]";

            return printStr;
        }
    }

    // Store and manage transitions firing sequence
    [Serializable]
    public class FiringSequence
    {
        public List<string> sequence; // Hold the names of the transitions fired in this sequence
        public Marking currentMarking; // Hold the lastest marking

        public FiringSequence(List<RawState> states, List<RawTransition> transitions)
        {
            sequence = new List<string>();
            currentMarking = new Marking(states, transitions);
        }

        // "Fire" a transition by adding its name and changing the related states' token count
        public void NewFire(int index)
        {
            RawTransition firingTransition = currentMarking.transitions[index];
            sequence.Add(firingTransition.name);

            foreach (string inState in firingTransition.inStates)
                currentMarking.states.Find(x => x.ID == inState).tokenCount--;

            foreach (string outState in firingTransition.outStates)
                currentMarking.states.Find(x => x.ID == outState).tokenCount++;
        }

        // Overriding ToString() to accurately represent this sequence
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

    // Main text space for displaying all markings
    [SerializeField] private TextMeshProUGUI text; 
    
    // The scrollbars
    [SerializeField] private Scrollbar horizontalScrollbar;
    [SerializeField] private Scrollbar verticalScrollbar;
    
    // Will be rotated to indicate that the application is calculating
    [SerializeField] private Transform loadingBackground;
    
    // Affects rotating speed
    [SerializeField] private AnimationCurve loadingCurve;
    
    // Input field for checking a marking's states and sequence
    [SerializeField] private TMP_InputField checkingMarkingField;
    
    // Shows the sequence from the mentioned input field
    [SerializeField] private TextMeshProUGUI firedSequence;

    // Simulates the real states and transitions
    private List<RawState> rawStates;
    private List<RawTransition> rawTransitions;
    
    // Holds every possible firing sequence (also means markings)
    private List<FiringSequence> firingSequences;
    
    private bool calculating = false;

    private void Start()
    {
        checkingMarkingField.onEndEdit.AddListener(OnEndEdit);
    }

    // Starting method
    public void UpdateMarkings()
    {
        if (calculating)
        {
            calculating = false;
            text.text = "Cancelled";
            loadingBackground.rotation = Quaternion.Euler(0, 0, 0);
            StopAllCoroutines();
            return;
        }
        
        calculating = true;
        horizontalScrollbar.value = 0;
        verticalScrollbar.value = 1;
        firedSequence.text = "[Your chosen marking's firing sequence will be displayed here]";
        Init();
        StartCoroutine(Calculate_CO());
        StartCoroutine(FunniText_CO());
        StartCoroutine(RotateLoadingBackground_CO());
    }

    // Match the raw states/transitions' properties with the real states/transition's properties
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

    // Recursive method inside a coroutine to prevent the application from freezing
    public IEnumerator Calculate_CO()
    {
        float startingTime = Time.time;
        
        firingSequences = new List<FiringSequence> { new FiringSequence(rawStates, rawTransitions) };

        yield return Recur(firingSequences[0]);

        IEnumerator Recur(FiringSequence currentSequence)
        {
            List<RawTransition> currentTransitions = currentSequence.currentMarking.transitions;

            // Loop through all transitions
            // If a transition is enabled, add a new firing sequence including that transition
            for (int i = 0; i < currentTransitions.Count; i++)
            {
                if (!currentSequence.currentMarking.CheckEnabled(currentTransitions[i].ID)) continue;

                FiringSequence newSequence = DeepClone(currentSequence);
                newSequence.NewFire(i);
                
                firingSequences.Add(newSequence);
                yield return new WaitForEndOfFrame(); // Span the operation through multiple frames
                
                // New transitions will be added on top of this sequence in the next calls
                yield return Recur(newSequence); 
            }
        }

        string displayText = "Total markings: " + firingSequences.Count + "\n" +"Time elapsed: " + (Time.time - startingTime).ToString("F") + "s\n\n";

        StartCoroutine(PrintAnswer_CO());
        calculating = false;

        // Span the printing process over several frames to prevent freezing
        IEnumerator PrintAnswer_CO()
        {
            for (int i = 0; i < firingSequences.Count; i++)
            {
                displayText += i + ". " + firingSequences[i].currentMarking + "\n";
                if (i % 100 == 0) yield return new WaitForEndOfFrame();
            }

            // Visual configurations when the printing process is done
            text.text = displayText;
            loadingBackground.rotation = Quaternion.Euler(0, 0, 0);
            float textBoxWidth = text.rectTransform.sizeDelta.x;
            if (textBoxWidth < text.preferredWidth + 10) textBoxWidth = text.preferredWidth + 30;
            text.rectTransform.sizeDelta = new Vector2(textBoxWidth, text.preferredHeight + 50);
            StopAllCoroutines();
        }
    }

    // Display hilarious, comedic, smiles-provoking texts while the user are waiting
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
        float extremelyLongWaitTime = 120f + Time.time;

        while (true)
        {
            while (random == prevRandom) random = Random.Range(0, funniTexts.Count);

            string randomText = funniTexts[random];
            string dots = "";

            for (int i = 0; i < 12; i++)
            {
                if (calculating) text.text = randomText + dots;
                else text.text = "Printing your answer, hold on" + dots;

                if (Time.time > extremelyLongWaitTime)
                {
                    text.text += "\n\nExtremely long waiting time?\nThere could be a infinite amount of markings\nCounting up to "
                                 + firingSequences.Count
                                 + " reachable markings";
                }
                else if (Time.time > longWaitTime)
                {
                    text.text += "\n\nLong waiting time? \nCounting up to "
                                 + firingSequences.Count
                                 + " reachable markings";
                }
                
                yield return new WaitForSeconds(0.3f);
                if (i % 3 == 2) dots += '.';
            }

            prevRandom = random;
        }
    }

    // Rotate the button, indicating that the process is still going on
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
    
    // Method for deep-copying anything. Needed for the mentioned recursive method
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

    // Event method for viewing a marking
    public void OnEndEdit(string strIndex)
    {
        if (firingSequences == null || firingSequences.Count == 0)
        {
            checkingMarkingField.text = "No finding were executed";
            return;
        }

        bool parse = int.TryParse(strIndex, out int index);
        if (!parse || index < 0)
        {
            checkingMarkingField.text = "Invalid input. Please choose a positive integer index";
        }
        else if (firingSequences.Count == 1 && index > 0)
        {
            checkingMarkingField.text = "There is only 1 possible marking, at index 0";
        }
        else if (index >= firingSequences.Count)
        {
            checkingMarkingField.text = "Index is too large. Please choose an index from 0 to " +
                                        (firingSequences.Count - 1);
        }
        else
        {
            new SetMarkingCommand(firingSequences[index].currentMarking).Execute();

            checkingMarkingField.text = index + ". " + firingSequences[index].currentMarking;
            firedSequence.text = firingSequences[index].ToString();
        }
    }
}