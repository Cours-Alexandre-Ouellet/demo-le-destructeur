using UnityEngine;

public class EtatAgressif : EtatRoomba
{
    private float tempsEnModeAgressif = 0.0f;
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

    public EtatAgressif()
    {
        tempsAgressivite = 10.0f;
        tempsRafraichissementPoursuite = 0.1f;
    }

    public void OnCommencer(Roomba roomba)
    {
        roomba.TransformerEnAgressive();
        tempsMiseAJourChemin = tempsRafraichissementPoursuite + 1.0f;
    }

    public EtatRoomba OnExecuter(Roomba roomba)
    {
        tempsEnModeAgressif += Time.deltaTime;
        Debug.Log(tempsEnModeAgressif); 
        tempsMiseAJourChemin += Time.deltaTime;

        // Permet de courir après le dinosaure
        if (tempsMiseAJourChemin > tempsRafraichissementPoursuite)
        {
            roomba.DeplacerVersTRex();
            tempsMiseAJourChemin = 0.0f;
        }

        // if(roomba.NombrePotsCasses > roomba.NombrePotsCasses precedent)
        // tempsEnModeAgressif = 0;

        if (tempsEnModeAgressif > tempsAgressivite)
        {
            return new EtatAttente();
        }

        return this;
    }

    public void OnSortie(Roomba roomba)
    {
        roomba.TransformerEnNormale();
    }
}