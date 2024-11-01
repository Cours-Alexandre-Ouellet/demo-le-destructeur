using UnityEngine;

/// <summary>
/// G�re le roomba en mode agressif. Dans ce mode, le visuel du roomba est modifi� et il se d�place
/// pour attaquer le TRex.
/// </summary>
public class EtatAgressif : EtatRoomba
{
    /// <summary>
    /// Temps �coul� depuis l'entr�e en mode agressif.
    /// </summary>
    private float tempsEnModeAgressif = 0.0f;

    /// <summary>
    /// Temps �coul� depuis la derni�re mise � jour du chemin.
    /// </summary>
    private float tempsMiseAJourChemin = 0.0f;

    /// <summary>
    /// Temps pour lequel l'agressivit� se d�roule
    /// </summary>
    private float tempsAgressivite;

    /// <summary>
    /// Temps de rafraichissement du chemin cible pour 
    /// que la roomba cible le TRex.
    /// </summary>
    private float tempsRafraichissementPoursuite;

    /// <summary>
    /// G�n�re un �tat agressif pour le Roomba.
    /// </summary>
    public EtatAgressif()
    {
        tempsAgressivite = 10.0f;
        tempsRafraichissementPoursuite = 0.1f;
    }

    /// <summary>
    /// Le roomba se modifie en roomba agressif.
    /// </summary>
    /// <param name="roomba">Le roomba g�r� par cet �tat.</param>
    public void OnCommencer(Roomba roomba)
    {
        roomba.TransformerEnAgressive();
        tempsMiseAJourChemin = tempsRafraichissementPoursuite + 1.0f;   // Force le recalcul du chemin
    }

    /// <summary>
    /// Met a jour p�riodiquement la destination du roomba pour attaquer le TRex.
    /// </summary>
    /// <param name="roomba">Le roomba g�r� par cet �tat.</param>
    /// <returns>L'�tat d'attente si le temps d'agressivit� est �coul�.</returns>
    public EtatRoomba OnExecuter(Roomba roomba)
    {
        // Si un nouveau pot est cass�, on r�initialise le temps d'agressivit�
        if (roomba.NombrePotsCassesDepuisDernierAgressif > 0)
        {
            tempsEnModeAgressif = 0;
            roomba.NombrePotsCassesDepuisDernierAgressif = 0;
        }

        // Incr�mente les compteurs de temps
        tempsEnModeAgressif += Time.deltaTime;
        tempsMiseAJourChemin += Time.deltaTime;

        // Permet de courir apr�s le dinosaure
        if (tempsMiseAJourChemin > tempsRafraichissementPoursuite)
        {
            roomba.DeplacerVersTRex();
            tempsMiseAJourChemin = 0.0f;
        }

        if (tempsEnModeAgressif > tempsAgressivite)
        {
            return new EtatAttente();
        }

        return this;
    }

    /// <summary>
    /// Remet le roomba en mode normal.
    /// </summary>
    /// <param name="roomba">Le roomba g�r� par cet �tat.</param>
    public void OnSortie(Roomba roomba)
    {
        roomba.TransformerEnNormale();
    }
}