using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramManager : MonoBehaviour
{
    public static ProgramManager Instance;

    public GameObject statePrefab;
    public GameObject transitionPrefab;
    public GameObject tokenPrefab;
    public GameObject arrowPrefab;
    public GameObject stateMenuPrefab;
    public GameObject transitionMenuPrefab;

    public DefaultMenu defaultMenu;
    public StateMenu stateMenu;
    public TransitionMenu transitionMenu;

    public Canvas canvas;
    public Mouse mouse;

    public float tokenScale = 0.3f;

    private int stateCounter = -1;
    private int transitionCounter = -1;

    public int StateCounter => ++stateCounter;
    public int TransitionCounter => ++transitionCounter;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
    }

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
    } public TransitionMenu NewTransitionMenu()
    {
        return Instantiate(transitionMenuPrefab, canvas.transform).GetComponent<TransitionMenu>();
    }

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