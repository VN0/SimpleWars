using UnityEngine;
using UnityEngine.UI;

public class TimeScaleChanger : MonoBehaviour {
	public Text text;

	void Start () {
		GetComponent<Slider> ().onValueChanged.AddListener (delegate {
			Time.timeScale = GetComponent<Slider> ().value;
		});
	}
	void Update () {
		if (Input.GetKeyDown(KeyCode.P)) {
			if (Time.timeScale > 0f) {
				Time.timeScale = 0f;
			} else {
				Time.timeScale = 1f;
			}
		}
		text.text = (Mathf.Round (Time.timeScale * 100)).ToString () + "%";
		GetComponent<Slider> ().value = Time.timeScale;
	}
}
