using UnityEngine;

/// <summary>
/// Supprime les objets qui tombent jusque dans cette boite
/// </summary>
public class SuppresseurHauteur : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
    }
}
