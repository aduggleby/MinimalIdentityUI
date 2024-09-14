using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.RateLimiting;

/// <summary>
/// Allows setting rate limiting policy on pages, folders and areas similar 
/// to the Authorize* extension methods.
/// 
/// Note this will only work if you endpoint routing is enabled. 
/// 
/// Order of pipeline execution is important:
/// Routing > Rate Limiter > MapRazorPages
/// 
/// Example pipeline:
/// app.UseRouting();
/// app.UseForwardedHeaders(); // Only necessary if behind a reverse proxy
/// app.UseRateLimiter();
/// app.UseAuthorization();
/// app.MapRazorPages();
/// 
/// Example usage:
/// builder.Services.AddRazorPages(options =>
/// {
///    options.Conventions.RateLimitPage("/Page", "policy");
///    options.Conventions.RateLimitFolder("/Folder", "policy");
///    options.Conventions.RateLimitAreaPage("Area", "/Page", "policy");
///    options.Conventions.RateLimitAreaFolder("Area", "/Folder", "policy");
/// });
/// </summary>
public static class RateLimitingPageConventionCollectionExtensions
{
	public static PageConventionCollection RateLimitPage(
		this PageConventionCollection conventions,
		string pageName,
		string policy)
	{
		ArgumentNullException.ThrowIfNull(conventions);

		if (string.IsNullOrEmpty(pageName))
		{
			throw new ArgumentException(nameof(pageName));
		}

		conventions.AddPageApplicationModelConvention(pageName, model =>
		{
			model.EndpointMetadata.Add(new EnableRateLimitingAttribute(policy));
		});
		return conventions;
	}

	public static PageConventionCollection RateLimitAreaPage(
		this PageConventionCollection conventions,
		string areaName,
		string pageName,
		string policy)
	{
		ArgumentNullException.ThrowIfNull(conventions);

		if (string.IsNullOrEmpty(areaName))
		{
			throw new ArgumentException(nameof(areaName));
		}

		if (string.IsNullOrEmpty(pageName))
		{
			throw new ArgumentException(nameof(pageName));
		}

		conventions.AddAreaPageApplicationModelConvention(areaName, pageName, model =>
		{
			model.EndpointMetadata.Add(new EnableRateLimitingAttribute(policy));
		});
		return conventions;
	}

	public static PageConventionCollection RateLimitFolder(
		this PageConventionCollection conventions,
		string pageName,
		string policy)
	{
		ArgumentNullException.ThrowIfNull(conventions);

		if (string.IsNullOrEmpty(pageName))
		{
			throw new ArgumentException(nameof(pageName));
		}

		conventions.AddFolderApplicationModelConvention(pageName, model =>
		{
			model.EndpointMetadata.Add(new EnableRateLimitingAttribute(policy));
		});

		return conventions;
	}

	public static PageConventionCollection RateLimitAreaFolder(
		this PageConventionCollection conventions,
		string areaName,
		string folderPath,
		string policy)
	{
		ArgumentNullException.ThrowIfNull(conventions);

		if (string.IsNullOrEmpty(areaName))
		{
			throw new ArgumentException(nameof(areaName));
		}

		if (string.IsNullOrEmpty(folderPath))
		{
			throw new ArgumentException(nameof(folderPath));
		}

		conventions.AddAreaFolderApplicationModelConvention(areaName, folderPath, model =>
		{
			model.EndpointMetadata.Add(new EnableRateLimitingAttribute(policy));
		});
		return conventions;
	}
}

