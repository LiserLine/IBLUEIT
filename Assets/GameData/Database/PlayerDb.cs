using System.Collections.Generic;

public class PlayerDb : IDatabase
{
    public static readonly List<Player> PlayerList = new List<Player>();

    public void Load()
    {
        throw new System.NotImplementedException();
    }

    public void Save()
    {
        throw new System.NotImplementedException();
    }
}