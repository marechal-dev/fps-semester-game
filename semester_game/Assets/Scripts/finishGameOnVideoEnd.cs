using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEditor;

public class finishGameOnVideoEnd : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    private void Start()
    {
        StartCoroutine(EndVideo());
        
    }

    IEnumerator EndVideo()
    {
        yield return new WaitForSeconds(6);
        Application.Quit();
        EditorApplication.isPlaying = false;
    }
}
