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
    /// �v�nement lorsque le roomba ramasse un morceau
    /// </summary>
    public UnityEvent<Transform> onMorceauRamasse;

    /// <summary>
    /// Liste des morceaux � ramasser
    /// </summary>
    private LinkedList<Transform> morceauxCasses;

    /// <summary>
    /// Indique si le roomba a des morceaux non ramass�s
    /// </summary>
    public bool PossedeMorceauARamasser => morceauxCasses.Count > 0;

    /// <summary>
    /// R�f�rence vers le component agent
    /// </summary>
    private NavMeshAgent agent;

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
    /// �tat actuel du Roomba.
    /// </summary>
    private EtatRoomba etat;

    /// <summary>
    /// �tat du roomba � la frame pr�c�dente.
    /// </summary>
    private EtatRoomba etatPrecedent;

    /// <summary>
    /// Destination actuelle de la Roomba
    /// </summary>
    public Vector3? Destination { get; set; }

    /// <summary>
    /// Compte le nombre de pots cass�s depuis le dernier mode agressif.
    /// </summary>
    public int NombrePotsCassesDepuisDernierAgressif { get; set; }

    /// <summary>
    /// Nombre de pots cass�s � partir duquel le roomba vire agressif.
    /// </summary>
    [SerializeField]    
    private int seuilAgressivite;

    /// <summary>
    /// Accesseur du nombre de pots cass�s � partir duquel le roomba vire agressif.
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
    /// Met � jour la destination du Roomba.
    /// </summary>
    /// <param name="destination">la position � atteindre pour le roomba.</param>
    public void DeplacerVers(Vector3 destination)
    {
        Destination = destination;
        agent.destination = destination;
    }

    /// <summary>
    /// Accesseur pour obtenir le prochain morceau � ramasser.
    /// </summary>
    /// <returns>La prochaine piece � ramasser</returns>
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
    /// Indique au Roomba qu'un morceau est � ramasser.
    /// </summary>
    /// <param name="morceau">Le morceau � ramasser</param>
    /// <param name="enTete">Indique si le morceau doit �tre ajout� en-t�te de la liste.</param>
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
    /// Ajoute des morceaux � ramasser � la Roomba.
    /// </summary>
    /// <param name="morceaux">La liste des morceaux � ramasser.</param>
    public void RamasserMorceaux(Transform[] morceaux)
    {
        NombrePotsCassesDepuisDernierAgressif++;

        foreach(Transform morceau in morceaux)
        {
            morceauxCasses.AddLast(morceau);
        }
    }

    /// <summary>
    /// D�place le roomba vers la position du TRex
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
