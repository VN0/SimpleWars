using UnityEngine;

public static class VectorExtension
{
    public static Vector3 Rotate(this Vector3 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
}

public class Engine : PartFunction {
	
	public float force = 200;
	public float turn = 60;
	public float consumption = 1;
    float rotation = 0;
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
            Destroy(fire);
            Destroy(this);
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
                rotation = -turn;
				GetComponent<Rigidbody2D>().AddTorque(turn * Time.deltaTime);
			}
			if (Input.GetKey(KeyCode.D)) {
                rotation = turn;
				GetComponent<Rigidbody2D>().AddTorque(-turn * Time.deltaTime);
			}
            else
            {
                rotation = 0;
            }
		}
	}
}
