using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Class used exclusively for the Item 1b //
public class _1BMarkings : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI markingCountText;

    void Start()
    {
        inputField.onValueChanged.AddListener(OnValueChanged);
    }

    public void OnValueChanged(string strMarkingCount)
    {
        if (strMarkingCount == "") markingCountText.text = "";
        else if (!int.TryParse(strMarkingCount, out int markingCount) || markingCount < 0)
            markingCountText.text = "?";
        else markingCountText.text = (0.5f * (markingCount + 1) * (markingCount + 2)).ToString();
    }
}