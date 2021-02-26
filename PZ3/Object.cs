using PZ3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace PZ3
{
    public class Object
    {
        public GeometryModel3D model { get; }
        public double X, Y, Z;
        public PowerEntity entity;
        public List<GeometryModel3D> models { get; }
        public bool IsSelected;

        public Object()
        {
        }

        public Object(GeometryModel3D model, PowerEntity entity)
        {
            this.model = model;

            this.X = model.Bounds.Location.X;
            this.Y = model.Bounds.Location.Y;
            this.Z = model.Bounds.Location.Z;
            this.entity = entity;
        }

        public Object(List<GeometryModel3D> models)
        {
            this.models = models;
        }
    }   
}

