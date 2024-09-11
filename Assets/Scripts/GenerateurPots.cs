using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    private List<PotFleur> potsFleur;
    
    /// <summary>
    /// Référence sur la roomba active
    /// </summary>
    [SerializeField]
    private Roomba roomba;

    /// <summary>
    /// Méthode appelée à l'initialisation de l'objet au début de l'itération
    /// de la boucle de jeu
    /// </summary>
    private void Awake()
    {
        potsFleur = new List<PotFleur>();
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
                indice++;
            }
            else
            {
                Debug.LogError($"L'objet {enfant.name} n'a pas de script EmplacementPot.");
            }
        }

        // Version améliorée qui récupère que les component EmplacementPot
        // emplacementsPot = transform.GetComponentsInChildren<EmplacementPot>();
    }

    /// <summary>
    /// Gestionnaire de l'action de création d'un pot de fleur
    /// </summary>
    /// <param name="contexte">Les données de l'action</param>
    public void CreerPot(InputAction.CallbackContext contexte)
    {
        // Si un emplacement est dispo et que l'événement est dans sa phase "started"
        if(potsFleur.Count >= emplacementsPot.Length || !contexte.started)
        {
            return;
        }

        // Recherche un emplacement disponible aléatoirement
        bool aEteCree = false;
        while(!aEteCree)
        {
            EmplacementPot emplacement = emplacementsPot[Random.Range(0, emplacementsPot.Length)];

            if(emplacement.EstOccupe)
            {
                continue;
            }

            aEteCree = true;

            // Crée le nouveau pot
            PotFleur nouveauPot = Instantiate(prototypePot);
            nouveauPot.transform.position = emplacement.transform.position;

            // Lie l'événement à la roomba
            nouveauPot.OnCasser.AddListener(roomba.RamasserMorceaux);

            // Met à jour l'emplacement
            emplacement.EstOccupe = true;

            // Ajoute la référence à la liste
            potsFleur.Add(nouveauPot);
        }

    }
}
