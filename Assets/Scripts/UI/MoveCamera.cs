using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private Tilemap tileMap;
    [SerializeField] private float dragSpeed = 1f;
    
    private Vector3 _dragOrigin;
    private Bounds _mapBounds;
    private Camera _cam;
    
    private float _camHeight;
    private float _camWidth;
    
    private void Start()
    {
        _cam = GetComponent<Camera>();
        
        _mapBounds = tileMap.localBounds;
        _camHeight = _cam.orthographicSize;
        _camWidth = _camHeight * _cam.aspect;
    }

    private void Update()
    {
        HandleDragCamera();
    }

    private void HandleDragCamera()
    {
        if (Input.GetMouseButtonDown(0))
            _dragOrigin = _cam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButton(0))
        {
            Vector3 currentPos = _cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 difference = _dragOrigin - currentPos;
            
            Vector3 newPosition = transform.position + difference * dragSpeed;
            
            newPosition.x = Mathf.Clamp(newPosition.x, _mapBounds.min.x, _mapBounds.max.x);
            newPosition.y = Mathf.Clamp(newPosition.y, _mapBounds.min.y, _mapBounds.max.y);
            
            transform.position = newPosition;
        }
    }


}