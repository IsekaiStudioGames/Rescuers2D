using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Electricity2D : MonoBehaviour {

    [Header("Connections")]
    public Transform startPoint;
    public Transform endPoint;

    [Header("Electricity Settings")]
    [Range(1, 10)] public int arcs = 5;
    [Range(0.1f, 1f)]public float displacement = 0.5f;
    [Range(0.01f, 0.1f)]public float changeInterval = 0.05f;

    [Header("Gameplay State")]
    public bool isPowered = true;
    public int dmg = 1;

    private LineRenderer lineRenderer;
    private Collider2D hazardCollider;
    private float timer = 0f;

    private void Awake() {
        lineRenderer = GetComponent<LineRenderer>();
        hazardCollider = GetComponent<Collider2D>();
    }

    private void Update() {
        
        if (!isPowered) {

            lineRenderer.enabled = false;
            if (hazardCollider != null) hazardCollider.enabled = false;
            return;
        }

        lineRenderer.enabled = true;
        if (hazardCollider != null) hazardCollider.enabled = true;

        timer += Time.deltaTime; 
        if (timer >= changeInterval) {
            
            GenerateLightning();
            timer = 0f;
        }
    }
    private void GenerateLightning() {

        List<Vector3> points = new();
        points.Add(startPoint.position);
        points.Add(endPoint.position);

        float currentDisplacement = displacement;

        for (int i = 0; i < arcs; i++) {
            for (int j = points.Count - 1; j > 0; j--) {

                Vector3 midPoint = (points[j] + points[j - 1] / 2f);

                Vector3 direction = (points[j] - points[j - 1].normalized);
                Vector3 normal = new(-direction.y, direction.x, 0);

                midPoint += normal * Random.Range(-currentDisplacement, currentDisplacement);
                points.Insert(j, midPoint);
            }
            currentDisplacement *= 0.5f;
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
    public void SetActive(bool status) {
        isPowered = status;
    }
    private void OnTriggerStay2D(Collider2D other) {

        if (!isPowered) return;

        if (other.CompareTag("Player")) {

            Debug.Log("DAMAGE THE PLAYER, HAHAHAA!!!!");
        }
    }
}