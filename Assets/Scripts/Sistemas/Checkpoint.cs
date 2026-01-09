using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    // Índice do checkpoint deve ser único e variar de 0 a N
    public int checkpointIndex = 0;

    public bool isFinishLine = false;

    void Reset()
    {
        // garante que o collider esteja como trigger no Editor
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }
}
