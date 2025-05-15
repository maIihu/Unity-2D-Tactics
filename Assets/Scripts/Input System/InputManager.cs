using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private List<MonoBehaviour> inputHandlers;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var input = new MouseInputData(Input.mousePosition, 0);
            foreach (var handler in inputHandlers)
            {
                if (handler is IInputHandler inputHandler)
                {
                    inputHandler.HandleInput(input);
                }
            }
        }
    }
    
    public void NotifyUIEvent(string elementName)
    {
        var input = new UIInputData(elementName);

        foreach (var handler in inputHandlers)
        {
            if (handler is IInputHandler inputHandler)
            {
                inputHandler.HandleInput(input);
            }
        }
    }
    
}
