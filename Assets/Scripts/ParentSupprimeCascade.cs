using UnityEngine;

/// <summary>
/// Dans une relation parent-enfant o� le parent doit se supprimer
/// si tous ses enfants sont d�truits, g�re le composant
/// parent qui se supprime d�s que tous ses enfants sont d�truits.
/// </summary>
public class ParentSupprimeCascade : MonoBehaviour
{
    /// <summary>
    /// G�re la suppression d'un enfant.
    /// </summary>
    /// <param name="enfantSupprime">L'enfant qui a �t� supprim�.</param>
    public void GererSuppression(GameObject enfantSupprime)
    {
        // En raison du s�quen�age des op�rations, s'il reste un 
        // enfant, alors c'est que le dernier est entrain d'�tre supprim�
        // et qu'il faut d�truire l'objet.
        if (transform.childCount < 2)
        {
            Destroy(gameObject);
        }
    }
}
