using HelixToolkit.Wpf;
using PZ3.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using Point = PZ3.Models.Point;

namespace PZ3
{
    //PR145-2016_David_Ceranic
    public partial class MainWindow : Window
    {
        private List<SubstationEntity> substationList = new List<SubstationEntity>();
        private List<NodeEntity> nodeList = new List<NodeEntity>();
        private List<SwitchEntity> switchList = new List<SwitchEntity>();
        private Dictionary<long ,MyLine> lineList = new Dictionary<long, MyLine>();


        private const double mapScale = 10;
        private Position mapper;

        private const string Map = @"..\..\Imgaes\map.jpg";
        private readonly Model3DGroup mainModel3DGroup = new Model3DGroup();

        private List<Object> instationedObject = new List<Object>();

        private GeometryModel3D hitgeo;
        public static ToolTip toolTip;

        public MainWindow()
        {
            InitializeComponent();
            toolTip = new ToolTip();
            viewPort.ToolTip = toolTip;
            AddModelToViewPort();
            viewPort.PanGesture = new MouseGesture(MouseAction.LeftClick);
            viewPort.PanGesture2 = new MouseGesture(MouseAction.LeftClick);
            viewPort.RotateGesture = new MouseGesture(MouseAction.MiddleClick);
            viewPort.RotateGesture2 = new MouseGesture(MouseAction.MiddleClick);
            InitializeMapper();
            DrawMap();

            Parser();
        }

        private void InitializeMapper()
        {
            mapper = new Position(mapScale, 0, 45.2325, 19.793909, 45.277031, 19.894459);
        }

        private void AddModelToViewPort()
        {
            var modelVisual = new ModelVisual3D { Content = mainModel3DGroup };
            viewPort.Children.Add(modelVisual);
        }

