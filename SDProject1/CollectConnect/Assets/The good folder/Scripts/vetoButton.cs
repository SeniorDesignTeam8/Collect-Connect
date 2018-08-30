using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vetoButton : MonoBehaviour
{

    handleVoting voting;
    public void agree()
    {
        voting = GameObject.Find("mainCanvas").GetComponent<handleVoting>();
        voting.agree();

    }
    public void disagree()
    {
        voting = GameObject.Find("mainCanvas").GetComponent<handleVoting>();
        voting.disagree();

    }
}
