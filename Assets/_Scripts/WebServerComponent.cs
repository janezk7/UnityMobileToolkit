using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebServerComponent: MonoBehaviour
{
	private SimpleHTTPServer _server;

	void Awake()
	{
		var appPath = Application.dataPath;
		var wwwPath = appPath.Substring(0, appPath.LastIndexOf("Assets")) + "www";
		_server = new SimpleHTTPServer(wwwPath, 8080);
		Debug.Log("Starting web server...");
	}

	private void OnDestroy()
	{
		_server?.Stop();
	}
}
