using System;
using System.Collections;
using UnityEngine;

public class UIInputHandler : MonoBehaviour, IInputHandler
{
    
    public void HandleInput(InputData input)
    {
        if (input is UIInputData uiInput)
        {
            //Debug.Log($"UI Button clicked: {uiInput.ElementName}");
            if (uiInput.ElementName == "ButtonStandby")
            {
                StartCoroutine(ClickToButtonStandby());
            }
            else if (uiInput.ElementName == "ButtonAttack")
            {
                ClickToButtonAttack();
            }
        }
    }


    private IEnumerator ClickToButtonStandby()
    {
        yield return StartCoroutine(CharacterManager.Instance.characterTurned.MoveAlongPath());
        CharacterManager.Instance.ActiveNewCharacterTurn();
    }

    private void ClickToButtonAttack()
    {
        
    }
}