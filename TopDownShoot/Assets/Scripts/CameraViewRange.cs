using UnityEngine;

namespace Tools
{
    public class CameraViewRange
    {
        class Line
        {
            public Vector3 point1;
            public Vector3 point2;
        }

        private Line[] boundLines;
        
        public CameraViewRange()
        {
            var _cameraMain = Camera.main;
            //var center = _cameraMain.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            var bottomLeft = _cameraMain.ViewportPointToRay(new Vector3(0, 0, 0));
            var topLeft = _cameraMain.ViewportPointToRay(new Vector3(0, 1, 0));
            var topRight = _cameraMain.ViewportPointToRay(new Vector3(1, 1, 0));
            var bottomRight = _cameraMain.ViewportPointToRay(new Vector3(1, 0, 0));

            var p1 = bottomLeft.GetPlaneCastPoint(); //左下
            var p2 = topLeft.GetPlaneCastPoint(); //左上
            var p3 = topRight.GetPlaneCastPoint(); //右上
            var p4 = bottomRight.GetPlaneCastPoint(); //右下

            boundLines = new Line[4];
            boundLines[0] = new Line { point1 = p1, point2 = p2 };
            boundLines[1] = new Line { point1 = p2, point2 = p3 };
            boundLines[2] = new Line { point1 = p3, point2 = p4 };
            boundLines[3] = new Line { point1 = p4, point2 = p1 };
        }
        
        public Vector3 GetRandomSpawnPoint()
        {
            int lineId = Random.Range(0, boundLines.Length);
            var line = boundLines[lineId];

            var rate = Random.Range(0, 1f);
            var point = Vector3.Lerp(line.point1, line.point2, rate);
            return point;
        }
    }
    
    public static class Camera_Ex
    {
        public static Vector3 GetPlaneCastPoint(this Ray ray, float planeHeight = 0) =>
            ray.origin + (ray.origin.y - planeHeight) / (-ray.direction.y) * ray.direction;
    }
}