using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Singleton class that handle common operations and resources ///
public class ProgramManager : MonoBehaviour
{
    // Singleton instance
    public static ProgramManager Instance;

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

    // Universal scale for a token
    public float tokenScale = 0.3f;

    // State and transition counters, with properties
    private int stateCounter = -1;
    private int transitionCounter = -1;
    public int StateCounter => ++stateCounter;
    public int TransitionCounter => ++transitionCounter;

    private void Awake()
    {
        Instance = this; // Singleton
    }

    void Start()
    {
        // Set the counters by counting all states and transitions in scene
        stateCounter += FindObjectsOfType<State>().Length;
        transitionCounter += FindObjectsOfType<Transition>().Length;
    }

    // Below are several methods for instantiating prefabs quickly
    // The instantiated object will be parented to the canvas
    public State NewState()
    {
        return Instantiate(statePrefab, canvas.transform).GetComponent<State>();
    }

    public Transition NewTransition()
    {
        return Instantiate(transitionPrefab, canvas.transform).GetComponent<Transition>();
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
}