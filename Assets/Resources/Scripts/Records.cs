using System.Xml.Serialization;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Serialized class used for reading building info XML
/// </summary>
[Serializable]
[XmlRoot(ElementName = "results")]
public class Records
{
    /// <summary>
    /// Array of building info.
    /// </summary>
    [XmlElement(ElementName = "row")]
    public GebouwStats[] rows; 
}

/// <summary>
/// Serialized class used by Records and are used to store the data.
/// </summary>
[Serializable]
public class GebouwStats
{
#region Doxygen declaration
    /**
     * @var adresId
     * Id of the building.
     * 
     * @var adres
     * Adres of the building.
     * 
     * @var postCode
     * Postcode of the building.
     * 
     * @var woonPlaats
     * Woonplaats of the building.
     * 
     * @var x
     * The X-position in real life, based on RD-new coordinate system.
     * 
     * @var y
     * The Y-position in real life, based on RD-new coordinate system.
     */
#endregion

    [XmlElement(ElementName = "ADRESID")]
	[SerializeField]
    public string adresId; 

    [XmlElement(ElementName = "ADRES")]
    public string adres; 

    [XmlElement(ElementName = "POSTCODE")]
    public string postCode; 

    [XmlElement(ElementName = "WOONPLAATS")]
    public string woonPlaats;

    [XmlElement(ElementName = "X")]
	[SerializeField]
    public string x;
    
    [XmlElement(ElementName = "Y")]
	[SerializeField]
    public string y;
}