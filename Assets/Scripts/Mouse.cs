using UnityEngine;

// Class that take care of all mouse operations. Work with Background //
public class Mouse : MonoBehaviour
{
    public Arc currentArc;
    public Destination origin;

    private void Start()
    {
        // Add Deselect() to the event that handles deselecting
        ProgramManager.Instance.canvas.GetComponent<Background>().onDeselectClick
            .AddListener(Deselect);
    }

    // Matches mouse position
    void Update()
    {
        transform.position = Input.mousePosition;
    }

    private void Deselect()
    {
        if (currentArc != null) Destroy(currentArc.gameObject);
    }
}