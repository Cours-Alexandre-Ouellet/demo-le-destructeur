using UnityEngine;

/// <summary>
/// Permet de g�rer les musiques dans le jeu selon le niveau d'action.
/// </summary>
[RequireComponent (typeof(AudioSource))]
public class ControleurMusique : MonoBehaviour
{
    /// <summary>
    /// Composant pour jouer des effets sonores.
    /// </summary>
    private AudioSource sourceAudio;

    /// <summary>
    /// R�f�rence vers la musique r�guli�re du jeu.
    /// </summary>
    [SerializeField]
    private AudioClip musiqueReguliere;

    /// <summary>
    /// R�f�rence vers la musique d'action du jeu.
    /// </summary>
    [SerializeField]    
    private AudioClip musiqueAction;

    private void Start()
    {
        sourceAudio = GetComponent<AudioSource> ();
    }

    /// <summary>
    /// Modifie la musique du jeu
    /// </summary>
    /// <param name="estAction">si le changement est pour introduire des actions ou non.</param>
    public void ChangerMusique(bool estAction)
    {
        sourceAudio.Stop ();
        sourceAudio.clip = estAction ? musiqueAction : musiqueReguliere;
        sourceAudio.Play ();
    }
}
