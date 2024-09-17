using UnityEngine;

/// <summary>
/// Gère la disparition d'un morceau après un certain délai 
/// </summary>
public class Morceau : MonoBehaviour
{
    /// <summary>
    /// Temps de vie total du morceau
    /// </summary>
    [SerializeField]
    private float tempsVie = 120.0f;

    /// <summary>
    /// Temps écoulé depuis la création de l'objet
    /// </summary>
    private float tempsDepuisCreation;

    /// <summary>
    /// Initialise les attributs interne de l'objet
    /// </summary>
    private void Awake()
    {
        tempsDepuisCreation = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        // Temps de vie écoulé
        if(tempsDepuisCreation > tempsVie)
        {
            Destroy(gameObject);
        }

        // Incrémentation du temps de vie
        tempsDepuisCreation += Time.deltaTime;  
    }
}
