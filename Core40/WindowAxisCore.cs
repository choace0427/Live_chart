using System;
using System.Collections.Generic;
using System.Linq;
using LiveCharts.Charts;
using LiveCharts.Definitions.Charts;
using LiveCharts.Dtos;

namespace LiveCharts
{
    /// <summary>
    /// Provides an axis that displays seperators based upon configured windows
    /// </summary>
    public class WindowAxisCore : AxisCore
    {
        private IAxisWindow _selectedWindow;

        private IAxisWindow SelectedWindow
        {
            get { return _selectedWindow; }
            set
            {
                if (Equals(_selectedWindow, value)) return;
                _selectedWindow = value;
                ((IWindowAxisView)View).SetSelectedWindow(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<AxisWindow> Windows { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        public WindowAxisCore(IAxisView view) : base(view)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        internal override CoreMargin PrepareChart(AxisOrientation source, ChartCore chart)
        {
            if (!(Math.Abs(TopLimit - BotLimit) > S * .01) || !ShowLabels) return new CoreMargin();

            var currentMargin = new CoreMargin();
            var tolerance = S / 10;

            InitializeGarbageCollector();

            // Determine which magnitude and unit to use
            var m = (!double.IsNaN(View.Unit) ? View.Unit : Magnitude);
            var u = (!double.IsNaN(View.Unit) ? View.Unit : 1);

            // Calculate the separators and the resolution
            var indices = CalculateSeparatorIndices(chart, source, u);

            // Draw the separators
            foreach (var index in indices)
            {
                DrawSeparator(index, tolerance, currentMargin, source);
            }

            return currentMargin;
        }

        internal IEnumerable<double> CalculateSeparatorIndices(ChartCore chart, AxisOrientation source, double unit)
        {
            if (!double.IsNaN(Separator.Step)) throw new Exception("Step should be NaN for WindowAxis separators");
            if (Windows == null) return Enumerable.Empty<double>();

            // Find the seperator resolution represented by the first available window
            double supportedSeparatorCount = 0;

            // Holder for the calculated separator indices and the proposed window
            var separatorIndices = new List<double>();
            IAxisWindow proposedWindow = AxisWindows.EmptyWindow;

            // Build a range of possible separator indices
            var rangeIndices = Enumerable.Range((int)Math.Floor(BotLimit), (int)Math.Floor(TopLimit - (EvaluatesUnitWidth ? unit : 0) - BotLimit)).Select(i => (double)i).ToList();

            // Make sure we have at least 2 separators to show
            if (Windows != null && rangeIndices.Count > 1)
            {
                foreach (var window in Windows)
                {
                    IEnumerable<double> proposedSeparatorIndices;

                    // Let the window validate our range
                    if (!window.TryGetSeparatorIndices(rangeIndices, out proposedSeparatorIndices))
                    {
                        // This window does not support this range. Skip it
                        continue;
                    }

                    separatorIndices = proposedSeparatorIndices.ToList();

                    // Validate the requirements of the window
                    supportedSeparatorCount = Math.Round(chart.ControlSize.Width / (window.MinimumSeparatorWidth * CleanFactor), 0);

                    if (supportedSeparatorCount < separatorIndices.Count)
                    {
                        // We do not support this range of separators. Skip the window
                        continue;
                    }

                    // Pick this window. It is the first who passed both validations and our best candidate
                    proposedWindow = window;
                    break;
                }
            }

            if (proposedWindow == null)
            {
                // All variables are still set to defaults
            }

            // Force the step of 1, as our preparechart will filter the X asis for valid separators, and will skip a few
            S = 1;

            Magnitude = Math.Pow(10, Math.Floor(Math.Log(supportedSeparatorCount) / Math.Log(10)));
            SelectedWindow = proposedWindow;

            return separatorIndices;
        }

        private void DrawSeparator(double x, double tolerance, CoreMargin currentMargin, AxisOrientation source)
        {
            SeparatorElementCore elementCore;

            var key = Math.Round(x / tolerance) * tolerance;

            if (!Cache.TryGetValue(key, out elementCore))
            {
                elementCore = new DateSeparatorElementCore { IsNew = true };
                Cache[key] = elementCore;
            }
            else
            {
                elementCore.IsNew = false;
            }

            // Determine whether this separator is a header now
            ((DateSeparatorElementCore)elementCore).IsHeader = SelectedWindow.IsHeader(x);

            View.RenderSeparator(elementCore, Chart);

            elementCore.Key = key;
            elementCore.Value = x;
            elementCore.GarbageCollectorIndex = GarbageCollectorIndex;

            var labelsMargin = elementCore.View.UpdateLabel(SelectedWindow.FormatAxisLabel(x), this, source);

            currentMargin.Width = labelsMargin.TakenWidth > currentMargin.Width
                ? labelsMargin.TakenWidth
                : currentMargin.Width;
            currentMargin.Height = labelsMargin.TakenHeight > currentMargin.Height
                ? labelsMargin.TakenHeight
                : currentMargin.Height;

            currentMargin.Left = labelsMargin.Left > currentMargin.Left
                ? labelsMargin.Left
                : currentMargin.Left;
            currentMargin.Right = labelsMargin.Right > currentMargin.Right
                ? labelsMargin.Right
                : currentMargin.Right;

            currentMargin.Top = labelsMargin.Top > currentMargin.Top
                ? labelsMargin.Top
                : currentMargin.Top;
            currentMargin.Bottom = labelsMargin.Bottom > currentMargin.Bottom
                ? labelsMargin.Bottom
                : currentMargin.Bottom;

            if (LastAxisMax == null)
            {
                elementCore.State = SeparationState.InitialAdd;
                return;
            }

            elementCore.State = SeparationState.Keep;
        }
    }
}