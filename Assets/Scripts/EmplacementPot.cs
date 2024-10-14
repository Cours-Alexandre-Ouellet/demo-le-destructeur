using UnityEngine;
using System;

/// <summary>
/// Mod�lise un emplacement auquel on peut faire appara�tre un pot
/// </summary>
public class EmplacementPot : MonoBehaviour
{
    public event Action<EmplacementPot> OnChangementDisponibilite;

    /// <summary>
    /// Indique si l'emplacement est occup� ou non
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
    /// Indique si l'emplacement est utilisable ou non (ex : pr�sentoir d�plac�)
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
    /// Position locale o� le pot sera g�n�r�
    /// </summary>
    [SerializeField]
    private Vector3 positionGeneration;

    /// <summary>
    /// Accesseur pour la position de g�n�ration
    /// </summary>
    public Vector3 PositionGeneration => transform.position + positionGeneration;

    private void Awake()
    {
        estOccupe = false;
        estUtilisable = true;
    }

    /// <summary>
    /// Indique s'il est possible de g�n�rer sur ce pr�sentoir
    /// </summary>
    /// <returns>True si l'on peut g�n�rer � cet emplacement, false sinon.</returns>
    public bool AccepteGeneration()
    {
        return !EstOccupe && EstUtilisable;
    }

    /// <summary>
    /// Ajoute un marqueur ou le pot se g�n�re
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(PositionGeneration - Vector3.right * 0.1f, PositionGeneration + Vector3.right * 0.1f);
        Gizmos.DrawLine(PositionGeneration - Vector3.forward * 0.1f, PositionGeneration + Vector3.forward * 0.1f);
    }
}
