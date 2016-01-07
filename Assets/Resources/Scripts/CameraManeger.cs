using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** 
 * @Author Dirk Hulshof.
 * @date   februari, 2014.
 * @version v0.3.
 * @brief Consist of = Script Varianten van Ontwerp Modellen.
 */

public class CameraManeger : MonoBehaviour
{
    public Camera cFirst;
    public Camera cSecond;
    public float defaultW;
    public float defaultH;
    public float valueS;
    public float convertValue;
    public float counterValue;
    private string isNotSelected;
    public int oldMaskF;
    public int oldMaskS;
    public int hideMaskF;
    internal List<Rect> cameraDefaultValues = new List<Rect>();
    private int windowNumbers;

    void Awake()
    {
        
        oldMaskS = cSecond.cullingMask;
        oldMaskF = cFirst.cullingMask;
        //hideMaskF = 1 >> 4;
        var c1FullScreen = new Rect (0,0,1,1);
        var c1SliderDefaultH = new Rect(0, 0.5f, 1, 0.5f);
        var c2SliderDefaultH = new Rect(0, 0, 1, 0.5f);

        var c1SliderDefaultV = new Rect(0, 0, 0.5f, 1);
        var c2SliderDefaultV = new Rect(0.5f, 0, 0.5f, 1); 
        cameraDefaultValues.Add(c1FullScreen);
        cameraDefaultValues.Add(c1SliderDefaultH);
        cameraDefaultValues.Add(c2SliderDefaultH);
        cameraDefaultValues.Add(c1SliderDefaultV);
        cameraDefaultValues.Add(c2SliderDefaultV);
    }  
    public void Start()
    {
        defaultH = 0.5f;
        defaultW = 0.5f;
        counterValue = 0.5f;
        convertValue = 0.5f;
        cFirst.rect = cameraDefaultValues[0];
        cSecond.enabled = false;
    }
    /**
  * Switches the active camera.
  * @param cameraNumber The camera to use.
  * @param buttonName The name of the button that's not active.
  */
    public void UseCamera(int cameraNumber, string buttonName)
    {
        isNotSelected = buttonName;
        windowNumbers = cameraNumber;
        VariantenVanOnwerpModelen();
    }
    public void deselectCamera(int cameraN, string butName)
    {
        if(cameraN == 1)
            cFirst.cullingMask ^= (1 << LayerMask.NameToLayer(butName));
        if (cameraN == 2)
            cSecond.cullingMask ^= (1 << LayerMask.NameToLayer(butName));
    }
    /**
     * Function VariantenVanOntwerpModelen show/hide cullingmask's in repective camera.
     * @param windowNumbers camera to use.
     * @param buttonName Name of the button (name of culling mask) that are toggled "false".
     */
    private void VariantenVanOnwerpModelen()
    {
        if (windowNumbers == 1)
        {
            cFirst.cullingMask ^= (1 << LayerMask.NameToLayer(isNotSelected));
        }
        if (windowNumbers == 2)
        {
            cSecond.cullingMask ^= (1 << LayerMask.NameToLayer(isNotSelected));
        }
    }
    public void SliderToggle(bool toggle)
    {
        if (toggle)
        {        
             cSecond.enabled = true;           
        }
        if(!toggle)
        {
            cFirst.rect = cameraDefaultValues[0];
            cSecond.enabled = false;        
        } 
    }

    public void SliderBar (float sliderValeu, bool hor, bool sliderOn)
    {
        if (sliderOn)
        {       
            if (hor) // Horizontal 
            {
               // Debug.Log("H");
                convertValue = (sliderValeu / Screen.height);             
                counterValue = (1 - convertValue);
                cFirst.rect = new Rect(0, counterValue, 1, convertValue);
                cSecond.rect = new Rect(0, 0, 1, counterValue);
                //Debug.Log(convertValue +" Conv");
               // Debug.Log(counterValue + " Count");
            }
            if (!hor) // Vertical
            {
                convertValue = (sliderValeu / Screen.width);
                counterValue = (1 - convertValue);
                cFirst.rect = new Rect(0, 0, convertValue, 1);
                cSecond.rect = new Rect(convertValue, 0, counterValue, 1);
            }
        }
    }

    public void ZoomIn(int cameraNumber, float zoom)
    {
        valueS = zoom;
        if (cameraNumber == 1)
        {
            cFirst.fieldOfView = valueS;
        }
        if (cameraNumber == 2)
        {
            cSecond.fieldOfView = valueS;
        }
    }
}