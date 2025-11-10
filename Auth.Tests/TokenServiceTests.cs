using Auth.Models;
using Auth.Services;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace Auth.Tests; 

public class TokenServiceTests
{
    [Fact]
    public void GerarTokenUsuarioValido()
    {
        var inMemorySettings = new Dictionary<string, string> {
            {"Jwt:Key", "Chave muito complexa para adivinhar" },
            {"Jwt:Issuer", "http://localhost" },
            {"Jwt:Audience", "TestUsers" }
        };
        IConfiguration fakeConfiguration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var tokenService = new TokenService(fakeConfiguration);
        var user = new User { Id = 1, Username = "testuser", Password = "password" };

        var tokenString = tokenService.GenerateToken(user);

        Assert.NotNull(tokenString);
        Assert.NotEmpty(tokenString);

        var handler = new JwtSecurityTokenHandler();
        var decodedToken = handler.ReadJwtToken(tokenString);

        Assert.Equal(user.Username, decodedToken.Subject);
        Assert.Equal("http://localhost", decodedToken.Issuer);



    }

}

