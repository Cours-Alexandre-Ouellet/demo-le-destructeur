using UnityEngine;

/// <summary>
/// G�re la physique des pots de fleurs
/// </summary>
public class PotFleur : MonoBehaviour
{
    /// <summary>
    /// Le pot de fleur affich� au joueur
    /// </summary>
    [SerializeField]
    private GameObject potInitial;

    /// <summary>
    /// Mod�le de pot de fleur cass�
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
            GameObject nouvelObjet = Instantiate(potCasse, transform);      // Nouveau pot cass�
        }
    }
}
