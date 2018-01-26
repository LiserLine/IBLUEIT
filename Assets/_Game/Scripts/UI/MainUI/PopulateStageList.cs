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

        var stageListPath = Utils.ReadAllText(Application.streamingAssetsPath + @"/GameSettings/StageList.csv");
        var grid = CsvParser2.Parse(stageListPath);

        for (var i = 0; i < grid.Length; i++)
        {
            var btnPrefab = Instantiate(buttonPrefab);
            btnPrefab.transform.SetParent(this.transform);
            btnPrefab.transform.localScale = Vector3.one;

            var holder = btnPrefab.AddComponent<StageHolder>();
            holder.StageToLoad = i + 1;

            btnPrefab.GetComponentInChildren<Text>().text = $"Fase {holder.StageToLoad}";

            if (PlayerData.Player.StagesOpened < i + 1)
                btnPrefab.GetComponent<Button>().interactable = false;
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
