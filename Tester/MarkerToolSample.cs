﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using System.Drawing.Drawing2D;

namespace Tester
{
    public partial class MarkerToolSample : Form
    {
        //Shortcut style
        ShortcutStyle shortCutStyle = new ShortcutStyle(Pens.Maroon);
        //Marker styles
        MarkerStyle YellowStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(180, Color.Yellow)));
        MarkerStyle RedStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(180, Color.Red)));
        MarkerStyle GreenStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(180, Color.Green)));

        public MarkerToolSample()
        {
            InitializeComponent();
            //append text
            fctb.Text = "This example shows how to create Marker Tool and usage of ShortcutStyle class.\nAlso VisualMarkerClick event handling is present.\nAlso it shows how to set priority of styles.\n\nSelect any text, please.";
            //add style explicitly to control for define priority of style drawing
            fctb.AddStyle(YellowStyle);//render first
            fctb.AddStyle(RedStyle);//red will be rendering over yellow
            fctb.AddStyle(GreenStyle);//green will be rendering over yellow and red
            fctb.AddStyle(shortCutStyle);//render last, over all other styles
        }

        private void fctb_SelectionChangedDelayed(object sender, EventArgs e)
        {
            //here we draw shortcut for selection area
            Range selection = fctb.Selection;
            //clear previous shortcuts
            fctb.VisibleRange.ClearStyle(shortCutStyle);
            //create shortcuts
            if (selection.Start != selection.End)//user selected one or more chars?
            {
                //find last char
                var r = selection.Clone();
                r.Normalize();
                r.Start = r.End;//go to last char
                r.GoLeft(true);//select last char
                //apply ShortCutStyle
                r.SetStyle(shortCutStyle);
            }
        }


        private void fctb_VisualMarkerClick(object sender, VisualMarkerEventArgs e)
        {
            //is it our style ?
            if (e.Style == shortCutStyle)
            {
                //show popup menu
                cmMark.Show(fctb.PointToScreen(e.Location));
            }
        }

        private void markAsYellowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TrimSelection();
            //set background style
            switch((string)((sender as ToolStripMenuItem).Tag))
            {
                case "yellow": fctb.Selection.SetStyle(YellowStyle); break;
                case "red": fctb.Selection.SetStyle(RedStyle); break;
                case "green": fctb.Selection.SetStyle(GreenStyle); break;
                case "lineBackground": fctb[fctb.Selection.Start.iLine].BackgroundBrush = Brushes.Pink; break;
            }
            //clear shortcut style
            fctb.Selection.ClearStyle(shortCutStyle);
        }

        private void TrimSelection()
        {
            var sel = fctb.Selection;

            //trim left
            sel.Normalize();
            while (char.IsWhiteSpace(sel.CharAfterStart) && sel.Start < sel.End)
                sel.GoRight(true);
            //trim right
            sel.Inverse();
            while (char.IsWhiteSpace(sel.CharBeforeStart) && sel.Start > sel.End)
                sel.GoLeft(true);
        }

        private void clearMarkedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fctb.Selection.ClearStyle(YellowStyle, RedStyle, GreenStyle);
            fctb[fctb.Selection.Start.iLine].BackgroundBrush = null;
        }

        private void fctb_PaintLine(object sender, PaintLineEventArgs e)
        {
            //draw current line marker
            if (e.LineIndex == fctb.Selection.Start.iLine)
                e.Graphics.FillEllipse(new LinearGradientBrush(new Rectangle(0, e.LineRect.Top, 15, 15), Color.LightPink, Color.Red, 45), 0, e.LineRect.Top, 15, 15);
        }
    }
}
