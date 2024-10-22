using TMPro;
using UnityEngine;

/// <summary>
/// R�alise l'affichage du nombre de pots qui ont �t� cass�s
/// </summary>
public class AffichagePotsCasses : MonoBehaviour
{
    /// <summary>
    /// Nombre de pots cass�s dans le jeu.
    /// </summary>
    private int nombrePotsCasses;

    /// <summary>
    /// �tiquette de texte pour afficher les pots cass�s.
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
        // R�cup�re la r�f�rence sur le pot cass�
        etiquettePotsCasses = GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// Augmente le compteur de pot cass�
    /// </summary>
    /// <param name="morceauxCasses">Les morceaux qui ont �t� cass�s</param>
    public void IncrementerPotsCasses(Transform[] morceauxCasses)    
    {
        nombrePotsCasses++;
        etiquettePotsCasses.SetText($"{nombrePotsCasses}"); 
    }
}
