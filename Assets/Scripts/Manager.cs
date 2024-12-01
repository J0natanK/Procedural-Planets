using UnityEngine;
using Random = System.Random;

public class Manager : MonoBehaviour
{
	[Header("Simulation Settings")]
	public int numBodies;
	public int seed;
	public float timeScale;

	[Header("Planetary Settings")]
	public int maxMoonsPerPlanet;
	public float minPlanetDstFromSun;
	public float maxPlanetDstFromSun;
	public float moonDstMultiplier;

	public static float sunMass = 10f;

	private Body[] bodies;
	private Random rng;

	void Start()
	{
		InitBodies();
	}

	void FixedUpdate()
	{
		SimulateGravity(Time.fixedDeltaTime * timeScale);
	}

	private void InitBodies()
	{
		bodies = new Body[numBodies + 1]; // +1 for the sun
		rng = new Random(seed);

		// Initialize Sun
		bodies[0] = new Body(
			sunMass,
			Vector2.zero,
			Vector2.zero,
			"Sun"
		);

		// Initialize planets and moons
		for (int i = 1; i < bodies.Length; i++)
		{
			Vector2 planetPosition = RandomInsideCircle(Vector2.zero, minPlanetDstFromSun, maxPlanetDstFromSun);
			Vector2 planetVelocity = CalculateOrbitalVelocity(planetPosition, sunMass);
			float planetMass = Random();
			float planetRadius = planetMass / 2f;

			int numMoons = rng.Next(0, maxMoonsPerPlanet);
			Body[] moons = CreateMoons(planetPosition, planetMass, planetRadius, numMoons);

			bodies[i] = new Body(
				planetMass,
				planetPosition,
				planetVelocity,
				$"Planet {i}",
				moons
			);
		}
	}

	private Body[] CreateMoons(Vector2 planetPosition, float planetMass, float planetRadius, int numMoons)
	{
		Body[] moons = new Body[numMoons];
		for (int j = 0; j < numMoons; j++)
		{
			Vector2 randomDirection = new Vector2(Random(-1, 1), Random(-1, 1)).normalized;
			Vector2 moonPosition = planetPosition + randomDirection * planetRadius * moonDstMultiplier;

			Vector2 sunOrbitVelocity = CalculateOrbitalVelocity(moonPosition, sunMass);
			Vector2 planetOrbitVelocity = CalculateOrbitalVelocity(moonPosition - planetPosition, planetMass);
			Vector2 moonVelocity = sunOrbitVelocity + planetOrbitVelocity;

			moons[j] = new Body(
				planetRadius / 3f,
				moonPosition,
				moonVelocity,
				$"Moon {j}"
			);
		}
		return moons;
	}

	private void SimulateGravity(float timeStep)
	{
		for (int i = 1; i < bodies.Length; i++)
		{
			bodies[i].UpdateVelocity(timeStep);
		}
		for (int i = 1; i < bodies.Length; i++)
		{
			bodies[i].UpdatePosition(timeStep);
		}
	}

	private Vector2 CalculateOrbitalVelocity(Vector2 centralDiff, float centralMass)
	{
		float distance = centralDiff.magnitude;
		float speed = Mathf.Sqrt(centralMass / distance);
		Vector2 tangentDirection = new Vector2(-centralDiff.y, centralDiff.x).normalized;
		return tangentDirection * speed;
	}

	private Vector2 RandomInsideCircle(Vector2 origin, float minDst, float maxDst)
	{
		while (true)
		{
			float angle = Random() * 2 * Mathf.PI;
			float radius = Mathf.Sqrt(Random());

			float x = radius * Mathf.Cos(angle);
			float y = radius * Mathf.Sin(angle);
			Vector2 randomPosition = new Vector2(x, y) * maxDst;

			if (randomPosition.magnitude > minDst)
			{
				return randomPosition + origin;
			}
		}
	}

	private float Random(float min = 0f, float max = 1f)
	{
		return (float)(rng.NextDouble() * (max - min) + min);
	}
}
