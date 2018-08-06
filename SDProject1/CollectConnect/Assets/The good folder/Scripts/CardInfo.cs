using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardInfo : MonoBehaviour {
    public string Lang;
	public string name;
    public string Location;
    public string ImageLoc;
    public string Description;
    public string Medium;
    public string People;
    public string Year;
    public string SourceLoc;
    string category, parameter;
    public List<string> Property;
    public int ID;
    public string completeDes;
    public Sprite cardPic;
    public Image museum;


    public void setCatParam(string cat, string par)
    {
        category = cat;
        parameter = par;
    }
    public void setCardDes(string des)
    {
        Description = des;
    }
    public void setCardLang(string lang)
    {
        Lang = lang;
    }
    public void setCardLocation(string loc)
    {
        Location = loc;
    }
    public void setImageLocation(string loc)
    {
        int delete= loc.LastIndexOf('.');
        loc = loc.Remove(delete);
        ImageLoc = loc;
        setImage();
    }
    public void setCardMedium(string med)
    {
        Medium = med;
    }
    public void setCardPeople(string pep)
    {
        People = pep;
    }
    public void setCardYear(string year)
    {
        Year = year;
    }
    public void setCardSourceLoc(string source)
    {
        SourceLoc = source;
    }
    public void setCardID(int id)
    {
        ID = id;
    }
    public void setImage()
    {
        if (ImageLoc != null)
        {
           // cardPic = Resources.Load(ImageLoc) as Sprite;
            cardPic = Resources.Load<Sprite>(ImageLoc);
        }
        int i = 0;

    }
    public string finalDescription()
    {
        string des="";
        if(name!=null)
        {
            des += name+ "\r\n";
        }
        if(Medium!=null)
        {
            des += Medium + "\r\n";
        }
        if(Description!=null)
        {
            des += Description + "\r\n";
        }
        if(People!=null&&Year!=null)
        {
            des += People+" "+Year+"\r\n";
        }
        else if (People != null)
        {
            des += People + "\r\n";
        }
        completeDes = des;
        return des;
    }

}
