using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EmplacementPot)), RequireComponent(typeof(Rigidbody))]
public class Presentoire : MonoBehaviour
{
    private Vector3 positionInitiale;

    private Quaternion rotationInitiale;

    private EmplacementPot emplacementPot;

    private new Rigidbody rigidbody;

    private new Renderer renderer;

    private Coroutine coroutineReplacement;

    /// <summary>
    /// Délai avant de replacer le présentoir renversé.
    /// </summary>
    [SerializeField]
    private float delaiReplacement = 20.0f;

    private void Awake()
    {
        coroutineReplacement = null;
    }

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        emplacementPot = GetComponent<EmplacementPot>();
        renderer = GetComponent<Renderer>();

        positionInitiale = transform.position;
        rotationInitiale = transform.rotation;

        StartCoroutine(Replacer());
    }

    private void OnCollisionExit(Collision collision)
    {
        if (positionInitiale != transform.position || rotationInitiale != transform.rotation)
        {
            emplacementPot.EstUtilisable = false;
            
            // On réinitialise le replacement chaque fois qu'on termine un contact
            if(coroutineReplacement != null)
            {
               StopCoroutine(coroutineReplacement);
            }
            coroutineReplacement = StartCoroutine(Replacer());
        }
    }

    private IEnumerator Replacer()
    {
        yield return new WaitUntil(() => rigidbody.IsSleeping());
        yield return new WaitForSeconds(delaiReplacement);

        transform.SetPositionAndRotation(positionInitiale, rotationInitiale);
        emplacementPot.EstUtilisable = true;

        rigidbody.linearVelocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        coroutineReplacement = null;
    }
}
