using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChessPlayer
{
    public CharacterTeam Team { get; set; }
    public List<CharacterBase> ActiveCharacters { get; set; }

    public ChessPlayer(CharacterTeam team)
    {
        Team = team;
        ActiveCharacters = new List<CharacterBase>();
    }

    public void AddChess(CharacterBase character)
    {
        if (!ActiveCharacters.Contains(character))
            ActiveCharacters.Add(character);
    }
    
    public void RemoveChess(CharacterBase character)
    {
        if (ActiveCharacters.Contains(character))
            ActiveCharacters.Remove(character);
    }

    public string GetTeamName()
    {
        if (Team == CharacterTeam.Blue) return "Blue";
        return "Red";
    }
}
