
/// <summary>
/// Représente un état de base pour le Roomba. 
/// </summary>
public interface EtatRoomba
{
    /// <summary>
    /// Appellée lorsque le Roomba entre dans cet état.
    /// </summary>
    /// <param name="roomba">Le roomba qui est géré par cet état.</param>
    public void OnCommencer(Roomba roomba);

    /// <summary>
    /// Appelee à chaque frame pour mettre à jour l'état du Roomba.
    /// </summary>
    /// <param name="roomba">Le roomba qui est géré par cet état.</param>
    /// <returns>L'état du roomba au porochain frame.</returns>
    public EtatRoomba OnExecuter(Roomba roomba);

    /// <summary>
    /// Appellée lorsque le Roomba sort de cet état.
    /// </summary>
    /// <param name="roomba">Le roomba qui est géré par cet état.</param>
    public void OnSortie(Roomba roomba);

}