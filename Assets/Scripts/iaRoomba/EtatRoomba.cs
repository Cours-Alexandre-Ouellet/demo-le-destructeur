
/// <summary>
/// Repr�sente un �tat de base pour le Roomba. 
/// </summary>
public interface EtatRoomba
{
    /// <summary>
    /// Appell�e lorsque le Roomba entre dans cet �tat.
    /// </summary>
    /// <param name="roomba">Le roomba qui est g�r� par cet �tat.</param>
    public void OnCommencer(Roomba roomba);

    /// <summary>
    /// Appelee � chaque frame pour mettre � jour l'�tat du Roomba.
    /// </summary>
    /// <param name="roomba">Le roomba qui est g�r� par cet �tat.</param>
    /// <returns>L'�tat du roomba au porochain frame.</returns>
    public EtatRoomba OnExecuter(Roomba roomba);

    /// <summary>
    /// Appell�e lorsque le Roomba sort de cet �tat.
    /// </summary>
    /// <param name="roomba">Le roomba qui est g�r� par cet �tat.</param>
    public void OnSortie(Roomba roomba);

}