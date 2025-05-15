
using UnityEngine;

public class MouseInputData : InputData
{
    public Vector3 MousePosition { get; private set; }
    public int MouseButton { get; private set; }

    public MouseInputData(Vector3 mousePosition, int mouseButton)
    {
        InputType = InputType.MouseClick;
        MousePosition = mousePosition;
        MouseButton = mouseButton;
    }
}
