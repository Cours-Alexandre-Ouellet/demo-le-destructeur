using UnityEngine;

/// <summary>
/// �tat de base pour le Roomba. Il est en attente et se d�place al�atoirement.
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
        // D�placer 
        if (roomba.Destination == null || roomba.transform.position == roomba.Destination)
        {
            roomba.DeplacerVers(new Vector3(Random.Range(-2.5f, 12.5f), 0.0f, Random.Range(-2.5f, 12.5f)));
        }

        // V�rifier si on doit changer d'�tat
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