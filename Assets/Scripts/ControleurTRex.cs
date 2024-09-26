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
    /// <param name="valeur">La valeur entrée entrée du déplacement sur deux axes.</param>
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
    /// <param name="valeur">La valeur entrée entrée du déplacement sur un axe.</param>
    public void Rotater(InputAction.CallbackContext contexte)
    {
        rotation = contexte.action.ReadValue<float>();
    }

    /// <summary>
    /// Cette méthode s'exécute une fois par frame et est utile pour les déplacements ayant une incidence sur la
    /// physique du jeu.
    /// </summary>
    private void FixedUpdate()
    {
        // Si un déplacement a lieu (norme plus grande que 0)
        if (deplacement.sqrMagnitude > 0)
        {
            // Déplacement dans le plan XZ
            Vector3 deplacementEffectif = (deplacement.y * transform.forward + deplacement.x * transform.right).normalized;
            rigidbody.position += deplacementEffectif * vitesse * Time.deltaTime;

            // Rotation autour de l'axe des Y
            rigidbody.rotation = rigidbody.rotation *
                Quaternion.AngleAxis(rotation * vitesseRotation * Time.deltaTime, Vector3.up);
                
        }
       
    }
}
