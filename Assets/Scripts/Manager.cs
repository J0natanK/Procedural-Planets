using UnityEngine;
using UnityEngine.Rendering;
using Random = System.Random;

public class Manager : MonoBehaviour
{
	public int numBodies;
	public int seed;

	public int maxMoonCount;

	public float minDistToSun;
	public float maxDistToSun;
	public float moonDst;

	public float timeScale;

	public static float sunMass = 10;

	Body[] bodies;
	Random rng;

	void Start()
	{
		bodies = new Body[numBodies + 1]; //+1 for sun

		bodies[0] = new Body(
			sunMass,
			Vector2.zero,
			Vector2.zero,
			"Sun"
		);

		rng = new Random(seed);
		for (int i = 1; i < bodies.Length; i++)
		{
			Vector2 pos = RandomInsideCircle(Vector2.zero, minDistToSun, maxDistToSun);
			Vector2 velocity = OrbitalVelocity(pos, sunMass, pos.magnitude);
			float mass = NextFloat();
			float radius = mass / 2;

			int moonCount = rng.Next(0, maxMoonCount);
			Body[] moons = new Body[moonCount];
			for (int j = 0; j < moonCount; j++)
			{
				Vector2 randomVector = new Vector2(NextFloat(-1, 1), NextFloat(-1, 1)).normalized;
				Vector2 moonPos = pos + randomVector * radius * moonDst;
				Vector2 moonVelocity = OrbitalVelocity(moonPos, sunMass, moonPos.magnitude);

				moons[j] = new Body(
					radius / 3,
					moonPos,
					moonVelocity,
					"Moon " + j
				);
			}

			bodies[i] = new Body(
				mass,
				pos,
				velocity,
				"Planet " + i,
				moons
			);

		}
	}

	void FixedUpdate()
	{
		float timeStep = Time.fixedDeltaTime * timeScale;

		for (int i = 1; i < bodies.Length; i++)
		{
			bodies[i].UpdateVelocity(timeStep);
		}
		for (int i = 1; i < bodies.Length; i++)
		{
			bodies[i].UpdatePosition(timeStep);
		}
	}

	Vector2 OrbitalVelocity(Vector2 pos, float centerMass, float centerDst)
	{
		float force = Mathf.Sqrt(centerMass / centerDst);

		Vector2 tangentDir = new Vector2(-pos.y, pos.x).normalized;
		Vector2 velocity = tangentDir * force;

		return velocity;
	}

	float NextFloat(float min = 0, float max = 1)
	{
		if (min == 0 && max == 1)
		{
			return (float)rng.NextDouble();
		}
		else
		{
			return (float)rng.NextDouble() * (max - min) + min;
		}
	}
	public Vector2 RandomInsideCircle(Vector2 origin, float minDist, float maxDist)
	{
		while (true)
		{
			// Generate a random angle between 0 and 2π
			float angle = NextFloat() * 2 * Mathf.PI;

			// Generate a random radius using square root for uniform distribution
			float r = Mathf.Sqrt(NextFloat());

			// Convert polar coordinates to Cartesian coordinates
			float x = r * (float)Mathf.Cos(angle);
			float y = r * (float)Mathf.Sin(angle);

			Vector2 pos = new Vector2(x, y) * maxDist;

			float distToSun = pos.magnitude;

			if (distToSun > minDist)
			{
				return pos + origin;
			}
		}
	}
}
