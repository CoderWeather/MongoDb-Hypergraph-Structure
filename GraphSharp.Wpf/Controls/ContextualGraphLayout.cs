using System.Collections.Generic;
using System.Windows;
using GraphSharp.Algorithms.Layout;
using GraphSharp.Algorithms.Layout.Contextual;
using QuickGraph;

namespace GraphSharp.Controls
{
	public class ContextualGraphLayout<TVertex, TEdge, TGraph> : GraphLayout<TVertex, TEdge, TGraph>
		where TVertex : class
		where TEdge : IEdge<TVertex>
		where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
	{
		static ContextualGraphLayout()
		{
			LayoutAlgorithmFactoryProperty.OverrideMetadata(typeof(ContextualGraphLayout<TVertex, TEdge, TGraph>),
				new PropertyMetadata(new ContextualLayoutAlgorithmFactory<TVertex, TEdge, TGraph>(), null,
					LayoutAlgorithmFactory_Coerce));
		}

		protected override bool CanLayout => SelectedVertex != null && base.CanLayout;

		protected override ILayoutContext<TVertex, TEdge, TGraph> CreateLayoutContext(
			IDictionary<TVertex, Point> positions, IDictionary<TVertex, Size> sizes)
		{
			return new ContextualLayoutContext<TVertex, TEdge, TGraph>(Graph, SelectedVertex, positions, sizes);
		}

		#region Dependency Properties

		/// <summary>
		///     Gets or sets the SelectedVertex which influences the Context.
		/// </summary>
		public TVertex SelectedVertex
		{
			get => (TVertex) GetValue(SelectedVertexProperty);
			set => SetValue(SelectedVertexProperty, value);
		}

		public static readonly DependencyProperty SelectedVertexProperty = DependencyProperty.Register("SelectedVertex",
			typeof(TVertex), typeof(ContextualGraphLayout<TVertex, TEdge, TGraph>),
			new UIPropertyMetadata(default(TVertex), SelectedVertex_PropertyChanged));

		private static void SelectedVertex_PropertyChanged(DependencyObject obj,
			DependencyPropertyChangedEventArgs args)
		{
			var gl = obj as ContextualGraphLayout<TVertex, TEdge, TGraph>;
			if (gl == null)
				return;

			//refresh the layout on context change
			gl.Relayout();
		}

		#endregion
	}
}