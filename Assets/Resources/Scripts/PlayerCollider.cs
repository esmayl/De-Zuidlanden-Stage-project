using UnityEngine;
using System.Collections;

public class PlayerCollider : MonoBehaviour
{
    public Vector3 positionT;
    public Vector3 positionC;
    public GameObject[] spawnP;
    public float sphereRadius;
    public Gui guiD;
    public int buttonN;
    public float cdTime;
    public int layerH;
    public int layerT;
    public Bounds mesh;

    public void Start()
    {
        sphereRadius = 0.5f;
    }
    public void Update()
    {
       positionT= transform.position;
    }
   public  void getB(int b)
    {
        buttonN = b;
    }

    void OnTriggerStay(Collider other)
    {
        mesh = other.bounds;
        while (Physics.CheckSphere(transform.position, sphereRadius, 1))
        {
            transform.position = new Vector3(mesh.extents.x + transform.position.x, transform.position.y, transform.position.z + mesh.extents.z);
            transform.LookAt(other.transform);
            for (var i = 0; i < 3; i++)
            {
                var bn = i;
                guiD.RevoveTrigger(i);
            }
           
        }
       
    }
    /*void SetTrigger()
    {
        if (buttonN == 0)
        {
            for (var i = 0; i < guiD.guiButtons.wegenL.Count; i++)
            {
                guiD.guiButtons.wegenL[i].GetComponent<BoxCollider>().isTrigger = false;
            }
        }
        if (buttonN == 1)
        {
            for (var i = 0; i < guiD.guiButtons.huizenL.Count; i++)
            {
                guiD.guiButtons.huizenL[i].GetComponent<BoxCollider>().isTrigger = false;
            }
        } 
        if (buttonN == 2)
        {
            for (var i = 0; i < guiD.guiButtons.huizenL.Count; i++)
            {
                guiD.guiButtons.naturaL[i].GetComponent<BoxCollider>().isTrigger = false;
            }
        }
    }*/
}
