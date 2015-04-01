using UnityEngine;
using System.Collections;

public class TrackBuildRTerrain
{
    public static void MergeTerrain(TrackBuildRTrack track, Terrain terrain)
    {
        TerrainData terrainData = terrain.terrainData;
        int terrainWidth = terrainData.heightmapWidth;
        int terrainHeight = terrainData.heightmapHeight;
        float terrainHeightmapY = terrain.terrainData.heightmapScale.y;
        float terrainY = terrain.transform.position.y / terrainHeightmapY;
        Vector3 meshScale = terrainData.heightmapScale;
        float terrainAccuracy = track.terrainAccuracy;
        float terrainMergeMargin = track.terrainMergeMargin;

        float[,] originalData = terrainData.GetHeights(0, 0, terrainWidth, terrainHeight);
        float[,] mergeData = new float[terrainWidth, terrainHeight];
        float[,] modifiedData = new float[terrainWidth, terrainHeight];
        int[,] modifiedPointLock = new int[terrainWidth, terrainHeight];

        Bounds trackBounds = new Bounds();
        int numberOfCurves = track.numberOfCurves;
        for (int i = 0; i < numberOfCurves; i++)
        {
            TrackBuildRPoint curve = track[i];
            if(curve.holder == null)
                continue;
            Renderer[] rends = curve.holder.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in rends)
            {
                trackBounds.Encapsulate(rend.bounds);
            }
        }

        Vector3 trackOffset = track.transform.position - terrain.transform.position;
        Vector3 trackScale = new Vector3(trackBounds.size.x / terrainData.size.x, 1.0f / terrain.terrainData.size.y, trackBounds.size.z / terrainData.size.z);

