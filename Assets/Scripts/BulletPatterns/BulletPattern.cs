using UnityEngine;
using System.Collections;

public class BulletPattern : MonoBehaviour
{

    private LevelManager levelManager;

    public void SetLevelManager(LevelManager lvlManager)
    {
        levelManager = lvlManager;
    }

    public void OnDestroy()
    {
        levelManager.DestroyCallback(this);
    }
}
