using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestion du menu principal de jeu
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class MenuJeu : MonoBehaviour
{
    /// <summary>
    /// Groupe du canvas pour g�rer l'affichage du menu
    /// </summary>
    private CanvasGroup groupeCanvas;

    /// <summary>
    /// R�f�rence vers le mixer pour g�rer les groupes sonores
    /// </summary>
    [SerializeField]
    private AudioMixer mixer;

    [Header("Son principal")]
    /// <summary>
    /// Volume minimal en d�cibels pour le groupe principal
    /// </summary>
    [SerializeField]
    private float minVolumeDecibel = -80.0f;

    /// <summary>
    /// Volume maximal en d�cibels pour le groupe principal
    /// </summary>
    [SerializeField]
    private float maxVolumeDecibel = 0.0f;

    private void Start()
    {
        // Recup�re le component
        groupeCanvas = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// Ouvre le menu principal
    /// </summary>
    public void Ouvrir()
    {
        // Changement des param�tres
        groupeCanvas.alpha = 1.0f;
        groupeCanvas.interactable = true;
        groupeCanvas.blocksRaycasts = true;

        // Met le jeu en pause
        Time.timeScale = 0.0f;
    }

    /// <summary>
    /// Ferme le menu principal
    /// </summary>
    public void Fermer()
    {
        // Changement des param�tres
        groupeCanvas.alpha = 0.0f;
        groupeCanvas.interactable = false;
        groupeCanvas.blocksRaycasts = false;

        // Permet au jeu de reprendre
        Time.timeScale = 1.0f;
    }

    /// <summary>
    /// Recommence une nouvelle partie en rechargeant la sc�ne
    /// </summary>
    public void Recommencer()
    {
        SceneManager.LoadScene("ScenePrincipale");
    }

    /// <summary>
    /// Quitte le jeu
    /// </summary>
    public void Quitter()
    {
        Application.Quit();
    }

    /// <summary>
    /// Change le volume du groupe ma�tre.
    /// </summary>
    /// <param name="volume">Le pourcentage du volume maximal � affecter.</param>
    public void ChangerVolumeMaitre(float volume)
    {
        float volumeDB = ConvertirLineaireVersExponentiel(volume);

        mixer.GetFloat("VolumeMaitre", out float volumeActuel);
        if (!Mathf.Approximately(volumeDB, volumeActuel))
        {
            mixer.SetFloat("VolumeMaitre", volumeDB);
        }
    }

    /// <summary>
    /// Convertit le pourcentage lin�aire du bouton vers le bon niveau de d�cibels afin que l'effet du contr�le
    /// corresponde au volume entendu.
    /// </summary>
    /// <param name="volumeLineaire">Le pourcentage de puissance sonore � affecter.</param>
    /// <returns>Le nombre de d�cibels qui correspond � cet effet.</returns>
    private float ConvertirLineaireVersExponentiel(float volumeLineaire)
    {
        float minDb = minVolumeDecibel / 10.0f - 12.0f;
        float maxDb = maxVolumeDecibel / 10.0f - 12.0f;
        float etenduDb = maxDb - minDb;
        float echelleExponentielle = Mathf.Lerp(1.0f, Mathf.Pow(2.0f, etenduDb), volumeLineaire);
        float pourcentageLog = Mathf.Log(echelleExponentielle, 2.0f) / etenduDb;
        float volumeDB = Mathf.Lerp(minVolumeDecibel, maxVolumeDecibel, pourcentageLog);

        return volumeDB;

    }
}
