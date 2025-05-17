using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterTeam
{
    Blue, Red
}

[CreateAssetMenu(menuName = "ScriptableObject/CharacterData")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public CharacterTeam team;
    public int health;
    public int damage;
    public int speed;
    public int moveRange;
    public int attackRange;
}
