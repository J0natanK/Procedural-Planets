using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body
{
	public Vector2 velocity;
	public Vector2 position;
	public float mass;

	public Transform t;
	Body[] moons;

	public Body(float mass, Vector2 position, Vector2 velocity, string name, Body[] moons = null)
	{
		t = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
		t.name = name;
		t.parent = GameObject.Find("Bodies").transform;
		t.localScale = Vector3.one * mass;

		this.position = position;
		this.velocity = velocity;
		this.mass = mass;

		this.moons = moons;

		if (moons != null)
		{
			for (int i = 0; i < moons.Length; i++)
			{
				moons[i].t.parent = t;
			}
		}

	}

	public void UpdateVelocity(float timeStep)
	{
		velocity += CalculateAcceleration(position, mass, Vector2.zero, Manager.sunMass) * timeStep;

		if (moons != null)
		{
			foreach (var moon in moons)
			{
				moon.velocity += CalculateAcceleration(moon.position, moon.mass, position, mass) * timeStep;
				moon.velocity += CalculateAcceleration(moon.position, moon.mass, Vector2.zero, Manager.sunMass) * timeStep;
				velocity += CalculateAcceleration(position, mass, moon.position, moon.mass) * timeStep;
			}
			foreach (var moon in moons)
			{
				moon.UpdatePosition(timeStep);
			}
		}

	}

	public void UpdatePosition(float timeStep)
	{
		position += velocity * timeStep;
		t.position = new Vector3(position.x, 0, position.y);
	}

	Vector2 CalculateAcceleration(Vector2 thisPos, float thisMass, Vector2 otherPos, float otherMass)
	{
		float sqrDst = (otherPos - thisPos).sqrMagnitude;
		Vector2 forceDir = (otherPos - thisPos).normalized;
		Vector2 force = forceDir * thisMass * otherMass / sqrDst;
		Vector2 acceleration = force / thisMass;

		return acceleration;
	}
}
