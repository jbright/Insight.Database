﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;

namespace Insight.Database.Providers
{
	/// <summary>
	/// Implements the Insight provider for Oracle ODP.NET connections.
	/// </summary>
	public class OracleInsightDbProvider : InsightDbProvider
	{
		/// <summary>
		/// Gets the type for the DbCommands supported by this provider.
		/// </summary>
		public override Type CommandType
		{
			get
			{
				return typeof(OracleCommand);
			}
		}

		/// <summary>
		/// Gets the type for ConnectionStringBuilders supported by this provider.
		/// </summary>
		public override Type ConnectionStringBuilderType
		{
			get
			{
				return typeof(OracleConnectionStringBuilder);
			}
		}

		/// <summary>
		/// Creates a new DbConnection supported by this provider.
		/// </summary>
		/// <returns>A new DbConnection.</returns>
		public override DbConnection CreateDbConnection()
		{
			return new OracleConnection();
		}

		/// <summary>
		/// Derives the parameter list for a given command.
		/// </summary>
		/// <param name="command">The command to use.</param>
		/// <returns>The list of parameters for the command.</returns>
		public override IList<IDbDataParameter> DeriveParameters(IDbCommand command)
		{
			if (command == null) throw new ArgumentNullException("command");

			var connection = command.Connection;
			bool autoClose = false;

			try
			{
				if (connection.State != ConnectionState.Open)
				{
					autoClose = true;
					connection.Open();
				}

				OracleCommandBuilder.DeriveParameters(command as OracleCommand);
			}
			finally
			{
				if (autoClose)
					connection.Close();
			}

			// make the list of parameters
			List<IDbDataParameter> parameters = command.Parameters.Cast<IDbDataParameter>().ToList();

			// clear the list so we can re-add them
			command.Parameters.Clear();

			return parameters;
		}
	}
}
