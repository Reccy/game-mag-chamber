using UnityEngine;
using System.Collections;

public class BulletPattern : MonoBehaviour
{
    public GameManager GameManager { set; get; }
    public LevelManager LevelManager { set; get; }
    public GameObject Player { set; get; }

    void Awake()
    {
        if(!GameManager || !LevelManager || !Player)
        {
            GameManager = Object.FindObjectOfType<GameManager>();
            LevelManager = Object.FindObjectOfType<LevelManager>();
            Player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    public void OnDestroy()
    {
        LevelManager.DestroyCallback(this);
    }
}
