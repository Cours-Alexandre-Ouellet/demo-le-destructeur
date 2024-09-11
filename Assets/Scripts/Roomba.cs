using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Contr�le les d�placements automatiques de la Roomba vers les d�bris
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]        // Oblige le GameObject � avoir un NavMeshAgent
public class Roomba : MonoBehaviour
{
    /// <summary>
    /// Liste des morceaux � ramasser
    /// </summary>
    private Queue<Transform> morceauxCasses;

    /// <summary>
    /// Destination actuelle de la Roomba
    /// </summary>
    private Vector3 destination;

    /// <summary>
    /// R�f�rence vers le component agent
    /// </summary>
    private NavMeshAgent agent;

    /// <summary>
    /// Mode dans lequel la roomba se trouve
    /// </summary>
    private bool modeRassagePot;

    private void Awake()
    {
        morceauxCasses = new();
        modeRassagePot = false;
    }

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// Ajoute des morceaux � ramasser � la Roomba.
    /// </summary>
    /// <param name="morceaux">La liste des morceaux � ramasser.</param>
    public void RamasserMorceaux(Transform[] morceaux)
    {
        foreach(Transform morceau in morceaux)
        {
            morceauxCasses.Enqueue(morceau);
        }
        
        if(!modeRassagePot)
        {
            AffecterDestination();
        }
    }

    public void OnTriggerEnter(Collider autreObjet)
    {
        if(autreObjet.gameObject.CompareTag("MorceauCasse"))
        {
            Destroy(autreObjet.gameObject); 
        }
    }

    /// <summary>
    /// G�n�re une distination al�atoire
    /// </summary>
    /// <returns>Un vecteur de position al�atoire sur le plan</returns>
    private Vector3 GenererDestinationAlea()
    {
        modeRassagePot = false;     // Mode al�atoire
        return new Vector3(Random.Range(-2.5f, 12.5f), 0.0f, Random.Range(-2.5f, 12.5f));
    }

    /// <summary>
    /// Affecte la destination de la roomba selon la pr�sence ou non de morceaux � ramasser
    /// </summary>
    private void AffecterDestination()
    {
        // Il y a des morceaux � ramasser
        if(morceauxCasses.TryDequeue(out Transform morceau))
        {
            modeRassagePot = true; 

            // Le morceau a d�j� �t� supprim�, donc on relance la m�thode
            if(morceau == null)
            {
                AffecterDestination();
                return;
            }
            else
            {
                // La destination est valide, on l'affecte
                destination = morceau.position;
            }
        }
        else
        {
            // La destination est mise � jour
            destination = GenererDestinationAlea();
        }

        // D�clenche le calcul d'un nouveau chemin
        agent.destination = destination;        
    }

    // Version it�rative
    /// <summary>
    /// Affecte la destination de la roomba selon la pr�sence ou non de morceaux � ramasser
    /// </summary>
    /*private void AffecterDestination()
    {
        // Il y a des morceaux � ramasser
        while(morceauxCasses.TryDequeue(out Transform morceau))
        {
            modeRassagePot = true;

            // Le morceau a d�j� �t� supprim�, donc on relance la m�thode
            if(morceau == null)
            {
                continue;
            }

            // La destination est valide, on l'affecte
            destination = morceau.position;
        }
        
        // Tous les morceaux sont ramass�s
        if(morceauxCasses.Count == 0) { 
            // La destination est mise � jour
            destination = GenererDestinationAlea();
        }

        // D�clenche le calcul d'un nouveau chemin
        agent.destination = destination;
    }*/


    /// <summary>
    /// Mise � jour de la Roomba
    /// </summary>
    void Update()
    {
        if(!agent.pathPending)      // Si un chemin n'est pas en cours de calcul
        {
            // Distance restante
            float distanceDest = Vector3.SqrMagnitude(destination - transform.position);
            
            // Si la distance restante est nulle, que la vitesse est nulle ou qu'aucun chemin n'a �t� assign�,
            // On g�n�re une nouvelle destination
            if(Mathf.Approximately(agent.velocity.sqrMagnitude, 0.0f) ||
                Mathf.Approximately(distanceDest, 0.0f) || !agent.hasPath)
            {
                AffecterDestination();
            }
        }
    }
}
