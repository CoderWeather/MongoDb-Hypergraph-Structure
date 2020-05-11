using System.Windows;
using HyperGraphSharp.Models;

namespace HyperGraphSharp.Controls
{
    public partial class HyperGraphLayout
    {
        public static readonly DependencyProperty GraphProperty = DependencyProperty.Register(
            "Graph", typeof(HyperGraph), typeof(HyperGraphLayout),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnGraphPropertyChanged));

        public HyperGraph Graph
        {
            get => (HyperGraph) GetValue(GraphProperty);
            set => SetValue(GraphProperty, value);
        }

        private static void OnGraphPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var graphLayout = d as HyperGraphLayout;
            graphLayout?.RecreateGraphElements();
            graphLayout?.Layout();
        }
    }
}