using UnityEngine;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour
{
    public GameObject Winner;
    public Text WinnerText;

    // Update is called once per frame
    void Update()
    {
        WinnerText.text = Winner.name;
    }
}
