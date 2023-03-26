using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using SimplePaint.Models.Shapes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using SimplePaint.Models.Safes;
using static SimplePaint.Models.Shapes.PropsN;

namespace SimplePaint.Models
{
    public class Mapper
    {
        private bool _updateNameLock;
        public string shapeName = "Линия 1";
        public string shapeColor = "Blue";
        public string shapeFillColor = "Yellow";
        public string? shapeNewName;
        public short shapeSelectShaper = -1;
        public int shapeThickness = 2;
        public SafeNum shapeWidth;
        public SafeNum shapeHeight;
        public SafeNum shapeHorizDiagonal;
        public SafeNum shapeVertDiagonal;
        public SafePoint shapeStartDot;
        public SafePoint shapeEndDot;
        public SafePoint shapeCenterDot;
        public SafePoints shapeDots;
        public SafeGeometry shapeCommands;
        private IShape _curShaper = Shapers[0];
        public readonly Transformator shapeTformer;
        public readonly ObservableCollection<ShapeListBoxItem> shapeS = new();
        private readonly Action<object?>? _upd;
        private readonly object? _inst;
        private readonly Dictionary<string, ShapeListBoxItem> _name2Shape = new();
        private readonly Dictionary<string, Shape> _dict = new();
        private static Dictionary<string, IShape> TShapers => new(Shapers.Select(shaper => new KeyValuePair<string, IShape>(shaper.Name, shaper)));
        public Mapper(Action<object?>? upd, object? inst)
        {
            shapeWidth = new(200, Update, this);
            shapeHeight = new(100, Update, this);
            shapeHorizDiagonal = new(100, Update, this);
            shapeVertDiagonal = new(200, Update, this);
            shapeStartDot = new(50, 50, Update, this);
            shapeEndDot = new(100, 100, Update, this);
            shapeCenterDot = new(150, 150, Update, this);
            shapeDots = new("50,50 100,100 50,100 100,50", Update, this);
            shapeCommands = new("M 10 70 l 30,30 10,10 35,0 0,-35 m 50 0 l 0,-50 10,0 35,35 m 50 0 l 0,-50 10,0 35,35z m 70 0 l 0,30 30,0 5,-35z", Update, this);
            shapeTformer = new(upd, inst);
            _upd = upd;
            _inst = inst;
        }
        private void AddShape(Shape newy, string? name = null) {
            name ??= shapeName; // Согл... XD    if (name == null) name = shapeName;    было изначально
            _dict[name] = newy;
            var item = new ShapeListBoxItem(name, this);
            shapeS.Add(item);
            _name2Shape[name] = item;
        }
        private static IShape[] Shapers => new IShape[] {
            new Shape1_Line(),
            new Shape2_BreakedLine(),
            new Shape3_Polygonal(),
            new Shape4_Rectangle(),
            new Shape5_Ellipse(),
            new Shape6_CompositeFigure(),
        };
        public void ChangeFigure(int n)
        {
            _curShaper = Shapers[n];
            if (!_updateNameLock) shapeNewName = GenName(_curShaper.Name);
            Update();
        }
        internal object GetProp(PropsN num)
        {
            return num switch
            {
                PName => shapeName,
                PColor => shapeColor,
                PFillColor => shapeFillColor,
                PThickness => shapeThickness,
                PWidth => shapeWidth,
                PHeight => shapeHeight,
                PHorizDiagonal => shapeHorizDiagonal,
                PVertDiagonal => shapeVertDiagonal,
                PStartDot => shapeStartDot,
                PEndDot => shapeEndDot,
                PCenterDot => shapeCenterDot,
                PDots => shapeDots,
                PCommands => shapeCommands,
                _ => 0
            };
        }
        internal void SetProp(PropsN num, object obj)
        {
            switch (num)
            {
                case PName: shapeName = (string)obj; break;
                case PColor: shapeColor = (string)obj; break;
                case PFillColor: shapeFillColor = (string)obj; break;
                case PThickness: shapeThickness = (int)obj; break;
            }
        }
        public bool ValidInput()
        {
            foreach (PropsN num in _curShaper.Props)
                if (GetProp(num) is ISafe @prop && !@prop.Valid) return false;
            return true;
        }
        public bool ValidName() => !_dict.ContainsKey(shapeName);
        private string GenName(string prefix)
        {
            prefix += " ";
            int n = 1;
            while (true)
            {
                string res = prefix + n;
                if (!_dict.ContainsKey(res)) return res;
                n += 1;
            }
        }
        public Shape? Create(bool preview)
        {
            Shape? newy = _curShaper.Build(this);
            if (newy == null) return null;
            if (preview)
            {
                newy.Name = "marker";
                return newy;
            }
            if (_name2Shape.TryGetValue(shapeName, out var value)) Remove(value);
            shapeTformer.Transform(newy, preview);
            AddShape(newy);
            shapeNewName = GenName(_curShaper.Name);
            return newy;
        }
        internal void Remove(ShapeListBoxItem item)
        {
            var name = item.Name;
            if (!_dict.ContainsKey(name)) return;
            var shape = _dict[name];
            if (shape.Parent is not Canvas @c) return;
            @c.Children.Remove(shape);
            shapeS.Remove(item);
            _name2Shape.Remove(name);
            _dict.Remove(name);
            shapeNewName = GenName(_curShaper.Name);
            Update();
        }
        public void Clear()
        {
            foreach (var item in _dict)
            {
                var shape = item.Value;
                if (shape.Parent is not Canvas @c) continue;
                @c.Children.Clear();
            }
            shapeS.Clear();
            _name2Shape.Clear();
            _dict.Clear();
            shapeNewName = GenName(_curShaper.Name);
            Update();
        }
        public void Export(bool isXml)
        {
            List<object> data = new();
            foreach (var item in _dict)
            {
                var shape = item.Value;
                foreach (var shaper in Shapers)
                {
                    var res = shaper.Export(shape);
                    if (res != null)
                    {
                        res["type"] = shaper.Name;
                        var tform = Transformator.Export(shape);
                        if (tform.Count > 0) res["transform"] = tform;
                        data.Add(res);
                        break;
                    }
                }
            }
            if (isXml)
            {
                var xml = Serializer.Obj2xml(data);
                if (xml == null) { return; }
                File.WriteAllText("../../../Export.xml", xml);
            }
            else
            {
                var json = Serializer.Obj2json(data);
                File.WriteAllText("../../../Export.json", json);
            }
        }
        public Shape[]? Import(bool isXml)
        {
            string name = isXml ? "Export.xml" : "Export.json";
            if (!File.Exists("../../../" + name)) { return null; }
            var data = File.ReadAllText("../../../" + name);
            var json = isXml ? Serializer.Xml2obj(data) : Serializer.Json2obj(data);
            if (json is not List<object?> @list) { return null; }
            List<Shape> res = new();
            Clear();
            foreach (object? item in @list)
            {
                if (item is not Dictionary<string, object?> @dict) { continue; }
                if (!@dict.ContainsKey("type") || @dict["type"] is not string @type) { continue; }
                if (!@dict.ContainsKey("name") || @dict["name"] is not string @shapeName) { continue; }
                if (!TShapers.ContainsKey(@type)) { continue; }
                var shaper = TShapers[@type];
                var newy = shaper.Import(@dict);
                if (newy == null) { continue; }
                if (@dict.TryGetValue("transform", out object? tform))
                    if (tform is not Dictionary<string, object?> @dict2) {}
                    else Transformator.Import(newy, @dict2);
                AddShape(newy, @shapeName);
                res.Add(newy);
            }
            shapeNewName = GenName(_curShaper.Name);
            return res.ToArray();
        }
        public void Select(ShapeListBoxItem? item)
        {
            if (item == null) return;
            var shape = _dict[item.Name];
            bool yeah;
            short n = 0;
            foreach (var shaper in Shapers)
            {
                yeah = shaper.Load(this, shape);
                if (yeah)
                {
                    shapeTformer.Disassemble(shape);
                    _updateNameLock = true;
                    shapeSelectShaper = n;
                    Update();
                    _updateNameLock = false;
                    break;
                }
                n++;
            }
        }
        public ShapeListBoxItem? ShapeTap(string name)
        {
            if (name.StartsWith("sn_")) name = name[3..];
            else if (name.StartsWith("sn|")) name = Serializer.Base64Decode(name.Split('|')[1]);
            else return null;
            if (_name2Shape.TryGetValue(name, out var item))
            {
                Select(item);
                return item;
            }
            return null;
        }
        private void Update()
        {
            _upd?.Invoke(_inst);
        }
        private static void Update(object? me)
        {
            if (me != null && me is Mapper @map) @map.Update();
        }
    }
}
