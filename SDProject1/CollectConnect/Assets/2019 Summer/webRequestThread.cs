using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading;


public static class webRequestThread 
{
    public static bool done;
    public static Thread webRequest;

    public static void startWebRequest(string uri, List<string> words, string original, string type)
    {
        
    }


    public static void pullFromWeb(string uri, List<string> words, string original, string type)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            webRequest.SendWebRequest();

            string[] separatingStrings = { @"{""word"":""", "......." };

            string[] pages = webRequest.downloadHandler.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                for (int i = 1; i < page; i++)
                {
                    int loc = pages[i].IndexOf('"');
                    pages[i] = pages[i].Remove(loc);
                    words.Add(pages[i]);
                    Debug.Log(original + ":   " + pages[i] + " : " + type + '\n');
                }
            }
            done = true;
        }
    }
}
