﻿using Magellan.Events;
using Magellan.Mvc;

namespace Magellan.Routing
{
    /// <summary>
    /// A <see cref="IRouteHandler"/> for Model-View-Controller routes.
    /// </summary>
    public class ControllerRouteHandler : IRouteHandler
    {
        private readonly IControllerFactory _controllerFactory;
        private readonly ModelBinderDictionary _modelBinders;
        private readonly ViewEngineCollection _viewEngines;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerRouteHandler"/> class.
        /// </summary>
        /// <param name="controllerFactory">The controller factory.</param>
        /// <param name="viewEngines">The view engines.</param>
        /// <param name="modelBinders">The model binders.</param>
        public ControllerRouteHandler(IControllerFactory controllerFactory, ViewEngineCollection viewEngines, ModelBinderDictionary modelBinders)
        {
            _controllerFactory = controllerFactory;
            _modelBinders = modelBinders;
            _viewEngines = viewEngines ?? ViewEngines.CreateDefaults();
        }

        /// <summary>
        /// Processes the navigation request.
        /// </summary>
        /// <param name="request">The navigation request information.</param>
        public void ProcessRequest(ResolvedNavigationRequest request)
        {
            var controllerName = request.RouteValues.GetOrDefault<string>("controller");
            
            request.ReportProgress(new BeginRequestNavigationEvent());
            request.ReportProgress(new ResolvingControllerNavigationEvent());
            var controllerLease = _controllerFactory.CreateController(request, controllerName);
            var controller = controllerLease.Controller;

            controller.Execute(
                new ControllerContext(
                    controllerLease.Controller, 
                    request, 
                    _viewEngines,
                    _modelBinders,
                    () =>
                        {
                            controllerLease.Dispose();
                            request.ReportProgress(new CompleteNavigationEvent());
                        }));
        }
    }
}
