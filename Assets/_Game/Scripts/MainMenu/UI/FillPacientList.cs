using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FillPacientList : MonoBehaviour
{
    private bool _populated;

    [SerializeField]
    private GameObject itemPrefab;

    [SerializeField]
    private Scrollbar scrollbar;

    private void OnEnable()
    {
        if (_populated)
        {
            var children = (from Transform child in transform select child.gameObject).ToList();
            children.ForEach(Destroy);
        }

        PacientDb.Instance.Load();

        var obstructiveTranslation = "Obstrutivo";
        var restrictiveTranslation = "Restritivo";
        var normalTranslation = "Normal";

        foreach (var pacient in PacientDb.Instance.PacientList)
        {
            var item = Instantiate(itemPrefab);
            item.transform.SetParent(this.transform);
            item.transform.localScale = Vector3.one;
            item.name = $"ITEM_{pacient.Id}_{pacient.Name}";

            var holder = item.AddComponent<PacientLoader>();
            holder.pacient = pacient;

            var disfunction = pacient.Condition == ConditionType.Normal ? normalTranslation
                : (pacient.Condition == ConditionType.Obstructive ? obstructiveTranslation : restrictiveTranslation);

            item.GetComponentInChildren<Text>().text = $"{pacient.Name} - {pacient.Birthday:dd/MM/yyyy} - {disfunction} - ID: {pacient.Id}";
        }

        StartCoroutine(AdjustGrip());

        _populated = true;
    }

    private IEnumerator AdjustGrip()
    {
        yield return null;
        scrollbar.value = 1;
    }
}