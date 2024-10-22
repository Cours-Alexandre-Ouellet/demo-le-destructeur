using TMPro;
using UnityEngine;

/// <summary>
/// Réalise l'affichage du nombre de pots qui ont été cassés
/// </summary>
public class AffichagePotsCasses : MonoBehaviour
{
    /// <summary>
    /// Nombre de pots cassés dans le jeu.
    /// </summary>
    private int nombrePotsCasses;

    /// <summary>
    /// Étiquette de texte pour afficher les pots cassés.
    /// </summary>
    private TextMeshProUGUI etiquettePotsCasses;

    /// <summary>
    /// Instance unique d'affichage.
    /// </summary>
    public static AffichagePotsCasses Instance { get; private set;}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        nombrePotsCasses = 0;

        // Assure qu'il y ait qu'une instance de cette classe
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Récupère la référence sur le pot cassé
        etiquettePotsCasses = GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// Augmente le compteur de pot cassé
    /// </summary>
    /// <param name="morceauxCasses">Les morceaux qui ont été cassés</param>
    public void IncrementerPotsCasses(Transform[] morceauxCasses)    
    {
        nombrePotsCasses++;
        etiquettePotsCasses.SetText($"{nombrePotsCasses}"); 
    }
}
