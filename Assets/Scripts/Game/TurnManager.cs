using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private static TurnManager _instance;
    public static TurnManager Instance
    {
        get { return _instance; }
    }
    
    public ChessPlayer ActivePlayer { get; set; }

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(gameObject);
        else
            _instance = this;
    }
    
    public void ActiveTurn()
    {
        Debug.Log(ActivePlayer.GetTeamName() + " TURN");
        //CharacterBase isCharacterTurn = ActivePlayer.ActiveCharacters;
    }
    
    
    public bool IsMyTurn(CharacterBase character)
    {
        return ActivePlayer.Team == character.data.team;
    }
}
