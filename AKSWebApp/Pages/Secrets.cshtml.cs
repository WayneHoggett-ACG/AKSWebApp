using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace AKSWebApp.Pages;

public class SecretsModel : PageModel
{
    private readonly ILogger<SecretsModel> _logger;

    private readonly IConfiguration _configuration;

    public SecretsModel(ILogger<SecretsModel> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public string? PageTitleSuffix { get; private set; }
    public string? Secret { get; private set; }
    public string? KeyVaultUrl { get; private set; }

    public void OnGet()
    {
        PageTitleSuffix = _configuration["ASPNETCORE_ENVIRONMENT"];
    }

public async Task<IActionResult> OnPostGetSecretAsync(string secretName, string keyVaultUrl)
    {
        PageTitleSuffix = _configuration["ASPNETCORE_ENVIRONMENT"];
        
        if (ModelState.IsValid)
        {
            try
            {
                KeyVaultUrl = keyVaultUrl;

                var client = new SecretClient(
                    new Uri(keyVaultUrl),
                    new DefaultAzureCredential());

                KeyVaultSecret secret = await client.GetSecretAsync(secretName);
                Secret = secret.Value;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", "Failed to retrieve secret: " + ex.Message);
            }
        }

        return Page();
    }
}