        float mergeWidth = track.terrainMergeWidth;
        AnimationCurve mergeCurve = track.mergeCurve;
        float minScaleUnit = Mathf.Min(meshScale.x, meshScale.z);
        for(int i = 0; i < numberOfCurves; i++)
        {
            TrackBuildRPoint curve = track[i];
            int storedPointSize = curve.storedPointSize;
            for(int p = 0; p < storedPointSize-1; p++)
            {
                Vector3 pointA = curve.sampledPoints[p];
                Vector3 pointB = curve.sampledPoints[p+1];
                Vector3 crossA = curve.sampledTrackCrosses[p];
                Vector3 crossB = curve.sampledTrackCrosses[p+1];
                float widthA = curve.sampledWidths[p] * terrainMergeMargin;
                float widthB = curve.sampledWidths[p + 1] * terrainMergeMargin;
//                float heightA = (pointA.y - terrainAccuracy) * trackScale.y - terrainY;
//                float heightB = (pointB.y - terrainAccuracy) * trackScale.y - terrainY;

                Vector3 lpointA = (pointA - crossA * (widthA + mergeWidth));
                Vector3 rpointA = (pointA + crossA * (widthA + mergeWidth));
                Vector3 lpointB = (pointB - crossB * (widthB + mergeWidth));
                Vector3 rpointB = (pointB + crossB * (widthB + mergeWidth));

                float crownA = curve.sampledCrowns[p] - terrainAccuracy;
                float crownB = curve.sampledCrowns[p + 1] - terrainAccuracy;

                float pointDistanceLeft = Vector3.Distance(lpointA, lpointB) * 1.8f;
                float pointDistanceRight = Vector3.Distance(rpointA, rpointB) * 1.8f;
                float pointDistance = Mathf.Max(pointDistanceLeft, pointDistanceRight);
                float pointFillResolution = minScaleUnit/ pointDistance * 0.125f;

                float heightLA = (lpointA.y - terrainAccuracy) * trackScale.y - terrainY;
                float heightRA = (rpointA.y - terrainAccuracy) * trackScale.y - terrainY;
                float heightLB = (lpointB.y - terrainAccuracy) * trackScale.y - terrainY;
                float heightRB = (rpointB.y - terrainAccuracy) * trackScale.y - terrainY;

                for (float pf = 0; pf < 1; pf += pointFillResolution)//point a to point b
                {
                    //Track Filler
                    Vector3 fillPoint = Vector3.Lerp(pointA, pointB, pf);
                    Vector3 fillCross = Vector3.Lerp(crossA, crossB, pf);
                    float fillWidth = Mathf.Lerp(widthA, widthB, pf);// *1.2f;
                    float fillCrown = Mathf.Lerp(crownA, crownB, pf);// *1.2f;
                    float fillTrackHeightL = Mathf.Lerp(heightLA, heightLB, pf);
                    float fillTrackHeightR = Mathf.Lerp(heightRA, heightRB, pf);

                    Vector3 leftTrackPoint = fillPoint - fillCross * fillWidth;
                    Vector3 rightTrackPoint = fillPoint + fillCross * fillWidth;

                    int leftX = Mathf.RoundToInt(((leftTrackPoint.x + trackOffset.x) / trackBounds.size.x * trackScale.x) * terrainData.heightmapWidth);
                    int leftY = Mathf.RoundToInt(((leftTrackPoint.z + trackOffset.z) / trackBounds.size.z * trackScale.z) * terrainData.heightmapHeight);
                    int rightX = Mathf.RoundToInt(((rightTrackPoint.x + trackOffset.x) / trackBounds.size.x * trackScale.x) * terrainData.heightmapWidth);
                    int rightY = Mathf.RoundToInt(((rightTrackPoint.z + trackOffset.z) / trackBounds.size.z * trackScale.z) * terrainData.heightmapHeight);

                    int diffX = leftX - rightX;
                    int diffY = leftY - rightY;
                    int trackCrossFillAmount = Mathf.Max(Mathf.Abs(diffX),1) * Mathf.Max(Mathf.Abs(diffY),1);
                    for (int f = 0; f < trackCrossFillAmount; f++)//left to right
                    {
                        float move = f / (float)trackCrossFillAmount;
                        int fillX = Mathf.RoundToInt(Mathf.Lerp(leftX, rightX, move));
                        int fillY = Mathf.RoundToInt(Mathf.Lerp(leftY, rightY, move));
                        if(fillX < 0 || fillY < 0 || fillX >= terrainWidth || fillY >= terrainHeight)
                            continue;
                        float crownHeight = Mathf.Sin(move * Mathf.PI) * fillCrown / terrainHeightmapY;
                        float fillTrackHeight = Mathf.Lerp(fillTrackHeightL, fillTrackHeightR, move) + crownHeight;
                        int currentPointLock = modifiedPointLock[fillY, fillX];
                        if (currentPointLock == 0 || mergeData[fillY, fillX] > fillTrackHeight)
                            mergeData[fillY, fillX] = fillTrackHeight;
                        modifiedPointLock[fillY, fillX] = 1;//point lock
                    }

                    //Merge
                    Vector3 leftMergePoint = leftTrackPoint - fillCross * mergeWidth;

                    int leftMergeLeftX = Mathf.RoundToInt(((leftMergePoint.x + trackOffset.x) / trackBounds.size.x * trackScale.x) * terrainData.heightmapWidth);
                    int leftMergeLeftY = Mathf.RoundToInt(((leftMergePoint.z + trackOffset.z) / trackBounds.size.z * trackScale.z) * terrainData.heightmapHeight);
                    int leftMergeRightX = Mathf.RoundToInt(((leftTrackPoint.x + trackOffset.x) / trackBounds.size.x * trackScale.x) * terrainData.heightmapWidth);
                    int leftMergeRightY = Mathf.RoundToInt(((leftTrackPoint.z + trackOffset.z) / trackBounds.size.z * trackScale.z) * terrainData.heightmapHeight);
//                  
                    int leftMergeDiffX = leftMergeLeftX - leftMergeRightX;
                    int leftMergeDiffY = leftMergeLeftY - leftMergeRightY;
                    int leftMergeFillAmount = Mathf.Max(Mathf.Abs(leftMergeDiffX), 1) * Mathf.Max(Mathf.Abs(leftMergeDiffY), 1);// Mathf.Max(Mathf.Abs(leftMergeDiffX), Mathf.Abs(leftMergeDiffY));
                    for (int f = 0; f < leftMergeFillAmount; f++)
                    {
                        float move = f / (float)leftMergeFillAmount;
                        int fillX = Mathf.RoundToInt(Mathf.Lerp(leftMergeLeftX, leftMergeRightX, move));
                        int fillY = Mathf.RoundToInt(Mathf.Lerp(leftMergeLeftY, leftMergeRightY, move));
                        if (fillX < 0 || fillY < 0 || fillX >= terrainWidth || fillY >= terrainHeight)
                            continue;
                        float curveStrength = mergeCurve.Evaluate(move);
                        float fillTrackHeight = fillTrackHeightL;
                        float mergeHeight = Mathf.Lerp(originalData[fillY, fillX], fillTrackHeight, curveStrength);
                        int pointLock = modifiedPointLock[fillY, fillX];
                        float currentMergeHeight = mergeData[fillY, fillX];
                        float currentDifference = Mathf.Abs(currentMergeHeight - fillTrackHeight);
                        float newDifference = Mathf.Abs(mergeHeight - fillTrackHeight);
                        if (newDifference < currentDifference && pointLock == 0)
                        {
                            mergeData[fillY, fillX] = mergeHeight;
                        }
                    }

                    Vector3 rightMergePoint = rightTrackPoint + fillCross * mergeWidth;
                    int rightMergeLeftX = Mathf.RoundToInt(((rightMergePoint.x + trackOffset.x) / trackBounds.size.x * trackScale.x) * terrainData.heightmapWidth);
                    int rightMergeLeftY = Mathf.RoundToInt(((rightMergePoint.z + trackOffset.z) / trackBounds.size.z * trackScale.z) * terrainData.heightmapHeight);
                    int rightMergeRightX = Mathf.RoundToInt(((rightTrackPoint.x + trackOffset.x) / trackBounds.size.x * trackScale.x) * terrainData.heightmapWidth);
                    int rightMergeRightY = Mathf.RoundToInt(((rightTrackPoint.z + trackOffset.z) / trackBounds.size.z * trackScale.z) * terrainData.heightmapHeight);
                    
                    //                  
                    int rightMergeDiffX = rightMergeLeftX - rightMergeRightX;
                    int rightMergeDiffY = rightMergeLeftY - rightMergeRightY;
                    int rightMergeFillAmount = Mathf.Max(Mathf.Abs(rightMergeDiffX),1) * Mathf.Max(Mathf.Abs(rightMergeDiffY),1);// Mathf.Max(Mathf.Abs(leftMergeDiffX), Mathf.Abs(leftMergeDiffY));
                    for (int f = 0; f < rightMergeFillAmount; f++)
                    {
                        float move = f / (float)rightMergeFillAmount;
                        int fillX = Mathf.RoundToInt(Mathf.Lerp(rightMergeLeftX, rightMergeRightX, move));
                        int fillY = Mathf.RoundToInt(Mathf.Lerp(rightMergeLeftY, rightMergeRightY, move));
                        if (fillX < 0 || fillY < 0 || fillX >= terrainWidth || fillY >= terrainHeight)
                            continue;
                        float curveStrength = mergeCurve.Evaluate(move);
                        float fillTrackHeight = fillTrackHeightR;
                        float mergeHeight = Mathf.Lerp(originalData[fillY, fillX], fillTrackHeight, curveStrength);
                        int pointLock = modifiedPointLock[fillY, fillX];
                        float currentMergeHeight = mergeData[fillY, fillX];
                        float currentDifference = Mathf.Abs(currentMergeHeight - fillTrackHeight);
                        float newDifference = Mathf.Abs(mergeHeight - fillTrackHeight);
                        if (newDifference < currentDifference && pointLock == 0)
                        {
                            mergeData[fillY, fillX] = mergeHeight;
                        }
                    }
                }
            }
        }

