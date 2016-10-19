using UnityEngine;
using System.Collections;

public class BulletPattern : MonoBehaviour
{
    public GameManager GameManager { set; get; }
    public LevelManager LevelManager { set; get; }
    public SoundManager SoundManager { set; get; }
    public GameObject Player { set; get; }

    void Start()
    {
        if (!GameManager || !LevelManager || !Player || !SoundManager)
        {
            GameManager = Object.FindObjectOfType<GameManager>();
            LevelManager = Object.FindObjectOfType<LevelManager>();
            Player = GameObject.FindGameObjectWithTag("Player");
            SoundManager = Object.FindObjectOfType<SoundManager>();
        }
    }

    public void OnDestroy()
    {
        LevelManager.DestroyCallback(this);
    }
}
