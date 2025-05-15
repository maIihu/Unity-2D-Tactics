
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinding
{
    private MapManager _map = MapManager.Instance;
    
    public List<GridNode> FindPath(GridNode startGridNode, GridNode targetGridNode)
    {
        List<GridNode> openList = new List<GridNode>();
        HashSet<GridNode> closeList = new HashSet<GridNode>();
        startGridNode.occupyingObject.TryGetComponent<CharacterBase>(out var currentCharacter);
        openList.Add(startGridNode);

        while (openList.Count > 0)
        {
            GridNode currentGridNode = openList.OrderBy(x => x.FCost).First();
            openList.Remove(currentGridNode);
            closeList.Add(currentGridNode);
            
            if (currentGridNode == targetGridNode)
                return RetracePath(startGridNode, targetGridNode);

            foreach (var neighbourTileNode in _map.GetNeighbourTile(currentGridNode))
            {
                if (closeList.Contains(neighbourTileNode) || !CanMoveThrough(neighbourTileNode, currentCharacter))
                    continue;
                
                var newGCost = currentGridNode.gCost + GetDistance(currentGridNode, neighbourTileNode);
                if (newGCost < neighbourTileNode.gCost || !openList.Contains(neighbourTileNode))
                {
                    neighbourTileNode.gCost = newGCost;
                    neighbourTileNode.hCost = GetDistance(neighbourTileNode, targetGridNode);
                    neighbourTileNode.parent = currentGridNode;
                    
                    if(!openList.Contains(neighbourTileNode))
                    {
                        openList.Add(neighbourTileNode);
                    }
                }
            }
        }

        return new List<GridNode>();
    }
    
    private bool CanMoveThrough(GridNode node, CharacterBase currentCharacter)
    {
        if (node.occupyingObject == null)
            return true;

        if (node.occupyingObject.TryGetComponent<CharacterBase>(out var otherCharacter))
        {
            return !currentCharacter.IsEnemy(otherCharacter); 
        }

        return false; 
    }

    
    private List<GridNode> RetracePath(GridNode startGridNode, GridNode endGridNode)
    {
        List<GridNode> path = new List<GridNode>();
        GridNode currentGridNode = endGridNode;

        while (currentGridNode != startGridNode)
        {
            path.Add(currentGridNode);
            currentGridNode = currentGridNode.parent;
        }
        path.Reverse();
        return path;
    }
    
    private int GetDistance(GridNode a, GridNode b)
    {
        int dstX = Mathf.Abs(a.GridLocation.x - b.GridLocation.x);
        int dstY = Mathf.Abs(a.GridLocation.y - b.GridLocation.y);
        return dstX + dstY;
    }
}