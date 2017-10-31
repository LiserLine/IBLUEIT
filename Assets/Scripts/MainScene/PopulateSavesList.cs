using UnityEngine;
using UnityEngine.UI;

public class PopulateSavesList : MonoBehaviour
{
    public GameObject ButtonPrefab;
    private bool _populated;

    public void OnEnable()
    {
        if(_populated)
        {
            var children = new System.Collections.Generic.List<GameObject>();

            foreach (Transform child in transform)
                children.Add(child.gameObject);

            children.ForEach(child => Destroy(child));
        }

        var obstructiveTranslation = LocalizationManager.Instance?.GetLocalizedValue("txtObstructive");
        var restrictiveTranslation = LocalizationManager.Instance?.GetLocalizedValue("txtRestrictive");
        var normalTranslation = LocalizationManager.Instance?.GetLocalizedValue("txtNormal");

        foreach (var plr in DatabaseManager.Instance.Players.PlayerList)
        {
            var btnPrefab = Instantiate(ButtonPrefab);  
            btnPrefab.transform.SetParent(this.transform);
            btnPrefab.transform.localScale = Vector3.one;

            var plrComponent = btnPrefab.AddComponent<PlayerHolder>();
            plrComponent.Player = plr;

            var buttonText = btnPrefab.GetComponentInChildren<Text>();

            var disfunction = plr.Disfunction == Disfunctions.Normal ? normalTranslation
                : (plr.Disfunction == Disfunctions.Obstructive ? obstructiveTranslation : restrictiveTranslation);

            var playerInfo = $"({plr.Id}) {plr.Name} - {plr.Birthday:dd/MM/yyyy} - {disfunction}";
            buttonText.text = playerInfo;
        }

        _populated = true;
    }
}
