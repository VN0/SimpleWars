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
    int slider100last = 0;
    int slider10last = 0;
    
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
        foreach (Tank tank in tanks)
        {
            fuel += tank.fuel;
        }
	}

    void Update ()
    {
        if(slider100last != slider100.value/100)
        {
            currentForce = slider100.value / 100;
            slider10.value = Mathf.RoundToInt(currentForce * 10);
            force.text = (currentForce * 100).ToString() + "%";
        }
        else if (slider10last != slider10.value/10)
        {
            currentForce = slider10.value / 10;
            slider100.value = Mathf.RoundToInt(currentForce * 100);
            force.text = (currentForce * 100).ToString() + "%";
        }
        currentFuel.rectTransform.offsetMax = new Vector2(currentFuel.rectTransform.offsetMax.x, (fuel / totalFuel) * fuelHeight);
        slider100last = (int) slider100.value;
        slider10last = (int) slider10.value;
    }
}