        for(int x = 0; x < terrainWidth; x++)
        {
            for(int y = 0; y < terrainHeight; y++)
            {
                bool isNotEdge = x > 0 && x < terrainWidth-1 && y > 0 && y < terrainHeight-1;
                if (mergeData[x, y] == 0)
                {
                    int mergeNeighbours = 0;
                    if(isNotEdge)
                    {
                        mergeNeighbours += (mergeData[x+1, y] != 0) ? 1 : 0;
                        mergeNeighbours += (mergeData[x-1, y] != 0) ? 1 : 0;
                        mergeNeighbours += (mergeData[x, y+1] != 0) ? 1 : 0;
                        mergeNeighbours += (mergeData[x, y-1] != 0) ? 1 : 0;
                    }
                    if(mergeNeighbours > 1)//if a hole is surounded by rasied terrain in two or more neighbouring places
                    {
                        float mergeHeight = 0;
                        mergeHeight += (mergeData[x + 1, y] != 0) ? mergeData[x + 1, y] : 0;
                        mergeHeight += (mergeData[x - 1, y] != 0) ? mergeData[x - 1, y] : 0;
                        mergeHeight += (mergeData[x, y + 1] != 0) ? mergeData[x, y + 1] : 0;
                        mergeHeight += (mergeData[x, y - 1] != 0) ? mergeData[x, y - 1] : 0;
                        modifiedData[x, y] = mergeHeight / mergeNeighbours;//clean up holes
                    }
                    else
                    {
                        modifiedData[x, y] = originalData[x, y];//use original
                    }
                }
                else
                {
                    modifiedData[x, y] = mergeData[x, y];
                }
            }
        }

