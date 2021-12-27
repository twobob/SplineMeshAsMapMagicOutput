using UnityEngine;
using System.Collections.Generic;
using System;
using Den.Tools.Matrices;
using MapMagic.Products;
using Den.Tools;
using MapMagic.Nodes;
using Den.Tools.Splines;
using MeshUtils;
using MapMagic.Terrains;
using System.Collections;
using System.Linq;
using SplineMesh;

namespace Twobob.Mm2
{
    [Serializable]
    [GeneratorMenu(
        menu = "Spline",
        name = "SplineMesh",
        section = 2,
        colorType = typeof(SplineSys),
        helpLink = "https://gitlab.com/denispahunov/mapmagic/wikis/output_generators/Spline")]
    public class MapSplineOutMark1 : OutputGenerator, IInlet<SplineSys>, IPrepare
    {
        public static Coord CoordFromVec3(Vector3 vec) { return new Coord((int)vec.x, (int)vec.y); }
        public static Coord CoordForTileFromVec3(Vector3 vec) { return new Coord((int)(vec.x * 0.001f), (int)(vec.y * 0.001f)); }

        public MapMagic.Core.MapMagicObject MapMagicObjectReference;
        public static MapMagic.Core.MapMagicObject MapMagicObjectRef;

       // public static RandomName namegenref;

        //common settings
        public GameObject[] prefabs = new GameObject[1];
        public PositioningSettingsSpline posSettings = null; // new PositioningSettings(); //to load older output
        public BiomeBlend biomeBlend = BiomeBlend.Random;

        public OutputLevel outputLevel = OutputLevel.Main;
        public override OutputLevel OutputLevel { get { return outputLevel; } }

        public bool guiMultiprefab;
        public bool guiProperties;

        //specific settings
        public bool allowReposition = true;
        public bool instantiateClones = false;

        //moved to PositioningSettings, and thus outdated:
        public bool objHeight = true;
        public bool relativeHeight = true;
        public bool guiHeight;
        public bool isRandomYaw = false;
        public bool useRotation = true;
        public bool takeTerrainNormal = false;
        public bool rotateYonly = false;
        public bool regardPrefabRotation = false;
        public bool guiRotation;
        public float offset;
        public float offsetRange;
        public bool mergeSegments;
        public bool spacingFromScale = false;
        public float spacing = 1f;
        public float spacingRange;
        public bool guiPositionSettings;
        public bool useScale = true;
        public bool scaleYonly = false;
        public bool regardPrefabScale = false;
        public float scale = 1f;
        public float scaleRange;
        public bool guiScale;



#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void EnlistInMenu() => MapMagic.Nodes.GUI.CreateRightClick.generatorTypes.Add(typeof(MapSplineOutMark1));
#endif

      private  float ScaleToWorldWithOffset(float val, float offset)
        {
            return val + offset;
            
           // return (TownGlobalObjectService.WorldMultiplier * val) + offset;
        }



        public static PositioningSettingsSpline CreatePosSettings(MapSplineOutMark1 output)
        {
            PositioningSettingsSpline ps = new PositioningSettingsSpline
            {
                objHeight = output.objHeight,
                relativeHeight = output.relativeHeight,
                guiHeight = output.guiHeight,
                isRandomYaw = output.isRandomYaw,
                useRotation = output.useRotation,
                takeTerrainNormal = output.takeTerrainNormal,
                rotateYonly = output.rotateYonly,
                regardPrefabRotation = output.regardPrefabRotation,
                guiRotation = output.guiRotation,
                offset = output.offset,
                offsetRange = output.offsetRange,
                mergeSegments = output.mergeSegments,
                spacingFromScale = output.spacingFromScale,
                spacing = output.spacing,
                spacingRange = output.spacingRange,
                guiPositionSettings = output.guiPositionSettings,
                scale = output.scale,
                scaleRange = output.scaleRange,
                useScale = output.useScale,
                scaleYonly = output.scaleYonly,
                regardPrefabScale = output.regardPrefabScale,
                guiScale = output.guiScale

            };
            return ps;
        }


