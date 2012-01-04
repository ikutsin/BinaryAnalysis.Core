using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace BinaryAnalysis.UI.Controls
{
    public class VerticalLayoutPanel : Panel
    {
        private LayoutEngine layoutEngine;

        public override LayoutEngine LayoutEngine
        {
            get { return layoutEngine ?? new VerticalLayoutEngine(); }
        }
    }

    class VerticalLayoutEngine : LayoutEngine
    {
        public override bool Layout(object container, LayoutEventArgs layoutEventArgs)
        {
            Control parent = container as Control;

            // Use DisplayRectangle so that parent.Padding is honored.
            Rectangle parentDisplayRectangle = parent.DisplayRectangle;
            Point nextControlLocation = parentDisplayRectangle.Location;

            foreach (Control c in parent.Controls.Cast<Control>().Where(c => c.Visible).Reverse())
            {
                nextControlLocation.Offset(c.Margin.Left, c.Margin.Top);
                c.Location = nextControlLocation;

                if (c.AutoSize)
                {
                    c.Size = c.GetPreferredSize(parentDisplayRectangle.Size);
                }

                nextControlLocation.X = parentDisplayRectangle.X;
                nextControlLocation.Y += c.Height + c.Margin.Bottom;
            }
            return false;
        }
    }
}
