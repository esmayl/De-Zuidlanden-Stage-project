using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkView))]
public class Networking : MonoBehaviour 
{
    //public int connectionPort = 25000;	
    internal string connectIP;
    internal string input = "Bericht";
    internal string name;
    internal bool connected =false;

    internal Vector2 scroll = new Vector2(1,1);
    internal string content;
    internal NetworkView net;
    internal NetworkPlayer netPlayer;

    NetworkConnectionError err;
    Vector2 rectangleWorld;

    void Awake()
    {
        WebClient test = new WebClient();
        test.Encoding = Encoding.UTF8;
        test.Headers["User-Agent"] = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2.15) Gecko/20110303 Firefox/3.6.15";
        //connectIP = test.DownloadString("http://www.google.nl/");

        IPAddress ip = null;
        //foreach (IPAddress i in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        //{
        //    Debug.LogError("" + i);
        //    ip = i;
        //    content = ip.ToString();
        //}

        //create server
        Network.InitializeServer(100, 80, false);
        MasterServer.RegisterHost("Visualization Gemeente Leeuwarden", "version1");
        
        //find server
        MasterServer.RequestHostList("Visualization Gemeente Leeuwarden");
    }

    void JoinServer()
    {
        //content = connectIP;        
        var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        //err = Network.Connect(System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList[0], 843);        
        HostData[] test = MasterServer.PollHostList();

        if (test.Length > 0)
        {
            err = Network.Connect(test[0]);
            Debug.Log("Connecting....");
        }
        else
        {
            Debug.Log("No host online!");
            Debug.Log("" + err);
            Debug.Log(test[0].gameType + test[0].gameName);
        }

        if (err != NetworkConnectionError.NoError)
        {
            Debug.LogError("Unable to connect to server: " + err);
        }
        else
        {
            Debug.Log("Connecting to server......");
        }
    }

    void OnFailedToConnectToMasterServer(NetworkConnectionError error)
    {
        Debug.Log("" + error);
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
        GUILayout.Window(0, new Rect(Screen.width / 2, Screen.height / 2, 300, 500), chatWindow, "");
        if (!connected)
        {

            if (GUI.Button(new Rect(70, 10, 100, 50), "Join server"))
            {
                JoinServer();
                if (Database.loaded)
                {
                    foreach (Bericht b in Database.chatHistory.berichten)
                    {
                        content += b.datum + " " + b.berichten + Environment.NewLine;
                    }
                }
            }

        }

        if (connected)
        {
            if (GUI.Button(new Rect(Screen.width / 2, Screen.height / 2, 100, 50), "disconnect"))
            {
                Network.Disconnect();
                connected = false;
            }

        }
    }

    void chatWindow(int windowId)
    {
        scroll = GUILayout.BeginScrollView(scroll);
        GUILayout.Box(content);
        GUILayout.EndScrollView();

        input = GUILayout.TextField(input);
        if (GUILayout.Button("Send"))
        {
            Network.isMessageQueueRunning = true;
            //net.RPC("PostMessage", RPCMode.Server, Player.playerName, input);
            content+= ": " + input + System.Environment.NewLine;
            //ChatFileAccess.SaveChat(content.text);  
        }

    }

    [RPC]
    public void Receive(string player, string message)
    {
        //if (player == Player.playerName)
        //{
        //    content += message + System.Environment.NewLine;
        //}
    }
    
}
