using Meta.XR.MRUtilityKit;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WallObjectSpawner : MonoBehaviour
{
    public float checkRadius = 0.5f; // Radius for the overlap test to check for existing objects
    public float targetTime = 60.0f;

    private int whileTries;
    public void SpawnObjectsOnWalls(ObjectSpawnInstance obj)
    {
        whileTries = 5;
        GameObject objectToSpawn = obj.gameObj;
        int numberOfObjectsToSpawn = obj.intValue;
        float offset = obj.offset;
        bool isWindow = obj.isWindow;
        bool isOnObject = obj.isOnObject;
        bool isOnDoor = obj.isOnDoor;
        float heightMin = obj.heightMin;
        float heightMax = obj.heightMax;

        MRUKRoom currentRoom = MRUK.Instance.GetCurrentRoom();
        List<MRUKAnchor> wallAnchors = new List<MRUKAnchor>();
        MRUKAnchor doorAnchor = null;

        foreach (var anchor in currentRoom.GetRoomAnchors())
        {
            if (anchor.HasLabel("WALL_FACE"))
            {
                wallAnchors.Add(anchor);
            }
        }

        List<MRUKAnchor> windowAnchors = new List<MRUKAnchor>();

        foreach (var anchor in currentRoom.GetRoomAnchors())
        {
            if (anchor.HasLabel("WINDOW_FRAME"))
            {
                windowAnchors.Add(anchor);
            }
        }

        if (isWindow)
        {
            numberOfObjectsToSpawn = windowAnchors.Count;

            Debug.Log($"[TEST] WINDOW ANCHORS: {windowAnchors.Count}");
        }
        else if (!isWindow && !isOnObject)
        {
            Debug.Log($"[TEST] WALL ANCHORS: {wallAnchors.Count}");
        }

        if (isOnDoor)
        {
            numberOfObjectsToSpawn = 1;
            foreach (var anchor in currentRoom.GetRoomAnchors())
            {
                if (anchor.HasLabel("DOOR_FRAME"))
                {
                    Debug.Log($"FOUND DOOR: {anchor.GetAnchorCenter()}");
                    doorAnchor = anchor;
                    break;
                }
            }
        }

        for (int i = 0; i < numberOfObjectsToSpawn; i++)
        {
            MRUKAnchor randomWallAnchor = null;
            HashSet<int> triedWallIndices = new HashSet<int>();

            if (isWindow)
            {
                randomWallAnchor = windowAnchors[i];
            }
            else if (isOnObject)
            {
                randomWallAnchor = currentRoom.FloorAnchor;
            }
            else if (isOnDoor)
            {
                randomWallAnchor = doorAnchor;
            }
            else if (!isWindow && !isOnObject)
            {
                bool foundValidWall = false;
                while (!foundValidWall)
                {
                    if (triedWallIndices.Count >= wallAnchors.Count)
                    {
                        Debug.LogError("No valid walls with planes found.");
                        return;
                    }

                    int randomIndex = Random.Range(0, wallAnchors.Count);
                    if (triedWallIndices.Contains(randomIndex))
                    {
                        continue;
                    }

                    randomWallAnchor = wallAnchors[randomIndex];
                    triedWallIndices.Add(randomIndex);

                    if (randomWallAnchor.PlaneRect.HasValue)
                    {
                        foundValidWall = true;
                    }
                }
            }

            if (randomWallAnchor.PlaneRect.HasValue || randomWallAnchor.VolumeBounds.HasValue)
            {
                Vector2 planeSize = Vector2.zero;
                Vector3 positionOnWall = Vector3.zero;
                Vector3 facingDirection = currentRoom.GetFacingDirection(randomWallAnchor).normalized;
                Vector3 worldPositionOnPlane = Vector3.zero;
                Vector3 randomPositionOnPlane = Vector3.zero;

                if (isOnDoor)
                {
                    Rect doorRect = randomWallAnchor.PlaneRect.Value;

                    // Calculate the position at the leftmost side of the door bounds
                    Vector3 doorCenter = randomWallAnchor.transform.TransformPoint(doorRect.center);
                    Vector3 leftMostSide = doorCenter + randomWallAnchor.transform.TransformVector(new Vector3(-doorRect.width / 2 + 0.1f, 0, 0));

                    // Adjust the position to be slightly in front of the door based on the door's normal (facing direction)
                    positionOnWall = leftMostSide + facingDirection * offset;
                    positionOnWall.y -= 0.1f;

                    // Ensure the position's rotation is set to zero
                    objectToSpawn.transform.rotation = Quaternion.identity;
                }
                else
                {
                    planeSize = randomWallAnchor.PlaneRect.Value.size;
                    randomPositionOnPlane = new Vector3(
                        Random.Range(-planeSize.x / 2, planeSize.x / 2),
                        Random.Range(-planeSize.y / 2, planeSize.y / 2),
                        0);

                    // Convert the random position on the plane to world space
                    worldPositionOnPlane = randomWallAnchor.transform.TransformPoint(randomPositionOnPlane);

                    randomWallAnchor.GetClosestSurfacePosition(worldPositionOnPlane, out positionOnWall);

                    // Adjust the position to be slightly in front of the wall based on the wall's normal (facing direction)
                    positionOnWall += facingDirection * offset;

                    // Keep trying to find a position until it's within the desired range
                    while (!(positionOnWall.y >= heightMin && positionOnWall.y <= heightMax))
                    {
                        if (whileTries <= 0)
                        {
                            break;
                        }
                        randomPositionOnPlane = new Vector3(
                            Random.Range(-planeSize.x / 2, planeSize.x / 2),
                            Random.Range(-planeSize.y / 2, planeSize.y / 2),
                            0);

                        worldPositionOnPlane = randomWallAnchor.transform.TransformPoint(randomPositionOnPlane);
                        randomWallAnchor.GetClosestSurfacePosition(worldPositionOnPlane, out positionOnWall);
                        positionOnWall += facingDirection * offset;

                        whileTries--;
                    }

                    whileTries = 5;
                }

                // Get all renderers of the object's children
                Renderer[] childRenderers = objectToSpawn.GetComponentsInChildren<Renderer>();

                // Combine bounds of all child renderers
                Bounds combinedBounds = new Bounds();
                foreach (Renderer renderer in childRenderers)
                {
                    combinedBounds.Encapsulate(renderer.bounds);
                }

                // Get the list of room anchors
                List<MRUKAnchor> roomAnchors = currentRoom.GetRoomAnchors();
                MRUKAnchor floorAnchor = currentRoom.GetFloorAnchor();

                roomAnchors.Remove(floorAnchor);

                // Initialize a flag to keep track of whether a free position is found
                bool foundFreePosition = false;
                int tries = 10;

                // Check for existing objects using an overlap test
                Collider[] hitColliders = Physics.OverlapBox(positionOnWall, combinedBounds.extents);

                // Keep trying to find a free position until one is found
                while (!foundFreePosition)
                {
                    if (tries <= 0)
                    {
                        foundFreePosition = true;
                    }
                    tries--;

                    // Check if the position overlaps with existing objects or anchors
                    bool overlaps = false;
                    foreach (MRUKAnchor anchor in roomAnchors)
                    {
                        Vector3 anchorPosition = anchor.GetAnchorCenter();
                        Vector3 anchorSize = anchor.GetAnchorSize();
                        Bounds anchorBounds = new Bounds(anchorPosition, anchorSize);
                        if (anchorBounds.Intersects(combinedBounds))
                        {
                            overlaps = true;
                            break;
                        }
                    }

                    if (hitColliders.Length == 0 && !overlaps) // No overlapping bounds found and no anchor overlaps, safe to spawn
                    {
                        Debug.Log("[TEST] Object's new position: " + positionOnWall);

                        // Set the flag to true to exit the loop
                        foundFreePosition = true;
                    }
                    else
                    {
                        // Debug a warning if the position is occupied and try a different position
                        Debug.LogWarning("[TEST] Position: " + positionOnWall + " is occupied by other objects or anchors. Trying another position.");

                        // Try a new position
                        while (!(positionOnWall.y >= heightMin && positionOnWall.y <= heightMax))
                        {
                            if (whileTries <= 0)
                            {
                                break;
                            }
                            randomPositionOnPlane = new Vector3(
                                Random.Range(-planeSize.x / 2, planeSize.x / 2),
                                Random.Range(-planeSize.y / 2, planeSize.y / 2),
                                0);

                            worldPositionOnPlane = randomWallAnchor.transform.TransformPoint(randomPositionOnPlane);
                            randomWallAnchor.GetClosestSurfacePosition(worldPositionOnPlane, out positionOnWall);
                            positionOnWall += facingDirection * 0.5f;
                            whileTries--;
                        }
                    }
                }

                GameObject spawnedObject;

                if (isWindow) {
                    Vector3 scale = randomWallAnchor.GetAnchorSize();
                    objectToSpawn.transform.localScale = new Vector3(scale.x - 1.25f, scale.y - 0.5f, 0.30f);

                    // If it's a window, spawn it at the center of the anchor
                    Vector3 spawnPosition = randomWallAnchor.GetAnchorCenter() + facingDirection * offset;
                    spawnPosition.y = spawnPosition.y - 0.5f;
                    spawnedObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
                    spawnedObject.transform.rotation = Quaternion.Euler(0, 90, 0);
                }
                else if (isOnDoor)
                {
                    spawnedObject = Instantiate(objectToSpawn, positionOnWall, Quaternion.identity);
                    spawnedObject.transform.rotation = Quaternion.identity;
                }
                else
                {
                    spawnedObject = Instantiate(objectToSpawn, positionOnWall, Quaternion.identity);
                }

                if (!isOnDoor)
                {
                    // Determine the rotation based on the facing direction
                    if (Mathf.Abs(facingDirection.x) > Mathf.Abs(facingDirection.z))
                    {
                        // Rotate around z-axis if x is dominant
                        float yRotation = facingDirection.x > 0 ? -90f : 90f;
                        spawnedObject.transform.Rotate(0, yRotation, 0);
                    }
                    else
                    {
                        // Rotate around x-axis if z is dominant
                        float yRotation = facingDirection.z > 0 ? -180f : 0f;
                        spawnedObject.transform.Rotate(0, yRotation, 0);
                    }
                }
                
                Debug.Log("[TEST] Object " + i + " has direction: " + facingDirection);
            }
            else
            {
                if (isWindow)
                    Debug.LogError("[TEST] Selected window anchor does not have a plane.");
                else
                    Debug.LogError("[TEST] Selected wall anchor does not have a plane.");
            }
        }
    }
}
