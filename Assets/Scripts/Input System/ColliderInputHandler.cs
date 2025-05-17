
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ColliderInputHandler : MonoBehaviour, IInputHandler
{

    private CharacterBase _characterSelected;
    
    public void HandleInput(InputData input)
    {
        if (input is MouseInputData mouseInput && mouseInput.MouseButton == 0)
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(mouseWorldPos, Vector2.zero);

            RaycastHit2D? characterHit = null;
            RaycastHit2D? overlayHit = null;

            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Character") && characterHit == null)
                    characterHit = hit;
                else if (hit.collider.CompareTag("GridOverlay") && overlayHit == null)
                    overlayHit = hit;
            }

            if (characterHit != null)
            {
                Debug.Log("CLICK TO CHARACTER");
                HandleCharacter(characterHit.Value.collider);
            }
            else if (overlayHit != null)
            {
                HandleOverlayNode(overlayHit.Value.collider);
            }
        }
    }

    private void HandleCharacter(Collider2D col)
    {
        Debug.Log("CLICK TO CHARACTER " + col.name);
        CharacterBase character = col.GetComponent<CharacterBase>();
        
        UIManager.Instance.ShowButton(UIManager.Instance.buttonView);
        UIManager.Instance.ChangeSpriteButtonView(character.GetComponent<SpriteRenderer>().sprite);
        UIManager.Instance.ChangePanelInformation(character);
        
        CharacterManager.Instance.characterTurned.HandleClickToEnemyCharacter(character);
    }

    private void HandleOverlayNode(Collider2D col)
    {
        UIManager.Instance.HideButton(UIManager.Instance.buttonView);
        UIManager.Instance.HideAttackInformation();
        UIManager.Instance.HidePanelInformation();
        GridNode gridNode = col.GetComponent<GridNode>();
        CharacterManager.Instance.characterTurned.HandleClickToGridNode(gridNode);
    }
}
