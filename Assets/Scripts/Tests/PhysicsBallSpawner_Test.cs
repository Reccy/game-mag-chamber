using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PhysicsBallSpawner_Test : MonoBehaviour {

    public GameObject ball;
    Text framerateText;

    void Awake()
    {
        framerateText = GameObject.Find("FramerateText").GetComponent<Text>();
    }

	void Update()
    {
        framerateText.text = "FPS: " + Mathf.Round(1f / Time.deltaTime).ToString();
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("debug");
        }
	}

    void FixedUpdate()
    {
        Instantiate(ball, new Vector3(Random.Range(-8, 8), transform.position.y, transform.position.z), Quaternion.identity);
    }
}
