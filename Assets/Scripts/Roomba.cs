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

    public bool PossedeMorceauARamasser => morceauxCasses.Count > 0;

    /// <summary>
    /// R�f�rence vers le component agent
    /// </summary>
    private NavMeshAgent agent;

    /// <summary>
    /// Nombre de pots qui ont �t� cass�s depuis le d�but du jeu
    /// </summary>
    private int nombrePotsCasses;
    public int NombrePotsCasses => nombrePotsCasses;

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
    }

    public void OnTriggerEnter(Collider autreObjet)
    {
        if(autreObjet.gameObject.CompareTag("MorceauCasse"))
        {
            Destroy(autreObjet.gameObject); 
        }
    }

    /// <summary>
    /// Change la fa�on d'afficher la roomba pour afficher les couteaux et afficher la 
    /// lumi�re agressive.
    /// </summary>
    public void TransformerEnAgressive()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.materials = materiauxAgressif;

        couteauDroit.SetActive(true);
        couteauGauche.SetActive(true);

        // D�clenche la r�action au mode agressif
        onChangementComportement?.Invoke(true);
    }

    /// <summary>
    /// Change la fa�on d'afficher la roomba pour masquer les couteaux et afficher la 
    /// lumi�re normale.
    /// </summary>
    public void TransformerEnNormale()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.materials = materiauxNormal;

        couteauDroit.SetActive(false);
        couteauGauche.SetActive(false);

        // D�clenche la r�action au mode agressif
        onChangementComportement?.Invoke(false);
    }
}
