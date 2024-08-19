using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {
    public float duration = 1.5f;
    public float magnitude = 0.05f;


    public IEnumerator Shake() {
        var originalPosition = transform.localPosition;

        var elapsed = 0.0f;

        while (elapsed < duration) {
            var x = Random.Range(-1f, 1f) * magnitude;
            var y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPosition.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}