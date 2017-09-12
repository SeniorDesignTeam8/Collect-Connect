using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ImageResizing : Card {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void MyCapture(string filename)
	{
		Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
		screenshot.ReadPixels(new Rect(0,0,Screen.width, Screen.height), 0, 0);
		screenshot.Apply();

		Texture2D newScreenshot = ScaleTexture(screenshot, 1024,576);

		byte[] bytes = newScreenshot.EncodeToPNG();
		File.WriteAllBytes(filename, bytes);
	}


	public Texture2D ScaleTexture(Texture2D source,int targetWidth,int targetHeight)
	{
		Texture2D result = new Texture2D(targetWidth,targetHeight,source.format,true);
		Color[] rpixels = result.GetPixels(0);
		float incX = ((float)1/source.width)*((float)source.width/targetWidth);
		float incY = ((float)1/source.height)*((float)source.height/targetHeight);

		for(int px = 0; px < rpixels.Length; px++)
		{
			rpixels[px] = source.GetPixelBilinear(incX*((float)px%targetWidth),
				incY*((float)Mathf.Floor(px/targetWidth)));
		}

		result.SetPixels(rpixels,0);
		result.Apply();
		return result;
	}
}
