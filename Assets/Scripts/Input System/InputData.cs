using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputType
{
    MouseClick, KeyBoard, UIEvent
}

public abstract class InputData
{
    public InputType InputType { get; protected set; }
}
