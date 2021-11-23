using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

/// Singleton class that handle common operations and resources ///
public class ProgramManager : MonoBehaviour
{
    // Singleton instance
    public static ProgramManager Instance;

    public bool isDisplaying = false;
    
    // Several prefabs for instantiating
    public GameObject statePrefab;
    public GameObject transitionPrefab;
    public GameObject tokenPrefab;
    public GameObject arrowPrefab;
    public GameObject stateMenuPrefab;
    public GameObject transitionMenuPrefab;

    // A single default menu, appear when right-clicking the background
    public DefaultMenu defaultMenu;

    // The ticker. Used for auto-firing
    public Ticker ticker;
    
    // This scene's canvas
    public Canvas canvas;

    // Mouse class will take care of mouse positions
    public Mouse mouse;
    
    public GameObject tooltip;

    // Universal scale for a token
    public float tokenScale = 0.3f;

    // State and transition counters, with properties
    private int stateCounter = -1;
    private int transitionCounter = -1;
    public int StateCounter => ++stateCounter;
    public int TransitionCounter => ++transitionCounter;

    public List<State> states;
    public List<Transition> transitions;
    public Stack<Command> history;
    
    private void Awake()
    {
        Instance = this; // Singleton
    }

    void Start()
    {
        states = new List<State>();
        transitions = new List<Transition>();

        foreach (State state in FindObjectsOfType<State>())
        {
            states.Add(state);
        }
        
        foreach (Transition transition in FindObjectsOfType<Transition>())
        {
            transitions.Add(transition);
        }
        
        // Set the counters by counting all states and transitions in scene
        stateCounter += states.Count;
        transitionCounter += transitions.Count;
        
        history = new Stack<Command>();
    }

    private void Update()
    {
        states.Sort(delegate(State A, State B)
        {
            Vector2 topLeft = new Vector2(-1000, 550);
            return Vector2.Distance(A.transform.position, topLeft)
                .CompareTo(Vector2.Distance(B.transform.position, topLeft));
        });
    }

    public Destination Find(string identifier)
    {
        int index = states.FindIndex(x => x.identifier == identifier);
        if (index != -1) return states[index];
        
        index = transitions.FindIndex(x => x.identifier == identifier);
        if (index != -1) return transitions[index];

        Debug.LogError("Destination doesn't exist.");
        return null;
    }
    
    #region InstantiateMethods
    // Below are several methods for instantiating prefabs quickly
    // The instantiated object will be parented to the canvas
    public State NewState()
    {
        State newState = Instantiate(statePrefab, canvas.transform).GetComponent<State>();
        states.Add(newState);
        return newState;
    }

    public Transition NewTransition()
    {
        Transition newTransition = Instantiate(transitionPrefab, canvas.transform).GetComponent<Transition>();
        transitions.Add(newTransition);
        return newTransition;
    }

    public Token NewToken()
    {
        return Instantiate(tokenPrefab, canvas.transform).GetComponent<Token>();
    }

    public Arc NewArrow()
    {
        return Instantiate(arrowPrefab, canvas.transform).GetComponent<Arc>();
    }

    public StateMenu NewStateMenu()
    {
        return Instantiate(stateMenuPrefab, canvas.transform).GetComponent<StateMenu>();
    }

    public TransitionMenu NewTransitionMenu()
    {
        return Instantiate(transitionMenuPrefab, canvas.transform).GetComponent<TransitionMenu>();
    }
    #endregion
    
    // Method for generating random string used in states and transitions' identifiers
    // The identifiers will be used to check each state/transition uniqueness
    public string RandomString(float seed)
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        string result = "";
        Random.InitState((int)(Time.time * 1000 + stateCounter * stateCounter +
                               transitionCounter * transitionCounter));
        result += chars[Random.Range(0, (int)seed) % chars.Length];
        for (int i = 0; i < 5; i++)
        {
            result += chars[Random.Range(0, (int)seed + result[result.Length - 1]) % chars.Length];
        }

        return result;
    }

    public void Undo()
    {
        if (history.Count > 0)
        {
            history.Peek().Unexecute();
            history.Pop();
        }
        else Debug.Log("History is empty");
    }
}