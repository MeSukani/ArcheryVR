using System.Collections.Generic;
using UnityEngine;

public class RotateTransforms : MonoBehaviour
{
    [SerializeField] private GameObject prefabToRotate;
    [SerializeField] private int numberOfInstances = 6;
    [SerializeField] private float zSpacing = 0.5f;
    [SerializeField] private float angularSpeed = 90f;
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    [SerializeField] private float rotationOffset = 30f;

    private List<Transform> instances = new List<Transform>();
    private float currentAngle = 0f;

    private void Start()
    {
        rotationAxis = rotationAxis.normalized;

        for (int i = 0; i < numberOfInstances; i++)
        {
            float zOffset = i * zSpacing;
            Vector3 spawnPosition = transform.position + new Vector3(0, 0, zOffset);

            GameObject instance = Instantiate(prefabToRotate, spawnPosition, Quaternion.identity, transform);
            Transform instanceTransform = instance.transform;
            instances.Add(instanceTransform);

            float offsetAngle = i * rotationOffset;
            instanceTransform.rotation = Quaternion.Euler(rotationAxis * offsetAngle);
        }
    }

    private void Update()
    {
        currentAngle += angularSpeed * Time.deltaTime;

        for (int i = 0; i < instances.Count; i++)
        {
            if (instances[i] != null)
            {
                float offsetAngle = i * rotationOffset;
                float totalAngle = currentAngle + offsetAngle;
                instances[i].rotation = Quaternion.Euler(rotationAxis * totalAngle);
            }
        }
    }
}