        public void Prepare(TileData data, Terrain terrain)
        {
            //resetting modified objects to real nulls - otherwise they won't appear in thread
            for (int p = 0; p < prefabs.Length; p++)
                if ((UnityEngine.Object)prefabs[p] == (UnityEngine.Object)null)  //if (prefabs[p] == null) 
                    prefabs[p] = null;
        }

        public List<ObjectsPool.Prototype> GetPrototypes()
        {
            List<ObjectsPool.Prototype> prototypes = new List<ObjectsPool.Prototype>();
            for (int p = 0; p < prefabs.Length; p++)
                if (!prefabs[p].IsNull())  //if (prefabs[p] != null) 
                    prototypes.Add(new ObjectsPool.Prototype()
                    {
                        prefab = prefabs[p],
                        allowReposition = false,
                        instantiateClones = false,
                        regardPrefabRotation = posSettings.regardPrefabRotation,
                        regardPrefabScale = posSettings.regardPrefabScale
                    });
            return prototypes;
        }


        public override void Generate(TileData data, StopToken stop)
        {

            //loading source
            if (stop != null && stop.stop) return;
            SplineSys src = data.ReadInletProduct(this);
            if (src == null) return;

            SplineSys copy = new SplineSys(src);

            if (copy.lines.Length == 0)
                return;

        //    var locality = TownGlobalObject.GetIndexAtCoord(data.area.Coord);

            copy.Clamp((Vector3)data.area.active.worldPos, (Vector3)data.area.active.worldSize);


            for (int i = 0; i < copy.lines.Length; i++)
            {

                for (int j = 0; j < copy.lines[i].segments.Length; j++)
                {
                    var start = copy.lines[i].segments[j].start;
                    var end = copy.lines[i].segments[j].end;


                 start.pos -=(Vector3)data.area.active.worldPos;

                 end.pos -= (Vector3)data.area.active.worldPos;

                }
            }

            if (stop != null && stop.stop) return;

            //adding to finalize
            if (enabled && copy.NodesCount > 0 ) 
            {
                data.StoreOutput(this, typeof(MapSplineOutMark1), this, copy);
               data.MarkFinalize(Finalize, stop);
            }
            else 
            data.RemoveFinalize(Finalize);
        }

        public static FinalizeAction finalizeAction = Finalize; //class identified for FinalizeData


        /*
         * 
         * What follows is an optional "wait on height finalized" workflow
         * currently not used
         */


//#if UNITY_EDITOR
//        [UnityEditor.InitializeOnLoadMethod]
//#endif
//        [RuntimeInitializeOnLoadMethod]
//        static void Subscribe() => Graph.OnOutputFinalized += FinalizeIfHeightFinalized;
//        static void FinalizeIfHeightFinalized(Type type, TileData tileData, IApplyData applyData, StopToken stop)
//        {
//            if (type == typeof(MapMagic.Nodes.MatrixGenerators.HeightOutput200))
//                tileData.MarkFinalize(finalizeAction, stop);
//        }



