using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PlayEngineVideos : MonoBehaviour
{
    private VideoPlayer _videoPlayer;

    void Awake() {
        _videoPlayer = GetComponent<VideoPlayer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Update background
        Debug.Log(PlayerOsopherDict.osopherQuestion.engineVideo.name);
        _videoPlayer.clip = PlayerOsopherDict.osopherQuestion.engineVideo;
    }
}
