﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Magellan.Routing
{
    /// <summary>
    /// The default implementation of <see cref="IRouteParser"/>.
    /// </summary>
    public class RouteParser : IRouteParser
    {
        private readonly List<SegmentRecognizer> _recognisers = new List<SegmentRecognizer>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteParser"/> class.
        /// </summary>
        /// <param name="recognizers">The recognizers.</param>
        public RouteParser(params SegmentRecognizer[] recognizers)
        {
            if (recognizers != null) _recognisers.AddRange(recognizers);
        }

        /// <summary>
        /// Gets the recognizers.
        /// </summary>
        /// <value>The recognizers.</value>
        public IList<SegmentRecognizer> Recognizers
        {
            get { return _recognisers; }
        }

        public ParsedRoute Parse(IRoute route, string routeSpecification, RouteValueDictionary defaults, RouteValueDictionary constraints)
        {
            var parts = routeSpecification.SplitUrlPath();
            var segments = new Segment[parts.Length];
            for (var i = 0; i < parts.Length; i++)
            {
                var segment = _recognisers.Select(x => x.Recognise(parts[i], defaults, constraints)).FirstOrDefault(x => x != null);
                if (segment == null)
                {
                    throw new InvalidRouteException(
                        route,
                        string.Format("Invalid route: {0}. The route segment '{1}' was not recognized as a valid segment.", route, parts[i])
                        );
                }
                segments[i] = segment;
            }

            return new ParsedRoute(defaults, constraints, segments);
        }
    }
}