

using UnityEngine;

public class EtatRamassage : EtatRoomba
{
    private Transform pieceCherchee;

    public void OnCommencer(Roomba roomba)
    {
        roomba.Destination = null;
    }

    public EtatRoomba OnExecuter(Roomba roomba)
    {
        if (roomba.NombrePotsCasses > 0 && roomba.NombrePotsCasses % 2 == 0)
        {
            return new EtatAgressif();
        }

        if (roomba.Destination == null || roomba.transform.position == roomba.Destination)
        {
            if (roomba.PossedeMorceauARamasser)
            {
                pieceCherchee = roomba.GetProchainePiece();
                roomba.DeplacerVers(pieceCherchee.position);
            }
            else
            {
                return new EtatAttente();
            }
        }

        return this;
    }

    public void OnSortie(Roomba roomba)
    {
        // remettre dans la queue
        if (pieceCherchee != null)
        {
            roomba.RamasserMorceau(pieceCherchee);
        }
    }
}