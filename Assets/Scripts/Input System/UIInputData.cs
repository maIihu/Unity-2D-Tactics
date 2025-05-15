using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInputData : InputData
{
    public string ElementName { get; private set; }

    public UIInputData(string elementName)
    {
        InputType = InputType.UIEvent;
        ElementName = elementName;
    }
}