using UnityEngine;

/// <summary>
/// Type de pot de fleur.
/// 
/// Chaque type donne un nombre de points différents lorsqu'il est cassé et 
/// présente une texture qui lui est unique.
/// </summary>
[CreateAssetMenu(fileName = "TypePotFleur", menuName = "Donnees jeux/Type pot fleur")]
public class TypePotFleur : ScriptableObject
{
    [SerializeField]
    private int point;
    public int Point => point;

    [SerializeField]
    private Material materialPot;
    public Material MaterialPot => materialPot;


}
