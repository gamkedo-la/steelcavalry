/**
 * Description: A set of common pieces of code for using with Unity projects.
 * Version: 1.0
 * Authors: Kornel
 * Copyright: © 2018 Gamkedo.
 **/

using UnityEngine;

/// <summary>
/// Klasa zawierająca funkcje pomocnicze.
/// </summary>
public static class Utilities
{
	/// <summary>
	/// Gets the position of the mouse in world space.
	/// </summary>
	/// <param name="mousePosition"></param>
	/// <param name="camera"></param>
	/// <param name="distance"></param>
	/// <returns></returns>
	public static Vector3 GetMouseWorldPosition( Vector3 mousePosition, Camera camera = null, float distance = 10f )
	{
		if ( camera == null )
			camera = Camera.main;

		mousePosition.z = distance; // Some distance from the camera
		return camera.ScreenToWorldPoint( mousePosition );
	}

	/// <summary>
	/// Converts a float from one range to another.
	/// </summary>
	/// <param name="originalStart">Start of the old range.</param>
	/// <param name="originalEnd">End of the old range.</param>
	/// <param name="newStart">Start of the new range.</param>
	/// <param name="newEnd">End of the new range.</param>
	/// <param name="value">Value to convert.</param>
	/// <returns>Valu in the new range.</returns>
	public static float ConvertRange( float originalStart, float originalEnd, float newStart, float newEnd, float value )
	{
		float scale = ( newEnd - newStart ) / ( originalEnd - originalStart );
		return ( newStart + ( ( value - originalStart ) * scale ) );
	}
}
