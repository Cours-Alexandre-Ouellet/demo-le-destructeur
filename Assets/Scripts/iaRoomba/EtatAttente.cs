using UnityEngine;

public class EtatAttente : EtatRoomba
{
    public void OnCommencer(Roomba roomba)
    {
        roomba.Destination = null;
    }

    public EtatRoomba OnExecuter(Roomba roomba)
    {
        // Déplacer 

        if (roomba.Destination == null || roomba.transform.position == roomba.Destination)
        {
            roomba.DeplacerVers(new Vector3(Random.Range(-2.5f, 12.5f), 0.0f, Random.Range(-2.5f, 12.5f)));
        }

        if (roomba.NombrePotsCasses > 0 && roomba.NombrePotsCasses % 2 == 0)
        {
            return new EtatAgressif();
        }
        else if (roomba.PossedeMorceauARamasser)
        {
            return new EtatRamassage();
        }
        else 
        {
            return this;
        }
    }

    public void OnSortie(Roomba roomba)
    {
        // Rien
    }
}