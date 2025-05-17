using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get { return _instance; }
    }
    
    [SerializeField] public GameObject buttonAttack;
    [SerializeField] public GameObject buttonView;
    [SerializeField] public GameObject buttonCancelAttack;
    [SerializeField] public GameObject panelInformation;
    [SerializeField] public GameObject attackInformation;

    private void Awake()
    {
        if (_instance != null && _instance != this) Destroy(gameObject);
        else _instance = this;
    }

    private void Start()
    {
        HideButton(buttonView);
        HideButton(buttonAttack);
        HideButton(buttonCancelAttack);
        HidePanelInformation();
        HideAttackInformation();
    }

    public void ShowButton(GameObject button)
    {
        button.SetActive(true);
    }

    public void HideButton(GameObject button)
    {
        button.SetActive(false);
    }

    public void ChangeSpriteButtonView(Sprite sprite)
    {
        buttonView.transform.Find("Image").GetComponent<Image>().sprite = sprite;
    }
    
    public void ShowPanelInformation()
    {
        RectTransform panel = panelInformation.GetComponent<RectTransform>();
        panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, 100);
    }
    
    public void ChangePanelInformation(CharacterBase character)
    {
        CharacterData data = character.characterData;
        Image nameImage = panelInformation.transform.Find("Name").GetComponent<Image>();
        if (data.team == CharacterTeam.Blue) nameImage.color = new Color(0f, 0f, 1f, 0.5f);
        else nameImage.color = new Color(1f, 0f, 0f, 0.5f); 
        
        panelInformation.transform.Find("Name").GetComponentInChildren<TextMeshProUGUI>().text =
            data.name;
        panelInformation.transform.Find("Stat/Health").GetComponentInChildren<TextMeshProUGUI>().text =
            "Health: " + data.health;
        panelInformation.transform.Find("Stat/Damage").GetComponentInChildren<TextMeshProUGUI>().text =
            "Damage: " + data.damage;
        panelInformation.transform.Find("Stat/Speed").GetComponentInChildren<TextMeshProUGUI>().text =
            "Speed: " + data.speed;
    }

    public void HidePanelInformation()
    {
        RectTransform panel = panelInformation.GetComponent<RectTransform>();
        panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, -500);
    }

    public void ShowAttackInformation()
    {
        RectTransform panel = attackInformation.GetComponent<RectTransform>();
        panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, -200);
    }

    public void ChangeAttackInformation(CharacterBase character, CharacterBase otherCharacter)
    {
        GameObject panelBlue = attackInformation.transform.Find("Panel Blue Character").gameObject;
        GameObject panelRed = attackInformation.transform.Find("Panel Red Character").gameObject;

        CharacterBase blueCharacter , redCharacter;
        if (character.characterData.team == CharacterTeam.Blue)
        {
            blueCharacter = character;
            redCharacter = otherCharacter;
        }
        else
        {
            blueCharacter = otherCharacter;
            redCharacter = character;
        }
        
        panelBlue.transform.Find("Image").GetComponent<Image>().sprite =
            blueCharacter.GetComponent<SpriteRenderer>().sprite;
        panelBlue.transform.Find("Stat/Health").GetComponent<TextMeshProUGUI>().text = "Health: " + blueCharacter.characterData.health;
        panelBlue.transform.Find("Stat/Damage").GetComponent<TextMeshProUGUI>().text = "Damage: " + blueCharacter.characterData.damage;
        
        panelRed.transform.Find("Image").GetComponent<Image>().sprite =
            redCharacter.GetComponent<SpriteRenderer>().sprite;
        panelRed.transform.Find("Stat/Health").GetComponent<TextMeshProUGUI>().text = "Health: " + redCharacter.characterData.health;
        panelRed.transform.Find("Stat/Damage").GetComponent<TextMeshProUGUI>().text = "Damage: " + redCharacter.characterData.damage;
    }
    
    public void HideAttackInformation()
    {
        RectTransform panel = attackInformation.GetComponent<RectTransform>();
        panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, -500);
    }
    
}
