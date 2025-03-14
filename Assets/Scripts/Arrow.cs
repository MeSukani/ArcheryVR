using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 10f;
    public Transform tip;
    public AudioClip hitSound;
    public GameObject scorePopup;

    private Rigidbody _rb;
    private bool _inAir = false;
    private Vector3 _lastPosition = Vector3.zero;
    private ParticleSystem _particleSystem;
    private TrailRenderer _trailRenderer;
    private AudioSource _hitSound;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        _trailRenderer = GetComponentInChildren<TrailRenderer>();
        _hitSound = gameObject.AddComponent<AudioSource>();
        _hitSound.clip = hitSound;
        PullInteraction.PullActionReleased += Release;

        Stop();
    }

    private void OnDestroy()
    {
        PullInteraction.PullActionReleased -= Release;
    }

    private void Release(float value)
    {
        PullInteraction.PullActionReleased -= Release;
        gameObject.transform.parent = null;
        _inAir = true;
        SetPhysics(true);

        Vector3 force = transform.forward * value * speed;
        _rb.AddForce(force, ForceMode.Impulse);

        StartCoroutine(RotateWithVelocity());

        _lastPosition = tip.position;
        _particleSystem.Play();
        _trailRenderer.emitting = true;
    }

    private IEnumerator RotateWithVelocity()
    {
        yield return new WaitForFixedUpdate();
        while (_inAir)
        {
            Quaternion newRotation = Quaternion.LookRotation(_rb.velocity, transform.up);
            transform.rotation = newRotation;
            yield return null;
        }
    }

    void FixedUpdate()
    {
        if (_inAir)
        {
            CheckCollision();
            _lastPosition = tip.position;
        }
    }

    private void CheckCollision()
    {
        if (Physics.Linecast(_lastPosition, tip.position, out RaycastHit hitInfo))
        {
            if (hitInfo.transform.gameObject.layer != 8)
            {
                if (hitInfo.transform.CompareTag("Target"))
                {
                    float distanceFromCenter = Vector3.Distance(hitInfo.point, hitInfo.transform.position);
                    int points = CalculatePoints(distanceFromCenter);
                    ScoreManager.Instance.AddScore(points);

                    Vector3 popupPosition = hitInfo.transform.position + Vector3.up * 0.5f;
                    GameObject popup = Instantiate(this.scorePopup, popupPosition, Quaternion.identity);
                    ScorePopup scorePopup = popup.GetComponent<ScorePopup>();
                    scorePopup.SetScore(points);

                    StartCoroutine(PlayHitSoundAndDestroy());
                }
                else
                {
                    Stop();
                    Destroy(gameObject);
                }
            }
        }
    }

    private int CalculatePoints(float distanceFromCenter)
    {
        if (distanceFromCenter < 0.2f) return 100;
        if (distanceFromCenter < 0.5f) return 50;
        return 25;
    }

    private void Stop()
    {
        _inAir = false;
        SetPhysics(false);
        _particleSystem.Stop();
        _trailRenderer.emitting = false;
    }

    private void SetPhysics(bool usePhysics)
    {
        _rb.useGravity = usePhysics;
        _rb.isKinematic = !usePhysics;
    }

    private IEnumerator PlayHitSoundAndDestroy()
    {
        if (_hitSound != null && _hitSound.clip != null && !_hitSound.isPlaying)
        {
            _hitSound.Play();
            yield return new WaitForSeconds(_hitSound.clip.length);
        }
        Destroy(gameObject);
    }
}