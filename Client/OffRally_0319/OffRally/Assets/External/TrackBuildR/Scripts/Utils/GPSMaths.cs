using UnityEngine;
using System.Collections;

public class GPSMaths
{

    private static float EARTH_RADIUS = 6378160.0f; // meters
    private static float DEGREE_LONG = 111319.9f; // meters

    public static float GPSAngleToMeters(float input)
    {
        return (EARTH_RADIUS * (input * Mathf.Deg2Rad));// / (Mathf.PI * 0.5f);
    }

    public static Vector3 Haversine(Vector3 point1, Vector3 point2)
    {
        float lat1 = Mathf.Deg2Rad * point1.x;
        float lat2 = Mathf.Deg2Rad * point2.x;
        float lon1 = Mathf.Deg2Rad * point1.z;
        float lon2 = Mathf.Deg2Rad * point2.z;

        float deltaLong = lon2 - lon1;

        float y = Mathf.Sin(deltaLong) * Mathf.Cos(lat2);
        float x = Mathf.Cos(lat1) * Mathf.Sin(lat2) - Mathf.Sin(lat1) * Mathf.Cos(lat2) * Mathf.Cos(deltaLong);
        return new Vector3(x, 0, y);
    }

    public static Vector3 Simple(Vector3 point1, Vector3 point2)
    {
        float lon1 = Mathf.Deg2Rad * point1.x;
        float lon2 = Mathf.Deg2Rad * point2.x;
        float lat1 = Mathf.Deg2Rad * point1.z;
        float lat2 = Mathf.Deg2Rad * point2.z;

        float deltaLong = lon2 - lon1;
        float deltaLat = lat2 - lat1;

        float x = (deltaLong * EARTH_RADIUS) * Mathf.Cos(lat2);
        float z = (deltaLat * EARTH_RADIUS);

        return new Vector3(x, 0, z);
    }


    public static Vector3 Pythagoras(Vector3 point1, Vector3 point2)
    {

//        float lon1 = Mathf.Deg2Rad * point1.x;
//        float lon2 = Mathf.Deg2Rad * point2.x;
//        float lat1 = Mathf.Deg2Rad * point1.z;
//        float lat2 = Mathf.Deg2Rad * point2.z;
        float lon1 = point1.z;
        float lon2 = point2.z;
        float lat1 = point1.x;
        float lat2 = point2.x;
        
        float deltaLong = lon2 - lon1;
        float deltaLat = lat2 - lat1;
        float x = deltaLat * DEGREE_LONG;
        float y = DEGREE_LONG  * (deltaLong * Mathf.Cos(point2.z));
            //        float x = EARTH_RADIUS * deltaLong * Mathf.Cos(lat1) * Mathf.Rad2Deg;
//        float y = EARTH_RADIUS * deltaLat * Mathf.Rad2Deg;
//        return new Vector3(x, 0, y);

        return new Vector3(x, 0, y);

//        float x = (lon2 - lon1) * Mathf.Cos((lat1 + lat2) / 2);
//        float y = (lat2 - lat1);
////        float d = Mathf.Sqrt(x * x + y * y) * EARTH_RADIUS;
////        float ratio = (lat2 - lat1) / (lon2 - lon1);
//        float r = EARTH_RADIUS;
//        return new Vector3(x * r, 0, y * r);
    }



    public static Vector3 Eleven(Vector3 point1, Vector3 point2)
    {
//        Debug.Log(point1.ToString("R")+" "+point2.ToString("R")+" "+Vector3.Distance(point1,point2));
        float lat1 = Mathf.Deg2Rad * point1.x;
        float lat2 = Mathf.Deg2Rad * point2.x;
        float lon1 = Mathf.Deg2Rad * point1.z;
        float lon2 = Mathf.Deg2Rad * point2.z;

        float x = (point2.x - point1.x) * 111000;
        float y = (point2.z - point1.z) * 111000;
        //        var d = Mathf.Sqrt(x * x + y * y) * EARTH_RADIUS;
        return new Vector3(x, 0, y);
    }
}
