using UnityEngine;
using System.Collections;

internal static class MoCapUtils
		{
	
		public static void TransformFromMatrix4x4 (Transform transform, Matrix4x4 matrix, bool abs)
		{
		if (abs == true)
		{
			transform.rotation = GetRotation(matrix);
			transform.position = GetPosition(matrix);
		}
		else
		{
			//WHY???
			transform.localScale = GetScale(matrix);
			transform.localRotation = GetRotation(matrix);
			
			transform.localPosition = GetPosition(matrix);
		}
		}
	
		public static Quaternion GetRotation (Matrix4x4 m)
		{
			Quaternion q = new Quaternion ();
			q.w = Mathf.Sqrt (Mathf.Max (0, 1 + m [0, 0] + m [1, 1] + m [2, 2])) / 2; 
			q.x = Mathf.Sqrt (Mathf.Max (0, 1 + m [0, 0] - m [1, 1] - m [2, 2])) / 2; 
			q.y = Mathf.Sqrt (Mathf.Max (0, 1 - m [0, 0] + m [1, 1] - m [2, 2])) / 2; 
			q.z = Mathf.Sqrt (Mathf.Max (0, 1 - m [0, 0] - m [1, 1] + m [2, 2])) / 2; 
			q.x *= Mathf.Sign (q.x * (m [2, 1] - m [1, 2]));
			q.y *= Mathf.Sign (q.y * (m [0, 2] - m [2, 0]));
			q.z *= Mathf.Sign (q.z * (m [1, 0] - m [0, 1]));
			q.Set(q.x,q.z,q.y,q.w);
			return q;
		}
	
		public static Vector3 GetPosition (Matrix4x4 m)
		{
			//return new Vector3 (m.m30*5, m.m32*5, m.m31*5);
			return new Vector3 (m.m30, m.m32, m.m31);
		}
	
		public static Vector3 GetScale (Matrix4x4 m)
		{
			return new Vector3 (Mathf.Sqrt (m.m00 * m.m00 + m.m01 * m.m01 + m.m02 * m.m02), Mathf.Sqrt (m.m10 * m.m10 + m.m11 * m.m11 + m.m12 * m.m12), Mathf.Sqrt (m.m20 * m.m20 + m.m21 * m.m21 + m.m22 * m.m22));
		}

	}			
