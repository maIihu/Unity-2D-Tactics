using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode : MonoBehaviour
{
    [SerializeField] private Sprite movableSprite;
    [SerializeField] private Sprite overlaySprite;
    [SerializeField] private Sprite unwalkableSprite;
    
    [HideInInspector]
    public int gCost; 
    public int hCost;
    
    
    public int FCost => gCost + hCost;

    [HideInInspector]
    public GridNode parent;
    public GameObject occupyingObject = null;
    
    public Vector2Int GridLocation
    {
        get { return new Vector2Int((int)transform.position.x, (int)transform.position.y); }
    }

    public void ShowMovableGridNode()
    {
        GetComponent<SpriteRenderer>().sprite = movableSprite;
    }
    
    public void HideGridNode()
    {
        GetComponent<SpriteRenderer>().sprite = overlaySprite;
    }

    public void ShowUnwalkableGridNode()
    {
        GetComponent<SpriteRenderer>().sprite = unwalkableSprite;

    }
}
