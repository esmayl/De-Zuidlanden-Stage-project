using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
/**
 * @Author Dirk Hulshof.
 * @date   februari, 2014.
 * @version v0.4.
 * @section DESCRIPTION
 * 
 * GUI Script voor "Variante van ontwerp modellen" + "Slider button's" + "Zoom in/out"
 */
public class Gui : MonoBehaviour
{
    public CameraManeger cameraManeger;
    public PlayerCollider playerC;
    public GUISkin skins;
    public Event e;

    internal List<GuiButtonss> ButtonGUI = new List<GuiButtonss>();
    internal GuiButtonss guiButtons = new GuiButtonss();
    public float vSliderValue;
    public float hSliderValue;
    internal float sliderV;
    internal float sliderH;
    public float value1F;
    public float value2F;
    public float fov;
    public bool stop;
    public Vector3 cPosition;
    public Vector2 scrollPositionSelected = Vector2.zero;
    public Vector2 scrollPosition = Vector2.zero;
    public Vector2 scrollVarianten = Vector2.zero;
    public Vector2 scrollVariantenC2 = Vector2.zero;
    public Vector2 scrollVariantSelected = Vector2.zero;

    public string buttonName;
    internal string hov;
    public string value1S = string.Empty;
    public string value2S = string.Empty;
    public string textMenu; // Text dat ann hooft van Menu komt
    public string nameToToggle;
    public string lname;
    public string valueZ;
    public string v = string.Empty;
    public string toolTips;
    public string[] bNC;
    public string[] wegen;
    public string[] huizen;
    public string[] natuur;
    public string[] tooltipS;

    public int counter;
    public int textureN;
    internal int buttonNumber;
    public int buttonNumberV;
    internal int windowNumber;
    public int buttonNum; //config buttons number;

    public int textWidth;
    public int textHeight;
    internal bool buttonH;
    public bool onGUI;
    public bool windowsLoaded;
    public bool loadingVariante;
    public bool sliderOn;
    public bool HorOrVert;
    public bool V;
    public bool H;
    public bool configButton;
    public bool selectS;
    public bool okCloseMenu = false;
    public bool textLayout1;
    public bool textLayout2;
    public bool okM; // bool for ok presse in window 6 (ok/cancel menu)
    public bool cancelM; //
    public bool menuV; // bool die het menu varianten van ontwerp modeln ac/deac.
    public bool menuV2;
    public bool enterPressed;
    public bool okButton;
    public bool mouseC;
    public bool Vwindow;
    public GameObject[] goArray;
    internal GUIStyle[] buttonsAc;
    internal GUIStyle presetN;
    internal GUIStyle openclose;
    internal GUIStyle confirm;
    internal GUIStyle cancel;
    internal GUIStyle VBname;
    internal GUIStyle Vtoggle;
    internal GUIStyle lagenB;
    internal GUIStyle zoomT;
    internal GUIStyle vertical;
    internal GUIStyle horizontal;
    internal GUIStyle presetBin;
    internal GUIStyle presetBac;
    internal GUIStyle preset;
    internal GUIStyle setingB;
    internal GUIStyle window1;
    internal GUIStyle sliderT;
    internal GUIStyle hideW;
    internal GUIStyle defaultB;
    internal GUIStyle toggle;
    internal GUIStyle buttonNameSkin;
    internal GUIStyle vSliderBar;
    internal GUIStyle hSliderBar;
    internal GUIStyle vSliderThumb;
    internal GUIStyle hSliderThumb;
    internal GUIStyle okB;
    internal GUIStyle horizontalB;
    internal GUIStyle verticalB;
    internal GUIStyle zoomtxt;
    internal GUIStyle window4;
    internal GUIStyle huidigS;
    internal GUIStyle toekomstigS;
    internal GUIStyle plus;
    internal GUIStyle minus;
    internal GUIStyle TextL;
    internal GUIStyle textW1;
    internal Rect buttonHr;
    internal Rect fixW1;
    internal Rect fixW2;
    internal Rect fixM1; // Window voor Meunu knop (Varianten van ontwerp modelen)
    internal Rect fixL1;

    private bool hide =true;
    private float openingSpeed = 10;