        private void DrawMap()
        {
            var builder = new MeshBuilder();
            builder.AddCube(BoxFaces.Bottom);
            var cube = builder.ToMesh();

            var mat = new DiffuseMaterial { Brush = new ImageBrush(new BitmapImage(new Uri(Map, UriKind.Relative))) };

            var surfaceModel = new GeometryModel3D(cube, mat)
            {
                BackMaterial = mat,
                Transform = new Transform3DGroup()
                {
                    Children = new Transform3DCollection()
                    {
                        new ScaleTransform3D(mapScale, mapScale, 0),
                        new TranslateTransform3D(mapScale/2, -mapScale/2, 0),
                        new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0,0,1),-180))
                    }

                }
            };

            mainModel3DGroup.Children.Add(surfaceModel);
        }



        Object CreateObject(PowerEntity entity, System.Windows.Media.Color color, double scale)
        {
            var builder = new MeshBuilder();
            builder.AddCube();
            var cube = builder.ToMesh();

            var mat = new DiffuseMaterial { Brush = new SolidColorBrush(color) };

            var position = mapper.Convert(entity.X, entity.Y);
            var surfaceModel = new GeometryModel3D(cube, mat)
            {
                BackMaterial = mat,
                Transform = new Transform3DGroup()
                {
                    Children = new Transform3DCollection()
                    {
                        new ScaleTransform3D(scale, scale, scale),
                        new TranslateTransform3D(position.X, position.Y, GetZ(position.X, position.Y, scale))
                    }
                    
                }
                
            };

            var bounds = surfaceModel.Bounds;

            mainModel3DGroup.Children.Add(surfaceModel);

            var Object = new Object(surfaceModel, entity);
            instationedObject.Add(Object);
            return Object;
        }

        public double GetZ(double x, double y, double scale)
        {
            var minX = x - scale / 2;
            var minY = y - scale / 2;
            var maxX = x + scale / 2;
            var maxY = y + scale / 2;
            double z = 0;

            foreach (var obj in instationedObject)
            {
                if((obj.X <maxX) & (obj.X > minX) & (obj.Y< maxY) & (obj.Y> minY))
                {
                    z += scale;
                }
            }

            return z;
        }

        private void Parser()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Geographic.xml");

            XmlNodeList nodeListSubstation = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Substations/SubstationEntity");
            XmlNodeList nodeListSwitch = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity");
            XmlNodeList nodeListNode = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity");
            XmlNodeList nodeListLine = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Lines/LineEntity");


            XmlNodeList tempList;
            double noviX, noviY;

            tempList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Substations/SubstationEntity");

            foreach (XmlNode node in tempList)
            {
                SubstationEntity substationEntity = new SubstationEntity();
                substationEntity.Id = long.Parse(node.SelectSingleNode("Id").InnerText, CultureInfo.InvariantCulture);
                substationEntity.Name = node.SelectSingleNode("Name").InnerText;

                LatLon.ToLatLon(double.Parse(node.SelectSingleNode("Y").InnerText, CultureInfo.InvariantCulture), double.Parse(node.SelectSingleNode("X").InnerText, CultureInfo.InvariantCulture), 34, out noviX, out noviY);
                substationEntity.X = noviX;
                substationEntity.Y = noviY;

                if(substationEntity.X> 45.277031 || substationEntity.Y> 19.894459 || substationEntity.X < 45.2325 || substationEntity.Y< 19.793909)
                {

                }
                else
                {
                    substationList.Add(substationEntity);
                    var dot = CreateObject(substationEntity, System.Windows.Media.Color.FromRgb(56, 86, 236), 0.07);
                }
            }


            tempList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity");

            foreach (XmlNode node in tempList)
            {
                NodeEntity nodeEntity = new NodeEntity();
                nodeEntity.Id = long.Parse(node.SelectSingleNode("Id").InnerText, CultureInfo.InvariantCulture);
                nodeEntity.Name = node.SelectSingleNode("Name").InnerText;

                LatLon.ToLatLon(double.Parse(node.SelectSingleNode("Y").InnerText, CultureInfo.InvariantCulture), double.Parse(node.SelectSingleNode("X").InnerText, CultureInfo.InvariantCulture), 34, out noviX, out noviY);
                nodeEntity.X = noviX;
                nodeEntity.Y = noviY;

                if (nodeEntity.X > 45.277031 || nodeEntity.Y > 19.894459 || nodeEntity.X < 45.2325 || nodeEntity.Y < 19.793909)
                {

                }
                else
                {
                    nodeList.Add(nodeEntity);
                    var dot = CreateObject(nodeEntity, System.Windows.Media.Color.FromRgb(0, 120, 120), 0.07);
                }
            }


            tempList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity");
            foreach (XmlNode node in tempList)
            {
                SwitchEntity switchEntity = new SwitchEntity();

                switchEntity.Id = long.Parse(node.SelectSingleNode("Id").InnerText, CultureInfo.InvariantCulture);
                switchEntity.Name = node.SelectSingleNode("Name").InnerText;
                switchEntity.Status = node.SelectSingleNode("Status").InnerText;

                LatLon.ToLatLon(double.Parse(node.SelectSingleNode("Y").InnerText, CultureInfo.InvariantCulture), double.Parse(node.SelectSingleNode("X").InnerText, CultureInfo.InvariantCulture), 34, out noviX, out noviY);
                switchEntity.X = noviX;
                switchEntity.Y = noviY;

                if (switchEntity.X > 45.277031 || switchEntity.Y > 19.894459 || switchEntity.X < 45.2325 || switchEntity.Y < 19.793909)
                {

                }
                else
                {
                    switchList.Add(switchEntity);

                    if (switchEntity.Status == "Closed")
                        CreateObject(switchEntity, System.Windows.Media.Color.FromRgb(230, 41, 41), 0.07);
                    if (switchEntity.Status == "Open")
                        CreateObject(switchEntity, System.Windows.Media.Color.FromRgb(58, 230, 64), 0.07);
                }
            }


            tempList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Lines/LineEntity");

            foreach (XmlNode node in tempList)
            {
                LineEntity lineEntity = new LineEntity();

                lineEntity.Id = long.Parse(node.SelectSingleNode("Id").InnerText, CultureInfo.InvariantCulture);
                lineEntity.Name = node.SelectSingleNode("Name").InnerText;
                if (node.SelectSingleNode("IsUnderground").InnerText.Equals("true"))
                    lineEntity.IsUnderground = true;
                else
                    lineEntity.IsUnderground = false;
                lineEntity.R = float.Parse(node.SelectSingleNode("R").InnerText, CultureInfo.InvariantCulture);
                lineEntity.ConductorMaterial = node.SelectSingleNode("ConductorMaterial").InnerText;
                lineEntity.LineType = node.SelectSingleNode("LineType").InnerText;
                lineEntity.ThermalConstantHeat = long.Parse(node.SelectSingleNode("ThermalConstantHeat").InnerText, CultureInfo.InvariantCulture);
                lineEntity.FirstEnd = long.Parse(node.SelectSingleNode("FirstEnd").InnerText, CultureInfo.InvariantCulture);
                lineEntity.SecondEnd = long.Parse(node.SelectSingleNode("SecondEnd").InnerText, CultureInfo.InvariantCulture);
                lineEntity.Vertices = new List<Point>();
                foreach (XmlNode pointNode in node.ChildNodes[9].ChildNodes)
                {
                    LatLon.ToLatLon(double.Parse(pointNode.SelectSingleNode("Y")?.InnerText ?? string.Empty, CultureInfo.InvariantCulture), double.Parse(pointNode.SelectSingleNode("X")?.InnerText ?? string.Empty, CultureInfo.InvariantCulture), 34, out noviX, out noviY);
                    lineEntity.Vertices.Add(new Point() { X = noviX, Y = noviY });
                }

                
                var first = instationedObject.FirstOrDefault(x => x.entity.Id == lineEntity.FirstEnd);
                var second = instationedObject.FirstOrDefault(x => x.entity.Id == lineEntity.SecondEnd);
                if (first == null || second == null)
                    continue;

                var v1 = mapper.Convert(lineEntity.Vertices[0].X, lineEntity.Vertices[0].Y);
                var vn = mapper.Convert(lineEntity.Vertices[lineEntity.Vertices.Count - 1].X, lineEntity.Vertices[lineEntity.Vertices.Count - 1].Y);

                position = 0;
                var mesh = new MeshGeometry3D();


                CreateLine(first.X,
                    first.Y,
                    v1.X,
                    v1.Y, mesh);

                for (int i = 0; i < lineEntity.Vertices.Count - 1; i++)
                {
                    var p1 = mapper.Convert(lineEntity.Vertices[i].X, lineEntity.Vertices[i].Y);
                    var p2 = mapper.Convert(lineEntity.Vertices[i + 1].X, lineEntity.Vertices[i + 1].Y);

                    CreateLine(p1.X,
                                  p1.Y,
                                  p2.X,
                                  p2.Y,mesh);
                }

                CreateLine(vn.X,
                    vn.Y,
                    second.X,
                    second.Y, mesh);

                var mat = new DiffuseMaterial { Brush = new SolidColorBrush(Colors.Black) };
                var surfaceModel = new GeometryModel3D(mesh, mat)
                {
                    BackMaterial = mat,
                    Transform = new Transform3DGroup()
                    {
                        Children = new Transform3DCollection()
                    {
                        new TranslateTransform3D(0,0,0.01)
                    }

                    }
                };
                mainModel3DGroup.Children.Add(surfaceModel);

                var lineComponent = new MyLine(first, second, lineEntity, surfaceModel);
                lineList.Add(lineEntity.Id, lineComponent);
            }
        }


        int position;
        void CreateLine(double x1, double y1, double x2, double y2, MeshGeometry3D mesh)
        {
            var dx = x2 - x1;
            var dy = y2 - y1;

            Vector3D normal = new Vector3D(-dy,dx, 0);
            normal.Normalize();

            Vector3D offset = normal * 0.005;

            var leftSide1 = new Vector3D(x1, y1, 0) - offset;
            var rightSide1 = new Vector3D(x1, y1, 0) + offset;

            var leftSide2 = new Vector3D(x2, y2, 0) - offset;
            var rightSide2 = new Vector3D(x2, y2, 0) + offset;

            mesh.Positions.Add((Point3D)leftSide1);
            mesh.Positions.Add((Point3D)leftSide2);
            mesh.Positions.Add((Point3D)rightSide1);
            mesh.Positions.Add((Point3D)rightSide2);
            
            mesh.TriangleIndices.Add(position + 0);
            mesh.TriangleIndices.Add(position + 1);
            mesh.TriangleIndices.Add(position + 3);

            mesh.TriangleIndices.Add(position + 0);
            mesh.TriangleIndices.Add(position + 3);
            mesh.TriangleIndices.Add(position + 2);
            position += 4;
        }


        void MouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point mouseposition = e.GetPosition(viewPort);
            Point3D testpoint3D = new Point3D(mouseposition.X, mouseposition.Y, 0);
            Vector3D testdirection = new Vector3D(mouseposition.X, mouseposition.Y, 10);

            PointHitTestParameters pointparams =
                     new PointHitTestParameters(mouseposition);
            RayHitTestParameters rayparams =
                     new RayHitTestParameters(testpoint3D, testdirection);
  
            hitgeo = null;
            VisualTreeHelper.HitTest(viewPort, null, HTResult, pointparams);
        }


        private HitTestResultBehavior HTResult(System.Windows.Media.HitTestResult rawresult)
        {

            RayHitTestResult rayResult = rawresult as RayHitTestResult;

            if (rayResult != null)
            {

                DiffuseMaterial darkSide =
                     new DiffuseMaterial(new SolidColorBrush(
                     System.Windows.Media.Colors.Aqua));
                bool gasit = false;
                for (int i = 0; i < instationedObject.Count; i++)
                {
                    if (instationedObject[i].model == rayResult.ModelHit)
                    {
                        if (instationedObject[i].IsSelected == false)
                        {
                            //hitgeo = (GeometryModel3D)rayResult.ModelHit;
                            //gasit = true;
                            toolTip.Content = ToolTipHelper.ToolTipEntity(new List<PowerEntity>() { instationedObject[i].entity });
                            toolTip.IsOpen = true;
                            TurnOffToolTip();
                            //ResetColor(hitgeo, hitgeo.Material, new MyLine(), instationedObject[i]);
                            //hitgeo.Material = darkSide;
                        }
                    }
                }
                foreach (var item in lineList.Values)
                {
                    if (item.model == rayResult.ModelHit)
                    {
                        if (item.IsSelected == false && item.p1.IsSelected == false && item.p2.IsSelected == false)
                        {
                            hitgeo = (GeometryModel3D)rayResult.ModelHit;
                            gasit = true;
                            toolTip.Content = $"Type: Line Entity\nId:{item.line.Id}\nName: {item.line.Name}\nIsUnderground: {item.line.IsUnderground}";
                            toolTip.IsOpen = true;
                            ResetColor(item.p1.model, item.p1.model.Material, item, new Object());
                            ResetColor(item.p2.model, item.p2.model.Material, item, new Object());
                            ResetColor(hitgeo, hitgeo.Material, item, new Object());
                            hitgeo.Material = darkSide;
                            item.p1.model.Material = darkSide;
                            item.p2.model.Material = darkSide;
                        }
                    }
                }
                if (!gasit)
                {
                    hitgeo = null;
                }
            }

            return HitTestResultBehavior.Stop;
        }

        async void ResetColor(GeometryModel3D geometry, Material material, MyLine p, Object objectWrapper)
        {
            objectWrapper.IsSelected = true;
            p.IsSelected = true;
            await Delay();
            geometry.Material = material;
            p.IsSelected = false;
            objectWrapper.IsSelected = false;
            toolTip.IsOpen = false;
        }

        async void TurnOffToolTip()
        {
            await Delay();
            toolTip.IsOpen = false;
        }

        async Task Delay()
        {
            await Task.Delay(2000);
        }
    }
}
