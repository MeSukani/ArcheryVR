using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BowTooltip : MonoBehaviour
{
    [SerializeField] private GameObject tooltipObject;
    [SerializeField] private Canvas tooltipCanvas;
    [SerializeField] private XRGrabInteractable bowInteractable;
    [SerializeField] private float fadeDuration = 1f;

    private bool isFading = false;

    private void Update()
    {
        if (bowInteractable.isSelected && !isFading)
        {
            StartCoroutine(FadeOut());
        }
    }

    private System.Collections.IEnumerator FadeOut()
    {
        isFading = true;
        float elapsedTime = 0f;
        float startAlpha = tooltipCanvas.GetComponent<CanvasGroup>().alpha;

        CanvasGroup canvasGroup = tooltipCanvas.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = tooltipCanvas.gameObject.AddComponent<CanvasGroup>();
        }

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration);
            canvasGroup.alpha = alpha;
            yield return null;
        }

        Destroy(tooltipObject);
    }
}