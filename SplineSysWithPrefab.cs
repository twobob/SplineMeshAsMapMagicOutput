using Den.Tools;
using Den.Tools.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Twobob.Mm2
{
    public class SplineSysWithPrefab : SplineSys
    {

        //  public SplineSys SplineSys;
        public ObjectsPool.Prototype chosenType;

        public float scale = 1f, scaleRange = 0;

        public bool mergeSegments;

        public bool spacingFromScale;

        public float spacing = 1f, spacingRange = 0;

        public float offset = 0, offsetRange = 0;

        public bool isRandomYaw = false;


        public SplineSysWithPrefab(SplineSys src) 
		{ 
			CopyLinesFrom(src.lines);

            scale = 1f;
            spacing = 1f;

            guiDrawNodes = src.guiDrawNodes;
			guiDrawSegments = src.guiDrawSegments;
			guiDrawDots = src.guiDrawDots;
			guiDotsCount = src.guiDotsCount;
			guiDotsEquidist = src.guiDotsEquidist;
		}



    public SplineSysWithPrefab()  // Just in case the serialisier gets all upset with itsself again.
        { 
        
        
        
        
        }



    }

}