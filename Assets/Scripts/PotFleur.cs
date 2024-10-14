using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

/// <summary>
/// Gère la physique des pots de fleurs
/// </summary>
public class PotFleur : MonoBehaviour
{
    /// <summary>
    /// Événement où le pot casse
    /// </summary>
    public UnityEvent<Transform[]> OnCasser;

    /// <summary>
    /// ÉVénement lorsqu'on détruit l'objet
    /// </summary>
    public UnityEvent<PotFleur> OnDestructionObjet;

    /// <summary>
    /// Le pot de fleur affiché au joueur
    /// </summary>
    [SerializeField]
    private GameObject potInitial;

    /// <summary>
    /// Modèle de pot de fleur cassé
    /// </summary>
    [SerializeField]
    private GameObject potCasse;

    /// <summary>
    /// La fleur dans le pot
    /// </summary>
    [SerializeField]
    private GameObject fleur;

    /// <summary>
    /// L'emplacement où il est créé.
    /// </summary>
    public EmplacementPot Emplacement {get; private set;}

    /// <summary>
    /// Appellée lorsque le pot entre en colision avec un autre objet
    /// </summary>
    /// <param name="collision">Données sur la collision</param>
    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("SupportPot"))
        {
            return;
        }

        // Force reçue par le pot
        Vector3 impulsion = 0.1f * collision.impulse;       // 0.1f pour diviser la force par 10 (nombre de morceaux)

        // S'il y a un pot, on le supprime
        if (potInitial != null)
        {
            Destroy(potInitial);                // Retire le pot complet
            GameObject nouveauPot = Instantiate(potCasse, transform);      // Nouveau pot cassé
            
            // Récupère le rigidbody de chaque morceau 
            Rigidbody[] morceaux = nouveauPot.GetComponentsInChildren<Rigidbody>(); 

            foreach (Rigidbody rigidbodyMorceau in morceaux)
            {
                // Applique la force adéquate
                rigidbodyMorceau.AddForce(impulsion, ForceMode.Impulse);
            }

            // Appelle l'événement pour indiquer qu'un pot a été cassé
            List<Transform> morceauARamasser = 
                new List<Transform>(nouveauPot.GetComponentsInChildren<Transform>());
            morceauARamasser.Add(fleur.transform);

            OnCasser?.Invoke(morceauARamasser.ToArray());

            // Liaison de l'événement de suppression
            EnfantSupprime potCasseSuppression =
                nouveauPot.GetComponent<EnfantSupprime>();

            ParentSupprimeCascade supresseurCascade = GetComponent<ParentSupprimeCascade>();

            potCasseSuppression.onSupprime.AddListener(supresseurCascade.GererSuppression);
        }
    }

    /// <summary>
    /// Permet d'assigner un emplacement au pot de fleur
    /// </summary>
    /// <param name="emplacement"></param>
    public void SetEmplacement(EmplacementPot emplacement)
    {
        transform.position = emplacement.PositionGeneration;
        Emplacement = emplacement;
    }

    /// <summary>
    /// Moment de la boucle ou le pot est détruit
    /// </summary>
    public void OnDestroy()
    {
        OnDestructionObjet?.Invoke(this);
    }
}
