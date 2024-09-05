using UnityEngine;

/// <summary>
/// Gère la physique des pots de fleurs
/// </summary>
public class PotFleur : MonoBehaviour
{
    /// <summary>
    /// Le pot de fleur affiché au joueur
    /// </summary>
    [SerializeField]
    private GameObject potInitial;

    /// <summary>
    /// Modèle de pot de fleur cassé
    /// </summary>
    [SerializeField]
    private GameObject potCasse;

    /// <summary>
    /// Action du joueur casser
    /// </summary>
    public void Casser()
    {
        // S'il y a un pot, on le supprime
        if (potInitial != null)
        {
            Destroy(potInitial);
            GameObject nouvelObjet = Instantiate(potCasse, transform);      // Nouveau pot cassé
        }
    }
}