        public static void Finalize(TileData data, StopToken stop)
        
        
        {
            if (stop != null && stop.stop) return;
            Noise random = new Noise(data.random, 12345);


            Dictionary<ObjectsPool.Prototype, List<Transition>> objs = new Dictionary<ObjectsPool.Prototype, List<Transition>>();


            foreach ((MapSplineOutMark1 output, SplineSys trns, MatrixWorld biomeMask)
                in data.Outputs<MapSplineOutMark1, SplineSys, MatrixWorld>(typeof(MapSplineOutMark1), inSubs: true))
            {

                List<ObjectsPool.Prototype> prototypes =
                   output.GetPrototypes();
                if (prototypes.Count == 0) continue;


                ObjectsPool.Prototype ran = prototypes[(int)(RandomGen.Range01() * prototypes.Count)];


                if (!objs.ContainsKey(ran)) objs.Add(ran, new List<Transition>());



                // Whats follows is a possible long hand per-object prefab -> prototypes link.
                // Currently not used.

                //foreach (ObjectsPool.Prototype prot in prototypes)
                //{

                //  //  if (!objs.ContainsKey(prot)) objs.Add(prototypes[(int)(RandomGen.Range01() * prototypes.Count)], new List<Transition>());


                //    if (!objs.ContainsKey(prot)) objs.Add(prot, new List<Transition>());



                //}

                //var points = trns.GetAllPoints();

                //int total = points.Length;

                ////objects
                //for (int i = 0; i < trns.lines.Length; i++)
                //{

                //    for (int j = 0; j < trns.lines[i].segments.Length; j++)
                //    {


                //        var point = points[i][j];

                //        Transition trn = new Transition(point.x, point.y, point.z); //using copy since it's changing in MoveRotateScale

                //        if (!data.area.active.Contains(trn.pos)) continue; //skipping out-of-active area
                //        if (PositioningSettings.SkipOnBiome(ref trn, output.biomeBlend, biomeMask, data.random)) continue; //after area check since uses biome matrix

                //        output.posSettings.MoveRotateScale(ref trn, data);

                //        trn.pos -= (Vector3)data.area.active.worldPos; //objects pool use local positions



                //        float rnd = random.Random(trn.hash);
                //        ObjectsPool.Prototype prototype = prototypes[(int)(rnd * prototypes.Count)];
                //        objs[prototype].Add(trn);
                //    }

                //}
            }

            if (stop != null && stop.stop) return;
           


            //purging if no outputs
            int splinesCount = data.OutputsCount(typeof(MapSplineOutMark1), inSubs: true);
            if (splinesCount == 0)
            {
                if (stop != null && stop.stop) return;
                data.MarkApply(ApplyData.Empty);
                return;
            }

            //merging splines

            List<SplineSysWithPrefab> mergedSpline = new List<SplineSysWithPrefab>();


            foreach ((MapSplineOutMark1 output, SplineSys product, MatrixWorld biomeMask)
                   in data.Outputs<MapSplineOutMark1, SplineSys, MatrixWorld>(typeof(MapSplineOutMark1), inSubs: true))
            {





                // We will use the position settings and jam it into the parseable output
                mergedSpline.Add(new SplineSysWithPrefab(product)
                {
                    chosenType = objs.Keys.ToArray()[mergedSpline.Count],

                    mergeSegments = output.posSettings.mergeSegments,

                    spacingFromScale = output.posSettings.spacingFromScale,

                    spacing = (output.posSettings.spacingFromScale) ? output.posSettings.scale : output.posSettings.spacing,

                    spacingRange = output.posSettings.spacingRange,


                    scale = output.posSettings.scale,

                    scaleRange = output.posSettings.scaleRange,

                    offset = output.posSettings.offset,

                    offsetRange = output.posSettings.offsetRange,

                    isRandomYaw = output.posSettings.isRandomYaw,
                }) ;
            }

            //pushing to apply
            if (stop != null && stop.stop) return;

            ApplyData applyData = new ApplyData()
            {
                prototypes = objs.Keys.ToArray(),
                transitions = objs.Values.ToArray(),
              //  terrainHeight = data.globals.height,
              //  objsPerIteration = data.globals.objectsNumPerFrame,
                splines = mergedSpline
            };
            Graph.OnOutputFinalized?.Invoke(typeof(MapSplineOutMark1), data, applyData, stop);

            data.MarkApply(applyData);
        }


        public class ApplyData : IApplyData // IApplyDataRoutine
        {
            public List<SplineSysWithPrefab> splines;

            public ObjectsPool.Prototype[] prototypes;
            public List<Transition>[] transitions;

            //  TODO we could just use this to level the final objects rather than the horror-story ray casts. 
            //      public float terrainHeight; //to get relative object height (since all of the terrain data is 0-1). //TODO: maybe move it to HeightData in "Height in meters" task
           
            // TODO we could limit the perframe activities mid-spawn          
           //    public int objsPerIteration = 500;


