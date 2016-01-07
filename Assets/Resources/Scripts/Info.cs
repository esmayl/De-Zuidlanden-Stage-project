using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Struct used to hold the building information.
/// </summary>
[Serializable]
public struct Info
{
#region Doxygen declaration
    /**
     * @var adres
     * The adress of the building.
     * 
     * @var postcode
     * The zipcode of the building.
     * 
     * @var woonplaats
     * The city of the building.
     * 
     * @var position
     * The real world position of the building.
     */
#endregion

    [SerializeField]
    public string adres;

    [SerializeField]
    public string postcode;
    public string woonplaats;
    public Vector3 position;
    
    /// <summary>
    /// Creates a new instance of Info
    /// </summary>
    /// <param name="adres"></param>
    /// <param name="postcode"></param>
    /// <param name="woonplaats"></param>
    /// <param name="position"></param>
    public Info(string adres, string postcode, string woonplaats, Vector3 position)
    {
        this.adres = adres;
        this.postcode = postcode;
        this.woonplaats = woonplaats;
        this.position = position;
    }
}
