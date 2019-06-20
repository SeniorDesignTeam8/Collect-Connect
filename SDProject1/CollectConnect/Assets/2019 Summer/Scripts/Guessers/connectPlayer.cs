using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class connectPlayer : MonoBehaviour
{
    int points;
    string keyword;
    [SerializeField] GameObject voteIcon;
    public GameObject _voteIcon;
    // Start is called before the first frame update
    void Start()
    {
        readyToCastVote();
    }
    public void readyToCastVote()
    {
      _voteIcon = Instantiate(voteIcon);
    }
    public void erasedVote()
    {
        Destroy(_voteIcon);
    }

}
