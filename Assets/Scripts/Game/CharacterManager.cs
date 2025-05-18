using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterManager : MonoBehaviour
{
    private static CharacterManager _instance;
    public static CharacterManager Instance
    {
        get { return _instance; }
    }
    
    [SerializeField] private GameObject[] blueCharacters;
    [SerializeField] private GameObject[] redCharacters;

    public Dictionary<int, CharacterBase> _charactersInRound;
    public CharacterBase characterTurned;
    
    public ChessPlayer RedPlayer { get; set; }
    public ChessPlayer BluePlayer { get; set; }
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(gameObject);
        else
            _instance = this;

        _charactersInRound = new Dictionary<int, CharacterBase>();
    }
    
    private void Start()
    {
        BluePlayer = new ChessPlayer(CharacterTeam.Blue);
        RedPlayer = new ChessPlayer(CharacterTeam.Red);
        List<Vector3Int> blueList = new(), redList = new();
        SetupPositionToSpawn(blueList, redList);
        SpawnCharacterInMap(blueCharacters, "Blue", blueList, BluePlayer.ActiveCharacters);
        SpawnCharacterInMap(redCharacters, "Red", redList, RedPlayer.ActiveCharacters);
        StartNewRound();
        ActiveNewCharacterTurn();
    }
    
    private void SetupPositionToSpawn(List<Vector3Int> blueList, List<Vector3Int> redList)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                blueList.Add(new Vector3Int(-6 + i, 6 - j, 0));
                redList.Add(new Vector3Int(6 - i, -6 + j, 0));
            }
        }
    }
    
    private void SpawnCharacterInMap(GameObject[] characters, String childName, List<Vector3Int> positionToSpawn, List<CharacterBase> teamCharacter)
    {
        foreach (var character in characters)
        {
            int randomPos = Random.Range(0, positionToSpawn.Count);
            
            GameObject newCharacter = Instantiate(character,  transform.Find("Character Container/" + childName));
            newCharacter.transform.localPosition = positionToSpawn[randomPos];
            newCharacter.transform.GetChild(0).gameObject.SetActive(false);
            
            teamCharacter.Add(newCharacter.GetComponent<CharacterBase>());
            newCharacter.GetComponent<CharacterBase>().currentNode = MapManager.Instance.Grids[positionToSpawn[randomPos]];
            MapManager.Instance.Grids[positionToSpawn[randomPos]].occupyingObject = newCharacter;
            positionToSpawn.RemoveAt(randomPos);
            
        }
    }

    private void StartNewRound()
    {
        List<CharacterBase> characterList = new List<CharacterBase>(); 
        characterList.AddRange(BluePlayer.ActiveCharacters);
        characterList.AddRange(RedPlayer.ActiveCharacters);
        
        characterList = characterList.OrderByDescending(x => x.characterData.speed).ToList();

        int i = 0;
        foreach (var character in characterList)
            _charactersInRound.Add(i++, character);
    }

    public void ActiveNewCharacterTurn()
    {
        if(_charactersInRound.Count <= 0)
            StartNewRound();
        
        int firstKey = _charactersInRound.Keys.First();
        characterTurned = _charactersInRound[firstKey];
    
        _charactersInRound.Remove(firstKey);
        characterTurned.ActiveTurn();
        
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>().MoveToPosition(characterTurned.transform.position);
    }
    
    

}
