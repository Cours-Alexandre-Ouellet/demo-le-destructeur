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
    /// Événement lorsque le roomba ramasse un morceau
    /// </summary>
    public UnityEvent<Transform> onMorceauRamasse;

    /// <summary>
    /// Liste des morceaux à ramasser
    /// </summary>
    private LinkedList<Transform> morceauxCasses;

    /// <summary>
    /// Indique si le roomba a des morceaux non ramassés
    /// </summary>
    public bool PossedeMorceauARamasser => morceauxCasses.Count > 0;

    /// <summary>
    /// Référence vers le component agent
    /// </summary>
    private NavMeshAgent agent;

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
    /// État actuel du Roomba.
    /// </summary>
    private EtatRoomba etat;

    /// <summary>
    /// État du roomba à la frame précédente.
    /// </summary>
    private EtatRoomba etatPrecedent;

    /// <summary>
    /// Destination actuelle de la Roomba
    /// </summary>
    public Vector3? Destination { get; set; }

    /// <summary>
    /// Compte le nombre de pots cassés depuis le dernier mode agressif.
    /// </summary>
    public int NombrePotsCassesDepuisDernierAgressif { get; set; }

    /// <summary>
    /// Nombre de pots cassés à partir duquel le roomba vire agressif.
    /// </summary>
    [SerializeField]    
    private int seuilAgressivite;

    /// <summary>
    /// Accesseur du nombre de pots cassés à partir duquel le roomba vire agressif.
    /// </summary>
    public int SeuilAgressivite => seuilAgressivite;

    private void Awake()
    {
        morceauxCasses = new();
        NombrePotsCassesDepuisDernierAgressif = 0;
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

    /// <summary>
    /// Met à jour la destination du Roomba.
    /// </summary>
    /// <param name="destination">la position à atteindre pour le roomba.</param>
    public void DeplacerVers(Vector3 destination)
    {
        Destination = destination;
        agent.destination = destination;
    }

    /// <summary>
    /// Accesseur pour obtenir le prochain morceau à ramasser.
    /// </summary>
    /// <returns>La prochaine piece à ramasser</returns>
    public Transform GetProchainePiece()
    {
        Transform prochainePiece = null;

        while (prochainePiece == null && morceauxCasses.Count > 0)
        {
            prochainePiece = morceauxCasses.First.Value;
            morceauxCasses.RemoveFirst();
        }

        return prochainePiece;
    }

    /// <summary>
    /// Indique au Roomba qu'un morceau est à ramasser.
    /// </summary>
    /// <param name="morceau">Le morceau à ramasser</param>
    /// <param name="enTete">Indique si le morceau doit être ajouté en-tête de la liste.</param>
    public void RamasserMorceau(Transform morceau, bool enTete)
    {
        if(enTete)
        {
            morceauxCasses.AddFirst(morceau);   
        }
        else
        {
            morceauxCasses.AddLast(morceau);
        }
    }

    /// <summary>
    /// Ajoute des morceaux à ramasser à la Roomba.
    /// </summary>
    /// <param name="morceaux">La liste des morceaux à ramasser.</param>
    public void RamasserMorceaux(Transform[] morceaux)
    {
        NombrePotsCassesDepuisDernierAgressif++;

        foreach(Transform morceau in morceaux)
        {
            morceauxCasses.AddLast(morceau);
        }
    }

    /// <summary>
    /// Déplace le roomba vers la position du TRex
    /// </summary>
    public void DeplacerVersTRex()
    {
        DeplacerVers(trex.transform.position);
    }

    public void OnTriggerEnter(Collider autreObjet)
    {
        if(autreObjet.gameObject.CompareTag("MorceauCasse"))
        {
            onMorceauRamasse?.Invoke(autreObjet.transform);
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
