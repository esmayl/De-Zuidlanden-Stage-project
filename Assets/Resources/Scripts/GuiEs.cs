using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Main class that handles every gui element.
/// </summary>
[Serializable]
public class GuiEs : MonoBehaviour
{
	#region Doxygen declarations
	/**
	 * @var buildingInfo
	 * A variable that holds the data of the currently selected building.
	 *
	 * @var bewegen
	 * The script that controls the player movement and rotation.
	 * 
	 * @var skin
	 * The gui skin to use for the gui elements.
	 * 
	 * @var sky
	 * The script that controls the time of day.
	 * 
	 * @var openWindowPos
	 * The position of the window on screen
	 * 
	 * @var counter
	 * The int used to switch between tabs (day/night,info,zoeken,options)
	 * 
	 * @var month
	 * The month to use in the time of day.
	 * 
	 * @var windowScroll
	 * The variable used to scroll down in the open window.
	 * 
	 * @var showMonthDetails
	 * The bool to open and close the month options.
	 * 
	 * @var showCloudOptions
	 * The bool to open close the cloud options.
	 * 
	 * @var Months
	 * The string array to display to the user.
	 * 
	 * @var Clouds
	 * The string array to display to the user.
	 */
	#endregion
	#region Public accessible variables

	public static Info buildingInfo = new Info();
    public static bool overGui = false;

	#endregion
	#region Public variables

	public Bewegen bewegen;
	public GUISkin skin ;
	public TOD_Sky sky;
    public string[] iconTooltips = new string[4];
    //public Client network;

    #endregion
	#region Private variables

    //gui windows variables
    private string selectedTooltip;
	Rect openWindowPos;
    Vector2 windowScroll = new Vector2(0, 0);
    private float padding;
    private float iconSize;
    private float boolWidth;
    private float boolHeight;
    private float arrowSize;
    private float sliderWidth;
    private float sliderHeight;
    private float textFieldWidth;
    private float textFieldHeight;
    private float labelWidth;
    public int counter = 6;//5 = day/night , 6 = info, 7 = search ,8 = options ,e
    int month = 1;

    //fps counter variables
    bool frameBool = false;
    float frames;
    float timer;
    float fps;
    float updateRate=1; // 1 updates per second

	//gui textures for opening and closing button
    GUIStyle leftArrow;
    GUIStyle rightArrow;
    GUIStyle arrowMaand;

	//bool showWeatherOptions = false;

	private static string[] Months =
	{
		"Januari",
		"Februari",
		"Maart",
		"April",
		"Mei",
		"Juni" ,
		"July",
		"Augustus",
		"September",
		"October",
		"November",
		"December"
	};

	private static string[] Clouds = {
		TOD_Weather.CloudType.None.ToString(),
		TOD_Weather.CloudType.Few.ToString(),
		TOD_Weather.CloudType.Scattered.ToString(),
		TOD_Weather.CloudType.Overcast.ToString(),
		TOD_Weather.CloudType.Broken.ToString()
	};
    private float openingSpeed = 5;
	
	//private string[] weatherTypes = {
	//    TOD_Weather.WeatherType.Normaal.ToString(),
	//    TOD_Weather.WeatherType.Stormachtig.ToString(),
	//    TOD_Weather.WeatherType.Mist.ToString(),
	//};

	private static bool opening = false;
	#endregion
	#region Unity functions

