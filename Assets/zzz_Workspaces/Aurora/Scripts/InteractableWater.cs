using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.IMGUI.Controls;
#endif

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[RequireComponent(typeof(EdgeCollider2D), typeof(WaterTriggerHandler))]
public class InteractableWater : MonoBehaviour {

    [Header("Springs")]
    [SerializeField] private float sprteConstant = 1.5f;
    [SerializeField] private float damping = 1.1f;
    [SerializeField, Range(0f, 0.1f)] private float spread = 0.05f;
    [SerializeField, Range(0f, 10f)] private float speedMultiplier = 5.5f;
    [SerializeField, Range(1, 5)] private int wavePropogationIterations = 4;

    [Header("Force")]
    public float ForceMultiplier = 0.2f;
    [Range(1f, 50f)] public float MaxForce = 5f;

    [Header("Collisions")]
    [SerializeField, Range(1f, 10f)] private float playerCollisionRadiusMiltiplier = 4.15f;

    [Header("Dimensions")]
    public float width = 10f;
    public float height = 4f;

    [Header("Mesh Generation")]
    public Material waterMat;
    [Range(2, 500)] public int numOfVertices = 70;
    

    private const int NUM_OF_Y_VERTICES = 2;

    [Header("Gizmo")]
    public Color GizmoColor = Color.white;

    private Mesh _mesh;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private EdgeCollider2D edgeCollider;
    private Vector3[] vertices;
    private int[] topVerticesIndex;

    private class WaterPoint {
        public float velocity, position, targetHeight;
    }
    private List<WaterPoint> waterPoints = new();

    private void Start() {
        
        edgeCollider = GetComponent<EdgeCollider2D>();

        GenerateMesh();
        ResetEdgeCollider();
        CreateWaterPoints();
    }
    private void Reset() {
        
        edgeCollider = GetComponent<EdgeCollider2D>();
        if (edgeCollider != null) edgeCollider.isTrigger = true;
    }
    private void OnValidate() {
        
        width = Mathf.Max(0.1f, width);
        height = Mathf.Max(0.1f, height);

        if (gameObject.activeInHierarchy) { 
            GenerateMesh(); 
        }
    }
    private void FixedUpdate() {

        if (waterPoints == null || waterPoints.Count == 0) return;

        for (int i = 1; i < waterPoints.Count - 1; i++) {
            
            WaterPoint point = waterPoints[i];
            float x = point.position - point.targetHeight;

            float acceleration = -sprteConstant * x - damping * point.velocity;

            point.velocity += acceleration * speedMultiplier * Time.fixedDeltaTime;
            point.position += point.velocity * speedMultiplier * Time.fixedDeltaTime;

            point.position = Mathf.Clamp(point.position, point.targetHeight - height, point.targetHeight + height);

            vertices[topVerticesIndex[i]].y = point.position;
        }

        float[] leftDeltas = new float[waterPoints.Count];
        float[] rightDeltas = new float[waterPoints.Count];

        for (int j = 0; j < wavePropogationIterations; j++) {

            for (int i = 1; i < waterPoints.Count - 1; i++) {

                leftDeltas[i] = spread * (waterPoints[i].position - waterPoints[i - 1].position);
                waterPoints[i - 1].velocity += leftDeltas[i];

                rightDeltas[i] = spread * (waterPoints[i].position - waterPoints[i + 1].position);
                waterPoints[i + 1].velocity += rightDeltas[i];
            }
            for (int i = 1; i < waterPoints.Count - 1; i++) {
                
                waterPoints[i - 1].position += leftDeltas[i];
                waterPoints[i + 1].position += rightDeltas[i];
            }
        }
        _mesh.vertices = vertices;
        _mesh.RecalculateBounds();
    }
    public void ResetEdgeCollider() {

        edgeCollider = GetComponent<EdgeCollider2D>();

        if (edgeCollider == null) {
            edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
        }
        if (vertices == null || topVerticesIndex == null || topVerticesIndex.Length == 0) {
            Debug.LogWarning("Mesh data missing. Automatically running GenerateMesh() before setting collider.");
            GenerateMesh(); // Replace this with the exact name of your _mesh creation method
        }
        if (vertices == null || topVerticesIndex == null || topVerticesIndex.Length == 0) return;

        Vector2[] newPoints = new Vector2[2];

        Vector2 firstPoint = new(vertices[topVerticesIndex[0]].x, vertices[topVerticesIndex[0]].y);
        newPoints[0] = firstPoint;

        Vector2 secondPoint = new(vertices[topVerticesIndex[^1]].x, vertices[topVerticesIndex[^1]].y);
        newPoints[1] = secondPoint;

        edgeCollider.offset = Vector2.zero;
        edgeCollider.points = newPoints;

        #if UNITY_EDITOR
        EditorUtility.SetDirty(edgeCollider);
        #endif
    }
    public void GenerateMesh() {

        _mesh = new Mesh();
        _mesh.name = "WaterMesh";
        #region Vertices
        vertices = new Vector3[numOfVertices * NUM_OF_Y_VERTICES];
        topVerticesIndex = new int[numOfVertices];

        for (int i = 0; i < NUM_OF_Y_VERTICES; i++) {
            for (int j = 0; j < numOfVertices; j++) {

                float xPos = (j / (float)(numOfVertices - 1)) * width - width / 2;
                float yPos = (i / (float)(NUM_OF_Y_VERTICES - 1)) * height - height / 2;
                vertices[i * numOfVertices + j] = new Vector3(xPos, yPos, 0f);

                if (i == NUM_OF_Y_VERTICES - 1) {
                    topVerticesIndex[j] = i * numOfVertices + j;
                }
            }
        }
        #endregion
        #region Triangles
        int[] triangles = new int[(numOfVertices - 1) * (NUM_OF_Y_VERTICES - 1) * 6];
        int index = 0;

        for (int i = 0; i < NUM_OF_Y_VERTICES - 1; i++) {
            for (int j = 0; j < numOfVertices - 1; j++) {
                
                int bottomLeft = i * numOfVertices + j;
                int bottomRight = bottomLeft + 1;
                int topLeft = bottomLeft + numOfVertices;
                int topRight = topLeft + 1;

                triangles[index++] = bottomLeft;
                triangles[index++] = topLeft;
                triangles[index++] = bottomRight;

                triangles[index++] = bottomRight;
                triangles[index++] = topLeft;
                triangles[index++] = topRight;
            }
        }
        #endregion
        #region UVs
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i++) {

            uvs[i] = new Vector2((vertices[i].x + width / 2) / width, (vertices[i].y + height / 2) / height);
        }
        if (meshRenderer == null) { meshRenderer = GetComponent<MeshRenderer>(); }
        if (meshFilter == null) { meshFilter = GetComponent<MeshFilter>(); }

