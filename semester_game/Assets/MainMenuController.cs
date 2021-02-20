using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    //AudioSource audioSource;

    //void Start()
   // {
     //   audioSource.clip = soundManager
       // audioSource = GetComponent<AudioSource>();
        //audioSource.Play();
    //}

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Debug for Unity Editor");
        Application.Quit();
    }
}
