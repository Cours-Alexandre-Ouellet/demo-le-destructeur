using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// Controle les actions du TRex dans le jeu. Il lit les entr�s de la personne joueuse et 
/// les r�alise dans le monde.
/// </summary>
[RequireComponent(typeof(Animator)), RequireComponent(typeof(AudioSource))]
public class ControleurTRex : MonoBehaviour
{
    /// <summary>
    /// Vitesse de d�placement du T-Rex.
    /// </summary>
    [SerializeField]
    private float vitesse;

    /// <summary>
    /// Vitesse de rotation du T-Rex.
    /// </summary>
    [SerializeField]
    private float vitesseRotation;

    /// <summary>
    /// Variable interne du d�placement de la frame
    /// </summary>
    private Vector2 deplacement;

    /// <summary>
    /// Variable interne de la rotaion de la frame.
    /// </summary>
    private float rotation;

    /// <summary>
    /// R�f�rence pour le contr�le de l'animation
    /// </summary>
    private Animator controleurAnimation;

    /// <summary>
    /// R�f�rence sur le rigidbody de l'objet
    /// </summary>
    private new Rigidbody rigidbody;

    /// <summary>
    /// R�f�rence sur le composante Audio
    /// </summary>
    private AudioSource sourceAudio;

    /// <summary>
    /// Le nombre de vies maximales du TRex
    /// </summary>
    [SerializeField]
    private int nombreViesMaximales;

    /// <summary>
    /// Permet d'accéder au nombre de vies maximales
    /// </summary>
    public int NombreViesMaximales => nombreViesMaximales;

    /// <summary>
    /// Le nombre de vies actuels du TRex.
    /// </summary>
    private int nombreVies;

    [Header("Boxcast du rugissement")]
    [SerializeField, Tooltip("Point d'origine locale du boxcollider.")]
    private Vector3 origineBoxcast = new Vector3(0, 1.6f, 2.6f);

    /// <summary>
    /// Direction de la projection du boxcollider. D�pend de l'angle du boxcollider.
    /// </summary>
    private Vector3 directionBoxcast;

    /// <summary>
    /// Indique si le BoxCast pour le rugissement a frappé un objet
    /// </summary>
    private bool aFrappe;

    /// <summary>
    /// Contient l'information sur le pot frappé
    /// </summary>
    private RaycastHit potFrappe;


    [SerializeField, Tooltip("Angle de projection autour de l'axe des X.")]
    private float angleBoxcast = 35.0f;

    [SerializeField, Tooltip("Taille de la bo�te de projection.")]
    private Vector3 tailleBoxcast = new Vector3(.25f, .275f, .25f);

    [SerializeField, Tooltip("Distance sur laquelle la bo�te est projet�e.")]
    private float distanceBoxcast = 5.0f;

    [SerializeField, Tooltip("Force de projection des objets.")]
    private float forceRugissement = 100.0f;

    /// <summary>
    /// Événement appelé lorsque le TRex perd une vie, le nombre de vies restantes est passé en paramètres.
    /// </summary>
    public UnityEvent<int> perdreVie;

    private void Start()
    {
        controleurAnimation = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        float angleRad = (90 + angleBoxcast) * Mathf.Deg2Rad;
        directionBoxcast = new Vector3(0.0f, Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        sourceAudio = GetComponent<AudioSource>();
        nombreVies = nombreViesMaximales;
    }

    /// <summary>
    /// M�thode qui re�oit les messages de d�placement du InputSystem.
    /// </summary>
    /// <param name="contexte">Le contexte de r�alisation de l'action.</param>
    public void Deplacer(InputAction.CallbackContext contexte)
    {
        deplacement = contexte.action.ReadValue<Vector2>();

        // � la premi�re frame on change l'animation pour courir
        if(contexte.started)
        {
            controleurAnimation.SetBool("EnDeplacement", true);
        }

        // � la derni�re frame on change l'animation pour immobile
        if(contexte.canceled)
        {
            controleurAnimation.SetBool("EnDeplacement", false);
        }
    }

    /// <summary>
    /// M�thode qui re�oit les messages de rotation du InputSystem.
    /// </summary>
    /// <param name="contexte">Le contexte de r�alisation de l'action.</param>
    public void Rotater(InputAction.CallbackContext contexte)
    {
        rotation = contexte.action.ReadValue<float>();
    }

    /// <summary>
    /// Action de rugir f�rocement
    /// </summary>
    /// <param name="contexte">Le contexte de r�alisation de l'action.</param>
    public void Rugir(InputAction.CallbackContext contexte)
    {
        
        if(contexte.started)
        {
            // Animation 
            controleurAnimation.SetTrigger("Rugir");
            controleurAnimation.SetBool("EnDeplacement", false);

            // Physique
            StartCoroutine(EffetRugissement());

            // Sonore
            sourceAudio.Play();
        }
    }

    /// <summary>
    /// Retire une vie au TRex
    /// </summary>
    private void RetirerVie()
    {
        nombreVies--;
        perdreVie?.Invoke(nombreVies);
    }

    /// <summary>
    /// Ex�cute les effets du rugissement du dinosaure
    /// </summary>
    /// <returns></returns>
    private IEnumerator EffetRugissement()
    {
        RetirerVie();

        // Attends 3 secondes avant d'ex�cuter une action
        yield return new WaitForSeconds(3.0f);

        // Projete une bo�te pour v�rifier les collisions
        aFrappe = Physics.BoxCast(
            transform.rotation * origineBoxcast + transform.position,
            tailleBoxcast,
            transform.rotation * directionBoxcast,
            out potFrappe,
            Quaternion.Euler(new Vector3(angleBoxcast, 0.0f, 0.0f)),
            distanceBoxcast,
            LayerMask.GetMask("PotFleur"));

        if(aFrappe)
        {
            potFrappe.rigidbody.AddForce(forceRugissement * transform.forward, ForceMode.Impulse);
        }

    }

    /// <summary>
    /// Cette m�thode s'ex�cute une fois par frame et est utile pour les d�placements ayant une incidence sur la
    /// physique du jeu.
    /// </summary>
    private void FixedUpdate()
    {
        // Si un d�placement a lieu (norme plus grande que 0)
        if(deplacement.sqrMagnitude > 0 &&
            !controleurAnimation.GetCurrentAnimatorStateInfo(0).IsName("TRex_Rugir"))
        {
            // D�placement dans le plan XZ
            Vector3 deplacementEffectif = (deplacement.y * transform.forward + deplacement.x * transform.right).normalized;
            rigidbody.position += deplacementEffectif * vitesse * Time.deltaTime;

            // Rotation autour de l'axe des Y
            rigidbody.rotation = rigidbody.rotation *
                Quaternion.AngleAxis(rotation * vitesseRotation * Time.deltaTime, Vector3.up);

            aFrappe = false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = aFrappe ? Color.green: Color.yellow;
        float distanceFrappe = aFrappe ? potFrappe.distance : distanceBoxcast;

        Vector3 pointDepartRayon = transform.rotation * origineBoxcast + transform.position;
        Vector3 pointFinRayon = (transform.rotation * directionBoxcast.normalized) * distanceFrappe;

        Vector3 origineCube = transform.rotation * origineBoxcast + transform.position +
            (transform.rotation * directionBoxcast.normalized) * distanceFrappe;
            

        //Dessine un rayon sur la trajectoire du boxcast
        Gizmos.DrawRay(pointDepartRayon, pointFinRayon);

        //Dessine un cube à la fin du boxcast
        Gizmos.DrawWireCube(origineCube, tailleBoxcast * 2.0f);
    }

}
