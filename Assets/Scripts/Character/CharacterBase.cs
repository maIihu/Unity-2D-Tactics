using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    public CharacterData data;
    public List<GridNode> MovableNodes { get; set; }
    public List<GridNode> UnwalableNodes;
    public List<GridNode> AttackAbleTiles { get; set; }
    public List<GridNode> IntendedPath { get; set; }
    public GridNode currentNode;

    private PathFinding _pathFinding;
    private GameObject _ghostInstance;
    
    private void Awake()
    {
        MovableNodes = new List<GridNode>();
        UnwalableNodes = new List<GridNode>();
        AttackAbleTiles = new List<GridNode>();
        _pathFinding = new PathFinding();
    }

    public void ActiveTurn()
    {
        Debug.Log(gameObject.name + " TURN");
        transform.GetChild(0).gameObject.SetActive(true);
        List<GridNode> surroundingGridNode = MapManager.Instance.GetSurroundingGridNode(currentNode, 10);
        foreach (var gridNode in surroundingGridNode)
        {
            if(!gridNode.occupyingObject)
                MovableNodes.Add(gridNode);
            else
                UnwalableNodes.Add(gridNode);
        }
        
        foreach (var gridNode in MovableNodes) gridNode.ShowMovableGridNode();
        foreach (var gridNode in UnwalableNodes) gridNode.ShowUnwalkableGridNode();
    }

    public void HandleClick(GridNode gridNode)
    {
        if (MovableNodes.Contains(gridNode))
        {
            IntendedPath = _pathFinding.FindPath(currentNode, gridNode);
            SpawnGhostInTarget(gridNode);
            DrawArrowPath.Instance.DrawPath(IntendedPath, currentNode);
        }
    }
    
    public void SpawnGhostInTarget(GridNode targetTile)
    {
        transform.GetChild(0).gameObject.SetActive(true);
        ResetGhost();
        _ghostInstance = Instantiate(gameObject);
        _ghostInstance.transform.position = new Vector3(targetTile.GridLocation.x, targetTile.GridLocation.y, 0);
        _ghostInstance.name = "Ghost_" + name;
        transform.GetChild(0).gameObject.SetActive(false);
        DestroyImmediate(_ghostInstance.GetComponent<Collider2D>());
        DestroyImmediate(_ghostInstance.GetComponent<CharacterBase>());
        SetGhostAlpha(0.5f);
    }
    
    private void SetGhostAlpha(float alpha)
    {
        SpriteRenderer sr = transform.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = alpha; 
            sr.color = c;
        }
    }

    private void ResetGhost()
    {
        if (_ghostInstance) Destroy(_ghostInstance);
        SetGhostAlpha(1f);
    }
    
    public IEnumerator MoveAlongPath()
    {
        ResetGhost();
        currentNode.occupyingObject = null;
        foreach (GridNode tile in IntendedPath)
        {
            Vector3 targetPosition = new Vector3(tile.GridLocation.x, tile.GridLocation.y, 0);
            while (Vector3.Distance(transform.localPosition, targetPosition) > 0.1f)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, Time.deltaTime * 5f); 
                yield return null;
            }
            currentNode = tile;
        }
        transform.position = new Vector3(currentNode.GridLocation.x, currentNode.GridLocation.y, 0);
        currentNode.occupyingObject = gameObject;
        RemovePath();
    }

    private void RemovePath()
    {
        foreach (var gridNode in MovableNodes) gridNode.HideGridNode();
        foreach (var gridNode in UnwalableNodes) gridNode.HideGridNode();
        
        MovableNodes.Clear();
        UnwalableNodes.Clear();
        IntendedPath.Clear();
        DrawArrowPath.Instance.DeletePath();
    }

    public bool IsEnemy(CharacterBase otherCharacter)
    {
        return data.team != otherCharacter.data.team;
    }
}
