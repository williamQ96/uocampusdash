using UnityEngine;

public static class PlayerReturnPosition
{
    public static Vector3 LastOutsidePosition = Vector3.zero;
    public static Quaternion LastOutsideRotation = Quaternion.identity;
    public static bool HasTeleportedIntoRoom = false; // Currently in room
    public static bool HasRecordedOutside = false;     // Prevent duplicate recording
}

