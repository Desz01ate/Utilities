using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Asp.Core.Based
{
    /// <summary>
    /// A controller with SignalR context integrated.
    /// </summary>
    /// <typeparam name="THub"></typeparam>
    [ApiController]
    public class SignalrController<THub> : ControllerBase
        where THub : Hub
    {
        /// <summary>
        /// SignalR hub context.
        /// </summary>
        protected readonly IHubContext<THub> _Hub;
        public SignalrController(IHubContext<THub> hub)
        {
            _Hub = hub;
        }
    }
    /// <summary>
    /// A controller with SignalR context integrated.
    /// </summary>
    /// <typeparam name="THub"></typeparam>
    [ApiController]
    public class SignalrController<THub1, THub2> : ControllerBase
        where THub1 : Hub
        where THub2 : Hub
    {
        /// <summary>
        /// SignalR hub context.
        /// </summary>
        protected readonly IHubContext<THub1> _Hub1;
        /// <summary>
        /// SignalR hub context.
        /// </summary>
        protected readonly IHubContext<THub2> _Hub2;
        public SignalrController(IHubContext<THub1> hub1, IHubContext<THub2> hub2)
        {
            _Hub1 = hub1;
            _Hub2 = hub2;
        }
    }
    /// <summary>
    /// A controller with SignalR context integrated.
    /// </summary>
    /// <typeparam name="THub"></typeparam>
    [ApiController]
    public class SignalrController<THub1, THub2, THub3> : ControllerBase
        where THub1 : Hub
        where THub2 : Hub
        where THub3 : Hub
    {
        /// <summary>
        /// SignalR hub context.
        /// </summary>
        protected readonly IHubContext<THub1> _Hub1;
        /// <summary>
        /// SignalR hub context.
        /// </summary>
        protected readonly IHubContext<THub2> _Hub2;
        /// <summary>
        /// SignalR hub context.
        /// </summary>
        protected readonly IHubContext<THub3> _Hub3;
        public SignalrController(IHubContext<THub1> hub1, IHubContext<THub2> hub2, IHubContext<THub3> hub3)
        {
            _Hub1 = hub1;
            _Hub2 = hub2;
            _Hub3 = hub3;
        }
    }
}
