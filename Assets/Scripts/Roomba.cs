using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Contrôle les déplacements automatiques de la Roomba vers les débris
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]        // Oblige le GameObject à avoir un NavMeshAgent
public class Roomba : MonoBehaviour
{
    /// <summary>
    /// Destination actuelle de la Roomba
    /// </summary>
    private Vector3 destination;

    /// <summary>
    /// Référence vers le component agent
    /// </summary>
    private NavMeshAgent agent;

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// Génère une distination aléatoire
    /// </summary>
    /// <returns>Un vecteur de position aléatoire sur le plan</returns>
    private Vector3 GenererDestinationAlea()
    {
        return new Vector3(Random.Range(-2.5f, 12.5f), 0.0f, Random.Range(-2.5f, 12.5f));
    }

    /// <summary>
    /// Mise à jour de la Roomba
    /// </summary>
    void Update()
    {
        if(!agent.pathPending)      // Si un chemin n'est pas en cours de calcul
        {
            // Distance restante
            float distanceDest = Vector3.SqrMagnitude(destination - transform.position);
            
            // Si la distance restante est nulle, que la vitesse est nulle ou qu'aucun chemin n'a été assigné,
            // On génère une nouvelle destination
            if(Mathf.Approximately(agent.velocity.sqrMagnitude, 0.0f) ||
                Mathf.Approximately(distanceDest, 0.0f) || !agent.hasPath)
            {
                destination = GenererDestinationAlea();
                agent.destination = destination;        // Déclenche le calcul d'un nouveau chemin
            }
        }
    }
}