    /**
     * Function Start - 
     * @param
     */
    void Awake()
    {
        fov = 60f;
        stop = false;
        H = false;
        V = false;
        cancelM = false;
        okM = false;
        textLayout1 = true;
        textLayout2 = false;
        enterPressed = false;
        value2S = "60";
        value1S = "60";
        selectS = false;
        sliderOn = false;
        loadingVariante = true;
        windowsLoaded = false;
        onGUI = false;

    }
    void Start()
    {
        value1F = 60.5f;
        textureN = 0;
        fixW1 = new Rect(0 - Screen.width * 0.088451f, 0, Screen.width * 0.088451f, Screen.height * 0.23148f);
        fixW2 = new Rect((Screen.width - fixW2.width), 0, Screen.width * 0.0906f, Screen.height * 0.10f);
        fixM1 = new Rect((Screen.width / 2 - fixM1.width / 2), (0 + fixM1.height/2), Screen.width * 0.0906f, Screen.height * 0.10f);
        fixL1 = new Rect(fixW1.width + 5f, 0, textWidth, textHeight);

        guiButtons.selectedLayers.Add("HWegen");
        guiButtons.selectedLayers.Add("HHuizen");
        guiButtons.selectedLayers.Add("HNatura");
        guiButtons.selectedLayers.Add("TWegen");
        guiButtons.selectedLayers.Add("THuizen");
        guiButtons.selectedLayers.Add("TNatura");
        bNC = new string[3];
        bNC[0] = "WEGEN";
        bNC[1] = "HUIZEN";
        bNC[2] = "NATURA";

            wegen = new string[2];
        wegen[0] = "HWegen";
        wegen[1] = "TWegen";
             huizen = new string[2];
        huizen[0] = "HHuizen";
        huizen[1] = "THuizen";
             natuur = new string[2];
        natuur[0] = "HNatura";
        natuur[1] = "TNatura";
            tooltipS = new string[3];
        tooltipS[0] = "Schakelt de wegen aan/uit";
        tooltipS[1] = "Schakelt de huizen aan/uit";
        tooltipS[2] = "Schakelt de bomen aan/uit";
        for (var i = 0; i < 6; i++)
        {
            guiButtons.buttonListc1.Add(false);
        }
        for (var i = 0; i < 6; i++)
        {
            guiButtons.CBShowHide.Add(false);
        }

        buttonNumber = guiButtons.buttonListc1.Count;
        TextL = skins.FindStyle("TextL");
        plus = skins.FindStyle("plus");
        minus = skins.FindStyle("min");
        window1 = skins.FindStyle("window1");
        zoomtxt = skins.FindStyle("ValueZoom");
        vSliderBar = skins.FindStyle("vSliderBar");
        vSliderThumb = skins.FindStyle("vSliderThumb");
        hSliderBar = skins.FindStyle("hSliderBar");
        hSliderThumb = skins.FindStyle("hSliderThumb");
        buttonNameSkin = skins.FindStyle("buttonName");
        toggle = skins.FindStyle("toggle");
        hideW = skins.FindStyle("HideW");
        okB = skins.FindStyle("Ok");
        horizontalB = skins.FindStyle("HorizontalB");
        verticalB = skins.FindStyle("VerticalB");
        setingB = skins.FindStyle("setingB");
        presetBac = skins.FindStyle("presetBac");
        horizontal = skins.FindStyle("horizontal");
        vertical = skins.FindStyle("vertical");
        zoomT = skins.FindStyle("zoomT");
        lagenB = skins.FindStyle("lagenB");
        Vtoggle = skins.FindStyle("Vtoggle");
        huidigS = skins.FindStyle("huidigS");
        toekomstigS = skins.FindStyle("toekomstig");
        presetN = skins.FindStyle("presetN");
        textW1 = skins.FindStyle("textW1");
        buttonsAc = new GUIStyle[8];        
        hSliderValue = Screen.height / 2;
        vSliderValue = Screen.width / 2;
		windowsLoaded = true;
        
       /* goArray = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        for (int i = 0; i < goArray.Length; i++)
        {
            if (goArray[i].GetComponent<BoxCollider>())
            {
                if (goArray[i].layer == 12 || goArray[i].layer == 9)
                {
                    guiButtons.huizenL.Add(goArray[i]);
                }
                if (goArray[i].layer == 11 || goArray[i].layer == 10)
                {
                    guiButtons.naturaL.Add(goArray[i]);
                }

                if (goArray[i].layer == 8 || goArray[i].layer == 14)
                {
                    guiButtons.wegenL.Add(goArray[i]);
                }
            }
        }
        for (var i = 0; i < guiButtons.huizenL.Count; i++)
        {
            if (guiButtons.huizenL[i].layer == 12)
            {
                guiButtons.huizenL[i].GetComponent<BoxCollider>().enabled = true;
            }
        }
        for (var i = 0; i < guiButtons.wegenL.Count; i++)
        {
            if (guiButtons.wegenL[i].layer == 11)
            {
                guiButtons.wegenL[i].GetComponent<BoxCollider>().enabled = true;
            }
        }
        for (var i = 0; i < guiButtons.naturaL.Count; i++)
        {
            if (guiButtons.naturaL[i].layer == 14)
            {
                guiButtons.naturaL[i].GetComponent<BoxCollider>().enabled = true;
            }
        }*/
    }
    /**
     * Function OnGui creates GUI windows to be filled with Buttons.
     * @param windowNumber add to respective windows.asd
     */
    void OnGUI()
    {
        GUI.skin = skins;
        Event e = Event.current;
        GUI.Label(fixM1, "HUIDIG", TextL);
        var depthG = GUI.depth;
        if (!sliderOn)
        {
            fixM1 = new Rect((Screen.width / 2 - fixM1.width / 2), (0 + fixM1.height/2), Screen.width * 0.0906f, Screen.height * 0.10f);
        
        }
        if (menuV || menuV2)
        {
            GUI.Window(3, fixM1, VariantenCheckBox, "LAGEN INSTELLEN", "window4"); 
            if (fixM1.Contains(e.mousePosition))
            {
                onGUI = true;
            }
            else
                onGUI = false;
            if (Input.GetMouseButtonDown(0) && !fixM1.Contains(e.mousePosition))
            {
                menuV = false;
            }
        }
        GUI.Window(1, fixW1, VariantenCheckBox, "", "window1");
        if (fixW1.Contains(e.mousePosition))
        {
            onGUI = true;
        }
        else
            onGUI = false;

        if (sliderOn)
        {
            GUI.Label(fixW2, "TOEKOMSTIG", TextL);
            if (H)
            {
                fixM1 = new Rect((Screen.width / 2 - fixM1.width/2), 0 + fixM1.height/2, Screen.width * 0.0906f, Screen.height * 0.10f);
                fixW2 = new Rect(Screen.width /2 - fixW2.width /2, Screen.height - fixW2.height, Screen.width * 0.0906f, Screen.height * 0.10f);
                if (fixW2.Contains(e.mousePosition))
                {
                    onGUI = true;
                }
                else
                    onGUI = false;

                buttonH = GUI.Button(new Rect(0, hSliderValue - (Screen.height * 0.04457f / 2), Screen.width, Screen.height * 0.09907f), "", "HorizontalB");
                buttonHr = new Rect(0, hSliderValue - (Screen.height * 0.04457f / 2), Screen.width, Screen.height * 0.09907f);
                if (buttonHr.Contains(e.mousePosition) && Input.GetMouseButtonDown(0))
                {
                    selectS = true;
                }
                if (selectS)
                {
                    hSliderValue = e.mousePosition.y;
                    if (Input.GetMouseButtonUp(0))
                    {
                        selectS = false;
                    }
                }
            }
            if (V)
            {
                fixM1 = new Rect((Screen.width / 4 - fixM1.width), Screen.height - fixM1.height, Screen.width * 0.0906f, Screen.height * 0.10f);
                fixW2 = new Rect((Screen.width-Screen.width / 4), Screen.height - fixW2.height, Screen.width * 0.0906f, Screen.height * 0.10f);
                if (fixW2.Contains(e.mousePosition))
                {
                    onGUI = true;
                }
                else
                    onGUI = false;

                buttonH = GUI.Button(new Rect(vSliderValue - (Screen.width * 0.02604f / 2), 0, Screen.width * 0.02604f, Screen.height), "", "VerticalB");
                buttonHr = new Rect(vSliderValue - (Screen.width * 0.02604f / 2), 0, Screen.width * 0.02604f, Screen.height);
                if (buttonHr.Contains(e.mousePosition) && Input.GetMouseButtonDown(0))
                {
                    selectS = true;
                }
                if (selectS)
                {
                    vSliderValue = e.mousePosition.x;
                    if (Input.GetMouseButtonUp(0))
                    {
                        selectS = false;
                    }
                }
            }
        }
      
        if (windowsLoaded)
        {
            SetWindows();
        }
        if (toolTips != "")
        {           
            var tLength = toolTips.Length;
            //Debug.Log(tLength);
            GUI.Label(new Rect(fixW1.width + 5f, e.mousePosition.y - skins.label.fixedHeight / 2, tLength * skins.label.fontSize * 0.6f, 40), toolTips);
          //  depthG = 1;
        }      
        //Debug.Log(sliderOn);
    }
    public void SetWindows()
    {
		skins.horizontalScrollbarThumb.fixedHeight = skins.horizontalScrollbar.fixedHeight;
		skins.horizontalScrollbarThumb.fixedWidth = (skins.horizontalScrollbar.fixedWidth * 0.15f);
		skins.button.fixedHeight = (fixW1.height * 0.1f);
		skins.button.fixedWidth = (fixW1.width * 0.5f - (skins.button.margin.left + skins.button.margin.right));
		hSliderBar.fixedHeight = Screen.height;
		hSliderBar.fixedWidth = 15f;
		hSliderThumb.fixedHeight = Screen.height;
		hSliderThumb.fixedWidth = 15f;
		vSliderThumb.fixedWidth = Screen.width;
		vSliderThumb.fixedHeight = 15f;
		vSliderBar.fixedWidth = Screen.width;
		vSliderBar.fixedHeight = 15f;

		toggle.fixedHeight = (fixW2.height * 0.08f);
		toggle.fixedWidth = (fixW2.width * 0.2f);
		toggle.padding.left = (int)(skins.toggle.fixedWidth + 10);
		hideW.fixedHeight = fixW1.height * 0.1f;
		hideW.fixedWidth = fixW1.width * 0.2f;
		skins.horizontalScrollbar.fixedHeight = fixW1.height * 0.1f;
		skins.horizontalScrollbar.fixedWidth = fixW1.width * 0.8f;
		horizontalB.fixedHeight = Screen.height *0.04457f;
		horizontalB.fixedWidth = Screen.width;
		verticalB.fixedHeight = Screen.height;
        verticalB.fixedWidth = Screen.width * 0.02604f;
		horizontal.fixedHeight = (fixW1.height * 0.13441f);
        horizontal.fixedWidth = fixW1.width * 0.44705f;
		vertical.fixedHeight = 0.13441f * fixW1.height;
		vertical.fixedWidth = fixW1.width * 0.44705f;
        presetBac.fixedHeight = fixW1.height * 0.136F;
		presetBac.fixedWidth = fixW1.width * 0.91764f;
		huidigS.fixedHeight = fixW1.height * 0.09677f;
		huidigS.fixedWidth = fixW1.width;
        plus.fixedHeight = fixW1.height * 0.08f;
        plus.fixedWidth = fixW1.width * 0.1176f;
        minus.fixedHeight = fixW1.height * 0.08f;
        minus.fixedWidth = fixW1.width * 0.1176f;
		lagenB.fixedHeight = fixW1.height * 0.0914f;
		lagenB.fixedWidth = fixW1.width;
        TextL.fixedHeight = fixM1.height;
        TextL.fixedWidth = fixM1.width;
        zoomtxt.fixedWidth = fixW1.width * 0.2352f ;
        zoomtxt.fixedHeight = fixW1.height * 0.08f;
		toekomstigS.fixedHeight = fixW1.height * 0.09677f;
		toekomstigS.fixedWidth = fixW1.width * 0.825f;
		presetN.fixedHeight = fixM1.height * 0.104f;
		presetN.fixedWidth = fixM1.width * 0.2930f;
        textW1.fixedHeight = fixW1.height * 0.052f;
        textW1.fixedWidth = fixW1.width * 0.55882f;
        zoomT.fixedHeight = fixW1.height * 0.08f;
        zoomT.fixedWidth = fixW1.width * 0.2941f;
        //Debug.Log(Screen.width);
        if (Screen.width <= 1400)
        {
            zoomT.fontSize = 15;
            zoomtxt.fontSize = 15;
        }
        else
        {
            zoomT.fontSize = 17;
            zoomtxt.fontSize = 17;
        }
		for (var i = 0; i < 8; i++)
		{
			buttonsAc[i] = new GUIStyle(presetBac);
		}
        windowsLoaded = false;
    }
    /**
     * Function VaraiantenCheckBox creates toggle button's in repective windows, checks witch ones are set to "false". Creates (Ok) button to set the respective chosen culling mask.
     * @param windowNumber Number of the respective window in GUI, yoused to separate diferent GUI windows & load window number with function.
     * @param allOptions Toggle button to enable/disable all button's created in respective GUI window's.
     * @param GUILayout.Button(OK) Checks witch buttons are toggle(false), and sends repective buttons names to Camera Maneger + GUI windowNumber "UseCamera ()". 
     */
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) { hide = !hide; }


        if (hide)
        {
            fixW1.x = Mathf.Clamp(fixW1.x += openingSpeed, (0 - Screen.width * 0.088451f), 0);
        }
        else
        {
            fixW1.x = Mathf.Clamp(fixW1.x -= openingSpeed, (0 - Screen.width * 0.088451f), 0);
        }


        if (!V && !H)
        {
            sliderOn = false;
            cameraManeger.SliderToggle(false);
        }
        var smooth = 20f;
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && onGUI==false || Input.GetAxis("Mouse ScrollWheel") > 0 && onGUI == false)
        {
            fov += Input.GetAxis("Mouse ScrollWheel") * smooth;
            fov = Mathf.Clamp(fov, 29.75f, 60f);         
            value1F = fov;
            var zviews = fov;
            cameraManeger.ZoomIn(1, zviews);
            cameraManeger.ZoomIn(2, zviews);
        }
        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            //Debug.Log("+");
            fov = fov - 1;
            fov = Mathf.Clamp(fov, 29.75f, 60f);
            value1F = fov;
            var zviews = fov;
            cameraManeger.ZoomIn(1, zviews);
            cameraManeger.ZoomIn(2, zviews);

        }
        if (Input.GetKey(KeyCode.KeypadMinus))
        {
            //Debug.Log("-");
            fov = fov + 1;
            fov = Mathf.Clamp(fov, 29.75f, 60f);
            value1F = fov;
            var zviews = fov;
            cameraManeger.ZoomIn(1, zviews);
            cameraManeger.ZoomIn(2, zviews);
        }
    }
    public void VariantenCheckBox(int windowNumber)
    {   
        var screenH = fixW1.height;
        var screenW = fixW1.width;
        GUILayout.Space(screenH * 0.056f);
        if (windowNumber == 1)
        {

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("WEERGAVE", "textW1");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(screenH * 0.064f);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            for (var i = 0; i < 3; i++)
            {
                GUILayout.Space(screenH * 0.012f);
                if (GUILayout.Button(new GUIContent( bNC[i], tooltipS[i]), buttonsAc[i]))
                {                                  
                    if (!guiButtons.CBShowHide[i])
                    {
                        guiButtons.CBShowHide[i] = true;
                        buttonsAc[i].normal.background = buttonsAc[i].onNormal.background;
                        buttonsAc[i].hover.background = buttonsAc[i].onHover.background;
                        buttonsAc[i].normal.textColor = buttonsAc[i].onNormal.textColor;
                        MaskToCamera(i); 
                    }
                    else
                    {
                        guiButtons.CBShowHide[i] = false;
                        buttonsAc[i].normal.background = buttonsAc[i].focused.background;
                        buttonsAc[i].hover.background = buttonsAc[i].onHover.background;
                        buttonsAc[i].normal.textColor = buttonsAc[i].focused.textColor;
                        MaskToCamera(i);                                            
                    }
                }
            }
     
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(screenH * 0.048f);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
           if (H = GUILayout.Toggle(H,new GUIContent( "", "Huidige en Toekomstige situatie onder elkaar"), "horizontal"))
            {
                sliderOn = true;
                //Debug.Log(H);
                cameraManeger.SliderToggle(H);
                cameraManeger.SliderBar(hSliderValue, true, H);
                if (V)
                {
                    V = false;
                }              
            } 
            GUILayout.Space(2f);
            if (V = GUILayout.Toggle(V, new GUIContent("", "Huidige en Toekomstige situatie naast elkaar"), "vertical")) 
            {
                cameraManeger.SliderToggle(V);
                sliderOn = true;
                cameraManeger.SliderBar(vSliderValue, false, V);
                if (H)
                {
                    H = false;
                }          
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(screenH * 0.072f);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("ZOOM", "zoomT");
            GUILayout.FlexibleSpace();
            valueZ = ((int)((((value1F * 100) / 60.5f) - 200) * -1)).ToString();
            valueZ = GUILayout.TextField(valueZ + "%", 4, "ValueZoom");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("", "Zoom in"),plus))
            {
                //Debug.Log("+");
                fov = fov - 1;
                fov = Mathf.Clamp(fov, 29.75f, 60f);
                value1F = fov;
                var zviews = fov;
                cameraManeger.ZoomIn(1, zviews);
                cameraManeger.ZoomIn(2, zviews);
            }
            GUILayout.Space(screenW * 0.017f);
            if (GUILayout.Button(new GUIContent("", "Zoom uit"),minus))
            {
                //Debug.Log("-");
                fov = fov + 1;
                fov = Mathf.Clamp(fov, 29.75f, 60f);
                value1F = fov;
                var zviews = fov;
                cameraManeger.ZoomIn(1, zviews);
                cameraManeger.ZoomIn(2, zviews);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(screenH * 0.072f);
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
        }
        if (Event.current.type == EventType.Repaint)
            toolTips = GUI.tooltip;
    }
    public void RevoveTrigger(int bnumm)
    {
        switch (bnumm)
        {
            case 0:
                for (var i = 0; i < guiButtons.wegenL.Count; i++)
                {
                    if (sliderOn)
                    {
                       // guiButtons.wegenL[i].GetComponent<BoxCollider>().enabled = false;
                        guiButtons.wegenL[i].GetComponent<BoxCollider>().isTrigger = false;
                    }
                    else
                    {
                        if (guiButtons.wegenL[i].layer == 11)
                        {
                         //   guiButtons.wegenL[i].GetComponent<BoxCollider>().enabled = false;
                            guiButtons.wegenL[i].GetComponent<BoxCollider>().isTrigger = false;
                        }
                        else
                        {
                           // guiButtons.wegenL[i].GetComponent<BoxCollider>().enabled = false;
                            guiButtons.wegenL[i].GetComponent<BoxCollider>().isTrigger = false;
                        }
                    }
                }
                break;
            case 1:
                for (var i = 0; i < guiButtons.huizenL.Count; i++)
                {
                    if (sliderOn)
                    {
                      //  guiButtons.huizenL[i].GetComponent<BoxCollider>().enabled = false;
                        guiButtons.huizenL[i].GetComponent<BoxCollider>().isTrigger = false;
                    }
                    else
                    {
                        if (guiButtons.huizenL[i].layer == 12)
                        {
                            //guiButtons.huizenL[i].GetComponent<BoxCollider>().enabled = false;
                            guiButtons.huizenL[i].GetComponent<BoxCollider>().isTrigger = false;
                        }
                        else
                        {
                           // guiButtons.huizenL[i].GetComponent<BoxCollider>().enabled = false;
                            guiButtons.huizenL[i].GetComponent<BoxCollider>().isTrigger = false;
                        }
                    }


                }
                break;
            case 2:
                for (var i = 0; i < guiButtons.naturaL.Count; i++)
                {
                    if (sliderOn)
                    {
                      //  guiButtons.naturaL[i].GetComponent<BoxCollider>().enabled = false;
                        guiButtons.naturaL[i].GetComponent<BoxCollider>().isTrigger = false;
                    }
                    else
                    {
                        if (guiButtons.naturaL[i].layer == 14)
                        {
                           // guiButtons.naturaL[i].GetComponent<BoxCollider>().enabled = false;
                            guiButtons.naturaL[i].GetComponent<BoxCollider>().isTrigger = false;
                        }
                        else
                        {
                          //  guiButtons.naturaL[i].GetComponent<BoxCollider>().enabled = false;
                            guiButtons.naturaL[i].GetComponent<BoxCollider>().isTrigger = false;
                        }
                    }

                }
                break;
        }
    }
    public void RemoveCollider(int bN)
    {
        switch (bN)
        {
            case 0:
                for (var i = 0; i < guiButtons.wegenL.Count; i++)
                {
                    if (H || V)
                    {
                        guiButtons.wegenL[i].GetComponent<BoxCollider>().enabled = false;
                     //   guiButtons.wegenL[i].GetComponent<BoxCollider>().isTrigger = false;
                    }
                    if (!H && !V)
                    {
                        if (guiButtons.wegenL[i].layer == 11)
                        {
                            guiButtons.wegenL[i].GetComponent<BoxCollider>().enabled = false;
                         //   guiButtons.wegenL[i].GetComponent<BoxCollider>().isTrigger = false;
                        }
                        else
                        {
                            guiButtons.wegenL[i].GetComponent<BoxCollider>().enabled = false;
                          //  guiButtons.wegenL[i].GetComponent<BoxCollider>().isTrigger = false;
                        }
                    }
                }
                break;
            case 1:
                for (var i = 0; i < guiButtons.huizenL.Count; i++)
                {
                    if (H || V)
                    {
                        guiButtons.huizenL[i].GetComponent<BoxCollider>().enabled = false;
                       // guiButtons.huizenL[i].GetComponent<BoxCollider>().isTrigger = false;
                    }
                    if (!H && !V)
                    {
                        if (guiButtons.huizenL[i].layer == 12)
                        {
                            //Debug.Log("asda");
                            guiButtons.huizenL[i].GetComponent<BoxCollider>().enabled = false;
                          //  guiButtons.huizenL[i].GetComponent<BoxCollider>().isTrigger = false;
                        }
                        else
                        {
                            guiButtons.huizenL[i].GetComponent<BoxCollider>().enabled = false;
                          //  guiButtons.huizenL[i].GetComponent<BoxCollider>().isTrigger = false;
                        }
                    }
                    
                        
                }
                break;
            case 2:
                for (var i = 0; i < guiButtons.naturaL.Count; i++)
                {
                    if (H || V)
                    {
                        guiButtons.naturaL[i].GetComponent<BoxCollider>().enabled = false;
                      //  guiButtons.naturaL[i].GetComponent<BoxCollider>().isTrigger = false;
                    }
                    if (!H && !V)
                    {
                        if (guiButtons.naturaL[i].layer == 14)
                        {
                            guiButtons.naturaL[i].GetComponent<BoxCollider>().enabled = false;
                        //    guiButtons.naturaL[i].GetComponent<BoxCollider>().isTrigger = false;
                        }
                        else
                        {
                          //  guiButtons.naturaL[i].GetComponent<BoxCollider>().enabled = false;
                         //   guiButtons.naturaL[i].GetComponent<BoxCollider>().isTrigger = false;
                        }
                    } 
                   
                }
                break;
        }
    }
    public void AadTrigger(int bnum)
    {
        switch (bnum)
        {
            case 0:
                for (var i = 0; i < guiButtons.wegenL.Count; i++)
                {
                    if (H || V)
                    {
                        //Debug.Log("NO");
                        guiButtons.wegenL[i].GetComponent<BoxCollider>().isTrigger = true;
                    }
                    if (!H && !V)
                    {
                        if (guiButtons.wegenL[i].layer == 11)
                        {
                            guiButtons.wegenL[i].GetComponent<BoxCollider>().isTrigger = true;
                        }
                        else
                        {
                            guiButtons.wegenL[i].GetComponent<BoxCollider>().isTrigger = true;
                        }
                    }
                }

                break;
            case 1:
                for (var i = 0; i < guiButtons.huizenL.Count; i++)
                {
                    if (H || V)
                    {
                        guiButtons.huizenL[i].GetComponent<BoxCollider>().isTrigger = true;
                    }
                    if (!H && !V)
                    {
                        if (guiButtons.huizenL[i].layer == 12)
                        {
                        }
                        else
                        {
                            guiButtons.huizenL[i].GetComponent<BoxCollider>().isTrigger = true;
                        }
                    }
                }
                break;
            case 2:
                for (var i = 0; i < guiButtons.naturaL.Count; i++)
                {
                    if (H || V)
                    {
                        guiButtons.naturaL[i].GetComponent<BoxCollider>().isTrigger = true;
                    }
                    if (!H && !V)
                    {
                        if (guiButtons.naturaL[i].layer == 14)
                        {
                        }
                        else
                        {
                            guiButtons.naturaL[i].GetComponent<BoxCollider>().isTrigger = true;
                        }
                    }
                }
                break;

        }
    }
    public void AadCollider(int Bn)
    {
        switch (Bn)
        {
            case 0:
                for (var i = 0; i < guiButtons.wegenL.Count; i++)
                {
                    if (H || V)
                    {                      
                        guiButtons.wegenL[i].GetComponent<BoxCollider>().enabled = true;
                        //guiButtons.wegenL[i].GetComponent<BoxCollider>().isTrigger = true;
                    }
                    if (!H && !V)
                    {
                        if (guiButtons.wegenL[i].layer == 11)
                        {
                            guiButtons.wegenL[i].GetComponent<BoxCollider>().enabled = false;
                        }
                        else
                        {
                            guiButtons.wegenL[i].GetComponent<BoxCollider>().enabled = true;
                          //  guiButtons.wegenL[i].GetComponent<BoxCollider>().isTrigger = true;
                        }
                    }
                }
                
                break;
            case 1:
                for (var i = 0; i < guiButtons.huizenL.Count; i++)
                {
                    if (H || V)
                    {
                        guiButtons.huizenL[i].GetComponent<BoxCollider>().enabled = true;
                      //  guiButtons.huizenL[i].GetComponent<BoxCollider>().isTrigger = true;
                    }
                    if (!H && !V)
                    {
                        if (guiButtons.huizenL[i].layer == 12)
                        {
                            guiButtons.huizenL[i].GetComponent<BoxCollider>().enabled = false;
                        }
                        else
                        {
                            guiButtons.huizenL[i].GetComponent<BoxCollider>().enabled = true;
                         //   guiButtons.huizenL[i].GetComponent<BoxCollider>().isTrigger = true;
                        }
                    }
                }
                break;
            case 2:
                for (var i = 0; i < guiButtons.naturaL.Count; i++)
                {
                    if (H || V)
                    {
                        guiButtons.naturaL[i].GetComponent<BoxCollider>().enabled = true;
                      //  guiButtons.naturaL[i].GetComponent<BoxCollider>().isTrigger = true;
                    }
                    if (!H && !V)
                    {
                        if (guiButtons.naturaL[i].layer == 14)
                        {
                            guiButtons.naturaL[i].GetComponent<BoxCollider>().enabled = false;
                        }
                        else
                        {
                            //Debug.Log("NO");
                            guiButtons.naturaL[i].GetComponent<BoxCollider>().enabled = true;
                         //   guiButtons.naturaL[i].GetComponent<BoxCollider>().enabled = true;
                        }
                    }
                }
                break;

        }
    }
    public void MaskToCamera(int butonN)
    {
        switch (butonN)
        {
            case 0:              
                cameraManeger.deselectCamera(1, wegen[0]);
                cameraManeger.deselectCamera(2, wegen[1]);
                cameraManeger.deselectCamera(2, wegen[0]);
                break;
            case 1:
                cameraManeger.deselectCamera(1, huizen[0]);
                cameraManeger.deselectCamera(2, huizen[1]);
                cameraManeger.deselectCamera(2, huizen[0]);
                break;
            case 2:
                cameraManeger.deselectCamera(1, natuur[0]);
                cameraManeger.deselectCamera(2, natuur[1]);
                cameraManeger.deselectCamera(2, natuur[0]);
                break;
        }
    }
}
/**
 * Class with array for Gui buttons arrangement.
 * @param buttonListc1 List of toggle buttons for Camera 1.
 * @param buttonListc2 List of toggle buttons for Camera 2.
 * @param buttonName List of strings with name of Layers.
 */
[Serializable]
public class GuiButtonss 
{
    [SerializeField]
    public List<bool> buttonListc1 = new List<bool>();
     [SerializeField]
    public List<bool> CBShowHide = new List<bool>();
    public List<string> selectedLayers = new List<string>();
    public List< GameObject> wegenL =  new List<GameObject>();
    public List<GameObject> huizenL = new List<GameObject>();
    public List<GameObject> naturaL = new List<GameObject>();
}
