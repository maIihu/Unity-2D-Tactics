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

    private CharacterBase _targetEnemy;
    private UIManager _uiManager;
    
    private void Awake()
    {
        _movableNodes = new List<GridNode>();
        _attackAbleNodes = new List<GridNode>();
        _unwalkableNodes = new List<GridNode>();
        _pathFinding = new PathFinding();
        
        _enemyInRange = new List<CharacterBase>();
        _uiManager = UIManager.Instance;
    }

    public void ActiveTurn()
    {
        //Debug.Log(gameObject.name + " TURN");
        _targetEnemy = null;
        
        transform.GetChild(0).gameObject.SetActive(true);
        var surroundingMoveRange = MapManager.Instance.GetSurroundingGridNode(currentNode, characterData.moveRange);
        var surroundingAttackRange = MapManager.Instance.GetSurroundingGridNode(currentNode, characterData.attackRange + characterData.moveRange);
        
        foreach (var gridNode in surroundingMoveRange)
        {
            if (gridNode == currentNode || !gridNode.occupyingObject)
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
        _uiManager.HideButton(_uiManager.buttonAttack);
        if (_movableNodes.Contains(gridNode))
        {
            _targetEnemy = null;
            _intendedPath = _pathFinding.FindPath(currentNode, gridNode);
            SpawnGhostInTarget(gridNode);
            DrawArrowPath.Instance.DrawPath(_intendedPath, currentNode);
        }
    }

    public void HandleClickToEnemyCharacter(CharacterBase enemy)
    {
        // can be add condition here to show button attack 
        
        // // if enemy in range attack 
        // if (surroundingAttackRange.Contains(enemy.currentNode))
        // {
        //     // show attack button in here
        //     // something ....
        //     _targetEnemy = enemy;
        // }
        // else
        // {
            // if enemy range out attack 
            if (_enemyInRange.Contains(enemy))
            {
                _targetEnemy = enemy;
                _uiManager.ShowButton(_uiManager.buttonAttack);
                
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
                if (_intendedPath != null)
                {
                    SpawnGhostInTarget(_intendedPath[^1]);
                    DrawArrowPath.Instance.DrawPath(_intendedPath, currentNode);
                }
            
        }
    }

    public IEnumerator Attack()
    {
        if (!_targetEnemy)
        {
            Debug.Log("CAN NOT ATTACK");
            yield break;
        }
        
        if (_intendedPath.Count > 0)
            yield return StartCoroutine(MoveAlongPath());
        
        Debug.Log(name + " ATTACK TO " + _targetEnemy.name);
        _targetEnemy = null;
        _uiManager.HideButton(_uiManager.buttonAttack);
    }
    
    private void SpawnGhostInTarget(GridNode targetTile)
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
            character.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
            character.transform.GetChild(0).gameObject.SetActive(false);
        }
        transform.GetChild(0).gameObject.SetActive(false);
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
