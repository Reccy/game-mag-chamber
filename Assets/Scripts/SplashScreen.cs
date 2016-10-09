using UnityEngine;
using System.Collections;

public class SplashScreen : MonoBehaviour
{
    public GameObject logo, credit;
    public float animationDuration = 0.2f;
    RectTransform logoRect, creditRect;
    GameManager gameManager;

    void Awake()
    {
        gameManager = Object.FindObjectOfType<GameManager>();
        logoRect = logo.GetComponent<RectTransform>();
        creditRect = credit.GetComponent<RectTransform>();
    }

    void Start()
    {
        logoRect.anchoredPosition = new Vector2(1000, 0);
        creditRect.anchoredPosition = new Vector2(-1000, 0);
    }

    void FixedUpdate()
    {
        float v1 = 0, v2 = 0;
        logoRect.anchoredPosition = new Vector2(Mathf.SmoothDamp(logoRect.anchoredPosition.x, 0, ref v1, animationDuration), 0);
        creditRect.anchoredPosition = new Vector2(Mathf.SmoothDamp(creditRect.anchoredPosition.x, 0, ref v2, animationDuration), -220);
    }

    void Update()
    {
        if (Input.anyKeyDown || Time.timeSinceLevelLoad > 5)
            gameManager.LoadScene("Level1");
    }
}
