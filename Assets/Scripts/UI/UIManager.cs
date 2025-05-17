using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get { return _instance; }
    }
    
    [SerializeField] public GameObject buttonAttack;
    [SerializeField] public GameObject buttonView;

    private void Awake()
    {
        if (_instance != null && _instance != this) Destroy(gameObject);
        else _instance = this;
    }

    private void Start()
    {
        HideButton(buttonView);
        HideButton(buttonAttack);
    }

    public void ShowButton(GameObject button)
    {
        button.SetActive(true);
    }

    public void HideButton(GameObject button)
    {
        button.SetActive(false);
    }
    
    
}
