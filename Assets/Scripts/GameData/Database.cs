using UnityEngine;

public class Database : MonoBehaviour
{
    public static AccountDb Accounts { get; } = new AccountDb();
    public static PlayerDb Players { get; } = new PlayerDb();

    

}
