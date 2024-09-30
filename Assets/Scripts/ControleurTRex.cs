using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controle les actions du TRex dans le jeu. Il lit les entrés de la personne joueuse et 
/// les réalise dans le monde.
/// </summary>
[RequireComponent(typeof(Animator))]
public class ControleurTRex : MonoBehaviour
{
    /// <summary>
    /// Vitesse de déplacement du T-Rex.
    /// </summary>
    [SerializeField]
    private float vitesse;

    /// <summary>
    /// Vitesse de rotation du T-Rex.
    /// </summary>
    [SerializeField]
    private float vitesseRotation;

    /// <summary>
    /// Variable interne du déplacement de la frame
    /// </summary>
    private Vector2 deplacement;

    /// <summary>
    /// Variable interne de la rotaion de la frame.
    /// </summary>
    private float rotation;

    /// <summary>
    /// Référence pour le contrôle de l'animation
    /// </summary>
    private Animator controleurAnimation;

    /// <summary>
    /// Référence sur le rigidbody de l'objet
    /// </summary>
    private Rigidbody rigidbody;

    private void Start()
    {
        controleurAnimation = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Méthode qui reçoit les messages de déplacement du InputSystem.
    /// </summary>
    /// <param name="contexte">Le contexte de réalisation de l'action.</param>
    public void Deplacer(InputAction.CallbackContext contexte)
    {
        deplacement = contexte.action.ReadValue<Vector2>();

        // À la première frame on change l'animation pour courir
        if(contexte.started)
        {
            controleurAnimation.SetBool("EnDeplacement", true);
        }

        // À la dernière frame on change l'animation pour immobile
        if(contexte.canceled)
        {
            controleurAnimation.SetBool("EnDeplacement", false);
        }
    }

    /// <summary>
    /// Méthode qui reçoit les messages de rotation du InputSystem.
    /// </summary>
    /// <param name="contexte">Le contexte de réalisation de l'action.</param>
    public void Rotater(InputAction.CallbackContext contexte)
    {
        rotation = contexte.action.ReadValue<float>();
    }

    /// <summary>
    /// Action de rugir férocement
    /// </summary>
    /// <param name="contexte">Le contexte de réalisation de l'action.</param>
    public void Rugir(InputAction.CallbackContext contexte)
    {
        if(contexte.started)
        {
            controleurAnimation.SetTrigger("Rugir");
            controleurAnimation.SetBool("EnDeplacement", false);
            StartCoroutine(EffetRugissement());
        }
    }

    /// <summary>
    /// Exécute les effets du rugissement du dinosaure
    /// </summary>
    /// <returns></returns>
    private IEnumerator EffetRugissement()
    {
        // Attends 3 secondes avant d'exécuter une action
        yield return new WaitForSeconds(3.0f);

        // Projete une boîte pour vérifier les collisions
        aFrappe = Physics.BoxCast(
            transform.rotation * origineBoxcast + transform.position,
            tailleBoxcast,
            directionBoxcast,
            out potFrappe,
            Quaternion.Euler(angleBoxcast),
            distanceBoxcast,
            LayerMask.GetMask("PotFleur"));

        if(aFrappe)
        {
            potFrappe.rigidbody.AddForce(10.0f * transform.forward);
        }

    }

    /// <summary>
    /// Cette méthode s'exécute une fois par frame et est utile pour les déplacements ayant une incidence sur la
    /// physique du jeu.
    /// </summary>
    private void FixedUpdate()
    {
        // Si un déplacement a lieu (norme plus grande que 0)
        if(deplacement.sqrMagnitude > 0 &&
            !controleurAnimation.GetCurrentAnimatorStateInfo(0).IsName("TRex_Rugir"))
        {
            // Déplacement dans le plan XZ
            Vector3 deplacementEffectif = (deplacement.y * transform.forward + deplacement.x * transform.right).normalized;
            rigidbody.position += deplacementEffectif * vitesse * Time.deltaTime;

            // Rotation autour de l'axe des Y
            rigidbody.rotation = rigidbody.rotation *
                Quaternion.AngleAxis(rotation * vitesseRotation * Time.deltaTime, Vector3.up);

            aFrappe = false;
        }
    }

    private bool aFrappe;

    private RaycastHit potFrappe;

    [Header("Boxcast du rugissement")]
    [SerializeField]
    private Vector3 origineBoxcast = new Vector3(0, 1.6f, 2.6f);

    [SerializeField]
    private Vector3 directionBoxcast = new Vector3(0.0f, -.43f, 0.82f);

    [SerializeField]
    private Vector3 angleBoxcast = new Vector3(35.0f, 0.0f, 0.0f);

    [SerializeField]
    private Vector3 tailleBoxcast = new Vector3(.25f, .275f, .25f);

    [SerializeField]
    private float distanceBoxcast = 5.0f;


    void OnDrawGizmos()
    {
        Gizmos.color = aFrappe ? Color.green: Color.red;
        float distanceFrappe = aFrappe ? potFrappe.distance : distanceBoxcast;

        Vector3 pointDepartRayon = transform.rotation * origineBoxcast + transform.position;
        Vector3 pointFinRayon = directionBoxcast.normalized * distanceFrappe;

        Vector3 origineCube = transform.rotation * origineBoxcast + transform.position +
            directionBoxcast.normalized * distanceFrappe;
            

        //Dessine un rayon sur la trajectoire du boxcast
        Gizmos.DrawRay(pointDepartRayon, pointFinRayon);

        //Dessine un cube à la fin du boxcast
        Gizmos.DrawWireCube(origineCube, tailleBoxcast * 2.0f);


    }

}
