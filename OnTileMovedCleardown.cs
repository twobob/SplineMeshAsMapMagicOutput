using MapMagic.Terrains;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTileMovedCleardown : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TerrainTile.OnTileMoved -= OnTileMoved;
        TerrainTile.OnTileMoved += OnTileMoved;
    }

    // Update is called once per frame
    private void OnTileMoved(TerrainTile tile)
    {
        for (int i = tile.transform.childCount - 1; i > 0; i--)
        {

            if ( //tile.transform.GetChild(i).name.StartsWith("__SPLINE__") ||
             tile.transform.GetChild(i).name.StartsWith("SPLINE"))
                DestroyImmediate(tile.transform.GetChild(i).gameObject);

        }

      
    }
}
