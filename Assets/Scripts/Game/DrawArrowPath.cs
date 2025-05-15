using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DrawArrowPath : MonoBehaviour
{
    private static DrawArrowPath _instance;
    public static DrawArrowPath Instance
    {
        get { return _instance; }
    }

    
    [SerializeField] private GameObject arrowPrefab;
    private List<GameObject> _arrowList;
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(gameObject);
        else
            _instance = this;
    }
    
    private void Start()
    {
        _arrowList = new List<GameObject>();
    }

    public void DrawPath(List<GridNode> path, GridNode current)
    {
        DeletePath();
        path.Insert(0, current); 

        for (int i = 0; i < path.Count - 1; i++)
        {
            GameObject newArrow = Instantiate(arrowPrefab);

            // Xoay mũi tên theo hướng di chuyển
            Vector2 dir = path[i + 1].GridLocation - path[i].GridLocation;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            newArrow.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            Vector3 startPos = new Vector3(path[i].GridLocation.x + 0.5f, path[i].GridLocation.y + 0.5f, 0f);
            newArrow.transform.position = startPos;
            
            _arrowList.Add(newArrow);
        }        
    }

    public void DeletePath()
    {
        foreach (var arrow in _arrowList)
            Destroy(arrow);
        
        _arrowList.Clear();
    }

}