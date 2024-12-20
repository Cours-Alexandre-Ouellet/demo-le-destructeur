using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

/// <summary>
/// G�re la physique des pots de fleurs
/// </summary>
public class PotFleur : MonoBehaviour
{
    /// <summary>
    /// �v�nement o� le pot casse
    /// </summary>
    public UnityEvent<Transform[]> OnCasser;

    /// <summary>
    /// �V�nement lorsqu'on d�truit l'objet
    /// </summary>
    public UnityEvent<PotFleur> OnDestructionObjet;

    /// <summary>
    /// Le pot de fleur affich� au joueur
    /// </summary>
    [SerializeField]
    private GameObject potInitial;

    /// <summary>
    /// Mod�le de pot de fleur cass�
    /// </summary>
    [SerializeField]
    private GameObject potCasse;

    /// <summary>
    /// La fleur dans le pot
    /// </summary>
    [SerializeField]
    private GameObject fleur;

    /// <summary>
    /// L'emplacement o� il est cr��.
    /// </summary>
    public EmplacementPot Emplacement { get; private set; }

    /// <summary>
    /// �tat du pot s'il est cass�
    /// </summary>
    private bool estCasse;

    /// <summary>
    /// Le type du pot de fleur
    /// </summary>
    private TypePotFleur type;

    private void Awake()
    {
        estCasse = false;
    }

    /// <summary>
    /// Initialise les valeurs dans le pot de fleur
    /// </summary>
    /// <param name="type">Le type de pot de fleur de ce pot.</param>
    public void Initialiser(TypePotFleur type)
    {
        this.type = type;
        GetComponentInChildren<MeshRenderer>().material = type.MaterialPot;
    }

    /// <summary>
    /// Appell�e lorsque le pot entre en colision avec un autre objet
    /// </summary>
    /// <param name="collision">Donn�es sur la collision</param>
    public void OnCollisionEnter(Collision collision)
    {
        // V�rifie que la collision n'a pas lieu avec un support et que le pot
        // n'est pas d�j� cass�. (�vite d'appeler deux fois l'�v�nement).
        if (collision.gameObject.CompareTag("SupportPot") || estCasse)
        {
            return;
        }

        // Force re�ue par le pot
        Vector3 impulsion = 0.1f * collision.impulse;       // 0.1f pour diviser la force par 10 (nombre de morceaux)

        // S'il y a un pot, on le supprime
        if (potInitial != null)
        {
            Destroy(potInitial);                // Retire le pot complet
            GameObject nouveauPot = Instantiate(potCasse, transform);      // Nouveau pot cass�

            // R�cup�re le rigidbody de chaque morceau 
            Rigidbody[] morceaux = nouveauPot.GetComponentsInChildren<Rigidbody>();

            foreach (Rigidbody rigidbodyMorceau in morceaux)
            {
                // Applique la force ad�quate
                rigidbodyMorceau.AddForce(impulsion, ForceMode.Impulse);
            }

            // Appelle l'�v�nement pour indiquer qu'un pot a �t� cass�
            List<Transform> morceauARamasser = new();
            foreach (Transform morceau in transform)
            {
                morceauARamasser.Add(morceau);
            }

            if (fleur != null)
            {
                morceauARamasser.Add(fleur.transform);
            }
            OnCasser?.Invoke(morceauARamasser.ToArray());
            estCasse = true;        // Met le pot dans l'�tat cass�

            // Ajout des points
            CompteurPoint.Instance.AjouterPoints(type.Point);

            // Liaison de l'�v�nement de suppression
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
    /// Moment de la boucle ou le pot est d�truit
    /// </summary>
    public void OnDestroy()
    {
        OnDestructionObjet?.Invoke(this);
    }
}
