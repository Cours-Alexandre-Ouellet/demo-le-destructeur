using UnityEngine;

/// <summary>
/// État de base pour le Roomba. Il est en attente et se déplace aléatoirement.
/// </summary>
public class EtatAttente : EtatRoomba
{
    /// <inheritdoc/>
    public void OnCommencer(Roomba roomba)
    {
        roomba.Destination = null;
    }

    /// <inheritdoc/>
    public EtatRoomba OnExecuter(Roomba roomba)
    {
        // Déplacer 
        if (roomba.Destination == null || roomba.transform.position == roomba.Destination)
        {
            roomba.DeplacerVers(new Vector3(Random.Range(-2.5f, 12.5f), 0.0f, Random.Range(-2.5f, 12.5f)));
        }

        // Vérifier si on doit changer d'état
        if (roomba.NombrePotsCassesDepuisDernierAgressif >= roomba.SeuilAgressivite)
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

    /// <inheritdoc/>
    public void OnSortie(Roomba roomba)
    {
        // Rien
    }
}