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
		protected event SnmpAsyncResponse _response;
		/// <summary>
		/// Internal storage for request target information.
		/// </summary>
		protected ITarget _target;

		#region Constructor(s)

		/// <summary>
		/// Constructor
		/// </summary>
		public Snmp()
			:base()
		{
			_response = null;
			_target = null;
		}

		#endregion Constructor(s)
	}
}
