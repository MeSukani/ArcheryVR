using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PullInteraction : XRBaseInteractable
{
    public static event Action<float> PullActionReleased;

    public Transform start, end;
    public GameObject notch;
    public float pullAmount { get; private set; } = 0f;

    [SerializeField] private AudioSource releaseSound;
    [SerializeField] private AudioSource pullSound;

    private LineRenderer _lineRenderer;
    private IXRSelectInteractor _pullingInteractor = null;
    private bool _wasPulling = false;

    protected override void Awake()
    {
        base.Awake();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        _pullingInteractor = args.interactorObject;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        _pullingInteractor = null;
        pullAmount = 0f;
        UpdateString();
        StopPullSound();
    }

    public void Release()
    {
        PullActionReleased?.Invoke(pullAmount);
        _pullingInteractor = null;
        pullAmount = 0f;
        notch.transform.localPosition = new Vector3(notch.transform.localPosition.x, notch.transform.localPosition.y, 0f);
        UpdateString();
        PlayReleaseSound();
        StopPullSound();
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);
        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            if (isSelected && _pullingInteractor != null)
            {
                Vector3 pullPosition = _pullingInteractor.transform.position;
                pullAmount = CalculatePull(pullPosition);
                UpdateString();
                UpdatePullSound();
            }
            else if (_wasPulling)
            {
                StopPullSound();
            }
        }
    }

    private float CalculatePull(Vector3 pullPosition)
    {
        Vector3 pullDirection = pullPosition - start.position;
        Vector3 targetDirection = end.position - start.position;
        float maxLength = targetDirection.magnitude;

        targetDirection.Normalize();
        float pullValue = Vector3.Dot(pullDirection, targetDirection) / maxLength;
        return Mathf.Clamp(pullValue, 0, 5);
    }

    private void UpdateString()
    {
        Vector3 linePosition = Vector3.forward * Mathf.Lerp(start.transform.localPosition.z, end.transform.localPosition.z, pullAmount);
        notch.transform.localPosition = new Vector3(notch.transform.localPosition.x, notch.transform.localPosition.y, linePosition.z + 0.17f);
        _lineRenderer.SetPosition(1, linePosition);
    }

    private void HapticFeedback()
    {
        if (_pullingInteractor != null)
        {
            ActionBasedController currentController = _pullingInteractor.transform.gameObject.GetComponent<ActionBasedController>();
            currentController.SendHapticImpulse(pullAmount, 0.1f);
        }
    }

    private void PlayReleaseSound()
    {
        if (releaseSound != null && !releaseSound.isPlaying)
        {
            releaseSound.Play();
        }
    }

    private void UpdatePullSound()
    {
        if (pullSound != null)
        {
            if (pullAmount > 0 && !_wasPulling)
            {
                pullSound.Play();
                _wasPulling = true;
            }
            else if (pullAmount <= 0 && _wasPulling)
            {
                StopPullSound();
            }
        }
    }

    private void StopPullSound()
    {
        if (pullSound != null && pullSound.isPlaying)
        {
            pullSound.Stop();
        }
        _wasPulling = false;
    }
}