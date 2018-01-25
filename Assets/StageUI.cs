/**
 * StageUI.cs
 * Created by: Renato Grimes
 * Created on: 25/01/2018 (dd/mm/yy)
 */

using UnityEngine;
using UnityEngine.UI;

public class StageUI : BasicUI<StageUI>
{
    [SerializeField]
    private Text stageIdText;

    private void OnEnable() => stageIdText.text = Spawner.StageToLoad.ToString();
}