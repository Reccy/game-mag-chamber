using UnityEngine;
using System.Collections;

public class BulletPattern : MonoBehaviour
{
    public GameManager GameManager { set; get; }
    public LevelManager LevelManager { set; get; }
    public GameObject Player { set; get; }

    void Start()
    {
        if(GameManager == null || LevelManager == null || Player == null)
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