            public void Apply(Terrain terrain)
            {
                MapMagic.Core.MapMagicObject mmo = Component.FindObjectOfType<MapMagic.Core.MapMagicObject>();

                // unless there is no spline data

                if (splines == null || splines.Count == 0 || splines[0].lines.Length == 0)
                {
                    return;
                }

                // By this point this should absolutely exist - 
                int totalNumberOfListsOfSplineMeshSplines = splines.Count;

                // There is nothing in the list
                if (totalNumberOfListsOfSplineMeshSplines == 0)
                {
                    return;
                }

                List<GameObject> thingsToActivate = new List<GameObject>();

                Coord data_area_cood = new Coord( (int) terrain.transform.parent.localPosition.x, (int) terrain.transform.parent.localPosition.z) * 0.001f;


                Coord locality = TownGlobalObject.GetIndexAtCoord(data_area_cood);


                // SplinePowerExtended

                var DynamicHolder = mmo.transform.Find(string.Format("Tile {0},{1}", data_area_cood.x, data_area_cood.z));

                // Create splines holder 

                var splineHolder = new GameObject
                {
                    name = "SPLINE_FOR_" + string.Format("Tile_{0},{1}", data_area_cood.x, data_area_cood.z)
                };


                splineHolder.transform.parent = DynamicHolder;

                splineHolder.transform.localPosition = new Vector3();


                Coord tilecoord = splineHolder.transform.parent.GetComponent<TerrainTile>().coord;


                // We walk over the nodes assuming pairs?

                for (int i = 0; i < totalNumberOfListsOfSplineMeshSplines; i++)
                {

                    SplineSys spline = splines[i];

                    var myarray = new List<SplineMesh.SplineNode>();

                    // No splines for us...
                    if (spline.NodesCount == 0)
                    {
                        continue;
                    }

                    var global = new List<SplineNode>();

                    var positionalFactor = 1f;

                    myarray = new List<SplineNode>();

                    SplineNode refnode = new SplineNode(Vector3.positiveInfinity, Vector3.positiveInfinity);

                    SplineNode startnode = refnode;
                    SplineNode endnode = refnode;

                    Segment lastSegment = new Segment();
                    Segment thissegment = new Segment();

                    lastSegment.end.pos = thissegment.start.pos = Vector3.positiveInfinity;

                    foreach (var road in spline.lines.Reverse())
                    {

                        if(!splines[i].mergeSegments) myarray = new List<SplineNode>();

                        foreach (var current in road.segments)
                        {
                            // We need to check if the last segments end connects to the next segments start or RenderOutSplinesSoFar;

                            thissegment = road.segments.Where(x => x.GetHashCode() == current.GetHashCode()).First();


                            thissegment.start.pos -= DynamicHolder.transform.localPosition;
                            thissegment.end.pos -= DynamicHolder.transform.localPosition;

                            if (splines[i].mergeSegments)
                            {
                                //  Render out the segments if they are far apart (square root of 200) - have some data get parsed - and have been selected for merging
                                if (Vector3.SqrMagnitude(lastSegment.end.pos - thissegment.start.pos) > 200f && myarray.Count > 1)
                                {
                                    RenderOutSplinesSoFar(splineHolder, i, myarray);
                                    myarray = new List<SplineNode>();
                                }
                            }


                            lastSegment = thissegment;
                            // setup bool fence for list.
                            // add start if we didnt.

                            bool startExists = global.Exists(element => element.Position == thissegment.start.pos * positionalFactor);

                            if (!startExists)
                            {
                                startnode = new SplineMesh.SplineNode(thissegment.start.pos * positionalFactor, thissegment.start.pos * positionalFactor);

                                myarray.Add(startnode);

                                global.Add(startnode);
                            }

                            // and add end if we didnt.

                            bool endExists = global.Exists(element => element.Position == thissegment.end.pos * positionalFactor);

                            if (!endExists)
                            {
                                endnode = new SplineMesh.SplineNode(thissegment.end.pos * positionalFactor, thissegment.end.pos * positionalFactor);

                                myarray.Add(endnode);

                                global.Add(endnode);

                            }

                        }


                        if (myarray.Count == 0)
                        {


                            continue;
                        }


                        if (myarray.Count == 1)
                        {

                            // give us two by hook or crook
                            if (myarray.Contains(startnode))
                            {
                                endnode = new SplineMesh.SplineNode(lastSegment.end.pos * positionalFactor, lastSegment.end.pos * positionalFactor);
                                myarray.Add(endnode);
                            }
                            else
                            {
                                startnode = new SplineMesh.SplineNode(lastSegment.start.pos * positionalFactor, lastSegment.start.pos * positionalFactor);
                                myarray.Add(startnode);
                            }


                            continue;
                        }




                        if ((myarray[1].Position - myarray[0].Position).sqrMagnitude == 0)
                        {

                            continue;
                        }


                        // Render out as just segment node pairs
                      if(!splines[i].mergeSegments)  RenderOutSplinesSoFar(splineHolder, i, myarray);

                    }

                    // Attempt to merge near pairs
                    if (splines[i].mergeSegments) RenderOutSplinesSoFar(splineHolder, i, myarray);

                }
            }

