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
        _intendedPath = new List<GridNode>();
        
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
        _uiManager.HideButton(_uiManager.buttonCancelAttack);
        
        _targetEnemy = null;

        if (gridNode == currentNode)
        {
            RemoveGhost();
            DrawArrowPath.Instance.DeletePath();
            return;
        }

        if (_movableNodes.Contains(gridNode))
        {
            _intendedPath = _pathFinding.FindPath(currentNode, gridNode);
            if (_intendedPath is { Count: > 0 }) SetIntendedPath(_intendedPath[^1]);
        }
    }

    public void HandleClickToEnemyCharacter(CharacterBase enemy)
    {
        if (_enemyInRange.Contains(enemy))  // enemy is range out attack 
        {
            _targetEnemy = enemy;
            _uiManager.ShowButton(_uiManager.buttonAttack);
            _uiManager.ShowButton(_uiManager.buttonCancelAttack);

            _uiManager.ChangeAttackInformation(this, enemy);
            _uiManager.ShowAttackInformation();
            
            // take surrounding node in enemy with attackRange of current character
            var surroundingNodeInEnemy =
                MapManager.Instance.GetSurroundingGridNode(enemy.currentNode, characterData.attackRange);
            List<List<GridNode>> pathsCanMove = new List<List<GridNode>>();
            foreach (var gridNode in surroundingNodeInEnemy)
            {
                if (!gridNode.occupyingObject)
                {
                    var path = _pathFinding.FindPath(currentNode, gridNode);
                    if (path.Count > 0)
                    {
                        pathsCanMove.Add(path);
                    }
                }
            }
            _intendedPath = pathsCanMove.OrderBy(p => p.Count).FirstOrDefault();
            if (_intendedPath is { Count: > 0 }) SetIntendedPath(_intendedPath[^1]);
        }
    }

    public IEnumerator Attack()
    {
        if (_targetEnemy == null || _targetEnemy.Equals(null))
        {
            Debug.Log("Enemy is already dead or destroyed");
            yield break;
        }
        if (_intendedPath.Count > 0) yield return StartCoroutine(MoveAlongPath());
        
        if (_targetEnemy == null || _targetEnemy.Equals(null))
        {
            Debug.Log("Enemy is already dead or destroyed");
            yield break;
        }

        Debug.Log(name + " ATTACK TO " + _targetEnemy.name);
        _targetEnemy.TakeDamage(characterData.damage);
        _targetEnemy = null;
        _uiManager.HideButton(_uiManager.buttonAttack);
        _uiManager.HideButton(_uiManager.buttonCancelAttack);
        _uiManager.HideAttackInformation();
    }

    private void SetIntendedPath(GridNode targetNode)
    {
        SpawnGhostInCurrentNode();
        DrawArrowPath.Instance.DrawPath(_intendedPath, currentNode);
        transform.position = targetNode.transform.position;
    }
    
    private void SpawnGhostInCurrentNode()
    {
        if (_ghostInstance) return;
        _ghostInstance = Instantiate(gameObject);
        _ghostInstance.transform.position = new Vector3(currentNode.GridLocation.x, currentNode.GridLocation.y, 0);
        _ghostInstance.name = "Ghost_" + name;
        _ghostInstance.transform.GetChild(0).gameObject.SetActive(false);
        DestroyImmediate(_ghostInstance.GetComponent<Collider2D>());
        DestroyImmediate(_ghostInstance.GetComponent<CharacterBase>());
        DestroyImmediate(_ghostInstance.GetComponent<Rigidbody2D>());
        SetGhostAlpha(0.5f);
    }
    
    private void SetGhostAlpha(float alpha)
    {
        SpriteRenderer sr = _ghostInstance.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = alpha; 
            sr.color = c;
        }
    }

    private void RemoveGhost()
    {
        transform.position = currentNode.transform.position;
        if (_ghostInstance) Destroy(_ghostInstance);
    }
    
    public IEnumerator MoveAlongPath()
    {
        RemoveGhost();
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
            if (character != null && character.transform.childCount > 0)
            {
                var child = character.transform.GetChild(0);
                if (child.TryGetComponent<SpriteRenderer>(out var sr))
                {
                    sr.color = Color.white;
                }
                child.gameObject.SetActive(false);
            }
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

    private void TakeDamage(int damage)
    {
        characterData.health -= damage;
        if (characterData.health <= 0) IsDeath();
    }

    private void IsDeath()
    {
        Debug.Log(name + " DEATH");
        if (characterData.team == CharacterTeam.Blue)
            CharacterManager.Instance.BluePlayer.ActiveCharacters.Remove(this);
        else
            CharacterManager.Instance.RedPlayer.ActiveCharacters.Remove(this);
        var entryToRemove = CharacterManager.Instance._charactersInRound
            .FirstOrDefault(pair => pair.Value == this);

        if (!entryToRemove.Equals(default(KeyValuePair<int, CharacterBase>)))
            CharacterManager.Instance._charactersInRound.Remove(entryToRemove.Key);

        currentNode.occupyingObject = null;
        Destroy(gameObject);
    }

}
