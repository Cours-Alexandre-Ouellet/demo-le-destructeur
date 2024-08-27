using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controle les actions du TRex dans le jeu. Il lit les entr�s de la personne joueuse et 
/// les r�alise dans le monde.
/// </summary>
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
    /// M�thode qui re�oit les messages de d�placement du InputSystem.
    /// </summary>
    /// <param name="valeur">La valeur entr�e entr�e du d�placement sur deux axes.</param>
    public void OnDeplacement(InputValue valeur)
    {
        deplacement = valeur.Get<Vector2>();
    }

    /// <summary>
    /// M�thode qui re�oit les messages de rotation du InputSystem.
    /// </summary>
    /// <param name="valeur">La valeur entr�e entr�e du d�placement sur un axe.</param>
    public void OnRotation(InputValue valeur)
    {
        rotation = valeur.Get<float>();
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
            transform.position += deplacementEffectif * vitesse * Time.deltaTime;

            // Rotation autour de l'axe des Y
            transform.Rotate(Vector3.up, rotation * vitesseRotation * Time.deltaTime);
        }
       
    }
}
