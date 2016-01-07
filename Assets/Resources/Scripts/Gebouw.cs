using UnityEngine;
using System;

/// <summary>
/// Scripts that holds the unique id and checks if the building is clicked.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class Gebouw : MonoBehaviour
{
    /**
     * @var buildingId
     * The id of this building
     * 
     * @var selectedBuilding
     * The currently selected building
     * 
     * @var selectionColor
     * The color used to show selection
     */

	public static GameObject selectedBuilding;
    public string buildingId = "0080200000379737";
    
    Color selectionColor;

    /// <summary>
    /// Checks if a renderer is attached if so, sample the main color and save it in selectionColor.
    /// Sets the main color to white after sampling.
    /// </summary>
    void Start()
    {
        if (transform.renderer != null)
        {
            transform.renderer.material.color = new Color(0.9f, 0.9f, 0.9f, 0);
            selectionColor = Color.white; 
        }
    }

    /// <summary>
    /// On mouse down send the building id to Database.cs
    /// </summary>
    void OnMouseUpAsButton()
    {

        Debug.Log("sending id: " + buildingId);
        Database.ChangeSelectedInfo(buildingId);

        ChangeSelected(transform.gameObject);
        selectedBuilding.renderer.material.color = selectionColor;
    }

    /// <summary>
    /// Checks if a building is selected(if yes set color to white) and set new building as selected
    /// </summary>
    /// <param name="selected">The newly selected building</param>
    static void ChangeSelected(GameObject selected)
    {
        if (selectedBuilding != null)
        {
            selectedBuilding.renderer.material.color = new Color(0.9f, 0.9f, 0.9f, 0);
        }
        selectedBuilding = selected;
    }
}
