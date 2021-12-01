using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

// Class used to display a short description when a user hover over an item //
public class HoveringTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string tooltipText;
    private GameObject tooltip;
    private bool running_CO;

    void Start()
    {
        tooltip = Instantiate(ProgramManager.Instance.tooltip, transform);
        tooltip.GetComponent<TMP_InputField>().text = tooltipText;
        tooltip.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!running_CO) StartCoroutine(ShowingTooltip_CO());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.SetActive(false);
        StopAllCoroutines();
        running_CO = false;
    }

    // Coroutine used to add a small delay before display the description
    IEnumerator ShowingTooltip_CO()
    {
        running_CO = true;
        float startingTime = Time.time;

        while (true)
        {
            if (Time.time - startingTime > 0.5f) tooltip.SetActive(true);
            yield return new WaitForEndOfFrame();
        }
    }
}