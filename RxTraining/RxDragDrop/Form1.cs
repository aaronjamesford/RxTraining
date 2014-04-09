using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reactive.Linq;

namespace RxDragDrop
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Drag(this.bluePanel).Subscribe(p => this.bluePanel.Location = p);
        }

        private static IObservable<Point> Drag(Control ctrl)
        {
            // TODO #10 Modify this code to implement a drag & drop using Reactive Extensions
            //          You should have learned all the tools so far to do this
            //          Below are some observables created from events. you will need these.
            //          The TakeUntil extension method will only take elements from the observable
            //          until either a specified time or until an element on another observable is published.

            var mousedown = Observable.FromEventPattern<MouseEventArgs>(ctrl, "MouseDown")
                .Select(args => new Size(args.EventArgs.Location));
            var mousemove = Observable.FromEventPattern<MouseEventArgs>(ctrl, "MouseMove")
                .Select(args => new Size(args.EventArgs.Location));
            var mouseup = Observable.FromEventPattern<MouseEventArgs>(ctrl, "MouseUp");

            return Observable.Empty<Point>();
        }
    }
}
