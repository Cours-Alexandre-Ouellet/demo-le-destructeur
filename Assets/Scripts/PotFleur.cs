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
    /// Appell�e lorsque le pot entre en colision avec un autre objet
    /// </summary>
    /// <param name="collision">Donn�es sur la collision</param>
    public void OnCollisionEnter(Collision collision)
    {
        // Force re�ue par le pot
        Vector3 impulsion = 0.1f * collision.impulse;       // 0.1f pour diviser la force par 10 (nombre de morceaux)

        // S'il y a un pot, on le supprime
        if (potInitial != null)
        {
            Destroy(potInitial);                // Retire le pot complet
            GameObject nouveauPot = Instantiate(potCasse, transform);      // Nouveau pot cass�
            
            // R�cup�re le rigidbody de chaque morceau 
            Rigidbody[] morceaux = nouveauPot.GetComponentsInChildren<Rigidbody>(); 

            foreach (Rigidbody rigidbodyMorceau in morceaux)
            {
                // Applique la force ad�quate
                rigidbodyMorceau.AddForce(impulsion, ForceMode.Impulse);
            }
        }
    }
}
