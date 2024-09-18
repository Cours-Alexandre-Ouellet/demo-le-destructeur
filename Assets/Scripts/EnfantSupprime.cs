using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Dans une relation parent-enfant où le parent doit se supprimer
/// si tous ses enfants sont détruits, gère le composant
/// enfant qui déclenche un événement lors de sa suppresion.
/// </summary>
public class EnfantSupprime : MonoBehaviour
{
    /// <summary>
    /// Événement déclencher lors de la suppresion
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
