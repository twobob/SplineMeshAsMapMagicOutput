using Den.Tools;
using MapMagic.Core;
using MapMagic.Terrains;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class AlignNodesToTerrainOnEnable : MonoBehaviour
{
    public static Coord CoordForTileFromVec3(Vector3 vec) { return new Coord((int)(vec.x * 0.001f), (int)(vec.y * 0.001f)); }

    public Transform mapMagicTransform;

    public int attempts = 0;
    public float checkVal = 0.005f;
    private int MaxAttempts = 20;
    SplineMesh.Spline spline; 
    void OnEnable()
    {
        RunIt();
        //  splineFormer.InvalidateMesh();
    }

  //  TerrainTile tile;

    public void RunIt()
    {

        spline = GetComponent<SplineMesh.Spline>();

        //  tile = spline.gameObject.transform.parent.parent.GetComponent<TerrainTile>();

        if (mapMagicTransform == null)

        mapMagicTransform = Component.FindObjectOfType<MapMagicObject>().transform;


        TryToFloor();


    }


    private void TryToFloor()
    {
        // list, right length
        List<float> testArr = new List<float>();

        // Fill it with 500's

        for (int i = 0; i < spline.nodes.Count; i++)
        {
            testArr.Add( 500f);
        }

        spline.enabled = false;

        int testcount = 0;



        int maxdepth = 5;

        Vector3 totalOffsetFromRoot = Vector3.zero;

        Transform newchild = transform.parent;

        for (int depth = 0; depth < maxdepth; depth++)
        {

            totalOffsetFromRoot = totalOffsetFromRoot + newchild.position;

            newchild = newchild.parent;
            if (newchild == mapMagicTransform)
            {
                break;
            }

        }

        totalOffsetFromRoot *= .5f;



           Coord tilecoord = CoordForTileFromVec3(totalOffsetFromRoot);

           var locality = TownGlobalObject.GetIndexAtCoord(tilecoord);

         //  Vector3 townOffset = -(tilecoord.ToTileSizeVector3() - locality.ToTileSizeVector3());




        for (int i = 0; i < spline.nodes.Count; i++)
        {
            var node = spline.nodes[i];

           // Vector3 modulod = new Vector3((node.Position.x - (totalOffsetFromRoot.x * 0.5f)) % 1000, node.Position.y, (node.Position.z - (totalOffsetFromRoot.z * 0.5f)) % 1000);
            Vector3 modulod = new Vector3((node.Position.x - (totalOffsetFromRoot.x )) % 1000, node.Position.y, (node.Position.z - (totalOffsetFromRoot.z )) % 1000);


            Vector3 newpos = totalOffsetFromRoot + modulod;



            //Vector2 testpos = new Vector2(
            //  newpos.x,
            //  newpos.z  );


            Vector2 testpos = new Vector2(
            node.Position.x + totalOffsetFromRoot.x,
            node.Position.z + totalOffsetFromRoot.z);



            testArr[i] = GetTerrainPos(testpos.x, testpos.y).y;
           
            node.Position = node.Direction = new Vector3(node.Position.x, testArr[i], node.Position.z);
            
             testcount += 1;
           
        }

     //   if (!spline.enabled)
            spline.enabled = true;


        if (testArr.Contains(0))
        {

            // Last pop get nuclear
            if (attempts > MaxAttempts - 4)
            {

                // check every single node.
                for (int i = 0; i < spline.nodes.Count; i++)
                {
                    var node = spline.nodes[i];

                    // move x closer
                    if (Mathf.Abs(node.Position.x % 1) <= (checkVal))
                    {
                        node.Position = new Vector3(Mathf.Round(node.Position.x), node.Position.y, node.Position.z);
                    }
                    // Move z closer
                    if (Mathf.Abs(node.Position.z % 1) <= (checkVal))
                    {
                        node.Position = new Vector3(node.Position.x, node.Position.y, Mathf.Round(node.Position.z));
                    }
                }

                checkVal *= 8;

            }

            if (attempts < MaxAttempts)
            {
                attempts += 1;
                Invoke(nameof(TryToFloor), 1);
                return;
            }
            else
            {
              
                Debug.LogFormat(gameObject, "Failed to floor splines {0} on countout", gameObject.name);
                return;
            }
        }

       
       
    }

    static Vector3 GetTerrainPos(float x, float y)
    {
        //Create object to store raycast data

        //Create origin for raycast that is above the terrain. I chose 500.
        Vector3 origin = new Vector3(x, 500f, y);

        //Send the raycast.
        // Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 501f);

        // TODO OPTION MASK SELECTION
        LayerMask mask = LayerMask.GetMask(MapLayers.CamWorldLayer);

        Ray ray = new Ray(origin, Vector3.down);


        Physics.Raycast(ray, out RaycastHit hit, 501f, mask);


        Debug.DrawRay(origin, Vector3.down, Color.red, 15f, false);

        //  Debug.Log("Terrain location found at " + hit.point);
        return hit.point;
    }


}
