using UnityEngine;
using System.Collections;

/// <summary>
/// Class placed on parent object of buildings with buildingId
/// </summary>
public class Positioning : MonoBehaviour
{
    #region Doxygen declaration

    /** 
     * @var positioned
     * If false, position child objects.
     */

    #endregion
    #region Public variables

    public bool positioned = false;

    #endregion
    #region Unity functions

    /// <summary>
    /// Position all child game objects corrosponding with their buildingId found in the Database.buildingInfoDict, if positioned is false. 
    /// </summary>
    void Update () 
    {       
        
        if (!positioned)
        {
            if (Database.loaded)
            {
                foreach (Transform t in transform)
                {
                    if (t.tag == "Huizen")
                    {
                        Debug.Log(t.name + " " + t.GetComponent<Gebouw>().buildingId);
                        if (t.GetComponent<Gebouw>())
                        {
                            Info temp = Database.FindBuilding(t.GetComponent<Gebouw>().buildingId);
                            t.position = temp.position;
                            Debug.Log(t.position);
                        }
                    }
                }
                positioned = true;
            }
        }


    }

    #endregion
}
