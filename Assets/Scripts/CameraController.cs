using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;

    private Vector3 _offset;

    private void Start()
    {
        _offset = transform.position - player.position;
    }

    private void Update()
    {
        var cameraTransform = transform;
        var cameraPosition = cameraTransform.position;
        cameraPosition.z = player.position.z + _offset.z;
        cameraTransform.position = cameraPosition;
        // transform.position = player.position + _offset;
    }
}
