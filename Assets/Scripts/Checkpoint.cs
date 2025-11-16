using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    [Tooltip("Índice do checkpoint. Deve ser único e variar de 0 a N-1")]
    public int checkpointIndex = 0;

    [Tooltip("Marque true apenas no checkpoint que representa a linha de chegada")]
    public bool isFinishLine = false;

    void Reset()
    {
        // garante que o collider esteja como trigger no Editor
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }
}
