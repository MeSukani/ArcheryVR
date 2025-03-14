using UnityEngine;
using TMPro;

public class ScorePopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private float floatSpeed = 0.5f; 
    [SerializeField] private float displayDuration = 1f; 
    [SerializeField] private float fadeDuration = 1f; 

    private CanvasGroup canvasGroup;
    private float timer = 0f;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 1f;
    }

    public void SetScore(int points)
    {
        scoreText.text = "+" + points;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // Float upward
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Start fading after displayDuration
        if (timer >= displayDuration)
        {
            StartCoroutine(FadeOut());
        }
    }

    private System.Collections.IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration);
            canvasGroup.alpha = alpha;
            yield return null;
        }

        Destroy(gameObject);
    }
}