            private void RenderOutSplinesSoFar(GameObject splineHolder, int i, List<SplineNode> myarray)
            {
                GameObject child = new GameObject();

                child.transform.parent = splineHolder.transform;

                SplineMesh.Spline splineScriptObj = child.GetComponent<SplineMesh.Spline>();
                //  if (splineScriptObj == null) splineScriptObj = splineHolder.transform.parent.GetComponentInChildren<SplineMesh.Spline>();
                if (splineScriptObj == null) splineScriptObj = child.gameObject.AddComponent<SplineMesh.Spline>();

                //finding holder
                SplineMesh.ExampleSower splineObj = child.GetComponent<SplineMesh.ExampleSower>();
                //   if (splineObj == null) splineObj = terrain.transform.parent.GetComponentInChildren<SplineMesh.ExampleSower>();
                if (splineObj == null) splineObj = child.gameObject.AddComponent<SplineMesh.ExampleSower>();


                Transform reft;
                GameObject go;

                //or creating it
                if (splineObj == null)
                {
                    go = new GameObject();

                }
                else
                {
                    go = child.gameObject;
                    reft = child.gameObject.transform;
                }


                go.transform.parent = splineHolder.transform;


                // TODO make this an actual hash and shove it in a table
                // string hash = string.Format("{0}_{1}_{4}_{5}|{2}_{3}", startvec.x, startvec.y, startvec.z, endvec.x, endvec.y, endvec.z);
                //  string fullhash = string.Format("{0}_{1}|{4}_{5}|{2}_{3}", startvec.x, startvec.y, startvec.z, endvec.x, endvec.y, endvec.z);
                string hash = string.Format("__SPLINE__{0}__{2}|{3}__{5}", myarray[0].Position.x, myarray[0].Position.y, myarray[0].Position.z, myarray[myarray.Count - 1].Position.x, myarray[myarray.Count - 1].Position.y, myarray[myarray.Count - 1].Position.z);

                var newSpline = go;


                // Extract the stuff we welded in the settings and reweld it to the output

                newSpline.name = hash + splines[i].chosenType.prefab.name;

                splineScriptObj.nodes = myarray;

                newSpline.transform.localPosition = new Vector3();


                splineObj.prefab = splines[i].chosenType.prefab;

                splineObj.isRandomYaw = splines[i].isRandomYaw;

                splineObj.spacing = splines[i].spacing;

                splineObj.spacingRange = splines[i].spacingRange;
                splineObj.offset = splines[i].offset;
                splineObj.offsetRange = splines[i].offsetRange;

                splineObj.scale = splines[i].scale;
                splineObj.scaleRange = splines[i].scaleRange;

                splineObj.spline.nodes = myarray.ToList();
                splineScriptObj.nodes = myarray.ToList();

                splineScriptObj.RefreshCurves();

                var scrp = newSpline.AddComponent<AlignNodesToTerrainOnEnable>();
                splineObj.Sow();

                scrp.RunIt();
            }

            public static ApplyData Empty
            { get { return new ApplyData() { splines = null }; } }



            public IEnumerator ApplyRoutine(Terrain terrain)
            {
                yield return null;

                Apply(terrain);

            }


            public int Resolution { get { return 0; } }


        }



        public override void ClearApplied(TileData data, Terrain terrain)
        {
            // IS Now HANDLED IN RunOnTileActionEvent.cs OnTileMoved(TerrainTile tile)


        }


        public int Resolution { get { return 0; } }


        public static void Purge(CoordRect rect, Terrain terrain)
        {

        }



    }
}



