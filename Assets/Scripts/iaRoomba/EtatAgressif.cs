using UnityEngine;

/// <summary>
/// Gère le roomba en mode agressif. Dans ce mode, le visuel du roomba est modifié et il se déplace
/// pour attaquer le TRex.
/// </summary>
public class EtatAgressif : EtatRoomba
{
    /// <summary>
    /// Temps écoulé depuis l'entrée en mode agressif.
    /// </summary>
    private float tempsEnModeAgressif = 0.0f;

    /// <summary>
    /// Temps écoulé depuis la dernière mise à jour du chemin.
    /// </summary>
    private float tempsMiseAJourChemin = 0.0f;

    /// <summary>
    /// Temps pour lequel l'agressivité se déroule
    /// </summary>
    private float tempsAgressivite;

    /// <summary>
    /// Temps de rafraichissement du chemin cible pour 
    /// que la roomba cible le TRex.
    /// </summary>
    private float tempsRafraichissementPoursuite;

    /// <summary>
    /// Génère un état agressif pour le Roomba.
    /// </summary>
    public EtatAgressif()
    {
        tempsAgressivite = 10.0f;
        tempsRafraichissementPoursuite = 0.1f;
    }

    /// <summary>
    /// Le roomba se modifie en roomba agressif.
    /// </summary>
    /// <param name="roomba">Le roomba géré par cet état.</param>
    public void OnCommencer(Roomba roomba)
    {
        roomba.TransformerEnAgressive();
        tempsMiseAJourChemin = tempsRafraichissementPoursuite + 1.0f;   // Force le recalcul du chemin
    }

    /// <summary>
    /// Met a jour périodiquement la destination du roomba pour attaquer le TRex.
    /// </summary>
    /// <param name="roomba">Le roomba géré par cet état.</param>
    /// <returns>L'état d'attente si le temps d'agressivité est écoulé.</returns>
    public EtatRoomba OnExecuter(Roomba roomba)
    {
        // Si un nouveau pot est cassé, on réinitialise le temps d'agressivité
        if (roomba.NombrePotsCassesDepuisDernierAgressif > 0)
        {
            tempsEnModeAgressif = 0;
            roomba.NombrePotsCassesDepuisDernierAgressif = 0;
        }

        // Incrémente les compteurs de temps
        tempsEnModeAgressif += Time.deltaTime;
        tempsMiseAJourChemin += Time.deltaTime;

        // Permet de courir après le dinosaure
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
    /// <param name="roomba">Le roomba géré par cet état.</param>
    public void OnSortie(Roomba roomba)
    {
        roomba.TransformerEnNormale();
    }
}