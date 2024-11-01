using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/// <summary>
/// Contrôleur responsable de générer des pots de fleurs à des emplacements déterminés.
/// </summary>
public class GenerateurPots : MonoBehaviour
{
    /// <summary>
    /// Référence vers le prefab de pot qui sera copié lors de l'initialisation
    /// </summary>
    [SerializeField]
    private PotFleur prototypePot;

    /// <summary>
    /// Liste des emplacements auxquels on peut générer des pots
    /// </summary>
    [SerializeField]
    private EmplacementPot[] emplacementsPot;

    /// <summary>
    /// Liste des pots de fleurs qui sont sur le jeu
    /// </summary>
    private List<EmplacementPot> emplacementsDisponibles;
    
    /// <summary>
    /// Référence sur la roomba active
    /// </summary>
    [SerializeField]
    private Roomba roomba;

    /// <summary>
    /// Nombre de pots initiaux créés au début de la partie
    /// </summary>
    [SerializeField]
    private int nombrePotsInitiaux = 2;

    /// <summary>
    /// Temps minimum entre chaque génération de pot
    /// </summary>
    [SerializeField]
    private float delaiMinEntreGeneration = 2.0f;

    /// <summary>
    /// Temps maximum entre chaque génération de pot
    /// </summary>
    [SerializeField]
    private float delaiMaxEntreGeneration = 10.0f;

    /// <summary>
    /// Indique si le générateur de pot est actif ou non.
    /// </summary>
    [SerializeField]
    private bool generateurActif = true;

    /// <summary>
    /// Méthode appelée à l'initialisation de l'objet au début de l'itération
    /// de la boucle de jeu
    /// </summary>
    private void Awake()
    {
        emplacementsDisponibles = new List<EmplacementPot>();
    }

    /// <summary>
    /// Méthode appelée à la première frame que l'objet est affiché.
    /// </summary>
    private void Start()
    {
        // Crée autant d'emplacement qui il y a de pots
        emplacementsPot = new EmplacementPot[transform.childCount];
        int indice = 0;

        // Permet de parcourir les enfants
        foreach(Transform enfant in transform)
        {
            // Essaie d'accéder au component, si possible le stocke dans emplacement
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
    /// Coroutine qui génère des pots de fleurs à des intervalles aléatoires
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
    /// Gestionnaire de l'action de création d'un pot de fleur
    /// </summary>
    public void CreerPot()
    {
        // Aucun emplacement n'est disponible
        if(emplacementsDisponibles.Count == 0) {
            return;
        }

        // Récupération d'un emplacement
        EmplacementPot emplacement = emplacementsDisponibles[Random.Range(0, emplacementsDisponibles.Count)];
        
        // Crée le nouveau pot
        PotFleur nouveauPot = Instantiate(prototypePot, transform);
        nouveauPot.SetEmplacement(emplacement);

        // Lie l'événement à la roomba et au compteur dans l'interface
        nouveauPot.OnCasser.AddListener(roomba.RamasserMorceaux);
        nouveauPot.OnCasser.AddListener(AffichagePotsCasses.Instance.IncrementerPotsCasses);

        // Met à jour l'emplacement
        emplacement.EstOccupe = true;

        // Liaison de l'événement de suppression
        nouveauPot.OnDestructionObjet.AddListener(RetirerReferencePot);
    }

    /// <summary>
    /// Retire le pot de fleur détruit de la gestion du générateur
    /// </summary>
    /// <param name="potDetruit">le pot détruit</param>
    public void RetirerReferencePot(PotFleur potDetruit)
    {
        potDetruit.Emplacement.EstOccupe = false;
    }
}
