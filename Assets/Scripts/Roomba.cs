using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

/// <summary>
/// Contrôle les déplacements automatiques de la Roomba vers les débris
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]        // Oblige le GameObject à avoir un NavMeshAgent
public class Roomba : MonoBehaviour
{
    /// <summary>
    /// Événement lorsque la roomba change d'état
    /// </summary>
    public UnityEvent<bool> onChangementComportement;

    /// <summary>
    /// Liste des morceaux à ramasser
    /// </summary>
    private Queue<Transform> morceauxCasses;

    public bool PossedeMorceauARamasser => morceauxCasses.Count > 0;

    /// <summary>
    /// Référence vers le component agent
    /// </summary>
    private NavMeshAgent agent;

    /// <summary>
    /// Nombre de pots qui ont été cassés depuis le début du jeu
    /// </summary>
    private int nombrePotsCasses;
    public int NombrePotsCasses => nombrePotsCasses;

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

    private EtatRoomba etat;

    private EtatRoomba etatPrecedent;

    /// <summary>
    /// Destination actuelle de la Roomba
    /// </summary>
    public Vector3? Destination { get; set; }

    private void Awake()
    {
        morceauxCasses = new();
        nombrePotsCasses = 0;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        materiauxNormal = GetComponent<MeshRenderer>().materials;
        etat = new EtatAttente();
        etatPrecedent = null;
    }

    private void Update()
    {
        if(etat != etatPrecedent)
        {
            etat.OnCommencer(this);
        }

        etatPrecedent = etat;
        etat = etat.OnExecuter(this);

        if (etatPrecedent != etat)
        {
            etatPrecedent.OnSortie(this);
        }
    }

    public void DeplacerVers(Vector3 destination)
    {
        Destination = destination;
        agent.destination = destination;
    }

    public Transform GetProchainePiece()
    {
        return morceauxCasses.Dequeue();
    }

    public void RamasserMorceau(Transform morceau)
    {
        morceauxCasses.Enqueue(morceau);
    }

    public void DeplacerVersTRex()
    {
        DeplacerVers(trex.transform.position);
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
    }

    public void OnTriggerEnter(Collider autreObjet)
    {
        if(autreObjet.gameObject.CompareTag("MorceauCasse"))
        {
            Destroy(autreObjet.gameObject); 
        }
    }

    /// <summary>
    /// Change la façon d'afficher la roomba pour afficher les couteaux et afficher la 
    /// lumière agressive.
    /// </summary>
    public void TransformerEnAgressive()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.materials = materiauxAgressif;

        couteauDroit.SetActive(true);
        couteauGauche.SetActive(true);

        // Déclenche la réaction au mode agressif
        onChangementComportement?.Invoke(true);
    }

    /// <summary>
    /// Change la façon d'afficher la roomba pour masquer les couteaux et afficher la 
    /// lumière normale.
    /// </summary>
    public void TransformerEnNormale()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.materials = materiauxNormal;

        couteauDroit.SetActive(false);
        couteauGauche.SetActive(false);

        // Déclenche la réaction au mode agressif
        onChangementComportement?.Invoke(false);
    }
}
