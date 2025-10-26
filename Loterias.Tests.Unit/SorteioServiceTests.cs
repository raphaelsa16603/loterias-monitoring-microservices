using Xunit;
using Moq;
using Loterias.QueryApiService.Services;
using Loterias.Shared.Interfaces;
using Loterias.Shared.Models;
using System.Threading.Tasks;

namespace Loterias.Tests.Unit;

public class SorteioServiceTests
{
    [Fact]
    public async Task ObterPorNumeroAsync_DeveRetornarSorteio_QuandoExistente()
    {
        // Arrange
        var mockRepo = new Mock<ISorteioRepository>();
        mockRepo.Setup(x => x.ObterPorNumeroAsync("MEGA_SENA", 1234))
                .ReturnsAsync(new Sorteio { NumeroJogo = 1234, TipoJogo = "MEGA_SENA" });

        var service = new SorteioService(mockRepo.Object);

        // Act
        var result = await service.ObterPorNumeroAsync("MEGA_SENA", 1234);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1234, result?.NumeroJogo);
    }
}
