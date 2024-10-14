using UnityEngine;
using System;

/// <summary>
/// Modélise un emplacement auquel on peut faire apparaître un pot
/// </summary>
public class EmplacementPot : MonoBehaviour
{
    public event Action<EmplacementPot> OnChangementDisponibilite;

    /// <summary>
    /// Indique si l'emplacement est occupé ou non
    /// </summary>
    public bool EstOccupe
    {
        get { return estOccupe; }
        set
        {
            estOccupe = value;
            OnChangementDisponibilite?.Invoke(this);
        }
    }
    private bool estOccupe;

    /// <summary>
    /// Indique si l'emplacement est utilisable ou non (ex : présentoir déplacé)
    /// </summary>
    public bool EstUtilisable
    {
        get { return estUtilisable; }
        set
        {
            estUtilisable = value;
            OnChangementDisponibilite?.Invoke(this);
        }
    }
    private bool estUtilisable;

    /// <summary>
    /// Position locale où le pot sera généré
    /// </summary>
    [SerializeField]
    private Vector3 positionGeneration;

    /// <summary>
    /// Accesseur pour la position de génération
    /// </summary>
    public Vector3 PositionGeneration => transform.position + positionGeneration;

    private void Awake()
    {
        estOccupe = false;
        estUtilisable = true;
    }

    /// <summary>
    /// Indique s'il est possible de générer sur ce présentoir
    /// </summary>
    /// <returns>True si l'on peut générer à cet emplacement, false sinon.</returns>
    public bool AccepteGeneration()
    {
        return !EstOccupe && EstUtilisable;
    }

    /// <summary>
    /// Ajoute un marqueur ou le pot se génère
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(PositionGeneration - Vector3.right * 0.1f, PositionGeneration + Vector3.right * 0.1f);
        Gizmos.DrawLine(PositionGeneration - Vector3.forward * 0.1f, PositionGeneration + Vector3.forward * 0.1f);
    }
}
