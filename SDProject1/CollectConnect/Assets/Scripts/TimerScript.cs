using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    public static int Timeleft;
    public Image CircleSlider;
    public Sprite MainSprite;
    public Sprite OtherSprite;
    private float _usualTime = 120.0f;
    private BoardManager _bM;
    private Button _rButton;
    public bool _isPaused = true;
    private int _lastButNum = -1;
    private float _lastClickTime = -99.0f;
    private const float DClickDelay = 0.25f;
    private Text _t;

    // Use this for initialization
    private void Start()
    {
        _t = GetComponent<Text>();
        _rButton = GetComponent<Button>();
        _bM = FindObjectOfType<BoardManager>();
        //InvokeRepeating("decreaseTime", 1, 1);

        _rButton.onClick.AddListener(ResetTimer);
        CircleSlider.fillAmount = 1.0f;
    }

    // Update is called once per frame
    private void Update()
	{

        if (Timeleft < 31)
        {
            _t.color = Color.red;
            CircleSlider.sprite = OtherSprite;
        }
        else
        {
            _t.color = Color.black;
            CircleSlider.sprite = MainSprite;
        }

        if (Timeleft < 0)
        {
            _bM.PassBtnHit();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isPaused = true;
            CancelInvoke();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            _isPaused = false;
            InvokeRepeating("DecreaseTime", 1, 1);
        }

        _t.text = "" + Timeleft;
        if(!_isPaused)
            CircleSlider.fillAmount -= (1.0f / _usualTime * Time.deltaTime);
    }

    private void DecreaseTime()
    {
        Timeleft--;
    }

    private void ResetTimer()
    {
        if (_lastClickTime > Time.time - DClickDelay && _lastButNum == 2)
        {
            CancelInvoke();
            Timeleft = 120;
            CircleSlider.fillAmount = 1.0f;
            InvokeRepeating("DecreaseTime", 1, 1);
        }
        else if (_lastClickTime > Time.time - DClickDelay && _lastButNum == 1)
        {
            _lastClickTime = Time.time;
            _lastButNum = 2;
        }
        else
        {
            _lastClickTime = Time.time;
            _lastButNum = 1;
        }
    }

	public void StartTimer()
	{
		InvokeRepeating("DecreaseTime", 1, 1);
	}

	public void StopTimer()
	{
		CancelInvoke();
	}
}
