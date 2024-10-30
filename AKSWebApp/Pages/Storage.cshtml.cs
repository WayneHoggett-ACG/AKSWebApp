using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AKSWebApp.Pages;

public class StorageModel : PageModel
{
    private readonly ILogger<StorageModel> _logger;

    private readonly IConfiguration _configuration;

    public StorageModel(ILogger<StorageModel> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public string? PageTitleSuffix { get; private set; }
    public List<string>? Blobs { get; private set; }
    public string? StorageAccountName { get; private set; }
    public string? BlobContainer { get; private set; }

    public void OnGet()
    {
        PageTitleSuffix = _configuration["ASPNETCORE_ENVIRONMENT"];
        Blobs = new List<string>();
    }

    public async Task<IActionResult> OnPostGetBlobsAsync(string storageAccountName, string blobContainer)
    {
        if (ModelState.IsValid)
        {
            try
            {
                BlobServiceClient client = new(
                    new Uri($"https://{storageAccountName}.blob.core.windows.net"),
                    new DefaultAzureCredential());

                var containerClient = client.GetBlobContainerClient(blobContainer);

                var blobs = new List<string>();

                await foreach (var blob in containerClient.GetBlobsAsync())
                {
                    blobs.Add(blob.Name);
                }

                Blobs = blobs;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", "Failed to retrieve blobs: " + ex.Message);
            }
        }

        return Page();
    }
}