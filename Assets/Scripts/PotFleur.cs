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
    /// Appellée lorsque le pot entre en colision avec un autre objet
    /// </summary>
    /// <param name="collision">Données sur la collision</param>
    public void OnCollisionEnter(Collision collision)
    {
        // Force reçue par le pot
        Vector3 impulsion = 0.1f * collision.impulse;       // 0.1f pour diviser la force par 10 (nombre de morceaux)

        // S'il y a un pot, on le supprime
        if (potInitial != null)
        {
            Destroy(potInitial);                // Retire le pot complet
            GameObject nouveauPot = Instantiate(potCasse, transform);      // Nouveau pot cassé
            
            // Récupère le rigidbody de chaque morceau 
            Rigidbody[] morceaux = nouveauPot.GetComponentsInChildren<Rigidbody>(); 

            foreach (Rigidbody rigidbodyMorceau in morceaux)
            {
                // Applique la force adéquate
                rigidbodyMorceau.AddForce(impulsion, ForceMode.Impulse);
            }
        }
    }
}
