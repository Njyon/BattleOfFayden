using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    // Input delegates
    public delegate void OnMouseMoved(Vector2 position);
    public delegate void OnMousePressedDelegate(Vector3 position);
    public delegate void OnMouseReleasedDelegate();

    public delegate void OnKeyPressedDelegate(KeyCode keyCode);
    public delegate void OnKeyReleasedDelegate(KeyCode keyCode);

    // Input callbacks
    public OnMouseMoved onMouseMoved;

    public OnMousePressedDelegate onMousePressed;
    public OnMouseReleasedDelegate onMouseReleased;

    public OnKeyPressedDelegate onKeyPressed;
    public OnKeyReleasedDelegate onKeyReleased;

    public bool blockInput = false;

    Vector3 mousePos;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        if (blockInput)
            return;

        //Mouse
        MouseUpdate();
        
        // Keyboard
        KeyboardUpdate();
    }

    void MouseUpdate()
    {
        if (blockInput)
            return;

        //if the RightMouseButton is Down
        if (Input.GetMouseButton(1) && onMousePressed != null)
        {
            RaycastHit mousePosition;

            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out mousePosition, 100);
            onMousePressed(mousePosition.point);
        }

        //if the RightMouseButton is Up
        if (Input.GetMouseButtonUp(1))
        {
            if (onMouseReleased != null)
                onMouseReleased();
        }
        
        mousePos = Input.mousePosition;
        if (onMouseMoved != null)
        {
            onMouseMoved(new Vector2(mousePos.x, mousePos.y));
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0 && Camera.main.transform.position.y < 35.0f)
        {
            Camera.main.transform.position -= Camera.main.transform.forward * 5.0f;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && Camera.main.transform.position.y > 20.0f)
        {
            Camera.main.transform.position += Camera.main.transform.forward * 5.0f;
        }
    }

    void KeyboardUpdate()
    {
        if (blockInput)
            return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (onKeyPressed != null)
                onKeyPressed(KeyCode.Q);
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            if (onKeyReleased != null)
                onKeyReleased(KeyCode.Q);
        }
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (onKeyPressed != null)
                onKeyPressed(KeyCode.W);
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            if (onKeyReleased != null)
                onKeyReleased(KeyCode.W);
        }

        if (Input.GetKey(KeyCode.Space))
            if (onKeyPressed != null)
                onKeyPressed(KeyCode.Space);
    }
}