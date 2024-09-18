using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Dans une relation parent-enfant o� le parent doit se supprimer
/// si tous ses enfants sont d�truits, g�re le composant
/// enfant qui d�clenche un �v�nement lors de sa suppresion.
/// </summary>
public class EnfantSupprime : MonoBehaviour
{
    /// <summary>
    /// �v�nement d�clencher lors de la suppresion
    /// </summary>
    public UnityEvent<GameObject> onSupprime;

    /// <summary>
    /// Moment dans la boucle de jeu ou l'objet se supprime
    /// </summary>
    public void OnDestroy()
    {
        onSupprime?.Invoke(gameObject);
    }
}
