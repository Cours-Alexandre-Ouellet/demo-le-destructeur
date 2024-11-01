using UnityEngine;

/// <summary>
/// G�re le roomba en mode de ramassage. Dans ce mode, le roomba se d�place vers les pots cass�s pour les ramasser.
/// </summary>
public class EtatRamassage : EtatRoomba
{
    /// <summary>
    /// La pi�ce que le roomba cherche � ramasser.
    /// </summary>
    private Transform morceauCherche;

    /// <inheritdoc/>
    public void OnCommencer(Roomba roomba)
    {
        roomba.Destination = null;
        roomba.onMorceauRamasse.AddListener(GererMorceauRamasser);
    }

    /// <summary>
    /// Le roomba recherche les pots cass�s pour les ramasser. Si des pots sont cass�s, il peut entrer en mode agressif.
    /// </summary>
    /// <param name="roomba">Le roomba g�r� l'�tat.</param>
    /// <returns>L'�tat du roomba � la prochaine frame.</returns>
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
                Debug.Log($"Roomba : ramassage piece ({morceauCherche.name}) � {morceauCherche.position}");
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
        roomba.onMorceauRamasse.RemoveListener(GererMorceauRamasser);       // Ne pas oublier de d�sabonner l'�v�nement !
    }

    /// <summary>
    /// V�rifie s'il s'agit du morceau recherch�, si oui alors on arrange pour une mise � jour du morceau cherch�
    /// </summary>
    /// <param name="morceau">Le morceau qui a �t� ramass�.</param>
    public void GererMorceauRamasser(Transform morceau)
    {
        if(morceauCherche == morceau)
        {
            morceauCherche = null;
        }
    }
}