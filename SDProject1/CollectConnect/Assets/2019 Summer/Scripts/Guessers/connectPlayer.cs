using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class connectPlayer : MonoBehaviour
{
    public int points=0;
    string keyword;
    [SerializeField] GameObject voteIcon;
    [SerializeField] GameObject votingKeyword;
    public GameObject _voteIcon;
    // Start is called before the first frame update
    void Start()
    {
  
    }
    public void readyToCastVote(GameObject parent)
    {
        GameObject keyword = Instantiate(votingKeyword,parent.transform);
        keyword.GetComponentInChildren<TextMeshProUGUI>().text = currentSelection.choice;
        keyword.GetComponent<castVote>().ownedBy = (int)ConnectGM.names.Player;
        _voteIcon = Instantiate(voteIcon);
    }
}
