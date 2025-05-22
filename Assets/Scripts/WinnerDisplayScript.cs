using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinnerDisplayScript : MonoBehaviour
{
    public Text WinnerText;
    

    // Update is called once per frame
    void Update()
    {
        WinnerText.text = GameOverScript.Winner;
    }
    public void OnClickRestart()
    {
        SceneManager.LoadScene("Game Screen");
    }
}