        meshRenderer.material = waterMat;

        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.uv = uvs;

        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();

        meshFilter.mesh = _mesh;
        #endregion

        #if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        #endif
    }
    private void CreateWaterPoints() {

        waterPoints.Clear();

        for (int i = 0; i < topVerticesIndex.Length; i++) {

            waterPoints.Add(new WaterPoint {

                position = vertices[topVerticesIndex[i]].y,
                targetHeight = vertices[topVerticesIndex[i]].y
            });
        }
    }
    public void Splash(Collider2D other, float force) {

        float radius = other.bounds.extents.x * playerCollisionRadiusMiltiplier;
        Vector2 center = other.transform.position;

        for (int i = 0; i < waterPoints.Count; i++) {

            Vector2 vertexWorldPosition = transform.TransformPoint(vertices[topVerticesIndex[i]]);

            if (IsPointInsideCircle(vertexWorldPosition, center, radius)) {
                
                waterPoints[i].velocity = force;
            }
        }
    }
    private bool IsPointInsideCircle(Vector2 point, Vector2 center, float radius) { 
        
        float distance = (point - center).sqrMagnitude;
        return distance <= radius * radius;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(InteractableWater))]
public class InteractableWaterEditor : Editor {

    private InteractableWater water;

    private void OnEnable() {
        water = (InteractableWater)target;
    }
    public override VisualElement CreateInspectorGUI() {

        VisualElement root = new();

        InspectorElement.FillDefaultInspector(root, serializedObject, this);

        root.Add(new VisualElement { style = { height = 10 } });

        Button generateMeshButton = new(() => water.GenerateMesh()) {

            text = "Generate Mesh"
        };
        root.Add(generateMeshButton);

        Button placeEdggeColliderButton = new(() => {
            Undo.RecordObject(water, "Reset Water Edge Collider");
            water.ResetEdgeCollider();

        }) { text = "Place Edge Collider" };
        root.Add(placeEdggeColliderButton);

        return root;
    }

    private void OnSceneGUI() {

        if (water.transform.localScale != Vector3.one) {


            if (Event.current.type == EventType.MouseUp || Event.current.type == EventType.Ignore) {

                Undo.RecordObject(water, "Scale Water Dimensions");
                Undo.RecordObject(water.transform, "Scale Water Dimensions");

                water.width = Mathf.Max(0.1f, water.width * Mathf.Abs(water.transform.localScale.x));
                water.height = Mathf.Max(0.1f, water.height * Mathf.Abs(water.transform.localScale.y));

                water.transform.localScale = Vector3.one;

                water.GenerateMesh();
                water.ResetEdgeCollider();
            }
        }
    }
}
#endif