using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

/// <summary>
/// Serialized class that is used to save and load saved chat history.
/// </summary>
[Serializable]
[XmlRoot(ElementName = "ChatHistory")]
public class ChatHistory
{
#region Doxygen declaration
    /** 
     * @var berichten
     * List of messages.
     */
#endregion

    [XmlElement(ElementName = "Chat")]
    [SerializeField]
    public List<Bericht> berichten;
}

/// <summary>
/// Used by ChatHistory to store data that has to be saved or loaded.
/// </summary>
[Serializable]
public class Bericht
{
#region Doxygen declaration
    /**
     * @var datum
     * The date of the messages.
     * 
     * @var berichten
     * The messages.
     */

#endregion
    [XmlElement(ElementName = "Datum")]
    public string datum;

    [XmlElement(ElementName = "Bericht")]
    public string berichten;
}
