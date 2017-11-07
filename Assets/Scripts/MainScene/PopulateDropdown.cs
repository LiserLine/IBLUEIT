using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopulateDropdown : MonoBehaviour
{
    private Dropdown _dropdown;

    private void Start()
    {
        _dropdown = GetComponent<Dropdown>();

        switch (_dropdown.name)
        {
            case "DropdownYear":
                GenerateYears();
                break;

            default:
                Debug.LogError("Undefined dropdown!");
                break;
        }
    }

    private void GenerateYears()
    {
        var yearList = new List<string>();

        for (int i = DateTime.Today.Year; i >= 1970; i--)
        {
            yearList.Add(i.ToString());
        }

        _dropdown.AddOptions(yearList);
    }
}
