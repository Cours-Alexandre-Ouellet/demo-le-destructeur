using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    private List<PotFleur> potsFleur;
    
    /// <summary>
    /// R�f�rence sur la roomba active
    /// </summary>
    [SerializeField]
    private Roomba roomba;

    /// <summary>
    /// M�thode appel�e � l'initialisation de l'objet au d�but de l'it�ration
    /// de la boucle de jeu
    /// </summary>
    private void Awake()
    {
        potsFleur = new List<PotFleur>();
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
                indice++;
            }
            else
            {
                Debug.LogError($"L'objet {enfant.name} n'a pas de script EmplacementPot.");
            }
        }

        // Version am�lior�e qui r�cup�re que les component EmplacementPot
        // emplacementsPot = transform.GetComponentsInChildren<EmplacementPot>();
    }

    /// <summary>
    /// Gestionnaire de l'action de cr�ation d'un pot de fleur
    /// </summary>
    /// <param name="contexte">Les donn�es de l'action</param>
    public void CreerPot(InputAction.CallbackContext contexte)
    {
        // Si un emplacement est dispo et que l'�v�nement est dans sa phase "started"
        if(potsFleur.Count >= emplacementsPot.Length || !contexte.started)
        {
            return;
        }

        // Recherche un emplacement disponible al�atoirement
        bool aEteCree = false;
        while(!aEteCree)
        {
            EmplacementPot emplacement = emplacementsPot[Random.Range(0, emplacementsPot.Length)];

            if(emplacement.EstOccupe)
            {
                continue;
            }

            aEteCree = true;

            // Cr�e le nouveau pot
            PotFleur nouveauPot = Instantiate(prototypePot);
            nouveauPot.transform.position = emplacement.transform.position;

            // Lie l'�v�nement � la roomba
            nouveauPot.OnCasser.AddListener(roomba.RamasserMorceaux);

            // Met � jour l'emplacement
            emplacement.EstOccupe = true;

            // Ajoute la r�f�rence � la liste
            potsFleur.Add(nouveauPot);
        }

    }
}
