
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButton : MonoBehaviour
{
    public void ClickToButtonPlay()
    {
        SceneManager.LoadScene("Game Play");
    }
}
