using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveDuration = 2.5f;

    public void MoveToPosition(Vector3 targetPosition)
    {
        StopAllCoroutines();
        StartCoroutine(MoveCamera(targetPosition));
    }

    private IEnumerator MoveCamera(Vector3 targetPosition)
    {
        Vector3 start = transform.position;
        targetPosition.z = start.z;

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / moveDuration); // Mượt hơn
            transform.position = Vector3.Lerp(start, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition; // Đảm bảo đúng vị trí cuối cùng
    }

}