using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    [SerializeField] private float minX = -3f;
    [SerializeField] private float maxX = 3f;
    [SerializeField] private float moveSpeed = 1f;

    private Vector3 startPosition;
    private float targetX;

    private void Start()
    {
        startPosition = transform.position;
        targetX = Random.Range(minX, maxX);
    }

    private void Update()
    {
        Vector3 targetPosition = startPosition + new Vector3(targetX, 0, 0);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            targetX = Random.Range(minX, maxX);
        }
    }
}