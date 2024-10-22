using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/// <summary>
/// Contr�leur responsable de g�n�rer des pots de fleurs � des emplacements d�termin�s.
/// </summary>
public class GenerateurPots : MonoBehaviour
{
    /// <summary>
    /// R�f�rence vers le prefab de pot qui sera copi� lors de l'initialisation
    /// </summary>
    [SerializeField]
    private PotFleur prototypePot;

    /// <summary>
    /// Liste des emplacements auxquels on peut g�n�rer des pots
    /// </summary>
    [SerializeField]
    private EmplacementPot[] emplacementsPot;

    /// <summary>
    /// Liste des pots de fleurs qui sont sur le jeu
    /// </summary>
    private List<EmplacementPot> emplacementsDisponibles;
    
    /// <summary>
    /// R�f�rence sur la roomba active
    /// </summary>
    [SerializeField]
    private Roomba roomba;

    /// <summary>
    /// Nombre de pots initiaux cr��s au d�but de la partie
    /// </summary>
    [SerializeField]
    private int nombrePotsInitiaux = 2;

    /// <summary>
    /// Temps minimum entre chaque g�n�ration de pot
    /// </summary>
    [SerializeField]
    private float delaiMinEntreGeneration = 2.0f;

    /// <summary>
    /// Temps maximum entre chaque g�n�ration de pot
    /// </summary>
    [SerializeField]
    private float delaiMaxEntreGeneration = 10.0f;

    /// <summary>
    /// Indique si le g�n�rateur de pot est actif ou non.
    /// </summary>
    [SerializeField]
    private bool generateurActif = true;

    /// <summary>
    /// M�thode appel�e � l'initialisation de l'objet au d�but de l'it�ration
    /// de la boucle de jeu
    /// </summary>
    private void Awake()
    {
        emplacementsDisponibles = new List<EmplacementPot>();
    }

    /// <summary>
    /// M�thode appel�e � la premi�re frame que l'objet est affich�.
    /// </summary>
    private void Start()
    {
        // Cr�e autant d'emplacement qui il y a de pots
        emplacementsPot = new EmplacementPot[transform.childCount];
        int indice = 0;

        // Permet de parcourir les enfants
        foreach(Transform enfant in transform)
        {
            // Essaie d'acc�der au component, si possible le stocke dans emplacement
            if(enfant.TryGetComponent(out EmplacementPot emplacement))
            {
                emplacementsPot[indice] = emplacement;
                emplacement.OnChangementDisponibilite += GererChangementDisponibiliteEmplacement;
                indice++;
            }
            else
            {
                Debug.LogError($"L'objet {enfant.name} n'a pas de script EmplacementPot.");
            }
        }
        emplacementsDisponibles = new List<EmplacementPot>();
        emplacementsDisponibles.AddRange(emplacementsPot);

        for(int i = 0; i < nombrePotsInitiaux; i++)
        {
            CreerPot();
        }

        StartCoroutine(GenererPot());
    }

    /// <summary>
    /// Ajoute ou retire un emplacement de la liste des emplacements disponibles.
    /// </summary>
    /// <param name="emplacement"></param>
    private void GererChangementDisponibiliteEmplacement(EmplacementPot emplacement)
    {
        bool emplacementEstListeDisponible = emplacementsDisponibles.Contains(emplacement);
        bool emplacementAccepteGeneration = emplacement.AccepteGeneration();

        if (emplacementEstListeDisponible && !emplacementAccepteGeneration)
        {
            emplacementsDisponibles.Remove(emplacement);
        }
        if (!emplacementEstListeDisponible && emplacementAccepteGeneration)
        {
            emplacementsDisponibles.Add(emplacement);
        }
    }

    /// <summary>
    /// Coroutine qui g�n�re des pots de fleurs � des intervalles al�atoires
    /// </summary>
    /// <returns></returns>

    private IEnumerator GenererPot()
    {
        while (generateurActif)
        {
            float tempsAttente = Random.Range(delaiMinEntreGeneration, delaiMaxEntreGeneration);
            yield return new WaitForSeconds(tempsAttente);
            CreerPot();
        }
    }


    /// <summary>
    /// Gestionnaire de l'action de cr�ation d'un pot de fleur
    /// </summary>
    public void CreerPot()
    {
        // Aucun emplacement n'est disponible
        if(emplacementsDisponibles.Count == 0) {
            return;
        }

        // R�cup�ration d'un emplacement
        EmplacementPot emplacement = emplacementsDisponibles[Random.Range(0, emplacementsDisponibles.Count)];
        
        // Cr�e le nouveau pot
        PotFleur nouveauPot = Instantiate(prototypePot, transform);
        nouveauPot.SetEmplacement(emplacement);

        // Lie l'�v�nement � la roomba et au compteur dans l'interface
        nouveauPot.OnCasser.AddListener(roomba.RamasserMorceaux);
        nouveauPot.OnCasser.AddListener(AffichagePotsCasses.Instance.IncrementerPotsCasses);

        // Met � jour l'emplacement
        emplacement.EstOccupe = true;

        // Liaison de l'�v�nement de suppression
        nouveauPot.OnDestructionObjet.AddListener(RetirerReferencePot);
    }

    /// <summary>
    /// Retire le pot de fleur d�truit de la gestion du g�n�rateur
    /// </summary>
    /// <param name="potDetruit">le pot d�truit</param>
    public void RetirerReferencePot(PotFleur potDetruit)
    {
        potDetruit.Emplacement.EstOccupe = false;
    }
}
