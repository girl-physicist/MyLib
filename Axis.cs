using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyLib.Enums;
using System.Drawing.Imaging;
using System.Security.AccessControl;


namespace MyLib
{
    public partial class Axis : UserControl
    {
        private static float _kx, _ky, _xBase, _yBase, _pixSize = 1;
        private float _xMax, _yMax;
        private int _xe, _ye, _xShift, _yShift;
        private int _cPixSize = 1;
        private int _drawMode = 0;
        private Color _picBackColor = Color.White; 
        private string _xName = "x";
        private string _yName = "y";
        private Enum _cAxisType = EAxisType.Full;
        private Enum _cPixType = EPixType.Circle;
        private Rectangle _rect = new Rectangle(0, 0, 1, 1);
        private Boolean _firstPass = true;
        private Graphics _graph;
        private Image _picImage;
        //оформление координат
        private Font _axisFont = new Font("Arial", 10);
        private Pen _axisPen = new Pen(Color.Gray);
        private SolidBrush _axisBrush = new SolidBrush(Color.Black);
        //оформление маркеров
        private Pen _pixPen = new Pen(Color.Black);
        private SolidBrush _pixBrush = new SolidBrush(Color.Black);
        //буффер для сохранения текущего отображения системы координат
        private Bitmap _buffStat;
        private Bitmap _buffDin;
        //объект для рисования
        private Graphics _gStat;
        private Graphics _gDin;


        //оси координат
        public float XBase
        {
            get => _xBase;
            set => _xBase = value;
        }

        public float YBase
        {
            get => _yBase;
            set => _yBase = value;
        }

        public string XName
        {
            get => _xName;
            set => _xName = value;
        }

        public string YName
        {
            get => _yName;
            set => _yName = value;
        }

        public Color AxisColor
        {
            get => _axisPen.Color;
            set => _axisPen.Color = value;
        }

        public Color AxisBkColor
        {
            get => _picBackColor;
            set => _picBackColor=value;
        }

        public Enum AxisType
        {
            get => _cAxisType;

            set => _cAxisType = value;
        }

        // маркер
        public Enum PixType
        {
            get => _cPixType;

            set => _cPixType = value;
        }

        public Color PixColor
        {
            get => _pixPen.Color;

            set
            {
                _pixPen.Color = value;
                _pixBrush.Color = value;
            }
        }

        public float PixSize
        {
            get => _pixSize;

            set => _pixSize = value;
        }

        public Axis(float xBase, float yBase)
        {
            XBase = xBase;
            YBase = yBase;
            InitializeComponent();
        }

        public void AxisDraw()
        {
            _rect.Width = this.Width;
            _rect.Height = this.Height;
            if (_firstPass == false)
            {
                _firstPass = false;
                _gStat.Dispose();
                _gDin.Dispose();
                _buffStat.Dispose();
                _buffDin.Dispose();
            }
            _buffStat = new Bitmap(Width, Height);
            _buffDin = new Bitmap(Width, Height);
            _gStat = Graphics.FromImage(_buffStat);
            _gDin = Graphics.FromImage(_buffDin);

            var lxName = Math.Round(_xBase, 1) + ", " + XName;
            var lyName = Math.Round(_yBase, 1) + ", " + YName;
            var xNameSize = _gStat.MeasureString(lxName, _axisFont);
            switch (_cAxisType)
            {
                case EAxisType.Full:
                    _gStat.DrawLine(_axisPen, Convert.ToInt32(_xMax / 2), 0, Convert.ToInt32(_xMax / 2), Convert.ToInt32(_yMax));
                    _gStat.DrawLine(_axisPen, 0, Convert.ToInt32(_yMax / 2), Convert.ToInt32(_xMax), Convert.ToInt32(_yMax / 2));
                    _gStat.DrawString(lxName, _axisFont, _axisBrush, Convert.ToInt32(_xMax - xNameSize.Width), Convert.ToInt32(_yMax / 2 - xNameSize.Height));
                    _gStat.DrawString(lyName, _axisFont, _axisBrush, Convert.ToInt32(_xMax / 2), 1);
                    _xShift = (int)(_xMax / 2);
                    _yShift = (int)(_yMax / 2);
                    _gStat.TranslateTransform(_xShift, _yShift, MatrixOrder.Append);
                    _gDin.TranslateTransform(_xShift, _yShift, MatrixOrder.Append);
                    _kx = _xMax / 2 / _xBase;
                    _ky = _yMax / 2 / _yBase;
                    break;

                case EAxisType.HalfRight:
                    _gStat.DrawLine(_axisPen, 0, Convert.ToInt32(_yMax / 2), Convert.ToInt32(_xMax), Convert.ToInt32(_yMax / 2));
                    _gStat.DrawString(lxName, _axisFont, _axisBrush, Convert.ToInt32(_xMax - xNameSize.Width), Convert.ToInt32(_yMax / 2 - xNameSize.Height));
                    _gStat.DrawString(lyName, _axisFont, _axisBrush, 1, 1);
                    _xShift = 0;
                    _yShift = (int)(_yMax / 2);
                    _gStat.TranslateTransform(_xShift, _yShift, MatrixOrder.Append);
                    _gDin.TranslateTransform(_xShift, _yShift, MatrixOrder.Append);
                    _kx = _xMax / _xBase;
                    _ky = _yMax / 2 / _yBase;
                    break;

                case EAxisType.HalfTop:
                    _gStat.DrawLine(_axisPen, Convert.ToInt32(_xMax / 2), 0, Convert.ToInt32(_xMax / 2), Convert.ToInt32(_yMax));

                    _gStat.DrawString(lxName, _axisFont, _axisBrush, Convert.ToInt32(_xMax - xNameSize.Width), Convert.ToInt32(_yMax - xNameSize.Height));
                    _gStat.DrawString(lyName, _axisFont, _axisBrush, Convert.ToInt32(_xMax / 2), 1);
                    _xShift = (int)(_xMax / 2);
                    _yShift = (int)(_yMax);
                    _gStat.TranslateTransform(_xShift, _yShift, MatrixOrder.Append);
                    _gDin.TranslateTransform(_xShift, _yShift, MatrixOrder.Append);
                    _kx = _xMax / 2 / _xBase;
                    _ky = _yMax / _yBase;
                    break;

                case EAxisType.Quarter:

                    _gStat.DrawString(lxName, _axisFont, _axisBrush, Convert.ToInt32(_xMax - xNameSize.Width), Convert.ToInt32(_yMax / 2 - xNameSize.Height));
                    _gStat.DrawString(lyName, _axisFont, _axisBrush, 1, 1);
                    _xShift = 0;
                    _yShift = (int)(_yMax);
                    _gStat.TranslateTransform(_xShift, _yShift, MatrixOrder.Append);
                    _gDin.TranslateTransform(_xShift, _yShift, MatrixOrder.Append);
                    _kx = _xMax / _xBase;
                    _ky = _yMax / _yBase;
                    break;
            }
            pictureBox1.Image = _buffStat.Clone(_rect, PixelFormat.Undefined);
        }

