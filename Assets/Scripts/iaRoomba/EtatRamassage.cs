using UnityEngine;

/// <summary>
/// Gère le roomba en mode de ramassage. Dans ce mode, le roomba se déplace vers les pots cassés pour les ramasser.
/// </summary>
public class EtatRamassage : EtatRoomba
{
    /// <summary>
    /// La pièce que le roomba cherche à ramasser.
    /// </summary>
    private Transform morceauCherche;

    /// <inheritdoc/>
    public void OnCommencer(Roomba roomba)
    {
        roomba.Destination = null;
        roomba.onMorceauRamasse.AddListener(GererMorceauRamasser);
    }

    /// <summary>
    /// Le roomba recherche les pots cassés pour les ramasser. Si des pots sont cassés, il peut entrer en mode agressif.
    /// </summary>
    /// <param name="roomba">Le roomba géré l'état.</param>
    /// <returns>L'état du roomba à la prochaine frame.</returns>
    public EtatRoomba OnExecuter(Roomba roomba)
    {
        if (roomba.NombrePotsCassesDepuisDernierAgressif >= roomba.SeuilAgressivite)
        {
            return new EtatAgressif();
        }

        if (roomba.Destination == null || morceauCherche == null || roomba.transform.position == roomba.Destination)
        {
            if (morceauCherche != null)
            {
                roomba.DeplacerVers(morceauCherche.position);
                Debug.Log($"Roomba : ramassage piece ({morceauCherche.name}) à {morceauCherche.position}");
            }

            if (roomba.PossedeMorceauARamasser)
            {
                morceauCherche = roomba.GetProchainePiece();
            }
            else
            {
                return new EtatAttente();
            }
        }

        return this;
    }

    /// <inheritdoc/>
    public void OnSortie(Roomba roomba)
    {
        // remettre dans la queue
        if (morceauCherche != null)
        {
            roomba.RamasserMorceau(morceauCherche, true);
        }
        roomba.onMorceauRamasse.RemoveListener(GererMorceauRamasser);       // Ne pas oublier de désabonner l'événement !
    }

    /// <summary>
    /// Vérifie s'il s'agit du morceau recherché, si oui alors on arrange pour une mise à jour du morceau cherché
    /// </summary>
    /// <param name="morceau">Le morceau qui a été ramassé.</param>
    public void GererMorceauRamasser(Transform morceau)
    {
        if(morceauCherche == morceau)
        {
            morceauCherche = null;
        }
    }
}