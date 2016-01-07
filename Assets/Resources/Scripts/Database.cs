using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Linq;

/// <summary>
/// Helper class to access pre-defined XML files.
/// </summary>
public class Database : MonoBehaviour
{
    #region Doxygen declarations
    
    /**
     * @var buildingInfoDict
     * The dictionary that holds all ids and the corrosponding building info. (fill with "records" variable u method: AddToDictionary)
     * 
     * @var chatHistory 
     * The type that holds all the chat history. (changed with method: ReadXML)
     * 
     * @var records
     * Serialized type used to read the XML file.
     * 
     * @var serial
     * XmlSerializer used to read from XML
     * 
     * @var loaded
     * Shows if the XML download is done.
     * 
     * @var selectedId
     * The id of the selected building, can only be changed with ChangeSelectedInfo().
     * 
     * @var scaleFactor
     * The multiplier used to convert X and Y position from the XML.
     */

#endregion
    #region Public accessible variables

    public static Dictionary<string, Info> buildingInfoDict = new Dictionary<string, Info>();
	
    [SerializeField]
    public static ChatHistory chatHistory = new ChatHistory ();

    [SerializeField]
	public static Records records = new Records();
    public static XmlSerializer serial;

    public static bool loaded;
    public static string selectedId;
	public static float scaleFactor =500000;

	public string xmlPath ="http://www.jellyfishdm.com/table_export_adressen_Leeuwarden.xml";

