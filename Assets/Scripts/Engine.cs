using UnityEngine;

public class Engine : PartFunction {
	
	public float force = 200f;
	public float turn = 60f;
	public float consumption = 1;
	bool active;
    public SpriteRenderer fire;
    Tank tank;

	void Start() {
        try {
            tank = transform.GetComponentInParent<Tank>();
            if(tank.fuel > 0)
            {
                active = true;
            }
        } catch {
            Destroy(this);
            Destroy(fire);
        }
	}

	void Update() {
		if (Input.GetKey (KeyCode.W) && active) {
			fire.enabled = true;
		}
		if (Input.GetKeyUp (KeyCode.W)) {
			fire.enabled = false;
		}
	}
	void FixedUpdate () {
		if (Input.GetKey(KeyCode.W) && active) {
			try {
				active = tank.GetFuel (consumption * Time.deltaTime);
			} catch {
				active = false;
			}
			GetComponent<Rigidbody2D>().AddForce (transform.up * force * Time.deltaTime);
			if (Input.GetKey(KeyCode.A)) {
				GetComponent<Rigidbody2D>().AddTorque(turn * Time.deltaTime);
			}
			if (Input.GetKey(KeyCode.D)) {
				GetComponent<Rigidbody2D>().AddTorque(-turn * Time.deltaTime);
			}
		}
	}
}
