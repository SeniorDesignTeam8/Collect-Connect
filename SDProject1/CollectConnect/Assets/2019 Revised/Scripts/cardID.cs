using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cardID : MonoBehaviour
{
    public int coll_id;
    public string frontImg;
    public string backImg;
    Sprite front;
    Sprite back;
    public Image art;
    public Image info;
    public void setImageName(string imgName, int id)
    {
        if(id<10)
        {
            frontImg = imgName + "0" + id.ToString()+"_F";
            backImg = imgName + "0" + id.ToString() + "_B";
        }
        else
        {
            frontImg = imgName + id.ToString() + "_F";
            backImg = imgName + id.ToString() + "_B";
        }

        front = Resources.Load<Sprite>(frontImg);
        back = Resources.Load<Sprite>(backImg);


    }
    public void setImage()
    {
        art.sprite = front;
        info.sprite = back;
    }
}
