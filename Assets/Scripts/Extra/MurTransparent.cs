using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Rend un mur transparent s'il se trouve entre le TRex et la cam�ra. Ce script doit �tre attach� au GameObject de la cam�ra
/// pour fonctionner correctement, m�me s'il n'utilise pas la cam�ra.
/// </summary>
public class MurTransparent : MonoBehaviour
{
    /// <summary>
    /// Le game object suivi par la cam�ra
    /// </summary>
    [SerializeField]
    private GameObject tRex;

    /// <summary>
    /// � quelle hauteur la transparence doit �tre appliqu�e.
    /// </summary>
    private float hauteurTransparence;

    /// <summary>
    /// Liste des structures qui ont le modificateur de transparence.
    /// </summary>
    private List<GameObject> structuresTransparentes;

    private void Awake()
    {
        structuresTransparentes = new();
    }

    /// <summary>
    /// V�rifie s'il y a un obstacle entre la cam�ra et le TRex.
    /// </summary>
    private void Update()
    {
        RaycastHit[] structuresFrappees = new RaycastHit[100];

        Vector3 direction = tRex.transform.position - transform.position;

        int nombreStructuresFrappes = Physics.BoxCastNonAlloc(transform.position,
            new Vector3(1.0f, 1.0f, 1.0f),
            direction.normalized,
            structuresFrappees,
            Quaternion.identity,
            direction.magnitude, 
            ~LayerMask.NameToLayer("Structure"));
        Debug.Log("Structure : " + nombreStructuresFrappes);

        List<GameObject> aAjouter = new List<GameObject>();
        List<GameObject> aRetirer = new List<GameObject>();


        bool[] structuresTransparentesVisitees = new bool[structuresTransparentes.Count];     // Par d�faut les bool�ens sont faux
        for(int i = 0; i < nombreStructuresFrappes; i++)
        {
            RaycastHit structureFrappee = structuresFrappees[i];
            GameObject gameObjectFrappe = structureFrappee.transform.gameObject;

            // Le GO est frapp�, s'il est d�j� visit� on le marque tel quel, sinon on l'ajoute � la liste
            if(ListeContient(structuresTransparentes, gameObjectFrappe, out int indice))        // �vite d'utiliser LINQ
            {
                structuresTransparentesVisitees[indice] = true;
            }
            else
            {
                RendreTransparent(gameObjectFrappe);
                aAjouter.Add(gameObjectFrappe);
            }
        }

        // Tous les GO non visit�s sont rendus opaques
        for (int i = 0; i < structuresTransparentesVisitees.Length; i++)
        {
            if(!structuresTransparentesVisitees[i])
            {
                RendreOpaque(structuresTransparentes[i]);
                aRetirer.Add(structuresTransparentes[i]);
            }
        }

        // Actualisation des listes de structures transparentes
        foreach(GameObject itemARetirer in aRetirer)
        {
            structuresTransparentes.Remove(itemARetirer);
        }
        structuresTransparentes.AddRange(aAjouter);
    }

    private void RendreTransparent(GameObject gameObject)
    {
        gameObject.GetComponent<Renderer>().material.SetFloat("_Remplissage", hauteurTransparence);
    }

    private void RendreOpaque(GameObject gameObject)
    {
        gameObject.GetComponent<Renderer>().material.SetFloat("_Remplissage", 1.0f);
    }

    private bool ListeContient(List<GameObject> liste, GameObject objet, out int indice)
    {
        for(int i = 0; i < liste.Count; i++)
        {
            if(liste[i].Equals(objet))
            {
                indice = i; 
                return true;
            }
        }

        indice = -1;
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, tRex.transform.position);
    }
}
