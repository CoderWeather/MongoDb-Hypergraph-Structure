using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using GraphSharp.Algorithms.EdgeRouting;
using GraphSharp.Algorithms.Highlight;
using GraphSharp.Algorithms.Layout;
using GraphSharp.Algorithms.OverlapRemoval;
using QuickGraph;

namespace GraphSharp.Controls
{
    public partial class GraphLayout<TVertex, TEdge, TGraph> 
        where TVertex : class
        where TEdge : IEdge<TVertex>
        where TGraph : class, IBidirectionalGraph<TVertex, TEdge>
    {
        #region Dependency Property registrations

        /// <summary>
        ///     If the graph has more vertex than this count, the animation will be disabled
        ///     automatically.
        /// </summary>
        public int AnimationDisablerVertexCount
        {
            get => (int)GetValue(AnimationDisablerVertexCountProperty);
            set => SetValue(AnimationDisablerVertexCountProperty, value);
        }

        public static readonly DependencyProperty AnimationDisablerVertexCountProperty =
            DependencyProperty.Register("AnimationDisablerVertexCount", typeof(int),
                typeof(GraphLayout<TVertex, TEdge, TGraph>), new UIPropertyMetadata(200));


        /// <summary>
        ///     If the graph has more edge than this count, the animations will be
        ///     disabled automatically.
        /// </summary>
        public int AnimationDisablerEdgeCount
        {
            get => (int)GetValue(AnimationDisablerEdgeCountProperty);
            set => SetValue(AnimationDisablerEdgeCountProperty, value);
        }

        public static readonly DependencyProperty AnimationDisablerEdgeCountProperty =
            DependencyProperty.Register("AnimationDisablerEdgeCount", typeof(int),
                typeof(GraphLayout<TVertex, TEdge, TGraph>), new UIPropertyMetadata(500));

        public override bool CanAnimate =>
            base.CanAnimate
         && AnimationLength > new TimeSpan(0, 0, 0, 0, 0)
         && Graph != null
         && (AnimationDisablerVertexCount < 0 || Graph.VertexCount < AnimationDisablerVertexCount)
         && (AnimationDisablerEdgeCount < 0 || Graph.EdgeCount < AnimationDisablerEdgeCount);

        //computing and status
        public static readonly DependencyProperty AsyncComputeProperty = DependencyProperty.Register("AsyncCompute",
            typeof(bool),
            typeof(
                GraphLayout
                <TVertex, TEdge
                    , TGraph>),
            new UIPropertyMetadata
                (false));

        public static readonly DependencyProperty EdgeRoutingAlgorithmFactoryProperty =
            DependencyProperty.Register("EdgeRoutingAlgorithmFactory",
                typeof(IEdgeRoutingAlgorithmFactory<TVertex, TEdge, TGraph>),
                typeof(GraphLayout<TVertex, TEdge, TGraph>),
                new PropertyMetadata(
                    new StandardEdgeRoutingAlgorithmFactory<TVertex, TEdge, TGraph>(),
                    null, EdgeRoutingAlgorithmFactory_Coerce));

        public static readonly DependencyProperty EdgeRoutingAlgorithmProperty;

        protected static readonly DependencyPropertyKey EdgeRoutingAlgorithmPropertyKey =
            DependencyProperty.RegisterReadOnly("EdgeRoutingAlgorithm",
                typeof(IEdgeRoutingAlgorithm<TVertex, TEdge, TGraph>),
                typeof(GraphLayout<TVertex, TEdge, TGraph>),
                new UIPropertyMetadata(null));

        public static readonly DependencyProperty EdgeRoutingAlgorithmTypeProperty =
            DependencyProperty.Register("EdgeRoutingAlgorithmType", typeof(string),
                typeof(GraphLayout<TVertex, TEdge, TGraph>),
                new PropertyMetadata(string.Empty, EdgeRoutingAlgorithmType_PropertyChanged));

        public static readonly DependencyProperty EdgeRoutingConstraintProperty =
            DependencyProperty.Register("EdgeRoutingConstraint", typeof(AlgorithmConstraints),
                typeof(GraphLayout<TVertex, TEdge, TGraph>),
                new PropertyMetadata(AlgorithmConstraints.Automatic,
                    EdgeRoutingConstraint_PropertyChanged));

        public static readonly DependencyProperty EdgeRoutingParametersProperty =
            DependencyProperty.Register("EdgeRoutingParameters", typeof(IEdgeRoutingParameters),
                typeof(GraphLayout<TVertex, TEdge, TGraph>), new PropertyMetadata(null));

        public static readonly DependencyProperty GraphProperty = DependencyProperty.Register(
			"Graph", typeof(TGraph), typeof(GraphLayout<TVertex, TEdge, TGraph>),
            new FrameworkPropertyMetadata
            (null,
                FrameworkPropertyMetadataOptions
                   .AffectsRender,
                Graph_PropertyChanged));

        public static readonly DependencyProperty HighlightAlgorithmFactoryProperty =
            DependencyProperty.Register("HighlightAlgorithmFactory",
                typeof(IHighlightAlgorithmFactory<TVertex, TEdge, TGraph>),
                typeof(GraphLayout<TVertex, TEdge, TGraph>),
                new PropertyMetadata(
                    new StandardHighlightAlgorithmFactory<TVertex, TEdge, TGraph>(),
                    HighlightAlgorithmFactory_PropertyChanged, HighlightAlgorithmFactory_Coerce));

        public static readonly DependencyProperty HighlightAlgorithmProperty;

        protected static readonly DependencyPropertyKey HighlightAlgorithmPropertyKey =
            DependencyProperty.RegisterReadOnly("HighlightAlgorithm",
                typeof(IHighlightAlgorithm<TVertex, TEdge, TGraph>),
                typeof(GraphLayout<TVertex, TEdge, TGraph>),
                new UIPropertyMetadata(null, HighlightAlgorithm_PropertyChanged));

        public static readonly DependencyProperty HighlightAlgorithmTypeProperty =
            DependencyProperty.Register("HighlightAlgorithmType", typeof(string),
                typeof(GraphLayout<TVertex, TEdge, TGraph>),
                new PropertyMetadata(string.Empty, HighlightAlgorithmType_PropertyChanged,
                    HighlightAlgorithmType_Coerce));

        public static readonly DependencyProperty HighlightParametersProperty =
            DependencyProperty.Register("HighlightParameters", typeof(IHighlightParameters),
                typeof(GraphLayout<TVertex, TEdge, TGraph>),
                new PropertyMetadata(null, null, HighlightParameters_Coerce));

        //algorithm factories
        public static readonly DependencyProperty LayoutAlgorithmFactoryProperty =
            DependencyProperty.Register("LayoutAlgorithmFactory",
                typeof(ILayoutAlgorithmFactory<TVertex, TEdge, TGraph>),
                typeof(GraphLayout<TVertex, TEdge, TGraph>),
                new PropertyMetadata(
                    new StandardLayoutAlgorithmFactory<TVertex, TEdge, TGraph>(), null,
                    LayoutAlgorithmFactory_Coerce));

        public static readonly DependencyProperty LayoutAlgorithmProperty;

        protected static readonly DependencyPropertyKey LayoutAlgorithmPropertyKey =
            DependencyProperty.RegisterReadOnly("LayoutAlgorithm", typeof(ILayoutAlgorithm<TVertex, TEdge, TGraph>),
                typeof(GraphLayout<TVertex, TEdge, TGraph>),
                new UIPropertyMetadata(null));

        public static readonly DependencyProperty LayoutAlgorithmTypeProperty =
            DependencyProperty.Register("LayoutAlgorithmType", typeof(string),
                typeof(GraphLayout<TVertex, TEdge, TGraph>),
                new PropertyMetadata(string.Empty, LayoutAlgorithmType_PropertyChanged));

        public static readonly DependencyProperty UiLayoutModeProperty = DependencyProperty.Register("UiLayoutMode",
            typeof(UiLayoutMode),
            typeof(
                GraphLayout
                <TVertex, TEdge,
                    TGraph>),
            new PropertyMetadata(
                UiLayoutMode.Automatic,
                LayoutMode_PropertyChanged,
                LayoutMode_Coerce));

        //algorithm parameters
        public static readonly DependencyProperty LayoutParametersProperty =
            DependencyProperty.Register("LayoutParameters", typeof(ILayoutParameters),
                typeof(GraphLayout<TVertex, TEdge, TGraph>), new PropertyMetadata(null));

        public static readonly DependencyProperty LayoutStateProperty;

        protected static readonly DependencyPropertyKey LayoutStatePropertyKey =
            DependencyProperty.RegisterReadOnly("LayoutState", typeof(LayoutState<TVertex, TEdge>),
                typeof(GraphLayout<TVertex, TEdge, TGraph>),
                new UIPropertyMetadata(null));

        public static readonly DependencyProperty LayoutStatusPercentProperty;

        protected static readonly DependencyPropertyKey LayoutStatusPercentPropertyKey =
            DependencyProperty.RegisterReadOnly("LayoutStatusPercent", typeof(double),
                typeof(GraphLayout<TVertex, TEdge, TGraph>),
                new UIPropertyMetadata(0.0));

        public static readonly DependencyProperty OverlapRemovalAlgorithmFactoryProperty =
            DependencyProperty.Register("OverlapRemovalAlgorithmFactory",
                typeof(IOverlapRemovalAlgorithmFactory<TVertex>),
                typeof(GraphLayout<TVertex, TEdge, TGraph>),
                new PropertyMetadata(new StandardOverlapRemovalAlgorithmFactory<TVertex>(), null,
                    OverlapRemovalAlgorithmFactory_Coerce));

        public static readonly DependencyProperty OverlapRemovalAlgorithmProperty;

        protected static readonly DependencyPropertyKey OverlapRemovalAlgorithmPropertyKey =
            DependencyProperty.RegisterReadOnly("OverlapRemovalAlgorithm", typeof(IOverlapRemovalAlgorithm<TVertex>),
                typeof(GraphLayout<TVertex, TEdge, TGraph>),
                new UIPropertyMetadata(null));

        public static readonly DependencyProperty OverlapRemovalAlgorithmTypeProperty =
            DependencyProperty.Register("OverlapRemovalAlgorithmType", typeof(string),
                typeof(GraphLayout<TVertex, TEdge, TGraph>),
                new PropertyMetadata(string.Empty, OverlapRemovalAlgorithmType_PropertyChanged));

        public static readonly DependencyProperty OverlapRemovalConstraintProperty =
            DependencyProperty.Register("OverlapRemovalConstraint", typeof(AlgorithmConstraints),
                typeof(GraphLayout<TVertex, TEdge, TGraph>),
                new PropertyMetadata(AlgorithmConstraints.Automatic,
                    OverlapRemovalConstraint_PropertyChanged));

        public static readonly DependencyProperty OverlapRemovalParametersProperty =
            DependencyProperty.Register("OverlapRemovalParameters", typeof(IOverlapRemovalParameters),
                typeof(GraphLayout<TVertex, TEdge, TGraph>), new PropertyMetadata(null));

        public static readonly DependencyProperty ShowAllStatesProperty = DependencyProperty.Register("ShowAllStates",
            typeof(bool),
            typeof(
                GraphLayout
                <TVertex,
                    TEdge, TGraph>
            ),
            new UIPropertyMetadata
                (false));

        public static readonly DependencyProperty StateCountProperty;

        protected static readonly DependencyPropertyKey StateCountPropertyKey =
            DependencyProperty.RegisterReadOnly("StateCount", typeof(int), typeof(GraphLayout<TVertex, TEdge, TGraph>),
                new UIPropertyMetadata(0, StateCount_PropertyChanged));

        public static readonly DependencyProperty StateIndexProperty = DependencyProperty.Register("StateIndex",
            typeof(int),
            typeof(
                GraphLayout
                <TVertex, TEdge,
                    TGraph>),
            new UIPropertyMetadata
            (0,
                StateIndex_PropertyChanged,
                StateIndex_Coerce));

        /// <summary>
        ///     Static Constructor. At this time it's only task to initialize the readonly Dependency Properties.
        /// </summary>
        static GraphLayout()
        {
            //initialize the readonly dependency properties
            StateCountProperty = StateCountPropertyKey.DependencyProperty;
            LayoutStatusPercentProperty = LayoutStatusPercentPropertyKey.DependencyProperty;
            LayoutAlgorithmProperty = LayoutAlgorithmPropertyKey.DependencyProperty;
            OverlapRemovalAlgorithmProperty = OverlapRemovalAlgorithmPropertyKey.DependencyProperty;
            EdgeRoutingAlgorithmProperty = EdgeRoutingAlgorithmPropertyKey.DependencyProperty;
            LayoutStateProperty = LayoutStatusPercentPropertyKey.DependencyProperty;
            HighlightAlgorithmProperty = HighlightAlgorithmPropertyKey.DependencyProperty;
        }

        public GraphLayout()
        {
            _lastNotificationTimestamp = DateTime.Now;
            _stopWatch = new Stopwatch();
            _verticesRemoved = new Queue<TVertex>();
            _verticesAdded = new Queue<TVertex>();
            VertexControls = new Dictionary<TVertex, VertexControl>();
            _notificationSyncRoot = new object();
            _notificationLayoutDelay = new TimeSpan(0, 0, 0, 0, 5);
            _layoutStates = new List<LayoutState<TVertex, TEdge>>();
            _edgesRemoved = new Queue<TEdge>();
            _edgesAdded = new Queue<TEdge>();
            EdgeControls = new Dictionary<TEdge, EdgeControl>();
            AddHandler(GraphElementBehaviour.HighlightTriggeredEvent,
                new HighlightTriggerEventHandler(HighlightTriggerEventHandler));
        }

        public void HighlightTriggerEventHandler(object sender, HighlightTriggeredEventArgs args)
        {
            if (Graph == null || HighlightAlgorithm == null)
                return;

            if (args.OriginalSource is VertexControl)
            {
                var vc = (VertexControl)args.OriginalSource;
                if (!(vc.Vertex is TVertex vertex) || !Graph.ContainsVertex(vertex))
                    return;

                if (args.IsPositiveTrigger)
                    HighlightAlgorithm.OnVertexHighlighting(vertex);
                else
                    HighlightAlgorithm.OnVertexHighlightRemoving(vertex);
            }
            else if (args.OriginalSource is EdgeControl ec)
            {
                var edge = default(TEdge);
                try
                {
                    edge = (TEdge)ec.Edge;
                }
                catch
                {
                    // ignored
                }

                if (Equals(edge, default(TEdge)) || !Graph.ContainsEdge(edge))
                    return;

                if (args.IsPositiveTrigger)
                    HighlightAlgorithm.OnEdgeHighlighting(edge);
                else
                    HighlightAlgorithm.OnEdgeHighlightRemoving(edge);
            }
        }

        #endregion

        #region Dependency Property Callbacks

        /// <summary>
        ///     Property Change Callback of the <code>StateIndex</code> dependency property. It call the animation,
        ///     so the layout will animate from one state to the another.
        /// </summary>
        protected static void StateIndex_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is GraphLayout<TVertex, TEdge, TGraph> gl)) return;

            var p = (int)e.NewValue;
            gl.ChangeState(p);
        }


        /// <summary>
        ///     Coerce Callback of the <code>StateIndex</code> dependency property.
        /// </summary>
        /// <returns>It coerces the value of the <see cref="StateIndex" /> between 0 and (<see cref="StateCount" />-1).</returns>
        protected static object StateIndex_Coerce(DependencyObject obj, object stateIndex)
        {
            var gl = obj as GraphLayout<TVertex, TEdge, TGraph>;
            var p = (int)stateIndex;

            if (gl == null) return p;
            if (p < 0 || gl._layoutStates.Count == 0) return 0;
            if (p > gl._layoutStates.Count - 1) return gl._layoutStates.Count - 1;
            return p;
        }

        /// <summary>
        ///     Property Change Callback of the <see cref="StateCount" /> dependency property.
        ///     It coerces the <see cref="StateIndex" />.
        /// </summary>
        protected static void StateCount_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is GraphLayout<TVertex, TEdge, TGraph> gl)
                gl.CoerceValue(StateIndexProperty);
        }

        protected static void LayoutMode_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var gl = (GraphLayout<TVertex, TEdge, TGraph>)obj;
            gl.OnRelayoutInduction(false);
        }

        /// <summary>
        ///     Coerce callback of the <see cref="UiLayoutMode" /> dependency property.
        /// </summary>
        private static object LayoutMode_Coerce(DependencyObject obj, object newValue)
        {
            var gl = (GraphLayout<TVertex, TEdge, TGraph>)obj;
            if (gl.Graph == null)
                return UiLayoutMode.Automatic;

            return newValue;
        }

        /// <summary>
        ///     It's called when the Graph dependency property changed.
        /// </summary>
        /// <param name="obj">The GraphLayout instance which Graph dependency property changed.</param>
        /// <param name="e">OldValue & NewValue</param>
        protected static void Graph_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var gl = (GraphLayout<TVertex, TEdge, TGraph>)obj;

            if (!(e.NewValue is TGraph g))
            {
                gl.UiLayoutMode = UiLayoutMode.Automatic;
                return;
            }

            gl.OnRelayoutInduction(true);
        }

        private void OnRelayoutInduction(bool tryKeepControls)
        {
            HighlightAlgorithm?.ResetHighlight();

            //recreate the graph elements
            RecreateGraphElements(tryKeepControls);

            //do the layout process again
            ReLayout();
        }

        protected static object LayoutAlgorithmFactory_Coerce(DependencyObject obj, object newValue)
        {
            var gl = (GraphLayout<TVertex, TEdge, TGraph>)obj;
            return newValue ?? gl.LayoutAlgorithmFactory;
        }

        protected static object EdgeRoutingAlgorithmFactory_Coerce(DependencyObject obj, object newValue)
        {
            var gl = (GraphLayout<TVertex, TEdge, TGraph>)obj;
            return newValue ?? gl.EdgeRoutingAlgorithmFactory;
        }

        protected static object OverlapRemovalAlgorithmFactory_Coerce(DependencyObject obj, object newValue)
        {
            var gl = (GraphLayout<TVertex, TEdge, TGraph>)obj;
            return newValue ?? gl.OverlapRemovalAlgorithmFactory;
        }

        protected static object HighlightAlgorithmFactory_Coerce(DependencyObject obj, object newValue)
        {
            var gl = (GraphLayout<TVertex, TEdge, TGraph>)obj;
            return newValue ?? gl.HighlightAlgorithmFactory;
        }

        private static void HighlightAlgorithmFactory_PropertyChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
        {
            var gl = (GraphLayout<TVertex, TEdge, TGraph>)obj;

            var highlightMethod = gl.HighlightAlgorithmType;
            gl.HighlightAlgorithmType = null;
            gl.HighlightAlgorithmType = highlightMethod;
        }

        protected static void LayoutAlgorithmType_PropertyChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
        {
            var gl = (GraphLayout<TVertex, TEdge, TGraph>)obj;
            var newAlgType = e.NewValue == null ? string.Empty : e.NewValue.ToString();

            //regenerate parameters
            gl.LayoutParameters = gl.LayoutAlgorithmFactory.CreateParameters(newAlgType, gl.LayoutParameters);

            if (gl.Graph != null) 
                gl.ReLayout();
        }

        protected static void EdgeRoutingAlgorithmType_PropertyChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
        {
            var gl = (GraphLayout<TVertex, TEdge, TGraph>)obj;

            var newAlgType = e.NewValue == null ? string.Empty : e.NewValue.ToString();

            //regenerate parameters
            gl.EdgeRoutingParameters = gl.EdgeRoutingAlgorithmFactory.CreateParameters(newAlgType,
                gl.EdgeRoutingParameters);

            if (gl.Graph != null) gl.RecalculateEdgeRouting();
        }

        protected static void EdgeRoutingConstraint_PropertyChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
        {
            var gl = (GraphLayout<TVertex, TEdge, TGraph>)obj;

            //regenerate parameters
            if (gl.Graph != null)
                gl.RecalculateEdgeRouting();
        }

        protected static void OverlapRemovalAlgorithmType_PropertyChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
        {
            var gl = (GraphLayout<TVertex, TEdge, TGraph>)obj;

            var newAlgType = e.NewValue == null ? string.Empty : e.NewValue.ToString();

            //regenerate parameters
            gl.OverlapRemovalParameters = gl.OverlapRemovalAlgorithmFactory.CreateParameters(newAlgType,
                gl.OverlapRemovalParameters);

            if (gl.Graph != null) gl.RecalculateOverlapRemoval();
        }

        protected static void OverlapRemovalConstraint_PropertyChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
        {
            var gl = (GraphLayout<TVertex, TEdge, TGraph>)obj;

            //regenerate parameters
            if (gl.Graph != null)
                gl.RecalculateOverlapRemoval();
        }

        private static object? HighlightAlgorithmType_Coerce(DependencyObject obj, object newValue)
        {
            var gl = (GraphLayout<TVertex, TEdge, TGraph>)obj;

            return !gl.HighlightAlgorithmFactory.IsValidMode(newValue as string) 
                ? null 
                : newValue;
        }

        protected static void HighlightAlgorithmType_PropertyChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
        {
            var gl = (GraphLayout<TVertex, TEdge, TGraph>)obj;

            var newAlgType = e.NewValue == null ? string.Empty : e.NewValue.ToString();

            //regenerate algorithm, parameters
            var parameters = gl.HighlightAlgorithmFactory.CreateParameters(newAlgType, gl.HighlightParameters);
            gl.HighlightAlgorithm = gl.HighlightAlgorithmFactory.CreateAlgorithm(newAlgType,
                gl.CreateHighlightContext(), gl,
                parameters);
        }

        private static void HighlightAlgorithm_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is IHighlightAlgorithm<TVertex, TEdge, TGraph> algo)
                algo.ResetHighlight();
        }

        private static object? HighlightParameters_Coerce(DependencyObject obj, object newValue)
        {
            var gl = (GraphLayout<TVertex, TEdge, TGraph>)obj;

            if (gl.HighlightAlgorithm == null) return null;
            gl.HighlightAlgorithm.TrySetParameters(newValue as IHighlightParameters);
            return gl.HighlightAlgorithm.Parameters;

        }

        #endregion

        #region Dependency Properties

        /// <summary>
        ///     If this property is <code>true</code> the algorithm will be executed on a separate <see cref="Thread" />.
        ///     Dependency Property.
        /// </summary>
        public bool AsyncCompute
        {
            get => (bool)GetValue(AsyncComputeProperty);
            set => SetValue(AsyncComputeProperty, value);
        }

        /// <summary>
        ///     Dependency property. Gets or sets the layout mode.
        /// </summary>
        public UiLayoutMode UiLayoutMode
        {
            get => (UiLayoutMode)GetValue(UiLayoutModeProperty);
            set => SetValue(UiLayoutModeProperty, value);
        }

        /// <summary>
        ///     Gets the status of the layout process in percent. Readonly Dependency Property.
        /// </summary>
        public double LayoutStatusPercent
        {
            get => (double)GetValue(LayoutStatusPercentProperty);
            protected set => SetValue(LayoutStatusPercentPropertyKey, value);
        }

        /// <summary>
        ///     If <code>true</code> all state of the layout process will be stored, so you can "replay" the layout, otherwise
        ///     only the start and end states will be stored. Dependency Property.
        /// </summary>
        public bool ShowAllStates
        {
            get => (bool)GetValue(ShowAllStatesProperty);
            set => SetValue(ShowAllStatesProperty, value);
        }

        /// <summary>
        ///     Gets or sets the index of the actually shown layout state. Dependency Property.
        /// </summary>
        public int StateIndex
        {
            get => (int)GetValue(StateIndexProperty);
            set => SetValue(StateIndexProperty, value);
        }

        /// <summary>
        ///     Gets the number of the layout states. Readonly Dependency Property.
        /// </summary>
        public int StateCount
        {
            get => (int)GetValue(StateCountProperty);
            protected set => SetValue(StateCountPropertyKey, value);
        }

        public LayoutState<TVertex, TEdge> LayoutState
        {
            get => (LayoutState<TVertex, TEdge>)GetValue(LayoutStateProperty);
            protected set => SetValue(LayoutStatePropertyKey, value);
        }

        /// <summary>
        ///     The algorithm which have been used/is running/will be executed. Dependency Property.
        /// </summary>
        public ILayoutAlgorithm<TVertex, TEdge, TGraph> LayoutAlgorithm
        {
            get => (ILayoutAlgorithm<TVertex, TEdge, TGraph>)GetValue(LayoutAlgorithmProperty);
            protected set => SetValue(LayoutAlgorithmPropertyKey, value);
        }

        /// <summary>
        ///     The algorithm used for the edge routing. (It can be the same object as the LayoutAlgorithm, in this case the
        ///     EdgeRoutingAlgorithm won't be run separately, the edge routing should be calculated in the LayoutAlgorithm).
        /// </summary>
        public IEdgeRoutingAlgorithm<TVertex, TEdge, TGraph> EdgeRoutingAlgorithm
        {
            get => (IEdgeRoutingAlgorithm<TVertex, TEdge, TGraph>)GetValue(EdgeRoutingAlgorithmProperty);
            protected set => SetValue(EdgeRoutingAlgorithmPropertyKey, value);
        }

        public IOverlapRemovalAlgorithm<TVertex> OverlapRemovalAlgorithm
        {
            get => (IOverlapRemovalAlgorithm<TVertex>)GetValue(OverlapRemovalAlgorithmProperty);
            protected set => SetValue(OverlapRemovalAlgorithmPropertyKey, value);
        }

        public IHighlightAlgorithm<TVertex, TEdge, TGraph> HighlightAlgorithm
        {
            get => (IHighlightAlgorithm<TVertex, TEdge, TGraph>)GetValue(HighlightAlgorithmProperty);
            protected set => SetValue(HighlightAlgorithmPropertyKey, value);
        }

        public ILayoutAlgorithmFactory<TVertex, TEdge, TGraph> LayoutAlgorithmFactory
        {
            get => (ILayoutAlgorithmFactory<TVertex, TEdge, TGraph>)GetValue(LayoutAlgorithmFactoryProperty);
            set => SetValue(LayoutAlgorithmFactoryProperty, value);
        }

        public string LayoutAlgorithmType
        {
            get => (string)GetValue(LayoutAlgorithmTypeProperty);
            set => SetValue(LayoutAlgorithmTypeProperty, value);
        }

        public IEdgeRoutingAlgorithmFactory<TVertex, TEdge, TGraph> EdgeRoutingAlgorithmFactory
        {
            get => (IEdgeRoutingAlgorithmFactory<TVertex, TEdge, TGraph>)GetValue(EdgeRoutingAlgorithmFactoryProperty);
            set => SetValue(EdgeRoutingAlgorithmFactoryProperty, value);
        }

        public string EdgeRoutingAlgorithmType
        {
            get => (string)GetValue(EdgeRoutingAlgorithmTypeProperty);
            set => SetValue(EdgeRoutingAlgorithmTypeProperty, value);
        }

        public IOverlapRemovalAlgorithmFactory<TVertex> OverlapRemovalAlgorithmFactory
        {
            get => (IOverlapRemovalAlgorithmFactory<TVertex>)GetValue(OverlapRemovalAlgorithmFactoryProperty);
            set => SetValue(OverlapRemovalAlgorithmFactoryProperty, value);
        }

        public string OverlapRemovalAlgorithmType
        {
            get => (string)GetValue(OverlapRemovalAlgorithmTypeProperty);
            set => SetValue(OverlapRemovalAlgorithmTypeProperty, value);
        }

        public IHighlightAlgorithmFactory<TVertex, TEdge, TGraph> HighlightAlgorithmFactory
        {
            get => (IHighlightAlgorithmFactory<TVertex, TEdge, TGraph>)GetValue(HighlightAlgorithmFactoryProperty);
            set => SetValue(HighlightAlgorithmFactoryProperty, value);
        }

        public string? HighlightAlgorithmType
        {
            get => (string)GetValue(HighlightAlgorithmTypeProperty);
            set => SetValue(HighlightAlgorithmTypeProperty, value);
        }

        public AlgorithmConstraints EdgeRoutingConstraint
        {
            get => (AlgorithmConstraints)GetValue(EdgeRoutingConstraintProperty);
            set => SetValue(EdgeRoutingConstraintProperty, value);
        }

        public AlgorithmConstraints OverlapRemovalConstraint
        {
            get => (AlgorithmConstraints)GetValue(OverlapRemovalConstraintProperty);
            set => SetValue(OverlapRemovalConstraintProperty, value);
        }

        public ILayoutParameters LayoutParameters
        {
            get => (ILayoutParameters)GetValue(LayoutParametersProperty);
            set => SetValue(LayoutParametersProperty, value);
        }

        public IEdgeRoutingParameters EdgeRoutingParameters
        {
            get => (IEdgeRoutingParameters)GetValue(EdgeRoutingParametersProperty);
            set => SetValue(EdgeRoutingParametersProperty, value);
        }

        public IOverlapRemovalParameters OverlapRemovalParameters
        {
            get => (IOverlapRemovalParameters)GetValue(OverlapRemovalParametersProperty);
            set => SetValue(OverlapRemovalParametersProperty, value);
        }

        public IHighlightParameters HighlightParameters
        {
            get => (IHighlightParameters)GetValue(HighlightParametersProperty);
            set => SetValue(HighlightParametersProperty, value);
        }

        /// <summary>
        ///     The graph we want to show. Dependency Property.
        /// </summary>
        public TGraph Graph
        {
            get => (TGraph)GetValue(GraphProperty);
            set => SetValue(GraphProperty, value);
        }

        #endregion
    }
}