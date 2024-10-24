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

    /// <summary>
    /// Le Trex pour lequel les vies sont affichées
    /// </summary>
    [SerializeField]
    private ControleurTRex tRex;

    /// <summary>
    /// Taille de l'icône dans l'interface
    /// </summary>
    [SerializeField]
    private int tailleIconeCoeur; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        coeurs = new List<Image>();

        // On génère un coeur par vie maximal
        for (int i = 0; i < tRex.NombreViesMaximales; i++)
        {
            coeurs.Add(GenererImageCoeur(i + 1));
        }
    }

    /// <summary>
    /// Crée un nouvel objet de jeu pour afficher des images de coeurs.
    /// </summary>
    /// <param name="indice">L'indice du coeur créé.</param>
    /// <returns>L'image ajoutée.</returns>
    private Image GenererImageCoeur(int indice)
    {
        // Crée le GameObject
        GameObject gameObjectImage = new GameObject($"Coeur {indice}");
        
        // Ajoute le composant Image
        Image imageCoeur = gameObjectImage.AddComponent<Image>();

        // Modifie le parent
        gameObjectImage.transform.SetParent(transform);

        // Modifie la position et l'échelle afin de placer correctement l'élément
        gameObjectImage.GetComponent<RectTransform>().sizeDelta = new Vector2(tailleIconeCoeur, tailleIconeCoeur);
        gameObjectImage.transform.localScale = Vector3.one;

        // Par défaut on affiche le coeur plein
        imageCoeur.sprite = coeurPlein;

        return imageCoeur;
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
