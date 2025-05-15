
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ColliderInputHandler : MonoBehaviour, IInputHandler
{
    private PathFinding _pathFinding;
    private MapManager _map;
    private CharacterBase _characterSelected;
     
    private void Awake()
    {
        _pathFinding = new PathFinding();
        _map = MapManager.Instance;
    }

    public void HandleInput(InputData input)
    {
        if (input is MouseInputData mouseInput && mouseInput.MouseButton == 0)
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
            if(hit.collider != null)
            {
                HandelClicked(hit.collider);
            }
        }
    }
    
    private void HandelClicked(Collider2D col)
    {
        switch (col.tag)
        {
            case "Character":
                HandleCharacter(col);
                break;
            case "GridOverlay":
                HandleOverlayNode(col);
                break;
        }
    }

    private void HandleCharacter(Collider2D col)
    {
        CharacterBase character = col.GetComponent<CharacterBase>();
        CharacterManager.Instance.characterTurned.HandleClickToEnemyCharacter(character);
    }

    private void HandleOverlayNode(Collider2D col)
    {
        GridNode gridNode = col.GetComponent<GridNode>();
        CharacterManager.Instance.characterTurned.HandleClickToGridNode(gridNode);
    }
}
