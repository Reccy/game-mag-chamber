using UnityEngine;
using System.Collections;

public class StartLevelFromGUI : MonoBehaviour {

	public void StartLevel()
    {
        Object.FindObjectOfType<LevelManager>().StartLevel();
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().ForceJump(this);
    }
}
