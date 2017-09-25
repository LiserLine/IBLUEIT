using UnityEngine;
using UnityEngine.UI;

public class PopulateSavesList : MonoBehaviour
{
    public GameObject ButtonPrefab;

    public void Start()
    {
        var obstructiveTranslation = LocalizationManager.Instance?.GetLocalizedValue("txtObstructive");
        var restrictiveTranslation = LocalizationManager.Instance?.GetLocalizedValue("txtRestrictive");
        var normalTranslation = LocalizationManager.Instance?.GetLocalizedValue("txtNormal");

        foreach (var plr in DatabaseManager.Instance.Players.PlayerList)
        {
            var go = Instantiate(ButtonPrefab);
            go.transform.SetParent(this.transform);
            go.transform.localScale = Vector3.one;

            var plrComponent = go.AddComponent<PlayerHolder>();
            plrComponent.Player = plr;

            var buttonText = go.GetComponentInChildren<Text>();

            var disfunction = plr.Disfunction == Disfunctions.Normal ? normalTranslation 
                : (plr.Disfunction == Disfunctions.Obstructive ? obstructiveTranslation : restrictiveTranslation);

            var playerInfo = $"({plr.Id}) {plr.Name} - {plr.Birthday:dd/MM/yyyy} - {disfunction}";
            buttonText.text = playerInfo;
        }
    }
}
