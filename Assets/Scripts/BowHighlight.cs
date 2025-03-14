using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BowHighlight : MonoBehaviour
{
    [SerializeField] private ParticleSystem highlightEffect;
    [SerializeField] private XRGrabInteractable bowInteractable;

    private void Start()
    {
        highlightEffect.Play();
    }

    private void Update()
    {
        if (bowInteractable.isSelected && highlightEffect.isPlaying)
        {
            highlightEffect.Stop();
        }
    }
}