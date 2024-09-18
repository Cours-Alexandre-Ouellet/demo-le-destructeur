using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Contrôle les déplacements automatiques de la Roomba vers les débris
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]        // Oblige le GameObject à avoir un NavMeshAgent
public class Roomba : MonoBehaviour
{
    /// <summary>
    /// Liste des morceaux à ramasser
    /// </summary>
    private Queue<Transform> morceauxCasses;

    /// <summary>
    /// Destination actuelle de la Roomba
    /// </summary>
    private Vector3 destination;

    /// <summary>
    /// Référence vers le component agent
    /// </summary>
    private NavMeshAgent agent;

    /// <summary>
    /// Mode dans lequel la roomba se trouve
    /// </summary>
    private bool modeRassagePot;

    /// <summary>
    /// Nombre de pots qui ont été cassés depuis le début du jeu
    /// </summary>
    private int nombrePotsCasses;

    /// <summary>
    /// Mode dans lequel la roomba est agressive
    /// </summary>
    private bool modeAgressif;

    /// <summary>
    /// Liste de matériaux utilisés dans la roomba agressive
    /// </summary>
    [SerializeField]
    private Material[] materiauxAgressif;

    /// <summary>
    /// Liste de materiaux utilisés dans la roomba normale
    /// </summary>
    private Material[] materiauxNormal;

    /// <summary>
    /// Couteau à droite de la roomba
    /// </summary>
    [SerializeField]
    private GameObject couteauDroit;

    /// <summary>
    /// Couteau à gauche de la roomba
    /// </summary>
    [SerializeField]
    private GameObject couteauGauche;

    /// <summary>
    /// TRex qui se déplace dans le jeu
    /// </summary>
    [SerializeField]
    private ControleurTRex trex;

    /// <summary>
    /// Temps pour lequel l'agressivité se déroule
    /// </summary>
    [SerializeField]
    private float tempsAgressivite;

    /// <summary>
    /// Temps de rafraichissement du chemin cible pour 
    /// que la roomba cible le TRex.
    /// </summary>
    [SerializeField]
    private float tempsRafraichissementPoursuite;

    private void Awake()
    {
        morceauxCasses = new();
        modeRassagePot = false;
        nombrePotsCasses = 0;
        modeAgressif = false;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        materiauxNormal = GetComponent<MeshRenderer>().materials;
    }

    /// <summary>
    /// Ajoute des morceaux à ramasser à la Roomba.
    /// </summary>
    /// <param name="morceaux">La liste des morceaux à ramasser.</param>
    public void RamasserMorceaux(Transform[] morceaux)
    {
        nombrePotsCasses++;

        foreach(Transform morceau in morceaux)
        {
            morceauxCasses.Enqueue(morceau);
        }
        
        // Condition pour rendre la roomba agressif
        if(nombrePotsCasses % 2 == 0) 
        {
            modeAgressif = true;
            TransformerEnAgressive();
        }
        else if(!modeRassagePot && !modeAgressif)
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
    /// Génère une distination aléatoire
    /// </summary>
    /// <returns>Un vecteur de position aléatoire sur le plan</returns>
    private Vector3 GenererDestinationAlea()
    {
        modeRassagePot = false;     // Mode aléatoire
        return new Vector3(Random.Range(-2.5f, 12.5f), 0.0f, Random.Range(-2.5f, 12.5f));
    }

    /// <summary>
    /// Affecte la destination de la roomba selon la présence ou non de morceaux à ramasser
    /// </summary>
    private void AffecterDestination()
    {
        if(modeAgressif)
        {
            destination = trex.transform.position;
            StartCoroutine(GererModeAggressif());
        }

        // Il y a des morceaux à ramasser
        else if(morceauxCasses.TryDequeue(out Transform morceau))
        {
            modeRassagePot = true; 

            // Le morceau a déjà été supprimé, donc on relance la méthode
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
            // La destination est mise à jour
            destination = GenererDestinationAlea();
        }

        // Déclenche le calcul d'un nouveau chemin
        agent.destination = destination;        
    }

    // Version itérative
    /// <summary>
    /// Affecte la destination de la roomba selon la présence ou non de morceaux à ramasser
    /// </summary>
    /*private void AffecterDestination()
    {
        // Il y a des morceaux à ramasser
        while(morceauxCasses.TryDequeue(out Transform morceau))
        {
            modeRassagePot = true;

            // Le morceau a déjà été supprimé, donc on relance la méthode
            if(morceau == null)
            {
                continue;
            }

            // La destination est valide, on l'affecte
            destination = morceau.position;
        }
        
        // Tous les morceaux sont ramassés
        if(morceauxCasses.Count == 0) { 
            // La destination est mise à jour
            destination = GenererDestinationAlea();
        }

        // Déclenche le calcul d'un nouveau chemin
        agent.destination = destination;
    }*/


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
                AffecterDestination();
            }
        }
    }

    /// <summary>
    /// Permet de gérer le temps dans le mode agressif. Si le mode expire, alors 
    /// la coroutine s'arrête.
    /// </summary>
    /// <returns>L'énumérateur de la coroutine.</returns>
    private IEnumerator GererModeAggressif()
    {
        float tempsEnModeAgressif = 0.0f;
        float tempsMiseAJourChemin = 0.0f;

        // Attend le temps d'agressivité
        while(tempsEnModeAgressif < tempsAgressivite)
        {
            tempsEnModeAgressif += Time.deltaTime;
            tempsMiseAJourChemin += Time.deltaTime;

            // Permet de courir après le dinosaure
            if(tempsMiseAJourChemin > tempsRafraichissementPoursuite)
            {
                agent.destination = trex.transform.position;
                tempsMiseAJourChemin = 0.0f;
            }

            // Attend la prochaine frame avant de continuer l'exécution de la boucle de jeu
            yield return null;
        }

        modeAgressif = false;
        TransformerEnNormale();
    }

    /// <summary>
    /// Change la façon d'afficher la roomba pour afficher les couteaux et afficher la 
    /// lumière agressive.
    /// </summary>
    private void TransformerEnAgressive()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.materials = materiauxAgressif;

        couteauDroit.SetActive(true);
        couteauGauche.SetActive(true);
    }

    /// <summary>
    /// Change la façon d'afficher la roomba pour masquer les couteaux et afficher la 
    /// lumière normale.
    /// </summary>
    private void TransformerEnNormale()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.materials = materiauxNormal;

        couteauDroit.SetActive(false);
        couteauGauche.SetActive(false);
    }
}
