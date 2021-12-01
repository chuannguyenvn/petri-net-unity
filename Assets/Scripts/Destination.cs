using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Base class for State and Transition //
public class Destination : MonoBehaviour, IPointerClickHandler, IPointerUpHandler,
    IBeginDragHandler,
    IEndDragHandler,
    IDragHandler
{
    public List<Destination> inDestinations;
    public List<Destination> outDestinations;

    public List<Token> tokens;
    protected Mouse mouse;
    protected Background background;

    public string identifier;
    public InputField inputField;
    public Menu menu;

    public virtual void Start()
    {
        mouse = ProgramManager.Instance.mouse;
        background = ProgramManager.Instance.canvas.GetComponent<Background>();

        // If no name is provided initially, get a default name of [Type][number]
        if (inputField.text == string.Empty)
        {
            if (GetType() == typeof(State))
                inputField.text = "S" + ProgramManager.Instance.StateCounter;
            else
                inputField.text = "T" + ProgramManager.Instance.TransitionCounter;
        }

        // Generate a unique identifier for the transition
        string randomStr = ProgramManager.Instance.RandomString(transform.position.magnitude);
        identifier = inputField.text.Replace('~', ' ') + "~" + randomStr;

        // Linking this state with the outDestinations by using arcs
        foreach (Destination destination in outDestinations)
        {
            Arc arc = Instantiate(ProgramManager.Instance.arrowPrefab,
                ProgramManager.Instance.canvas.transform).GetComponent<Arc>();

            arc.origin = transform;
            arc.target = destination.transform;
        }

        StartCoroutine(Spawn_CO());
    }

    // Coroutine to animate the spawning process
    IEnumerator Spawn_CO()
    {
        float spawnTime = 0.3f;
        while (spawnTime > 0)
        {
            transform.localScale = Vector3.one * 
                                   (1 + Mathf.Sin(spawnTime * 30) * 0.05f * spawnTime / 0.5f);
            spawnTime -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.localScale = Vector3.one;
    }

    // Various methods for handling mouse events
    #region EventFunctions
    public virtual void OnPointerClick(PointerEventData eventData)
    {
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        transform.SetSiblingIndex(transform.parent.childCount - 1);
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        background.onDeselectClick.Invoke();
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        background.onDeselectClick.Invoke();
    }
    #endregion

    // When being destroyed, properly remove all relationships
    private void OnDestroy()
    {
        foreach (Destination destination in inDestinations)
        {
            int index = destination.outDestinations.FindIndex(x => x.identifier == identifier);
            if (index != -1) destination.outDestinations.RemoveAt(index);
        }

        foreach (Destination destination in outDestinations)
        {
            int index = destination.inDestinations.FindIndex(x => x.identifier == identifier);
            if (index != -1) destination.inDestinations.RemoveAt(index);
        }

        if (menu != null) Destroy(menu.gameObject);
    }

    // When enabled, enable the menu as well
    public virtual void OnEnable()
    {
        if (menu != null) menu.transform.localPosition = Vector3.zero;
    }

    // When disabled, disable the menu as well
    public virtual void OnDisable()
    {
        if (menu != null) menu.transform.position = Vector3.up * 10000;
    }
}