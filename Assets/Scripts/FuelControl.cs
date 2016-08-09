using UnityEngine;
using UnityEngine.UI;

public class FuelControl : MonoBehaviour {

    public Text force;
    public Slider slider100;
    public Slider slider10;
    public Image currentFuel;
    public short fuelHeight;
    public GameObject vehicle;
    public float currentForce = 0;

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
        else if (Input.GetButtonDown("Accelerate") && currentForce < 0.99)
        {
            currentForce += 0.1f;
            slider10.value = Mathf.RoundToInt(currentForce * 10);
            slider100.value = Mathf.RoundToInt(currentForce * 100);
        }
        else if (Input.GetButtonDown("Decelerate") && currentForce > 0.01)
        {
            currentForce -= 0.1f;
            slider10.value = Mathf.RoundToInt(currentForce * 10);
            slider100.value = Mathf.RoundToInt(currentForce * 100);
        }
        force.text = (Mathf.RoundToInt(currentForce * 100)).ToString() + "%";
        currentFuel.rectTransform.offsetMax = new Vector2(currentFuel.rectTransform.offsetMax.x, (fuel / totalFuel) * fuelHeight - fuelHeight);
        slider100last = slider100.value;
        slider10last = slider10.value;
    }
}
