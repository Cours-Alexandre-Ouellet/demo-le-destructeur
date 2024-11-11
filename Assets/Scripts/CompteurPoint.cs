using TMPro;
using UnityEngine;

/// <summary>
/// Accumule et affiche le total de points 
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class CompteurPoint : MonoBehaviour
{
    /// <summary>
    /// Gère l'instance unique du compteur de points
    /// </summary>
    public static CompteurPoint Instance { get; private set; }

    /// <summary>
    /// Le texte qui affiche le nombre de points
    /// </summary>
    private TextMeshProUGUI compteur;

    /// <summary>
    /// Le nombre de points totaux
    /// </summary>
    private int points;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Plusieur 'CompteurPoints' trouve. L'objet '" + gameObject.name + "' est detruit.");
            Destroy(gameObject);
        }

        points = 0;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        compteur = GetComponent<TextMeshProUGUI>();
        
    }

    /// <summary>
    /// Augmente le total de point d'une certaine valeur.
    /// </summary>
    /// <param name="pointsGagnes">Le nombre de points duquel augmenter le total.</param>
    public void AjouterPoints(int pointsGagnes)
    {
        points += pointsGagnes;
        compteur.text = points.ToString();
    }
}
