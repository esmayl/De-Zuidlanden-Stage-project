using UnityEngine;
using System.Collections;

public class lightOn : MonoBehaviour {

    public TOD_Sky sky;
    public Texture2D nightTexture;
    public Texture2D dayTexture;

    Color dayColor = new Color();
    Color nightColor = new Color();
    bool lerping = true;

	// Use this for initialization
	void Start () {
        sky = GameObject.Find("Sky Dome").GetComponent<TOD_Sky>();
        dayColor = RenderSettings.ambientLight;

        //calculate percentage of 255 , rgb = 38 which is 7.5 percent of 255
        nightColor = new Color((255 / 100 * 7.5f) / 100, (255 / 100 * 7.5f) / 100, (255 / 100 * 7.5f) / 100);
	}
	
	// Update is called once per frame
	void Update () 
    {

        Color oldColor = RenderSettings.ambientLight;

        if (sky.IsNight)
        {
            //Debug.Log("NIGHT");
            if (RenderSettings.ambientLight != nightColor)
            {
                RenderSettings.ambientLight = Color.Lerp(oldColor, nightColor, Time.deltaTime);
            }

        }

        if (sky.IsDay)
        {
            //Debug.Log("DAY");
            if (RenderSettings.ambientLight != dayColor)
            {
                RenderSettings.ambientLight = Color.Lerp(oldColor, dayColor, 100);
            }
        }




        //while(sky.LerpValue < 0.4f && lerping)
        //{
        //    if (sky.LerpValue > 0.25f)
        //    {
        //        transform.GetChild(0).gameObject.SetActive(true);
        //        transform.renderer.material.mainTexture = nightTexture;
        //        RenderSettings.ambientLight = Color.Lerp(dayColor, nightColor, 10);
        //    }
        //    if (sky.LerpValue < 0.25f)
        //    {
        //        lerping = false;
        //    }
        //
        //}
        
        //if (sky.IsDay)
        //{
        //    transform.GetChild(0).gameObject.SetActive(false);
        //    transform.renderer.material.mainTexture = dayTexture;
        //    RenderSettings.ambientLight = dayColor;
        //}
	
	}

}
