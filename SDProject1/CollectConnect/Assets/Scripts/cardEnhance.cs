using UnityEngine;

public class cardEnhance : MonoBehaviour {

    void Start ()
    {
        gameObject.GetComponent<Renderer>().enabled = false; //makes everything invisible on start
    }

    public void makeAppear()
    {
        gameObject.GetComponent<Renderer>().enabled = true;
    }

    public void disappear()
    {
        gameObject.GetComponent<Renderer>().enabled = false;
    }
}
