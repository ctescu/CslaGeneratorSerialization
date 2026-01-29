using Csla;
using Csla.Configuration;
using Csla.DataPortalClient;
using CslaGeneratorSerialization.IntegrationTests.ReferenceTypes.StringTestsDomain;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace CslaGeneratorSerialization.IntegrationTests.ReferenceTypes;

internal sealed class CustomProxy : DataPortalProxy
{
	public CustomProxy(ApplicationContext applicationContext)
		: base(applicationContext)
	{
	}

	public override string DataPortalUrl => "http://localhost:5000/dataportal";

	protected override Task<byte[]> CallDataPortalServer(byte[] serialized, string operation, string? routingToken, bool isSync) => Task.FromResult(serialized);
}

internal static class DataPortalTests
{
	[Test]
	public static async Task RoundtripAsync()
	{
		var provider = Shared.ServiceProvider;
		var proxy = new CustomProxy(provider.GetRequiredService<ApplicationContext>());
		var portal = provider.GetRequiredService<IDataPortal<StringData>>();
		var data = await portal.CreateAsync();

		data.Contents = "ABC";

		var result = await proxy.Update(data, new Csla.Server.DataPortalContext(provider.GetRequiredService<ApplicationContext>(),true), true);
				
		Assert.That(result.ReturnObject, Is.Not.Null);
	}
}