        public void PixDraw(float x, float y, Color cColor, byte drawMode)
        {
            _xe = Convert.ToInt32(x * _kx);
            _ye = Convert.ToInt32(-y * _ky);
            _pixPen.Color = cColor;
            _pixBrush.Color = _pixPen.Color;
            _cPixSize = Convert.ToInt32(_kx * PixSize);
            if (_cPixSize >= 1) return;
            _cPixSize = 1;

            ToDetermineGraph(drawMode);

            switch (_cPixType)
            {
                case EPixType.Point:
                    _graph.DrawEllipse(_pixPen, _xe - _cPixSize / 2, _ye - _cPixSize / 2, _cPixSize, _cPixSize);
                    _graph.FillEllipse(_pixBrush, _xe - _cPixSize / 2, _ye - _cPixSize / 2, _cPixSize, _cPixSize);
                    break;
                case EPixType.Cross:
                    var x1 = _xe;
                    var x2 = _xe;
                    var y1 = _ye - _cPixSize / 2;
                    var y2 = _ye + _cPixSize / 2;
                    _graph.DrawLine(_pixPen, x1, y1, x2, y2);
                    x1 = _xe - _cPixSize / 2;
                    x2 = _xe + _cPixSize / 2;
                    y1 = _ye;
                    y2 = _ye;
                    _graph.DrawLine(_pixPen, x1, y1, x2, y2);
                    break;
                case EPixType.Circle:
                    _graph.DrawEllipse(_pixPen, _xe - _cPixSize / 2, _ye - _cPixSize / 2, _cPixSize, _cPixSize);
                    break;
                case EPixType.Rect:
                    _graph.FillRectangle(_pixBrush, _xe - _cPixSize / 2, _ye - _cPixSize / 2, _cPixSize, _cPixSize);
                    break;
            }
            CaseDrawMode(drawMode);
        }

        public void DrawAxisLine(float x1, float y1, float x2, float y2, byte drawMode)
        {
            var xe1 = Convert.ToInt32(x1 * _kx);
            var ye1 = Convert.ToInt32(-y1 * _ky);
            var xe2 = Convert.ToInt32(x2 * _kx);
            var ye2 = Convert.ToInt32(-y2 * _kx);

            ToDetermineGraph(drawMode);
            _graph.DrawLine(_pixPen,xe1,ye1,xe2,ye2);
            CaseDrawMode(drawMode);
        }

        private void ToDetermineGraph(byte drawMode)
        {
            if (drawMode == 0)
            {
                _graph = _gStat;
            }
            else if (drawMode == 1)
            {
                _graph = _gStat;
            }
            else if (drawMode == 2)
            {
                _graph = _gDin;
            }
        }

        private void CaseDrawMode(byte drawMode)
        {
            if (drawMode == 0)
            {
                _picImage = _buffStat.Clone(_rect, PixelFormat.Undefined);
            }

            if (drawMode == 1)
            {
                _gDin.DrawImage(_buffStat, Convert.ToSingle(-_xShift), Convert.ToSingle(-_yShift), _buffDin.Width, _buffDin.Height);
            }
        }

        public void DinToPic()
        {
            _picImage = _buffDin.Clone(_rect, PixelFormat.Undefined);
            _gDin.Clear(_picBackColor);
        }

        public void StatToPic()
        {
            _picImage = _buffStat.Clone(_rect, PixelFormat.Undefined);
        }

        public void StatToDin()
        {
            _gDin.DrawImage(_buffStat, Convert.ToSingle(-_xShift), Convert.ToSingle(-_yShift), _buffDin.Width,_buffDin.Height);
        }

        public void ClearPic()
        {
            AxisDraw();
        }

        public void ClearDin()
        {
            _gDin.Clear(_picBackColor);
        }

        public void ClearStat()
        {
            _gStat.Clear(_picBackColor);
        }

        private void UserControlResize(object sender, EventArgs e)  // handles this.Resize
        {
            pictureBox1.Top = 0;
            pictureBox1.Left = 0;
            pictureBox1.Width = this.Width;
            pictureBox1.Height = this.Height;
            _xMax = this.Width;
            _yMax = this.Height;

        }
    }
}
