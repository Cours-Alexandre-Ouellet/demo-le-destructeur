using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

/// <summary>
/// Contr�le les d�placements automatiques de la Roomba vers les d�bris
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]        // Oblige le GameObject � avoir un NavMeshAgent
public class Roomba : MonoBehaviour
{
    /// <summary>
    /// �v�nement lorsque la roomba change d'�tat
    /// </summary>
    public UnityEvent<bool> onChangementComportement;

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

    /// <summary>
    /// Nombre de pots qui ont �t� cass�s depuis le d�but du jeu
    /// </summary>
    private int nombrePotsCasses;

    /// <summary>
    /// Mode dans lequel la roomba est agressive
    /// </summary>
    private bool modeAgressif;

    /// <summary>
    /// Liste de mat�riaux utilis�s dans la roomba agressive
    /// </summary>
    [SerializeField]
    private Material[] materiauxAgressif;

    /// <summary>
    /// Liste de materiaux utilis�s dans la roomba normale
    /// </summary>
    private Material[] materiauxNormal;

    /// <summary>
    /// Couteau � droite de la roomba
    /// </summary>
    [SerializeField]
    private GameObject couteauDroit;

    /// <summary>
    /// Couteau � gauche de la roomba
    /// </summary>
    [SerializeField]
    private GameObject couteauGauche;

    /// <summary>
    /// TRex qui se d�place dans le jeu
    /// </summary>
    [SerializeField]
    private ControleurTRex trex;

    /// <summary>
    /// Temps pour lequel l'agressivit� se d�roule
    /// </summary>
    [SerializeField]
    private float tempsAgressivite;

    /// <summary>
    /// Temps de rafraichissement du chemin cible pour 
    /// que la roomba cible le TRex.
    /// </summary>
    [SerializeField]
    private float tempsRafraichissementPoursuite;

    private Coroutine gestionModeAgressif;

    private float tempsEnModeAgressif;

    private void Awake()
    {
        morceauxCasses = new();
        modeRassagePot = false;
        nombrePotsCasses = 0;
        modeAgressif = false;
        gestionModeAgressif = null;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        materiauxNormal = GetComponent<MeshRenderer>().materials;
    }

    /// <summary>
    /// Ajoute des morceaux � ramasser � la Roomba.
    /// </summary>
    /// <param name="morceaux">La liste des morceaux � ramasser.</param>
    public void RamasserMorceaux(Transform[] morceaux)
    {
        nombrePotsCasses++;

        foreach(Transform morceau in morceaux)
        {
            morceauxCasses.Enqueue(morceau);
        }
        
        // Si elle est d�j� agressive 
        if(modeAgressif)
        {
            tempsEnModeAgressif = 0.0f;
        }

        // Condition pour rendre la roomba agressif
        if(nombrePotsCasses % 2 == 0 && !modeAgressif) 
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
        if(modeAgressif)
        {
            destination = trex.transform.position;

            if (gestionModeAgressif == null)
            {
                gestionModeAgressif = StartCoroutine(GererModeAggressif());
            }
            else
            {

            }
        }

        // Il y a des morceaux � ramasser
        else if(morceauxCasses.TryDequeue(out Transform morceau))
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

    /// <summary>
    /// Permet de g�rer le temps dans le mode agressif. Si le mode expire, alors 
    /// la coroutine s'arr�te.
    /// </summary>
    /// <returns>L'�num�rateur de la coroutine.</returns>
    private IEnumerator GererModeAggressif()
    {
        tempsEnModeAgressif = 0.0f;
        float tempsMiseAJourChemin = 0.0f;

        // Attend le temps d'agressivit�
        while(tempsEnModeAgressif < tempsAgressivite)
        {
            tempsEnModeAgressif += Time.deltaTime;
            tempsMiseAJourChemin += Time.deltaTime;

            // Permet de courir apr�s le dinosaure
            if(tempsMiseAJourChemin > tempsRafraichissementPoursuite)
            {
                agent.destination = trex.transform.position;
                tempsMiseAJourChemin = 0.0f;
            }

            // Attend la prochaine frame avant de continuer l'ex�cution de la boucle de jeu
            yield return null;
        }

        modeAgressif = false;
        TransformerEnNormale();
        gestionModeAgressif = null;
    }

    /// <summary>
    /// Change la fa�on d'afficher la roomba pour afficher les couteaux et afficher la 
    /// lumi�re agressive.
    /// </summary>
    private void TransformerEnAgressive()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.materials = materiauxAgressif;

        couteauDroit.SetActive(true);
        couteauGauche.SetActive(true);

        // D�clenche la r�action au mode agressif
        onChangementComportement?.Invoke(true);

        // Rafra�chissement du chemin
        AffecterDestination();
    }

    /// <summary>
    /// Change la fa�on d'afficher la roomba pour masquer les couteaux et afficher la 
    /// lumi�re normale.
    /// </summary>
    private void TransformerEnNormale()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.materials = materiauxNormal;

        couteauDroit.SetActive(false);
        couteauGauche.SetActive(false);

        // D�clenche la r�action au mode agressif
        onChangementComportement?.Invoke(false);

        // Recalcule imm�diatement la destination
        AffecterDestination();
    }
}
