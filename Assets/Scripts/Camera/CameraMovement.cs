using UnityEngine;
using ThirteenPixels.Soda;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    GlobalTransform player;
    [SerializeField]
    float cameraCatchUpDistance = 2f;
    [SerializeField]
    float cameraSpeed = 0.01f;

    Vector3? cameraDestination;

    private void Update() //Тут можно прописать как надо двигать камеру в зависимости от положения player, если поле длиннее экрана
    {
        if (Mathf.Abs(player.componentCache.position.z - transform.position.z) > cameraCatchUpDistance)
            cameraDestination = new Vector3(transform.position.x, transform.position.y, player.componentCache.position.z);
        if (cameraDestination != null && Mathf.Abs(player.componentCache.position.z - transform.position.z) < cameraCatchUpDistance/2)
            cameraDestination = null;
        if (cameraDestination != null)
            transform.position = Vector3.Lerp(transform.position, cameraDestination.Value, cameraSpeed);
    }
}
