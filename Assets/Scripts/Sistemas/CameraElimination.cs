using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;


public class CameraElimination : MonoBehaviour
{
    [Header("Ajustes da Câmera 2D")]
    public Vector2 offset = new Vector2(0f, 1.5f);
    public float smoothTime = 0.15f;

    [Header("Eliminação")]
    public float timeOutsideCameraToEliminate = 5f;

    private List<CarLapCounter> cars;
    private Transform leader;
    private Vector3 velocity = Vector3.zero;
    private float cameraZ;

    public static event Action<CarLapCounter> OnCarEliminated;


    [SerializeField] private CameraLimits cameraLimits;

    // tempo fora da câmera por carro
    private Dictionary<CarLapCounter, float> outsideTimer = new Dictionary<CarLapCounter, float>();

    void Start()
    {
        cars = FindObjectsOfType<CarLapCounter>().ToList();
        cameraZ = transform.position.z;

        // inicializa timers
        foreach (var car in cars)
        {
            outsideTimer[car] = 0f;
        }
    }

    void LateUpdate()
    {
        if (cars == null || cars.Count == 0) return;

        // remove carros destruídos
        cars = cars.Where(c => c != null).ToList();

        // líder = 1º colocado
        var leaderCar = cars.OrderBy(c => c.GetCarPosition()).FirstOrDefault();
        if (leaderCar == null) return;

        leader = leaderCar.transform;

        Vector3 desired = new Vector3(
            leader.position.x + offset.x,
            leader.position.y + offset.y,
            cameraZ
        );

        Vector3 desiredPosition = Vector3.SmoothDamp(
            transform.position,
            desired,
            ref velocity,
            smoothTime
        );

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

        HandleElimination();
    }

    void HandleElimination()
    {
        Camera cam = Camera.main;

        foreach (var car in cars.ToList())
        {
            if (car == null) continue;

            Vector3 viewportPos = cam.WorldToViewportPoint(car.transform.position);

            bool isOutside =
                viewportPos.x < 0f || viewportPos.x > 1f ||
                viewportPos.y < 0f || viewportPos.y > 1f;

            if (isOutside)
            {
                outsideTimer[car] += Time.deltaTime;

                if (outsideTimer[car] >= timeOutsideCameraToEliminate)
                {
                    EliminateCar(car);
                }
            }
            else
            {
                outsideTimer[car] = 0f; // voltou para a câmera
            }
        }
    }

    void EliminateCar(CarLapCounter car)
{
    Debug.Log($"🚨 {car.gameObject.name} foi eliminado!");

    cars.Remove(car);
    outsideTimer.Remove(car);

    // dispara evento
    OnCarEliminated?.Invoke(car);

    car.gameObject.SetActive(false);
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
