using UnityEngine;
using System.Collections;

public class TestDelete : MonoBehaviour {

    private LevelManager levelManager;

    public void SetLevelManager(LevelManager lvlManager)
    {
        levelManager = lvlManager;
    }

	void Start()
    {
        Destroy(this.gameObject, 1);
    }

    public void OnDestroy()
    {
        levelManager.DestroyCallback(this);
    }
}
