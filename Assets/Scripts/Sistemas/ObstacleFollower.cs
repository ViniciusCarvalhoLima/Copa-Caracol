using UnityEngine;

public class ObstacleFollower : MonoBehaviour
{
    [Header("Caminho")]
    [SerializeField] private Transform[] waypoints;

    [Header("Movimento")]
    public float speed = 5f;
    public bool loop = true;
    public float reachDistance = 0.1f;

    private int currentIndex = 0;
    private float fixedZ; // Z fixo do obstáculo

    void Start()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("ObstacleFollower: Nenhum waypoint definido.");
            enabled = false;
            return;
        }

        // salva o Z original do obstáculo
        fixedZ = transform.position.z;

        // posiciona no primeiro waypoint, mantendo o Z
        transform.position = new Vector3(
            waypoints[0].position.x,
            waypoints[0].position.y,
            fixedZ
        );
    }

    void Update()
    {
        MoveAlongPath();
    }

    void MoveAlongPath()
    {
        Transform target = waypoints[currentIndex];

        // alvo com Z fixo
        Vector3 targetPos = new Vector3(
            target.position.x,
            target.position.y,
            fixedZ
        );

        Vector3 direction = (targetPos - transform.position).normalized;

        transform.position += direction * speed * Time.deltaTime;

        // garante Z fixo (segurança extra)
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            fixedZ
        );

        // rotação apenas no eixo Z (2D)
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        if (Vector2.Distance(transform.position, targetPos) <= reachDistance)
        {
            currentIndex++;

            if (currentIndex >= waypoints.Length)
            {
                currentIndex = loop ? 0 : waypoints.Length - 1;
            }
        }
    }
}
