using System;
using System.Collections.Generic;
using System.Text;

namespace SnmpSharpNet
{
	public class Snmp:UdpTransport
	{
		/// <summary>
		/// Internal event to send result of the async request to.
		/// </summary>
		protected event SnmpAsyncResponse Response;
		/// <summary>
		/// Internal storage for request target information.
		/// </summary>
		protected ITarget target;

		#region Constructor(s)

		/// <summary>
		/// Constructor
		/// </summary>
		public Snmp()
			:base(false)
		{
			Response = null;
			target = null;
		}

		#endregion Constructor(s)
	}
}
