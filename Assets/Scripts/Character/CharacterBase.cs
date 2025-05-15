using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterBase : MonoBehaviour
{
    private List<GridNode> _movableNodes;
    private List<GridNode> _attackAbleNodes;
    private List<GridNode> _unwalkableNodes;
    private List<GridNode> _intendedPath;

    private List<CharacterBase> _enemyInRange;
    
    private PathFinding _pathFinding;
    private GameObject _ghostInstance;
    
    public CharacterData characterData;
    public GridNode currentNode;
    
    private void Awake()
    {
        _movableNodes = new List<GridNode>();
        _attackAbleNodes = new List<GridNode>();
        _unwalkableNodes = new List<GridNode>();
        _pathFinding = new PathFinding();
        
        _enemyInRange = new List<CharacterBase>();
    }

    public void ActiveTurn()
    {
        //Debug.Log(gameObject.name + " TURN");
        transform.GetChild(0).gameObject.SetActive(true);
        var surroundingMoveRange = MapManager.Instance.GetSurroundingGridNode(currentNode, characterData.moveRange);
        var surroundingAttackRange = MapManager.Instance.GetSurroundingGridNode(currentNode, characterData.attackRange + characterData.moveRange);
        
        foreach (var gridNode in surroundingMoveRange)
        {
            if(!gridNode.occupyingObject)
                _movableNodes.Add(gridNode);
            else
                _unwalkableNodes.Add(gridNode);
        }

        foreach (var gridNode in surroundingAttackRange)
        {
            _attackAbleNodes.Add(gridNode);
            if(gridNode.occupyingObject != null && gridNode.occupyingObject.TryGetComponent<CharacterBase>(out var otherCharacter) && IsEnemy(otherCharacter))
            {
                _enemyInRange.Add(otherCharacter);
            }
        }
        ShowGridNodeInCanAction();
    }
    
    private void ShowGridNodeInCanAction(){
        foreach (var gridNode in _attackAbleNodes) gridNode.ShowUnwalkableGridNode();
        foreach (var gridNode in _movableNodes) gridNode.ShowMovableGridNode();
        foreach (var gridNode in _unwalkableNodes) gridNode.ShowUnwalkableGridNode();
        foreach (var character in _enemyInRange)
        {
            character.transform.GetChild(0).gameObject.SetActive(true);
            character.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
    
    public void HandleClickToGridNode(GridNode gridNode)
    {
        if (_movableNodes.Contains(gridNode))
        {
            _intendedPath = _pathFinding.FindPath(currentNode, gridNode);
            SpawnGhostInTarget(gridNode);
            DrawArrowPath.Instance.DrawPath(_intendedPath, currentNode);
        }
    }

    public void HandleClickToEnemyCharacter(CharacterBase enemy)
    {
        if (_enemyInRange.Contains(enemy))
        {
            // take surrounding node in enemy with attackRange of current character
            var surroundingNodeInEnemy =
                MapManager.Instance.GetSurroundingGridNode(enemy.currentNode, characterData.attackRange);
            List<List<GridNode>> pathsCanMove = new List<List<GridNode>>();
            foreach (var gridNode in surroundingNodeInEnemy)
            {
                var path = _pathFinding.FindPath(currentNode, gridNode);
                if (path.Count > 0)
                {
                    pathsCanMove.Add(path);
                }
            }
            _intendedPath = pathsCanMove.OrderBy(p => p.Count).FirstOrDefault();
            SpawnGhostInTarget(_intendedPath[_intendedPath.Count - 1]);
            DrawArrowPath.Instance.DrawPath(_intendedPath, currentNode);
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
        foreach (GridNode tile in _intendedPath)
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
        foreach (var gridNode in _movableNodes) gridNode.HideGridNode();
        foreach (var gridNode in _unwalkableNodes) gridNode.HideGridNode();
        foreach (var gridNode in _attackAbleNodes)gridNode.HideGridNode();
        foreach (var character in _enemyInRange)
        {
            character.transform.GetChild(0).gameObject.SetActive(false);
            character.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
        }
        
        _movableNodes.Clear();
        _unwalkableNodes.Clear();
        _intendedPath.Clear();
        _enemyInRange.Clear();
        _attackAbleNodes.Clear();
        
        DrawArrowPath.Instance.DeletePath();
    }

    public bool IsEnemy(CharacterBase otherCharacter)
    {
        return characterData.team != otherCharacter.characterData.team;
    }
    
}
