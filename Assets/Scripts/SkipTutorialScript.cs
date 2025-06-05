using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SkipTutorialScript : MonoBehaviour
{
    [SerializeField] private VideoPlayer _videoPlayer;

    void Start()
    {
        // Optional: Start automatically
        if (_videoPlayer != null)
        {
            _videoPlayer.Play();
        }
    }

    
    public void SkipTutorial()
    {
        _videoPlayer.Stop();
        SceneManager.LoadScene("Game Screen");
    }
}
