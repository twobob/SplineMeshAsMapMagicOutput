using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine.Profiling;

using Den.Tools;
using Den.Tools.Matrices;
using Den.Tools.GUI;
using MapMagic.Core;
using MapMagic.Products;
using MapMagic.Nodes.GUI;
using MapMagic.Nodes;

namespace Twobob.Mm2
{

    public partial class SplinesEditor 
    {

        [Draw.Editor(typeof(MapSplineOutMark1))]
        public static void DrawObjectsOutput(MapSplineOutMark1 gen)
        {
            if (gen.posSettings == null) gen.posSettings = MapSplineOutMark1.CreatePosSettings(gen);

            using (Cell.LineStd)
                DrawObjectPrefabs(ref gen.prefabs, gen.guiMultiprefab, treeIcon: true);

            gen.allowReposition = false;


            using (Cell.LinePx(0))
            using (Cell.Padded(2, 2, 0, 0))
            {
                using (Cell.LineStd) Draw.ToggleLeft(ref gen.guiMultiprefab, "Multi-Prefab");

                Cell.EmptyRowPx(4);
                DrawPositioningSettings(gen.posSettings, billboardRotWaring: false);
            }
        }


    }
}