using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class EndUIManager : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer video;
    // Start is called before the first frame update
    void Start()
    {
        video.Play();
        //結束時
        video.loopPointReached += EndReached;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void EndReached(VideoPlayer video)
    {
        SceneManager.LoadScene("Menu");
    }  

    public void Click(){
        SceneManager.LoadScene("Menu");
    }
}
