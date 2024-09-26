using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controle les actions du TRex dans le jeu. Il lit les entr�s de la personne joueuse et 
/// les r�alise dans le monde.
/// </summary>
[RequireComponent(typeof(Animator))]
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
    private Rigidbody rigidbody;

    private void Start()
    {
        controleurAnimation = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// M�thode qui re�oit les messages de d�placement du InputSystem.
    /// </summary>
    /// <param name="valeur">La valeur entr�e entr�e du d�placement sur deux axes.</param>
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
    /// <param name="valeur">La valeur entr�e entr�e du d�placement sur un axe.</param>
    public void Rotater(InputAction.CallbackContext contexte)
    {
        rotation = contexte.action.ReadValue<float>();
    }

    /// <summary>
    /// Cette m�thode s'ex�cute une fois par frame et est utile pour les d�placements ayant une incidence sur la
    /// physique du jeu.
    /// </summary>
    private void FixedUpdate()
    {
        // Si un d�placement a lieu (norme plus grande que 0)
        if (deplacement.sqrMagnitude > 0)
        {
            // D�placement dans le plan XZ
            Vector3 deplacementEffectif = (deplacement.y * transform.forward + deplacement.x * transform.right).normalized;
            rigidbody.position += deplacementEffectif * vitesse * Time.deltaTime;

            // Rotation autour de l'axe des Y
            rigidbody.rotation = rigidbody.rotation *
                Quaternion.AngleAxis(rotation * vitesseRotation * Time.deltaTime, Vector3.up);
                
        }
       
    }
}
