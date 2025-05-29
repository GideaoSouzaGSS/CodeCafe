using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CodeCafe.ApiService.Features.Auth.DTOs;
using CodeCafe.Tests.Common.Fixtures;

namespace CodeCafe.Tests.Common.Helpers;

public static class AuthHelper
{
    public static async Task<AuthResponse> RegisterUserAsync(
        HttpClient client, 
        string email, 
        string password, 
        string name)
    {
        var registerData = new
        {
            Email = email,
            Password = password,
            Name = name
        };
        
        var content = new StringContent(
            JsonSerializer.Serialize(registerData),
            Encoding.UTF8,
            "application/json");
            
        var response = await client.PostAsync("/api/auth/register", content);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<AuthResponse>();
    }
    
    public static async Task<AuthResponse> LoginAsync(
        HttpClient client, 
        string email, 
        string password)
    {
        var loginData = new
        {
            Email = email,
            Password = password
        };
        
        var content = new StringContent(
            JsonSerializer.Serialize(loginData),
            Encoding.UTF8,
            "application/json");
            
        var response = await client.PostAsync("/api/auth/login", content);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<AuthResponse>();
    }
    
    public static HttpClient CreateAuthorizedClient(TestWebApplicationFactory factory, string token)
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        return client;
    }
}