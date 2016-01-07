using UnityEngine;
using System.Collections;
using System.Net;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkView))]
public class Client : MonoBehaviour {
		
	public int connectionPort = 25000;	
	public string connectIP = "127.0.0.1";
    public string myDomainName;
    
    internal string input;
    internal string name;
    internal bool connected = false;

    internal Vector2 scroll;
    internal GUIContent content;
    internal NetworkView net;
    internal NetworkPlayer netPlayer;

    NetworkConnectionError err;
    Vector2 rectangleWorld;

    void Awake()
    {
        //ChatFileAccess.CreateChat();

        //StartCoroutine("CheckIP");
        myDomainName = Application.absoluteUrl;

        
        Network.InitializeServer(100, 8080, false);
        //MasterServer.RegisterHost("Visualization Gemeente Leeuwarden", "version1");
 
        //find server
        //MasterServer.RequestHostList("Visualization Gemeente Leeuwarden");
    }

	void JoinServer()
	{



		//err = Network.Connect(connectIP, connectionPort);        
        //if (err != NetworkConnectionError.NoError)
        //{
        //    err = Network.Connect(connectIP,connectionPort);
        //    Debug.Log("Connecting....");
        //}
        //else
        //    Debug.Log("No host online!");
        //Debug.Log("" + err); 
        //Debug.Log(test[0].gameType + test[0].gameName);
		//if(err != NetworkConnectionError.NoError)		
		//	Debug.LogError("Unable to connect to server: " + err);			
		//else
		//Debug.Log("Connecting to server......");
	}

	void OnConnectedToServer()
	{
		Debug.Log("Connected to server.");

        net = GetComponent<NetworkView>();
        input = "";
        netPlayer = new NetworkPlayer();
        netPlayer = Network.player;
        name = netPlayer.guid;
        connected = true;
	}

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        if (info == NetworkDisconnection.Disconnected)
        {
            
            connected = false;
        }
    }
	
	void OnGUI()
	{
        rectangleWorld = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        rectangleWorld.y = Screen.height/2;

		if(!connected)
		{
			if(GUI.Button(new Rect(70, 10, 500, 50), ""+myDomainName))
			{
				JoinServer();

                //set content to the chatHistory xml
                //ChatHistory temp = (ChatHistory)Database.LoadXML("ChatHistory.xml",typeof(ChatHistory));
                //foreach(Bericht cH in temp.chatHistory)
                //content += ch.bericht +System.newLine;

                content = new GUIContent("test");
			}
		}

        if (connected)
        {
            GUI.Window(10, new Rect(Screen.width - 250, 10, 200, 300), chatWindow, "");
            if (GUI.Button(new Rect(Screen.width/2, Screen.height/2, 100, 50), "disconnect")) 
            {
                Network.Disconnect();
                connected = false; 
            }

        }
	}

    IEnumerator CheckIP()
    {
        WWW myExtIPWWW = new WWW("http://checkip.dyndns.org");
        if (myExtIPWWW == null) yield break;
        yield return myExtIPWWW;
        myDomainName = myExtIPWWW.text;
        myDomainName = myDomainName.Substring(myDomainName.IndexOf(":") + 2);
        myDomainName = myDomainName.Substring(0, myDomainName.IndexOf("<"));
        Debug.Log(myDomainName);
        Network.Connect(myDomainName, 8080);
    }

    void chatWindow(int windowId)
    {
        scroll = GUILayout.BeginScrollView(scroll);
            GUILayout.Box(content);
        GUILayout.EndScrollView();

        input = GUILayout.TextField(input);
        //if (GUILayout.Button("Send"))
        //{
        //    Network.isMessageQueueRunning = true;
        //    //net.RPC("PostMessage", RPCMode.Server, Player.playerName, input);
        //    content.text += ": " + input+System.Environment.NewLine;
        //    //ChatFileAccess.SaveChat(content.text);  
        //}

    }
    [RPC]
    public void Receive(string player , string message)
    {
        //if (player == Player.playerName)
        //{
        //    content.text += message + System.Environment.NewLine;
        //}
    }
}
