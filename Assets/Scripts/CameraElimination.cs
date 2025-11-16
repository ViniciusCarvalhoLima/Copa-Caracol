using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class CameraElimination : MonoBehaviour
{
    [Header("Ajustes da Câmera 2D")]
    public Vector2 offset = new Vector2(0f, 1.5f);   // apenas X e Y
    public float smoothTime = 0.15f;                 // suavização

    private List<CarLapCounter> cars;
    private Transform leader;
    private Vector3 velocity = Vector3.zero;
    private float cameraZ; // z fixo da câmera

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
        transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
    }
}
