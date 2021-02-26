using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PZ3
{
    public class Position
    {
        private double scaleX;
        private double scaleY;
        private double xMin;
        private double xMax;
        private double yMin;
        private double yMax;
        private double scaleBasedOffSet; 

        public Position(double mapScale, double mapCenter, double xMin, double yMin, double xMax, double yMax)
        {
            this.xMin = xMin;
            this.xMax = xMax;
            this.yMin = yMin;
            this.yMax = yMax;
            this.scaleBasedOffSet = mapCenter - mapScale / 2;
            scaleX = (xMax - xMin) / mapScale * -1; //??
            scaleY = (yMax - yMin) / mapScale;

        }

        public Point Convert(double x, double y)
        {
            double xScaled = (x - xMin) / scaleX; //???
            double yScaled = (y - yMin) / scaleY;
            return new Point(xScaled + scaleBasedOffSet, yScaled + scaleBasedOffSet);
        }
    }
}
