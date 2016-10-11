using UnityEngine;
using System.Collections;

public class BulletPattern : MonoBehaviour
{
    public GameManager GameManager { set; get; }
    public LevelManager LevelManager { set; get; }
    public GameObject Player { set; get; }
    public GameObject GreenParticles { set; get; }
    public GameObject RedParticles { set; get; }
    public GameObject BlueParticles { set; get; }
    public enum SpawnColor { Red, Green, Blue };
    public SpawnColor spawnColor;

    void Start()
    {
        if (!GameManager || !LevelManager || !Player)
        {
            GameManager = Object.FindObjectOfType<GameManager>();
            LevelManager = Object.FindObjectOfType<LevelManager>();
            Player = GameObject.FindGameObjectWithTag("Player");
        }

        RedParticles = LevelManager.redParticles;
        GreenParticles = LevelManager.greenParticles;
        BlueParticles = LevelManager.blueParticles;

        if (spawnColor == null)
            spawnColor = SpawnColor.Red;

        Vector3 particlePosition = new Vector3(transform.position.x, transform.position.y, 100);

        switch(spawnColor)
        {
            case SpawnColor.Red:
                Destroy(Instantiate(RedParticles, particlePosition, Quaternion.identity), 5);
                break;
            case SpawnColor.Green:
                Destroy(Instantiate(GreenParticles, particlePosition, Quaternion.identity), 5);
                break;
            case SpawnColor.Blue:
                Destroy(Instantiate(BlueParticles, particlePosition, Quaternion.identity), 5);
                break;
        }
    }

    public void OnDestroy()
    {
        LevelManager.DestroyCallback(this);
    }
}
