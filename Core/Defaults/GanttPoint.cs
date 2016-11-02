﻿//The MIT License(MIT)

//copyright(c) 2016 Alberto Rodriguez

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System;

namespace LiveCharts.Defaults
{
    /// <summary>
    /// Defines a Gantt point in a cartesian chart
    /// </summary>
    public class GanttPoint : IObservableChartPoint
    {
        private double _startPoint;
        private double _endPoint;

        /// <summary>
        /// Initializes a new instance of GanttPoint class.
        /// </summary>
        public GanttPoint()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of GanttPoint class with given start and end points.
        /// </summary>
        public GanttPoint(double startPoint, double endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }
         
        /// <summary>
        /// Gets or sets point start
        /// </summary>
        public double StartPoint
        {
            get { return _startPoint; }
            set
            {
                _startPoint = value;
                OnPointChanged();
            }
        }

        /// <summary>
        /// Gets or sets point end
        /// </summary>
        public double EndPoint
        {
            get { return _endPoint; }
            set
            {
                _endPoint = value;
                OnPointChanged();
            }
        }

        /// <summary>
        /// PointChanged event
        /// </summary>
        public event Action PointChanged;

        /// <summary>
        /// OnPoint property changed method
        /// </summary>
        protected virtual void OnPointChanged()
        {
            if (PointChanged != null)
                PointChanged.Invoke();
        }
    }
}