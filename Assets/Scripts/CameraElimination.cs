using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraElimination : MonoBehaviour
{
    [Header("Ajustes da Câmera 2D")]
    public Vector2 offset = new Vector2(0f, 1.5f);   // apenas X e Y
    public float smoothTime = 0.15f;                 // suavização

    private List<CarLapCounter> cars;
    private Transform leader;
    private Vector3 velocity = Vector3.zero;
    private float cameraZ; // z fixo da câmera

    [SerializeField] private CameraLimits cameraLimits;

    void Start()
    {
        // pega todos os carros na cena
        cars = FindObjectsOfType<CarLapCounter>().ToList();
        cameraZ = transform.position.z; // guarda z inicial (ex: -10)
    }

    void LateUpdate()
    {
        if (cars == null || cars.Count == 0) return;

        // define líder (menor posição = 1º lugar)
        var leaderCar = cars.OrderBy(c => c.GetCarPosition()).FirstOrDefault(c => c != null);
        if (leaderCar == null) return;

        leader = leaderCar.transform;

        // posição desejada somente em X e Y (mantém Z da câmera)
        Vector3 desired = new Vector3(leader.position.x + offset.x,
                                      leader.position.y + offset.y,
                                      cameraZ);

        // suaviza usando SmoothDamp (melhor para 2D que Lerp em muitos casos)
        Vector3 desiredPosition = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);

        // calcula as margens da câmera baseado no tamanho
        float cameraHeight = Camera.main.orthographicSize * 2f;
        float cameraWidth = cameraHeight * Camera.main.aspect;
        float halfWidth = cameraWidth * 0.5f;
        float halfHeight = cameraHeight * 0.5f;

        Vector3 clampedPosition = new Vector3(
            Mathf.Clamp(desiredPosition.x, cameraLimits.min.x + halfWidth, cameraLimits.max.x - halfWidth),
            Mathf.Clamp(desiredPosition.y, cameraLimits.min.y + halfHeight, cameraLimits.max.y - halfHeight),
            desiredPosition.z
        );

        transform.position = clampedPosition;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 bottomLeft = new Vector3(cameraLimits.min.x, cameraLimits.min.y, 0);
        Vector3 topRight = new Vector3(cameraLimits.max.x, cameraLimits.max.y, 0);
        Vector3 topLeft = new Vector3(cameraLimits.min.x, cameraLimits.max.y, 0);
        Vector3 bottomRight = new Vector3(cameraLimits.max.x, cameraLimits.min.y, 0);
        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
    }
}

[Serializable]
public class CameraLimits
{
    public Vector2 min;
    public Vector2 max;
}