        terrainData.SetHeights(0, 0, modifiedData);
        terrain.terrainData = terrainData;
    }

    /// <summary>
    /// Warning - not working correctly - yet...
    /// </summary>
    /// <param name="track"></param>
    /// <param name="terrain"></param>
    /// <param name="curve"></param>
    public static void MergeTerrain(TrackBuildRTrack track, Terrain terrain, TrackBuildRPoint curve)
    {
        //        ResetTerrain(track, terrain);

        TerrainData terrainData = terrain.terrainData;
        int terrainWidth = terrainData.heightmapWidth;
        int terrainHeight = terrainData.heightmapHeight;
        float terrainHeightmapY = terrain.terrainData.heightmapScale.y;
        float terrainY = terrain.transform.position.y / terrainHeightmapY;
        Vector3 meshScale = terrainData.heightmapScale;
        float terrainAccuracy = track.terrainAccuracy;
        float terrainMergeMargin = track.terrainMergeMargin;

        float[,] originalData = terrainData.GetHeights(0, 0, terrainWidth, terrainHeight);
        float[,] mergeData = new float[terrainWidth,terrainHeight];
        float[,] modifiedData = new float[terrainWidth,terrainHeight];
        int[,] modifiedPointLock = new int[terrainWidth,terrainHeight];

        Bounds trackBounds = new Bounds();
        if(curve.holder != null)
        {
            Renderer[] rends = curve.holder.GetComponentsInChildren<Renderer>();
            foreach(Renderer rend in rends)
                trackBounds.Encapsulate(rend.bounds);
        }

        Vector3 trackOffset = track.transform.position - terrain.transform.position;
        Vector3 trackScale = new Vector3(trackBounds.size.x / terrainData.size.x, 1.0f / terrain.terrainData.size.y, trackBounds.size.z / terrainData.size.z);

        float mergeWidth = track.terrainMergeWidth;
        AnimationCurve mergeCurve = track.mergeCurve;
        float minScaleUnit = Mathf.Min(meshScale.x, meshScale.z);
        int storedPointSize = curve.storedPointSize;
        for(int p = 0; p < storedPointSize - 1; p++)
        {
            Vector3 pointA = curve.sampledPoints[p];
            Vector3 pointB = curve.sampledPoints[p + 1];
            Vector3 crossA = curve.sampledTrackCrosses[p];
            Vector3 crossB = curve.sampledTrackCrosses[p + 1];
            float widthA = curve.sampledWidths[p] * terrainMergeMargin;
            float widthB = curve.sampledWidths[p + 1] * terrainMergeMargin;
            //                float heightA = (pointA.y - terrainAccuracy) * trackScale.y - terrainY;
            //                float heightB = (pointB.y - terrainAccuracy) * trackScale.y - terrainY;

            Vector3 lpointA = (pointA - crossA * (widthA + mergeWidth));
            Vector3 rpointA = (pointA + crossA * (widthA + mergeWidth));
            Vector3 lpointB = (pointB - crossB * (widthB + mergeWidth));
            Vector3 rpointB = (pointB + crossB * (widthB + mergeWidth));

            float crownA = curve.sampledCrowns[p] - terrainAccuracy;
            float crownB = curve.sampledCrowns[p + 1] - terrainAccuracy;

            float pointDistanceLeft = Vector3.Distance(lpointA, lpointB) * 1.8f;
            float pointDistanceRight = Vector3.Distance(rpointA, rpointB) * 1.8f;
            float pointDistance = Mathf.Max(pointDistanceLeft, pointDistanceRight);
            float pointFillResolution = minScaleUnit / pointDistance * 0.125f;

            float heightLA = (lpointA.y - terrainAccuracy) * trackScale.y - terrainY;
            float heightRA = (rpointA.y - terrainAccuracy) * trackScale.y - terrainY;
            float heightLB = (lpointB.y - terrainAccuracy) * trackScale.y - terrainY;
            float heightRB = (rpointB.y - terrainAccuracy) * trackScale.y - terrainY;

            for(float pf = 0; pf < 1; pf += pointFillResolution)//point a to point b
            {
                //Track Filler
                Vector3 fillPoint = Vector3.Lerp(pointA, pointB, pf);
                Vector3 fillCross = Vector3.Lerp(crossA, crossB, pf);
                float fillWidth = Mathf.Lerp(widthA, widthB, pf);// *1.2f;
                float fillCrown = Mathf.Lerp(crownA, crownB, pf);// *1.2f;
                float fillTrackHeightL = Mathf.Lerp(heightLA, heightLB, pf);
                float fillTrackHeightR = Mathf.Lerp(heightRA, heightRB, pf);

                Vector3 leftTrackPoint = fillPoint - fillCross * fillWidth;
                Vector3 rightTrackPoint = fillPoint + fillCross * fillWidth;

                int leftX = Mathf.RoundToInt(((leftTrackPoint.x + trackOffset.x) / trackBounds.size.x * trackScale.x) * terrainData.heightmapWidth);
                int leftY = Mathf.RoundToInt(((leftTrackPoint.z + trackOffset.z) / trackBounds.size.z * trackScale.z) * terrainData.heightmapHeight);
                int rightX = Mathf.RoundToInt(((rightTrackPoint.x + trackOffset.x) / trackBounds.size.x * trackScale.x) * terrainData.heightmapWidth);
                int rightY = Mathf.RoundToInt(((rightTrackPoint.z + trackOffset.z) / trackBounds.size.z * trackScale.z) * terrainData.heightmapHeight);

                int diffX = leftX - rightX;
                int diffY = leftY - rightY;
                int trackCrossFillAmount = Mathf.Max(Mathf.Abs(diffX), 1) * Mathf.Max(Mathf.Abs(diffY), 1);
                for(int f = 0; f < trackCrossFillAmount; f++)//left to right
                {
                    float move = f / (float)trackCrossFillAmount;
                    int fillX = Mathf.RoundToInt(Mathf.Lerp(leftX, rightX, move));
                    int fillY = Mathf.RoundToInt(Mathf.Lerp(leftY, rightY, move));
                    if(fillX < 0 || fillY < 0 || fillX > terrainWidth || fillY > terrainHeight)
                        continue;
                    float crownHeight = Mathf.Sin(move * Mathf.PI) * fillCrown / terrainHeightmapY;
                    float fillTrackHeight = Mathf.Lerp(fillTrackHeightL, fillTrackHeightR, move) + crownHeight;
                    int currentPointLock = modifiedPointLock[fillY, fillX];
                    if(currentPointLock == 0 || mergeData[fillY, fillX] > fillTrackHeight)
                        mergeData[fillY, fillX] = fillTrackHeight;
                    modifiedPointLock[fillY, fillX] = 1;//point lock
                }

                //Merge
                Vector3 leftMergePoint = leftTrackPoint - fillCross * mergeWidth;

                int leftMergeLeftX = Mathf.RoundToInt(((leftMergePoint.x + trackOffset.x) / trackBounds.size.x * trackScale.x) * terrainData.heightmapWidth);
                int leftMergeLeftY = Mathf.RoundToInt(((leftMergePoint.z + trackOffset.z) / trackBounds.size.z * trackScale.z) * terrainData.heightmapHeight);
                int leftMergeRightX = Mathf.RoundToInt(((leftTrackPoint.x + trackOffset.x) / trackBounds.size.x * trackScale.x) * terrainData.heightmapWidth);
                int leftMergeRightY = Mathf.RoundToInt(((leftTrackPoint.z + trackOffset.z) / trackBounds.size.z * trackScale.z) * terrainData.heightmapHeight);
                //                  
                int leftMergeDiffX = leftMergeLeftX - leftMergeRightX;
                int leftMergeDiffY = leftMergeLeftY - leftMergeRightY;
                int leftMergeFillAmount = Mathf.Max(Mathf.Abs(leftMergeDiffX), 1) * Mathf.Max(Mathf.Abs(leftMergeDiffY), 1);// Mathf.Max(Mathf.Abs(leftMergeDiffX), Mathf.Abs(leftMergeDiffY));
                for(int f = 0; f < leftMergeFillAmount; f++)
                {
                    float move = f / (float)leftMergeFillAmount;
                    int fillX = Mathf.RoundToInt(Mathf.Lerp(leftMergeLeftX, leftMergeRightX, move));
                    int fillY = Mathf.RoundToInt(Mathf.Lerp(leftMergeLeftY, leftMergeRightY, move));
                    if(fillX < 0 || fillY < 0 || fillX > terrainWidth || fillY > terrainHeight)
                        continue;
                    float curveStrength = mergeCurve.Evaluate(move);
                    float fillTrackHeight = fillTrackHeightL;
                    float mergeHeight = Mathf.Lerp(originalData[fillY, fillX], fillTrackHeight, curveStrength);
                    int pointLock = modifiedPointLock[fillY, fillX];
                    float currentMergeHeight = mergeData[fillY, fillX];
                    float currentDifference = Mathf.Abs(currentMergeHeight - fillTrackHeight);
                    float newDifference = Mathf.Abs(mergeHeight - fillTrackHeight);
                    if(newDifference < currentDifference && pointLock == 0)
                    {
                        mergeData[fillY, fillX] = mergeHeight;
                    }
                }

                Vector3 rightMergePoint = rightTrackPoint + fillCross * mergeWidth;
                int rightMergeLeftX = Mathf.RoundToInt(((rightMergePoint.x + trackOffset.x) / trackBounds.size.x * trackScale.x) * terrainData.heightmapWidth);
                int rightMergeLeftY = Mathf.RoundToInt(((rightMergePoint.z + trackOffset.z) / trackBounds.size.z * trackScale.z) * terrainData.heightmapHeight);
                int rightMergeRightX = Mathf.RoundToInt(((rightTrackPoint.x + trackOffset.x) / trackBounds.size.x * trackScale.x) * terrainData.heightmapWidth);
                int rightMergeRightY = Mathf.RoundToInt(((rightTrackPoint.z + trackOffset.z) / trackBounds.size.z * trackScale.z) * terrainData.heightmapHeight);

                //                  
                int rightMergeDiffX = rightMergeLeftX - rightMergeRightX;
                int rightMergeDiffY = rightMergeLeftY - rightMergeRightY;
                int rightMergeFillAmount = Mathf.Max(Mathf.Abs(rightMergeDiffX), 1) * Mathf.Max(Mathf.Abs(rightMergeDiffY), 1);// Mathf.Max(Mathf.Abs(leftMergeDiffX), Mathf.Abs(leftMergeDiffY));
                for(int f = 0; f < rightMergeFillAmount; f++)
                {
                    float move = f / (float)rightMergeFillAmount;
                    int fillX = Mathf.RoundToInt(Mathf.Lerp(rightMergeLeftX, rightMergeRightX, move));
                    int fillY = Mathf.RoundToInt(Mathf.Lerp(rightMergeLeftY, rightMergeRightY, move));
                    if(fillX < 0 || fillY < 0 || fillX > terrainWidth || fillY > terrainHeight)
                        continue;
                    float curveStrength = mergeCurve.Evaluate(move);
                    float fillTrackHeight = fillTrackHeightR;
                    float mergeHeight = Mathf.Lerp(originalData[fillY, fillX], fillTrackHeight, curveStrength);
                    int pointLock = modifiedPointLock[fillY, fillX];
                    float currentMergeHeight = mergeData[fillY, fillX];
                    float currentDifference = Mathf.Abs(currentMergeHeight - fillTrackHeight);
                    float newDifference = Mathf.Abs(mergeHeight - fillTrackHeight);
                    if(newDifference < currentDifference && pointLock == 0)
                    {
                        mergeData[fillY, fillX] = mergeHeight;
                    }
                }
            }
        }

        for(int x = 0; x < terrainWidth; x++)
        {
            for(int y = 0; y < terrainHeight; y++)
            {
                bool isNotEdge = x > 0 && x < terrainWidth - 1 && y > 0 && y < terrainHeight - 1;
                if(mergeData[x, y] == 0)
                {
                    int mergeNeighbours = 0;
                    if(isNotEdge)
                    {
                        mergeNeighbours += (mergeData[x + 1, y] != 0) ? 1 : 0;
                        mergeNeighbours += (mergeData[x - 1, y] != 0) ? 1 : 0;
                        mergeNeighbours += (mergeData[x, y + 1] != 0) ? 1 : 0;
                        mergeNeighbours += (mergeData[x, y - 1] != 0) ? 1 : 0;
                    }
                    if(mergeNeighbours > 1)//if a hole is surounded by rasied terrain in two or more neighbouring places
                    {
                        float mergeHeight = 0;
                        mergeHeight += (mergeData[x + 1, y] != 0) ? mergeData[x + 1, y] : 0;
                        mergeHeight += (mergeData[x - 1, y] != 0) ? mergeData[x - 1, y] : 0;
                        mergeHeight += (mergeData[x, y + 1] != 0) ? mergeData[x, y + 1] : 0;
                        mergeHeight += (mergeData[x, y - 1] != 0) ? mergeData[x, y - 1] : 0;
                        modifiedData[x, y] = mergeHeight / mergeNeighbours;//clean up holes
                    }
                    else
                    {
                        modifiedData[x, y] = originalData[x, y];//use original
                    }
                }
                else
                {
                    modifiedData[x, y] = mergeData[x, y];
                }

                terrainData.SetHeights(0, 0, modifiedData);
                terrain.terrainData = terrainData;
            }
        }
    }

    public static void ConformTrack(TrackBuildRTrack track, Terrain terrain)
    {
        TerrainData terrainData = terrain.terrainData;
        int terrainWidth = terrainData.heightmapWidth;
        int terrainHeight = terrainData.heightmapHeight;
        float terrainHeightmapY = terrain.terrainData.heightmapScale.y;
        float terrainY = terrain.transform.position.y / terrainHeightmapY;
        float conformAccuracy = track.conformAccuracy;

        float[,] originalData = terrainData.GetHeights(0, 0, terrainWidth, terrainHeight);

        Bounds trackBounds = new Bounds();
        int numberOfCurves = track.numberOfCurves;
        for (int i = 0; i < numberOfCurves; i++)
        {
            TrackBuildRPoint curve = track[i];
            if (curve.holder == null)
                continue;
            Renderer[] rends = curve.holder.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in rends)
            {
                trackBounds.Encapsulate(rend.bounds);
            }
        }

        Vector3 trackOffset = track.transform.position - terrain.transform.position;
        Vector3 trackScale = new Vector3(trackBounds.size.x / terrainData.size.x, 1.0f / terrain.terrainData.size.y, trackBounds.size.z / terrainData.size.z);

        int realNumberOfPoints = track.realNumberOfPoints;
        for(int i = 0; i < realNumberOfPoints; i++)
        {
            TrackBuildRPoint point = track[i];
            Vector3 trackPointPosition = point.position;
            int pointX = Mathf.RoundToInt(((trackPointPosition.x + trackOffset.x) / trackBounds.size.x * trackScale.x) * terrainData.heightmapWidth);
            int pointY = Mathf.RoundToInt(((trackPointPosition.z + trackOffset.z) / trackBounds.size.z * trackScale.z) * terrainData.heightmapHeight);

            pointX = Mathf.Clamp(pointX, 0, terrainWidth-1);
            pointY = Mathf.Clamp(pointY, 0, terrainHeight-1);

            trackPointPosition.y = originalData[pointY, pointX] * terrain.terrainData.size.y - terrainY + conformAccuracy;
            point.position = trackPointPosition;

            Vector3 controlPoint = point.forwardControlPoint;
            pointX = Mathf.RoundToInt(((controlPoint.x + trackOffset.x) / trackBounds.size.x * trackScale.x) * terrainData.heightmapWidth);
            pointY = Mathf.RoundToInt(((controlPoint.z + trackOffset.z) / trackBounds.size.z * trackScale.z) * terrainData.heightmapHeight);

            pointX = Mathf.Clamp(pointX, 0, terrainWidth - 1);
            pointY = Mathf.Clamp(pointY, 0, terrainHeight - 1);

            controlPoint.y = originalData[pointY, pointX] * terrain.terrainData.size.y - terrainY + conformAccuracy;
            point.forwardControlPoint = controlPoint;

            point.isDirty = true;
        }
        track.RecalculateCurves();
    }

    public static void ResetTerrain(TrackBuildRTrack track, Terrain terrain)
    {
        TerrainData terrainData = terrain.terrainData;
        int terrainWidth = terrainData.heightmapWidth;
        int terrainHeight = terrainData.heightmapHeight;

        float[,] tData = terrainData.GetHeights(0, 0, terrainWidth, terrainHeight);
        for(int x = 0; x < terrainWidth; x++)
            for(int y = 0; y< terrainHeight; y++)
                tData[x, y] = 1;

        terrainData.SetHeights(0, 0, tData);
        terrain.terrainData = terrainData;
    }
}
