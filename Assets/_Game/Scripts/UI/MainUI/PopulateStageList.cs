using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PopulateStageList : MonoBehaviour
{
    [SerializeField]
    private GameObject buttonPrefab;

    [SerializeField]
    private Scrollbar scrollbar;

    private bool populated;

    private void OnEnable()
    {
        if (populated)
        {
            var children = (from Transform child in transform select child.gameObject).ToList();
            children.ForEach(Destroy);
        }

        var stageListPath = Utils.ReadCsv(Application.streamingAssetsPath + @"/GameSettings/StageList.csv");
        var grid = CsvParser2.Parse(stageListPath);

        for (var i = 1; i < grid.Length; i++)
        {
            var btnPrefab = Instantiate(buttonPrefab);
            btnPrefab.transform.SetParent(this.transform);
            btnPrefab.transform.localScale = Vector3.one;

            var holder = btnPrefab.AddComponent<StageHolder>();
            holder.StageToLoad = i;

            btnPrefab.GetComponentInChildren<Text>().text = $"Nível {holder.StageToLoad}";
            btnPrefab.GetComponent<Button>().interactable = Pacient.Loaded.StagesOpened >= i;
        }

        StartCoroutine(Grip());

        populated = true;
    }

    private IEnumerator Grip()
    {
        yield return null;
        scrollbar.value = 1;
    }
}
