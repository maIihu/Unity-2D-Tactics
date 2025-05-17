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
                StartCoroutine(ClickToButtonAttack());
            }
            else if (uiInput.ElementName == "ButtonShow")
            {
                UIManager.Instance.ShowPanelInformation();
            }
            else if (uiInput.ElementName == "ButtonClosePanelInfor")
            {
                UIManager.Instance.HidePanelInformation();
            }
            else if (uiInput.ElementName == "ButtonCancelAttack")
            {
                ClickToButtonCancelAttack();
            }
        }
    }

    private void ClickToButtonCancelAttack()
    {
        UIManager.Instance.HideButton(UIManager.Instance.buttonCancelAttack);
        UIManager.Instance.HideButton(UIManager.Instance.buttonAttack);
        UIManager.Instance.HideAttackInformation();

    }
    private IEnumerator ClickToButtonStandby()
    {
        yield return StartCoroutine(CharacterManager.Instance.characterTurned.MoveAlongPath());
        UIManager.Instance.HideButton(UIManager.Instance.buttonAttack);
        UIManager.Instance.HideButton(UIManager.Instance.buttonCancelAttack);
        UIManager.Instance.HideButton(UIManager.Instance.buttonView);
        UIManager.Instance.HideAttackInformation();

        CharacterManager.Instance.ActiveNewCharacterTurn();
    }

    private IEnumerator ClickToButtonAttack()
    {
        yield return StartCoroutine(CharacterManager.Instance.characterTurned.Attack());
        CharacterManager.Instance.ActiveNewCharacterTurn();
    }
}