    #endregion
    #region Unity functions
    /// <summary>
	/// Checks if the "table_export_adressen_Leeuwarden.xml" exists, if found read it to "records".
	/// Checks if the "ChatHistory.xml" exists, if found read it to chatHistory.
    /// </summary>
    public void Awake()
    {
        //StartCoroutine(ReadXML("http://www.jellyfishdm.com/ChatHistory.xml",typeof(ChatHistory)));
        StartCoroutine(ReadXML(xmlPath, typeof(Records)));
    }
    #endregion
    #region Custom functions
    /// <summary>
	/// Adds all Info from the "records" to "buildingInfoDict" dictionary.
    /// Only read position data until the first comma is found.
    /// </summary>
    public static void AddToDictionary()
    {
        Info info = new Info();
        float positionX;
        float positionY;
        string id;

        for (int i = 0; i < records.rows.Length; i++)
        {
            id = records.rows[i].adresId;
            info.adres = records.rows[i].adres;
            info.postcode = records.rows[i].postCode;
            info.woonplaats = records.rows[i].woonPlaats;

			int indexX = records.rows[i].x.IndexOf(",");
			int indexY = records.rows[i].y.IndexOf(",");

			if(indexX != -1)
			{
				positionX =float.Parse(records.rows[i].x.Substring(0,indexX));
			}
			else
			{
				positionX =float.Parse(records.rows[i].x);
			}

			if(indexY != -1)
			{
				positionY = float.Parse(records.rows[i].y.Substring(0,indexY));
			}
			else
			{
				positionY = float.Parse(records.rows[i].y);
			}

			info.position = new Vector3(positionX- scaleFactor/4, 0, positionY- scaleFactor);            

            if (buildingInfoDict.ContainsKey(id))
            {
                Debug.Log(string.Format(" Duplicate Key : {0} ", id));
            }
            else
            {
                buildingInfoDict.Add(id, info);
            }
        }
    }
    /// <summary>
    /// Change the shown info in the GUI to the selected info.
    /// </summary>
    /// <param name="newId">Id used to find the selected obj in the dictionary.</param>
    public static void ChangeSelectedInfo(string newId)
    {
        string oldId = selectedId;
        selectedId = newId;
        Info result = new Info();

        if (selectedId != oldId)
        {
            foreach (KeyValuePair<string, Info> pair in buildingInfoDict)
            {
                if (pair.Key == selectedId)
                {
                    result.adres = pair.Value.adres;
                    result.postcode = pair.Value.postcode;
                    result.woonplaats = pair.Value.woonplaats;
                    result.position = pair.Value.position;
                }
            }
            GuiEs.ShowInfo(result);
        }
    }    
    /// <summary>
    /// Returns the building associated with the Id.
    /// </summary>
    /// 
    /// @code
    /// public class Example : MonoBehaviour
    /// {
    ///     string path = "http://www.jellyfishdm.com/table_export_adressen_Leeuwarden.xml";
    ///     
    ///     StartCoroutine(ReadXML(path, typeof(Records)));
    ///     
    ///     Info buildingInfo = Database.FindBuilding(id); 
    ///     Debug.Log(string.format("{0},{1},{2},{3}",buildingInfo.adres,buildingInfo.postcode,buildingInfo.woonplaats,buildingInfo.position));
    /// }
    /// @endcode
    /// 
    /// <param name="Id">Id of the building.</param>
    /// <returns>Info found corresponding to the Id.</returns>
    public static Info FindBuilding(string Id)
    {
        Info result = new Info();

        if (buildingInfoDict.ContainsKey(Id))
        {
            foreach (KeyValuePair<string, Info> pair in buildingInfoDict)
            {
                if (pair.Key == Id)
                {
                    result.adres = pair.Value.adres;
                    result.postcode = pair.Value.postcode;
                    result.position = pair.Value.position;
                    result.woonplaats = pair.Value.woonplaats;
                    return result;
                }
            }
        }
        else
        {
            Debug.LogError("Not found");
            return result;
        }

        return new Info();
    }
    /// <summary>
    /// Reads the specified XML.
    /// </summary>
    /// works with url's and local paths(local paths dont work in webplayer).
    /// 
	/// @code
    /// public class Example : MonoBehaviour
    /// {
    /// 
    /// string buildingId = "0080200000490694";
    /// 
	/// ReadXML("www.domainname.com/table_export_adressen_Leeuwarden.xml",typeof(Records));//placed in Database.buildingInfoDict
    /// ReadXML("www.domainname.com/ChatHistory.xml", typeof(ChatHistory));//placed in Database.chatHistory
    /// ReadXML("ChatHistory.xml",typeof(ChatHistory));// points to projectPath\ChatHistory.xml next to projectPath\Assets
    /// 
    /// Info dictionaryElement = Database.FindBuilding(buildingId);
    /// Info dictionaryElementAlternate = Database.buildingInfoDict[buildingId];
    /// 
    /// }
	/// @endcode
	/// <param name="xmlName">Name of the XML to load.</param>
	/// <param name="type">The type of the info in the XML.</param>
    public static IEnumerator ReadXML(string url, System.Type type)
    {
        if (url.StartsWith("http"))
        {
            WWW w = new WWW(url);
            yield return w;

            if (w.isDone)
            {
                if (type == typeof(ChatHistory))
                {
                    Debug.Log("Loading history");
                    XmlSerializer serial = new XmlSerializer(typeof(ChatHistory));
                    StringReader reader = new StringReader(w.text);
                    chatHistory = (ChatHistory)serial.Deserialize(reader);
                }

                if (type == typeof(Records))
                {
                    Debug.Log("Loading Records");
                    XmlSerializer serial = new XmlSerializer(typeof(Records));
                    StringReader reader = new StringReader(w.text);
                    records = (Records)serial.Deserialize(reader);
                    loaded = true;
                    AddToDictionary();
                }
            }
        }
        else
        {
            FileStream file = new FileStream(Application.dataPath+"/"+url, FileMode.Open);
			if (type == typeof(Records))
            {
                XmlSerializer serial = new XmlSerializer(typeof(Records));
                records = (Records)serial.Deserialize(file);
                AddToDictionary();
                loaded = true;
            }
            if (type == typeof(ChatHistory))
            {
                XmlSerializer serial = new XmlSerializer(typeof(ChatHistory));
                chatHistory = (ChatHistory)serial.Deserialize(file);
            }
            Debug.Log("Loading Done!");
        
        }

       
    }
    /// <summary>
    /// Save an object like ChatHistory or Records to the xmlDoc.
    /// </summary>
    /// Only works local.
	/// 
	/// Example:
	/// @code
    /// public class Example : MonoBehaviour
    /// {
    /// 
    /// Bericht testBericht = new Bericht();
	/// testBericht.berichten = "Bericht";
	/// testBericht.datum = System.DataTime.now;
    /// 
	/// ChatHistory test = new ChatHistory();
    /// test.chatHistory = new List<Bericht>();
    /// test.chatHistory.Add(testBericht);
    /// 
    /// SaveXML("ChatHistory.xml",test,typeof(ChatHistory));
    /// 
    /// }
	/// @endcode
    /// 
    /// <param name="xmlDoc">Name of the XML document to save, if not existing creates new XML.</param>
    /// <param name="chatBericht">Chat bericht to save. (if has less than 1 element will create a empty XML)</param>
    /// <param name="type">type of the data to save(W.I.P), use typeof(ChatHistory).</param>
    public void SaveXML(string xmlDoc,ChatHistory chatBericht,System.Type type)
    {   
        FileStream file;
        XmlSerializer serial;

        if (File.Exists(xmlDoc)&& chatBericht.berichten.Count()>0)
        {
            file = new FileStream(xmlDoc, FileMode.Open);
            serial = new XmlSerializer(type);
            ChatHistory tempSave = (ChatHistory)serial.Deserialize(file);

            for (int i = 0; i < chatBericht.berichten.Count(); i++)
            {
                tempSave.berichten.Add(chatBericht.berichten[i]);
            }

            file.Dispose();
            file = new FileStream(xmlDoc, FileMode.Create);

            serial.Serialize(file,tempSave);
        }
        else
        {
            file = new FileStream(xmlDoc, FileMode.CreateNew);
            serial = new XmlSerializer(type);
            serial.Serialize(file, chatBericht);
        }
    }
    #endregion 
}        