using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrackScript : MonoBehaviour
{
	public List<Transform> trackPieces;
	public float speed;

	protected float trackLimit;

	void Awake()
	{
		if (trackPieces.Count > 2)
			trackLimit = Mathf.Abs(trackPieces[1].position.x - trackPieces[0].position.x);

		trackLimit += trackLimit/2;
		//trackLimit -= 0.02f; // epsilon :-/
	}

	void Update()
	{
		foreach (var track in trackPieces)
		{
			var newPosition = track.position.x - speed * Time.deltaTime;
			if (trackLimit > 0 && track.position.x <= -trackLimit)
				newPosition = trackLimit;

			track.position = new Vector3(newPosition, track.position.y, track.position.z);

		}
	}
}