	/// <summary>
	/// Sets the window width,height and position according to the screen, turns on the Day/night.
	/// </summary>
	void Start()
	{

     iconSize = Screen.width/40;
     padding = Screen.width / 512;
     boolWidth = Screen.width / 10;
     boolHeight = Screen.height / 31.7f;
     arrowSize = Screen.width / 60;
     sliderWidth = Screen.width / 20;
     sliderHeight = Screen.height / 240;
     textFieldWidth = Screen.width / 15+padding;
     textFieldHeight = Screen.height / 72;
     labelWidth = Screen.width / 28;

     arrowMaand = skin.FindStyle("Maand arrow");

     Application.targetFrameRate = 60;

	 sky.Components.Time.ProgressDate = true;
	 sky.Components.Time.ProgressMoonPhase = true;

     openWindowPos = new Rect(Screen.width / 9 * 10, Screen.height / 4 * 3-padding*3, iconSize * 4 + padding*3.5f, Screen.height / 5.4f);

	 sky.Cycle.UTC = 1f;
	 sky.Cycle.Latitude = 52f;
	 sky.Cycle.Longitude = 5f;
	}


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ChangeOpening(true);
        }
    }
	/// <summary>
	/// Main function that opens and closes windows.
	/// </summary>
	void OnGUI()
	{

        GUI.skin = skin;

        overGui = OnTheGUI(openWindowPos);

        //text size relative to screen res

        //fps
        frames++;
        timer += Time.deltaTime;

        if (timer > 1.0 / updateRate) { fps = frames / timer; frames = 0; timer -= 1.0f / updateRate; }
        if (Input.GetKeyDown(KeyCode.P)) { frameBool = true; }
        if (Input.GetKeyDown(KeyCode.O)) { frameBool = false; }
        //fps end

        //months fix
        if (month > Months.Length)
        {
            month = 0;
        }
        if (month < 0)
        {
            month = Months.Length;
        }
        GUI.Box(new Rect(openWindowPos.x,openWindowPos.y+iconSize,openWindowPos.width,openWindowPos.height),"",skin.window); 


        //opening and closing animation
       	if (opening)
       	{
            openWindowPos.x = Mathf.Clamp(openWindowPos.x += openingSpeed, Screen.width - (iconSize * 4 + padding * 5), Screen.width);
       	}
       	else
       	{
       	    openWindowPos.x = Mathf.Clamp(openWindowPos.x -= openingSpeed, Screen.width - (iconSize * 4 + padding* 5), Screen.width);
       	}
        GUI.Window(10, new Rect(openWindowPos.x, openWindowPos.y, openWindowPos.width, openWindowPos.height*2), TabHandler, "",new GUIStyle());
        Tooltip();
	}

	#endregion
	#region Custom functions
	
    /// <summary>
	/// Used by Database.cs to refresh the info shown on screen.
	/// </summary>
	/// <param name="newBuildingInfo">Building info to show on screen.</param>
	public static void ShowInfo(Info newBuildingInfo)
	{
		Debug.Log("Receiving info " + newBuildingInfo.adres);
		buildingInfo = newBuildingInfo;
        ChangeOpening(false);
	}

    public static void ChangeOpening(bool optional)
    {
        if (!optional) { opening = false; }
        else opening = !opening;
    }

    /// <summary>
    /// Change the normal texture to the hover texture dependent on counter
    /// </summary>
	void ChangeGUIGlow()
	{
        for (int i = 5; i <= 9; i++)
        {
            if (i != counter)
            {
                skin.customStyles[i].normal = skin.customStyles[i].onNormal; 
                skin.customStyles[i].hover = skin.customStyles[i].onHover;
                skin.customStyles[i].overflow = new RectOffset(-1, 0, -1, -1);      
            }
        }

		skin.customStyles[counter].normal = skin.customStyles[counter].onFocused;
        skin.customStyles[counter].hover = skin.customStyles[counter].normal;
        skin.customStyles[counter].overflow = new RectOffset(0, 0, 0, 0);
        
	}

	#endregion
	#region GUI functions
	/// <summary>
	/// Top row of buttons in the open window.
	/// </summary>
	void TopRow()
	{
		//top row buttons
		GUILayout.BeginHorizontal ();
        int p = 0;
		for (int i = 5; i <= 9; i++)
		{

			if(i!=9)
			{				 

				if (GUILayout.Button(new GUIContent("",iconTooltips[p]), skin.customStyles[i],GUILayout.Width(iconSize),GUILayout.Height(iconSize))) 
				{ 
					counter = i; 
				}
				GUILayout.Space(padding);

			}
			//if(i == 7){GUILayout.Label(new GUIContent("",iconTooltips[p]),skin.customStyles[i],GUILayout.Width(iconSize),GUILayout.Height (iconSize)); GUILayout.Space(padding);}
            p++;
		}
		
		GUILayout.EndHorizontal();
    }
	/// <summary>
	/// Day/night tab.
	/// </summary>
	public void DayNight()
	{
   
                    GUILayout.Space(padding*2);

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(padding * 4);

                    GUILayout.Label(new GUIContent("TIJDSTIP",""));

                    GUILayout.EndHorizontal();


                    GUILayout.BeginHorizontal();
                    GUILayout.Space(padding * 3);

                    GUILayout.BeginVertical();                    
                    GUILayout.Space(padding * 4);

                    sky.Cycle.Hour = GUILayout.HorizontalSlider(Mathf.Round(sky.Cycle.Hour), 0, 23.5f, GUI.skin.horizontalSlider, GUI.skin.horizontalSliderThumb, GUILayout.Width(sliderWidth-padding*3), GUILayout.Height(sliderHeight));

                    //to get 2 decimals : decimal.Round((decimal)sky.Cycle.Hour, 2, MidpointRounding.AwayFromZero)
                    GUILayout.EndVertical();

                    GUILayout.Label(new GUIContent(""+sky.Cycle.Hour,iconTooltips[4]),GUILayout.Width(labelWidth-padding*4));

                    GUILayout.EndHorizontal();


                    GUILayout.BeginHorizontal();
                    GUILayout.Space(padding * 4);
            GUILayout.Label(new GUIContent("MAAND", iconTooltips[5]), skin.label, GUILayout.Width(labelWidth));


            GUILayout.EndHorizontal();
                GUILayout.Space(padding * 2);

                GUILayout.BeginHorizontal();
                    GUILayout.Space(padding*2);
                    if (GUILayout.Button(arrowMaand.normal.background,skin.label, GUILayout.Width(arrowSize), GUILayout.Height(arrowSize)))
                    {
                        month--;
                        if (month < 0)
                        {
                            month = Months.Length-1;
                        }
                        sky.Cycle.Month = month;
                    }

                    GUILayout.BeginVertical();
                    GUILayout.Space(padding*2.2f);
                    GUILayout.Label(new GUIContent(Months[month],iconTooltips[6]),skin.textField,GUILayout.Width(textFieldWidth-padding),GUILayout.Height(textFieldHeight+padding*2.5f));
                    GUILayout.EndVertical();
                
                    GUILayout.Space(padding*2);
                    if (GUILayout.Button(arrowMaand.hover.background,skin.label, GUILayout.Width(arrowSize), GUILayout.Height(arrowSize)))
                    {
                        month++;
                        if (month >= Months.Length-1)
                        {
                            month = 0;
                        }
                        sky.Cycle.Month = month;
                    }

                    GUILayout.EndHorizontal();

				//GUILayout.BeginVertical();
				//
				//    GUILayout.BeginHorizontal();
				//    GUILayout.Label("Weersomstandigheden");
				//    GUILayout.FlexibleSpace();
				//    GUILayout.EndHorizontal();
				//
				//    GUILayout.BeginHorizontal();
				//    if (GUILayout.Button("",GUI.skin.FindStyle("Drop-down"))) { showWeatherOptions = !showWeatherOptions; }
				//    if (showWeatherOptions)
				//    {
				//        int weatherType = GUILayout.SelectionGrid((int)sky.Components.Weather.Weather - 1, weatherTypes, 1) + 1;
				//        sky.Components.Weather.Weather = (TOD_Weather.WeatherType)weatherType;
				//    }
				//    GUILayout.FlexibleSpace();
				//    GUILayout.EndHorizontal();
				//
				//GUILayout.EndVertical();
				//GUILayout.Space(Screen.height / 32)
	}
	/// <summary>
	/// Search tab.
	/// </summary>
	public void Search()
	{
		GUILayout.Space(padding*2);
		GUILayout.BeginVertical();

        GUILayout.Button("Zoeken.", skin.label);

		GUILayout.EndVertical();
	}
	/// <summary>
	/// Buiding info tab.
	/// </summary>
	public void InfoWindow()
	{
		GUILayout.Space(padding*4);
		GUILayout.BeginVertical ();

        GUILayout.Label("ADRES " + buildingInfo.adres, skin.label,GUILayout.Width(labelWidth),GUILayout.Height(textFieldHeight));
        GUILayout.Space(padding);
        GUILayout.Label("POSTCODE " + buildingInfo.postcode, skin.label, GUILayout.Width(labelWidth), GUILayout.Height(textFieldHeight));
        GUILayout.Space(padding);
        GUILayout.Label("WOONPLAATS " + buildingInfo.woonplaats, skin.label, GUILayout.Width(labelWidth), GUILayout.Height(textFieldHeight));
        GUILayout.Space(padding);
        if (frameBool) { GUILayout.Box("FPS: " + fps, skin.label, GUILayout.Width(labelWidth), GUILayout.Height(textFieldHeight)); }
        if (!Database.loaded) { GUILayout.Label("EEN OGENBLIK GEDULD \nALSTUBLIEFT.  \n\nDE ADRESGEGEVENS \nWORDEN GEDOWNLOAD.", skin.label, GUILayout.Width(labelWidth)); }
		GUILayout.EndVertical ();
	}
	/// <summary>
	/// Options tab.
	/// </summary>
	public void Options()
	{
        /// GO here
		GUILayout.Space(padding*2);
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();
        GUILayout.Space(padding * 2);
        bewegen.vliegen = GUILayout.Toggle(bewegen.vliegen,new GUIContent("",iconTooltips[7]), skin.FindStyle("Vliegen"), GUILayout.Width(boolWidth), GUILayout.Height(boolHeight));
		GUILayout.EndHorizontal();
		GUILayout.Space(padding*2);
			GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Space(padding*2);
            GUILayout.Label("BEWEEG SNELHEID " + Mathf.Round(bewegen.maxSpeed * 10), skin.label, GUILayout.Width(labelWidth));
            GUILayout.EndHorizontal();
            GUILayout.Space(padding * 4);
            GUILayout.BeginHorizontal();
            GUILayout.Space(padding * 2);
            bewegen.maxSpeed = GUILayout.HorizontalSlider(bewegen.maxSpeed, 0.1f, 0.5f,skin.horizontalSlider,skin.horizontalSliderThumb,GUILayout.Height(sliderHeight),GUILayout.Width(sliderWidth));
            GUILayout.EndHorizontal();
            GUILayout.Space(padding * 4);
            GUILayout.Label("KIJK SNELHEID " + Mathf.Round(bewegen.rotationSpeed*10), skin.label, GUILayout.Width(labelWidth));

            GUILayout.BeginVertical();
            GUILayout.Space(padding * 4);
            GUILayout.BeginHorizontal();
            GUILayout.Space(padding * 2);
            bewegen.rotationSpeed = GUILayout.HorizontalSlider(bewegen.rotationSpeed, 0.1f, 1f, skin.horizontalSlider, skin.horizontalSliderThumb, GUILayout.Width(sliderWidth),GUILayout.Height(sliderHeight));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
						
		GUILayout.EndVertical();

           
		GUILayout.EndVertical();
	}

    void Tooltip()
    {
        if (overGui && selectedTooltip !="")
        {
            GUI.Label(new Rect(openWindowPos.x,openWindowPos.y-(iconSize/1.5f),openWindowPos.width,20),selectedTooltip,skin.textArea);
        }
    }

	/// <summary>
	/// GUI window functions that handles which tab to open acording to counter variable.
	/// </summary>
	/// <param name="windowId"></param>
	public void TabHandler(int windowId)
	{
		switch (counter)
		{
			case 5:
					ChangeGUIGlow();
					TopRow();                    
					DayNight();
					break;
			case 6:
					ChangeGUIGlow();
					TopRow();
					InfoWindow();
					break;
			case 8:
					ChangeGUIGlow();
					TopRow();
					Options();
					break;
			case 9:
					ChangeGUIGlow();
					TopRow();
					Search();
					break;
			default:
					TopRow();
					break;
		}
        if (Event.current.type == EventType.Repaint)
        {
            selectedTooltip = GUI.tooltip;
        }
        
	}

    //W.I.P
    void SelectResolution(int width, int height)
    {
        if (width > 1920) { return; }
        if (height > 1080) { return; }
        //Change icons to new resolution
    }

    public static bool OnTheGUI(Rect position)
    {
        if(position.Contains(Event.current.mousePosition))
        {
            overGui = true;
            return overGui;
        }
        else
        {
            overGui = false;
            return overGui;
        }
    }
	#endregion
}
