using UnityEngine;

public class MouseManager : MonoBehaviour, IMouseManager
{
    Vector3 initialMousePosition;

    void Start()
    {
        initialMousePosition = Input.mousePosition;
    }

    public Vector2 GetMouseDelta()
    {
        // get the total delta of the mouse from the first frame
        // of gameplay
        return initialMousePosition - Input.mousePosition;
    }

    public Vector2 GetMousePosition()
    {
        return Input.mousePosition;
    }

    public float GetScroll()
    {
        return Input.GetAxis("Mouse ScrollWheel");
    }
}
