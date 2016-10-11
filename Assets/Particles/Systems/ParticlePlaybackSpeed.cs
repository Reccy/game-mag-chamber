using UnityEngine;
using System.Collections;

public class ParticlePlaybackSpeed : MonoBehaviour {
    GameManager gameManager;

    void Awake()
    {
        gameManager = Object.FindObjectOfType<GameManager>();
    }

    void FixedUpdate()
    {
        this.gameObject.GetComponent<ParticleSystem>().playbackSpeed = gameManager.slowMotionMultiplier;
    }
}
