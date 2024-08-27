using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controle les actions du TRex dans le jeu. Il lit les entrés de la personne joueuse et 
/// les réalise dans le monde.
/// </summary>
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
    /// Méthode qui reçoit les messages de déplacement du InputSystem.
    /// </summary>
    /// <param name="valeur">La valeur entrée entrée du déplacement sur deux axes.</param>
    public void OnDeplacement(InputValue valeur)
    {
        deplacement = valeur.Get<Vector2>();
    }

    /// <summary>
    /// Méthode qui reçoit les messages de rotation du InputSystem.
    /// </summary>
    /// <param name="valeur">La valeur entrée entrée du déplacement sur un axe.</param>
    public void OnRotation(InputValue valeur)
    {
        rotation = valeur.Get<float>();
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
            transform.position += deplacementEffectif * vitesse * Time.deltaTime;

            // Rotation autour de l'axe des Y
            transform.Rotate(Vector3.up, rotation * vitesseRotation * Time.deltaTime);
        }
       
    }
}
