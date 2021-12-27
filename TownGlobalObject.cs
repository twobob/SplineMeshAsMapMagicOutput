using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Security.Cryptography;
using Den.Tools;
using UnityEngine;
using Twobob;
using System.Linq;
using System.Collections.Generic;
//using Town;
using System.Text;

// REF: https://dotnetfiddle.net/6P71ow

//public static class ConcurrentDictionaryExtensions
//{
//    public static TValue LazyGetOrAdd<TKey, TValue>(
//        this ConcurrentDictionary<TKey, Lazy<TValue>> dictionary,
//        TKey key,
//        Func<TKey, TValue> valueFactory)
//    {
//        if (dictionary == null) throw new ArgumentNullException("dictionary");
//        var result = dictionary.GetOrAdd(key, new Lazy<TValue>(() => valueFactory(key)));
//        return result.Value;
//    }

//    public static TValue AddOrUpdate<TKey, TValue>(this ConcurrentDictionary<TKey, Lazy<TValue>> dictionary, TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
//    {
//        if (dictionary == null) throw new ArgumentNullException("dictionary");
//        var result = dictionary.AddOrUpdate(key, new Lazy<TValue>(() => addValueFactory(key)), (key2, old) => new Lazy<TValue>(() => updateValueFactory(key2, old.Value)));
//        return result.Value;
//    }
//}



//public static AOTABundle bundle;

public static class TownGlobalObject
{
  //  public static Town.Town homeTown;


    public static bool InitialTownsGenerated = false;

    // We know how many items we want to insert into the ConcurrentDictionary.
    // So set the initial capacity to some prime number above that, to ensure that
    // the ConcurrentDictionary does not need to be resized while initializing it.
    // static int NUMITEMS = 49;
    static int NUMITEMS = 81;

    // For now this will mean a hard limit of 10/10 size cites split over a space from the 0,0 center
    // So we can use every ODD quare number for this calc to get a limit

    //  1, 9, 25, 49, 81, 121
    // shows that a perimiter of (4 layers of 10*10 squared) 
    // cities is the upper maximum for now until we figure out a better system,

    // public  static int initialCapacity = 53;
    public static int initialCapacity = 83;

    // The higher the concurrencyLevel, the higher the theoretical number of operations
    // that could be performed concurrently on the ConcurrentDictionary.  However, global
    // operations like resizing the dictionary take longer as the concurrencyLevel rises.
    // For the purposes of this example, we'll compromise at numCores * 2.
    static int numProcs = System.Environment.ProcessorCount;
   public static int concurrencyLevel = numProcs * 2;

  //  public static int CityTileModulo = 5;  // MUST BE MULTIPLE OF 2 ?


    // 
    // ADJUST THE TOWN TO BUNDLEs

   // public static LazyConcurrentDictionary<Coord, Town.Town> towns = new LazyConcurrentDictionary<Coord, Town.Town>();
   // public static Dictionary<Coord, AOTABundle> bundles; //= new Dictionary<Coord, AOTABundle>();
  //  public static Dictionary<Coord, Town.Town> townsData =  new Dictionary<Coord, Town.Town>();

    public static List<Coord> renderedTowns = new List<Coord>();
    public static List<BoxCollider> renderedBoxColliders = new List<BoxCollider>();

   // public static TownMeshRenderer MeshRenderer;

    public static bool TownWaitingToRender { get; set; }
    public static string NextTownPreviewName { get; set; }

   // public static Queue<TownMeshRenderer> TownsWaitingToRender = new Queue<TownMeshRenderer>();

    public static int LastPreviewedTownId = 0;
    public static bool PreviewActive = false;

