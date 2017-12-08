using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {

	/// <summary>
	/// Gets the camera related direction.
	/// </summary>
	/// <returns><see cref="Vector3"/>The camera related direction.</returns>
	/// <param name="self">Transform itself.</param>
	/// <param name="inputDir">Input direction.</param>
	/// <param name="camTrans">Camera's transform.</param>
	public static Vector3 GetCameraRelatedDirection(this Transform self, Vector3 inputDir, Transform camTrans){
		Vector3 camPosAtSelfHeight = new Vector3 (camTrans.position.x, self.position.y, camTrans.position.z);
		Vector3 abovePos = self.position + Vector3.up * 5;
		Vector3 relativeForward = abovePos - camPosAtSelfHeight;

		Vector3 dirFromSelfToCamAtSelfHeight = self.position - camPosAtSelfHeight;
		Vector3 relativeRight = Quaternion.AngleAxis (90, Vector3.up) * dirFromSelfToCamAtSelfHeight;

		return (inputDir.z * Vector3.Scale(relativeForward, new Vector3(1, 0, 1)) + inputDir.x * relativeRight);
	}

	/// <summary>
	/// Clamps the rotation around X axis.
	/// </summary>
	/// <returns>The rotation around X axis.</returns>
	/// <param name="q">Q.</param>
	/// <param name="minX">Minimum x.</param>
	/// <param name="maxX">Max x.</param>
	public static Quaternion ClampRotationAroundXAxis(Quaternion q, float minX, float maxX)
	{
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1.0f;

		float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);

		angleX = Mathf.Clamp (angleX, minX, maxX);

		q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

		return q;
	}
}
