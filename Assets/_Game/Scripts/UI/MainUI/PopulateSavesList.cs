using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PopulateSavesList : MonoBehaviour
{
    [SerializeField]
    private GameObject ButtonPrefab;

    private bool _populated;

    private void OnEnable()
    {
        if (_populated)
        {
            var children = (from Transform child in transform select child.gameObject).ToList();
            children.ForEach(Destroy);
        }

        var obstructiveTranslation = LocalizationManager.Instance?.GetLocalizedValue("txtObstructive");
        var restrictiveTranslation = LocalizationManager.Instance?.GetLocalizedValue("txtRestrictive");
        var normalTranslation = LocalizationManager.Instance?.GetLocalizedValue("txtNormal");

        foreach (var plr in PlayerDb.Instance.PlayerList)
        {
            var btnPrefab = Instantiate(ButtonPrefab);
            btnPrefab.transform.SetParent(this.transform);
            btnPrefab.transform.localScale = Vector3.one;

            var plrComponent = btnPrefab.AddComponent<PlayerDataHolder>();
            plrComponent.PlayerData = plr;

            var buttonText = btnPrefab.GetComponentInChildren<Text>();

            var disfunction = plr.Disfunction == DisfunctionType.Normal ? normalTranslation
                : (plr.Disfunction == DisfunctionType.Obstructive ? obstructiveTranslation : restrictiveTranslation);
            
            buttonText.text = $"({plr.Id}) {plr.Name} - {plr.Birthday:dd/MM/yyyy} - {disfunction}";
        }

        _populated = true;
    }
}
