using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulateDropdownYear : MonoBehaviour
{
    [SerializeField]
    private Dropdown dropdown;

    private void Start() => FillWithYears();

    private void FillWithYears()
    {
        var yearList = new List<string>();

        for (var i = DateTime.Today.Year; i >= 1970; i--)
            yearList.Add(i.ToString());

        dropdown.AddOptions(yearList);
    }
}
