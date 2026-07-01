using UnityEngine;

public class GroundScroller : MonoBehaviour
{
    [SerializeField] private Transform[] tiles; // 3 tiles de suelo
    [SerializeField] private float longitudTile = 50f;
    [SerializeField] private float limiteRetorno = -60f;

    void Update()
    {
        foreach (Transform tile in tiles)
        {
            tile.Translate(Vector3.back * LevelScroller.Instance.VelocidadActual * Time.deltaTime);

            if (tile.position.z < limiteRetorno)
            {
                float maxZ = GetMaxZ();
                tile.position = new Vector3(tile.position.x, tile.position.y, maxZ + longitudTile);
            }
        }
    }

    float GetMaxZ()
    {
        float max = float.MinValue;
        foreach (Transform t in tiles)
            if (t.position.z > max) max = t.position.z;
        return max;
    }
}
