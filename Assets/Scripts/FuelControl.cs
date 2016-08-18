using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FuelControl : MonoBehaviour {

    public Text force;
    public Slider slider100;
    public Slider slider10;
    public Image currentFuel;
    public short fuelHeight;
    public GameObject vehicle;
    public float currentForce = 0;
    [HideInInspector]
    public bool turnLeft;
    [HideInInspector]
    public bool turnRight;

    float totalFuel = 0;
    float fuel = 0;
    Tank[] tanks;
    float slider100last = 0;
    float slider10last = 0;

    void Start ()
    {
        vehicle = GameObject.Find("Vehicle");
        tanks = vehicle.GetComponentsInChildren<Tank>();
        foreach (Tank tank in tanks)
        {
            totalFuel += tank.capacity;
        }
    }
	
	void FixedUpdate ()
    {
        fuel = 0;
        if (tanks != null)
        {
            foreach (Tank tank in tanks)
            {
                fuel += tank.fuel;
            }
        }
	}

    void Update ()
    {
        if (Input.GetButton("Turn Left"))
        {
            turnLeft = true;
        }
        if (Input.GetButtonUp("Turn Left"))
        {
            turnLeft = false;
        }
        if (Input.GetButton("Turn Right"))
        {
            turnRight = true;
        }
        if (Input.GetButtonUp("Turn Right"))
        {
            turnRight = false;
        }
        if (slider100last != slider100.value)
        {
            currentForce = slider100.value / 100;
            slider10.value = Mathf.RoundToInt(currentForce * 10);
        }
        else if (slider10last != slider10.value)
        {
            currentForce = slider10.value / 10;
            slider100.value = Mathf.RoundToInt(currentForce * 100);
        }
        else if (Input.GetButtonDown("Increase Thottle") && currentForce < 0.99)
        {
            currentForce += 0.1f;
            slider10.value = Mathf.RoundToInt(currentForce * 10);
            slider100.value = Mathf.RoundToInt(currentForce * 100);
        }
        else if (Input.GetButtonDown("Decrease Thottle") && currentForce > 0.01)
        {
            currentForce -= 0.1f;
            slider10.value = Mathf.RoundToInt(currentForce * 10);
            slider100.value = Mathf.RoundToInt(currentForce * 100);
        }
        force.text = (Mathf.RoundToInt(currentForce * 100)).ToString() + "%";
        currentFuel.rectTransform.offsetMax = new Vector2(currentFuel.rectTransform.offsetMax.x, (fuel / totalFuel) * fuelHeight - fuelHeight);
        slider100last = slider100.value;
        slider10last = slider10.value;
        
        if (Input.GetKeyDown(KeyCode.Return) && SceneManager.GetActiveScene().name != "Builder")
        {
            SceneManager.LoadScene("Builder");
            /*foreach (GameObject o in Object.FindObjectsOfType<GameObject>())
            {
                Destroy(o);
            }*/
            Destroy(GameObject.Find("Vehicle"));
        }
    }

    public void TurnLeft ()
    {
        turnLeft = true;
    }
    public void StopTurnLeft ()
    {
        turnLeft = false;
    }
    public void TurnRight ()
    {
        turnRight = true;
    }
    public void StopTurnRight ()
    {
        turnRight = false;
    }
}