    public static Coord GetClosestTownCoord(List<Coord> locations, Coord fromThis)
    {
        StringBuilder sb = new StringBuilder();
        Coord directionToTarget = new Coord();
        Coord bestTarget = new Coord(10000);
        float closestDistanceSqr = Mathf.Infinity;
        Coord currentPosition = fromThis;
        foreach (Coord potentialTarget in locations)
        {

        

            directionToTarget = potentialTarget - currentPosition;
           float dSqrToTarget = directionToTarget.SqrMagnitude;


          //  float dSqrToTarget = Coord.DistanceSq(potentialTarget, currentPosition);
            sb.Append(potentialTarget);
            if (dSqrToTarget == 0)
            {
                bestTarget = potentialTarget;
                break;
            }
            if (dSqrToTarget < closestDistanceSqr)
            {
                
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }
        //Debug.LogFormat("Final Mag was {0} : for {1} : matching {2} in list: {3} of {4} length", closestDistanceSqr, fromThis, bestTarget, sb.ToString(), locations.Count);
        return bestTarget;
    }



    public static Coord GetClosest(Coord startPosition, List<Coord> pickups)
    {
        Coord bestTarget = new Coord();
        float closestDistanceSqr = Mathf.Infinity;
        Coord directionToTarget = new Coord();

        StringBuilder sb = new StringBuilder();


        foreach (Coord potentialTarget in pickups)
        {
            directionToTarget = potentialTarget - startPosition;

            float dSqrToTarget = directionToTarget.SqrMagnitude;

            if (dSqrToTarget < closestDistanceSqr)
            {
                sb.Append(potentialTarget);
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }


       // Debug.LogFormat("Final Mag was {0} : for {1} : matching {2} in list: {3} of {4} length" , directionToTarget.Magnitude, startPosition, bestTarget, sb.ToString(), pickups.Count);
        return bestTarget;
    }

    public static float GetClosestMagnitude(Coord startPosition, List<Coord> pickups)
    {
        Coord bestTarget = new Coord();
        float closestDistanceSqr = Mathf.Infinity;
        Coord directionToTarget = new Coord();


        foreach (Coord potentialTarget in pickups)
        {
            directionToTarget = potentialTarget - startPosition;

            float dSqrToTarget = directionToTarget.SqrMagnitude;

            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }


    
        return directionToTarget.Magnitude;
    }



    public static Coord GetIndexAtCoord(Coord place)
    {

        return place;

        
       // List<Coord> coords = townsData.Keys.ToList<Coord> ();


       //  Debug.LogFormat("{0} results in {1}", coords.Count, nameof(townsData.Keys));

        //foreach (var item in coords)
        //{
        //    Debug.LogFormat("{0}:{1},", item.x, item.z);

        //}


    //    Coord bestTarget = GetClosestTownCoord(coords, place);

        //Coord bestTarget = GetClosest (place, coords);

  //      Debug.LogFormat("{0}:{1}, for {2} as {3}", bestTarget.x, bestTarget.z, place, nameof(bestTarget));
  //
     //   return bestTarget;
        

        ////     |------------------000|000---------------------------| desired tile distribution
        ////     |----------------------000|000-------------------------|  0 = search range.
        ////     |---^-----^-----^-----^---X-^-----^-----^-----^-----^--|  X=guy.  ^=City           
        ////     |------------------9876543210123456789-----------------|   city at 0 - player a -2
        ////     |---------------9876543210123456789--------------------|



        //int resultminx = place.x;
        //int resultmaxx = place.x;
        //int distanceminx = 0;
        //int distancemaxx = 0;

        //int resultminz = place.z;
        //int resultmaxz = place.z;
        //int distanceminz = 0;
        //int distancemaxz = 0;

        //int walk = 1;

        //// are we on city? if not decrement and check again   // And store the distance
        //while (! (resultminx % CityTileModulo == 0))
        //{ resultminx -= walk;
        //    distanceminx++;
        //}

        //// are we on city? if not increment and check again  // And store the distance

        //while (!(resultmaxx % CityTileModulo == 0))
        //{ resultmaxx  += walk;
        //    distancemaxx++;
        //}


        //// are we on city? if not decrement and check again   // And store the distance
        //while (!(resultminz % CityTileModulo == 0))
        //{
        //    resultminz -= walk;
        //    distanceminz++;
        //}

        //// are we on city? if not increment and check again  // And store the distance

        //while (!(resultmaxz % CityTileModulo == 0))
        //{
        //    resultmaxz += walk;
        //    distancemaxz++;
        //}

        //// Compare the distances walked   and do somethng

        //int resultX = (distanceminx < distancemaxx) ? resultminx : resultmaxx;

        //int resultZ = (distanceminz < distancemaxz) ? resultminz : resultmaxz;


        //return new Coord(resultX, resultZ);

    }

    //public static Town.Town MakeTown(
    //    Coord index, 
    //    int townOffsetX = 0, // WE USE THESE <---- TO DO THE SUB-TILE RENDER OFFSETS FROM TOWN INFO
    //    int townOffsetZ = 0, 
    //    int seed =0,
    //    int patches = 0)
    //{

    // //   Debug.Log("*** REALLY * MAKING A TOWN ***********");
       

    //    Town.TownOptions opt = new Town.TownOptions();
        
    //    // TODO. make global Tile size from Terrain
    //    opt.mapOffset = new Town.Geom.Vector2(index.x * 1000, index.z *1000 );
    //    opt.townOffset = new Town.Geom.Vector2(townOffsetX * 1000, townOffsetZ * 1000);
    //    opt.Patches = (patches ==0)? RandomGen.NextValidRandomPatchAmountFromTGOSRange() : patches ;
    //    opt.Overlay = TownGlobalObjectService.ProduceOverlay;
    //    opt.Water = false; // ( RandomGen.Next() % 2 == 0)?true:false;
    //    opt.CityDetail = true;
    //    opt.Walls = true;
    //    opt.Towers = true;
    //    //   Debug.Log(opt.Patches + " patches requested");
    //    opt.Seed = (seed == 0) ? opt.Seed : seed;

    //    opt.coord = index;

    //    Town.Town town = new Town.Town(opt);
    //   // town.coord = index;
    //   // town.Options.coord = index;

    //    MeshRenderer = new TownMeshRenderer(town, opt, TownGlobalObjectService.rendererOptions);

    //    TownGlobalObject.TownsWaitingToRender.Enqueue(MeshRenderer);

    //    TownGlobalObject.TownWaitingToRender = true;


    //    //Debug.LogFormat("{0} is {1} for location with {2} of {3} and {4} of {5} for {6} of size {7} ", 
    //    //    nameof(index), 
    //    //    index, 
    //    //    nameof(opt.mapOffset), 
    //    //    opt.mapOffset, 
    //    //    nameof(opt.townOffset), 
    //    //    opt.townOffset, 
    //    //    town.name,
    //    //    opt.Patches
    //    //    );


      

    //    return town;


     


    //}


    //public static AOTABundle SetBundle(Coord index, AOTABundle bundle)
    //{

    //    bundles[index] = bundle;// AddOrUpdate(index, (ka) => bundle, (k, v) => v);
    //    return bundle;

    //}

    //public static AOTABundle SetBundle(Coord index, AOTABundle bundle)
    //{

    //    towns.AddOrUpdate(index, (ka) => bundle, (k, v) =>  v);
    //    return bundle;  

    //}



    //public static AOTABundle GetBundle(Coord index)
    //{

    //    // Make request granular

    //    Coord lookup = GetIndexAtCoord(index);

    //    // ensure we actually have a set of records and we only get them once by using lazy.

    //    //  Debug.LogFormat("{0} is {1} and {2} for {3}" , nameof(lookup), lookup, index , nameof(index));

    //    var lazyTown = towns.GetOrAdd(lookup, k => MakeTown(lookup));
    //    // var lazyTown2 = towns.AddOrUpdate(lookup, k => {  }  );

    //    //  Debug.LogFormat("{0} is {1} for lazy location {3} at {2}", nameof(lookup), lookup, index, lazyTown.name);

    //    // Only here MakeTown() will be called.
    //    var concreteTown = lazyTown;
    //    //
    //    // Debug.LogFormat("{0} is {1} for concreteTown town {4} at {2} with {3} patches", nameof(concreteTown.mapOffset), concreteTown.mapOffset, index, concreteTown.Patches.Count, concreteTown.name);

    //    return lazyTown;

    //}

    //public static Town.Town GetTown(Coord index)
    //{

    //    // Make request granular

    //    Coord lookup = GetIndexAtCoord(index);

    //    // ensure we actually have a set of records and we only get them once by using lazy.

    //    //  Debug.LogFormat("{0} is {1} and {2} for {3}" , nameof(lookup), lookup, index , nameof(index));

    //    var lazyTown = towns.GetOrAdd(lookup, k => MakeTown(lookup));
    //    // var lazyTown2 = towns.AddOrUpdate(lookup, k => {  }  );

    //    //  Debug.LogFormat("{0} is {1} for lazy location {3} at {2}", nameof(lookup), lookup, index, lazyTown.name);

    //    // Only here MakeTown() will be called.
    //    var concreteTown = lazyTown;
    //    //
    //    // Debug.LogFormat("{0} is {1} for concreteTown town {4} at {2} with {3} patches", nameof(concreteTown.mapOffset), concreteTown.mapOffset, index, concreteTown.Patches.Count, concreteTown.name);

    //    return lazyTown;

    //}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
//    public static bool CanGetTownBundle(Coord index)
//    {

//        // Make request granular



//        Coord lookup = GetIndexAtCoord(index);


//        // Dont request granular

//        //  Coord lookup = index;



//        // ensure we actually have a set of records and we only get them once by using lazy.

//      //    Debug.LogFormat("GetTownBundle() {0} is {1} and {2} for {3}", nameof(lookup), lookup, index , nameof(index));  



//        // This time round we dont get it this way....

//        //Debug.LogFormat("a lookup of {0},{1}", lookup.x, lookup.z);

//        //if (!bundles.ContainsKey(lookup))
//        //{
//        //    return false;
//        //}
//        //else
//        //{
//        //    return true;
//        //}

     


//        //   var lazyTown = new AOTABundle(index); // towns.GetOrAdd(lookup, k => new AOTABundle(index)); // MakeTown(lookup));

//        //var lazyTown = SetBundle(index, bundle); // MakeTown(lookup));


//        //  var lazyTown2 = towns.

//        //  Debug.LogFormat("{0} is {1} for lazy location {3} at {2}", nameof(lookup), lookup, index, lazyTown.name);

//        // Only here MakeTown() will be called.
//        //   var concreteTown = lazyTown;
//        //
//        // Debug.LogFormat("{0} is {1} for concreteTown town {4} at {2} with {3} patches", nameof(concreteTown.mapOffset), concreteTown.mapOffset, index, concreteTown.Patches.Count, concreteTown.name);

//        //   return lazyTown;

//    }

   
}

//public static class RandomGen
//{
//    private static RNGCryptoServiceProvider _global =
//        new RNGCryptoServiceProvider();
//    [ThreadStatic]
//    private static System.Random _local;

   
//    //public static int NextValidRandomPatchAmountFromTGOSRange()
//    //{
//    //    return Next(TownGlobalObjectService.PatchCap, TownGlobalObjectService.PatchFloor);
//    //}

//    public static bool FlipACoin()
//    {
//        System.Random inst = _local;
//        if (inst == null)
//        {
//            byte[] buffer = new byte[4];
//            _global.GetBytes(buffer);
//            _local = inst = new System.Random(
//                BitConverter.ToInt32(buffer, 0));
//        }
//        return inst.Next() % 2 == 0;
//    }



//    public static int Next(int Ceil = int.MaxValue , int Floor = int.MinValue)
//    {
//        System.Random inst = _local;
//        if (inst == null)
//        {
//            byte[] buffer = new byte[4];
//            _global.GetBytes(buffer);
//            _local = inst = new System.Random(
//                BitConverter.ToInt32(buffer, 0));
//        }
//        return Mathf.Max(Floor, inst.Next() % Mathf.Max(1, Ceil));
//    }
//}



public class LazyConcurrentDictionary<TKey, TValue>
{
    private readonly ConcurrentDictionary<TKey, Lazy<TValue>> concurrentDictionary;

    public LazyConcurrentDictionary()
    {

        // Let's make sure it get setup with our prime values
        this.concurrentDictionary = new ConcurrentDictionary<TKey, Lazy<TValue>>(TownGlobalObject.concurrencyLevel, TownGlobalObject.initialCapacity);
    }

    public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
    {
        var lazyResult = this.concurrentDictionary.GetOrAdd(key, k => new Lazy<TValue>(() => valueFactory(k), LazyThreadSafetyMode.ExecutionAndPublication));

        return lazyResult.Value;
    }

  //  AddOrUpdate<TKey, TValue>(this ConcurrentDictionary<TKey, Lazy<TValue>> dictionary, TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)


    public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
    {
      //  var lazyResult = this.concurrentDictionary.AddOrUpdate(key, k => new Lazy<TValue>(() => valueFactory(k), LazyThreadSafetyMode.ExecutionAndPublication));

        var lazyResult = this.concurrentDictionary.AddOrUpdate(key, new Lazy<TValue>(() => addValueFactory(key)), (key2, old) => new Lazy<TValue>(() => updateValueFactory(key2, old.Value)));
        return lazyResult.Value;

       
    }


}

