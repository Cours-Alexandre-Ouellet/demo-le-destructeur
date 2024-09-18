using UnityEngine;

/// <summary>
/// Dans une relation parent-enfant où le parent doit se supprimer
/// si tous ses enfants sont détruits, gère le composant
/// parent qui se supprime dès que tous ses enfants sont détruits.
/// </summary>
public class ParentSupprimeCascade : MonoBehaviour
{
    /// <summary>
    /// Gère la suppression d'un enfant.
    /// </summary>
    /// <param name="enfantSupprime">L'enfant qui a été supprimé.</param>
    public void GererSuppression(GameObject enfantSupprime)
    {
        // En raison du séquençage des opérations, s'il reste un 
        // enfant, alors c'est que le dernier est entrain d'être supprimé
        // et qu'il faut détruire l'objet.
        if (transform.childCount < 2)
        {
            Destroy(gameObject);
        }
    }
}
