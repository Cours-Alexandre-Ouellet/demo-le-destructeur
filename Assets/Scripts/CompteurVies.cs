using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Affiche le nombre de vies du TRex
/// </summary>
public class CompteurVies : MonoBehaviour
{
    /// <summary>
    /// Les composantes qui affichent les coeurs du TRex
    /// </summary>
    private List<Image> coeurs;

    /// <summary>
    /// L'image d'un coeur plein
    /// </summary>
    [SerializeField]
    private Sprite coeurPlein;

    /// <summary>
    /// L'image d'un coeur vide
    /// </summary>
    [SerializeField]
    private Sprite coeurVide;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Récupère les enfants qui ont une image.
        coeurs = new List<Image>(GetComponentsInChildren<Image>());
    }

    /// <summary>
    /// Retire une vie du TRex
    /// </summary>
    /// <param name="viesRestantes">Nombre de vies restantes au TRex</param>
    public void RetirerVie(int viesRestantes)
    {
        Image coeurPerdu = coeurs[viesRestantes];
        coeurPerdu.sprite = coeurVide;
    }
}
