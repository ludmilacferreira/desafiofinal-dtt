using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using ApiMina.Controllers;
using ApiMina.Dtos;
using DesafioFinal.Data;
using DesafioFinal.Models;
using System;
using System.Threading.Tasks;

namespace ApiMina.Tests
{
    public class EquipamentosControllerTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task Create_DeveCriarEquipamento_QuandoDadosValidos()
        {
            // Arrange
            var context = GetDbContext();
            var controller = new EquipamentosController(context);

            var dto = new CreateEquipamentoDto
            {
                Codigo = "EQ001",
                Tipo = "Caminhao",
                Modelo = "Modelo X",
                Horimetro = 100,
                StatusOperacional = "Operacional",
                DataAquisicao = DateTime.UtcNow,
                LocalizacaoAtual = "Mina A"
            };

            // Act
            var result = await controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(1, await context.Equipamentos.CountAsync());
        }

        [Fact]
        public async Task Create_DeveRetornarConflict_QuandoCodigoDuplicado()
        {
            var context = GetDbContext();

            context.Equipamentos.Add(new Equipamento
            {
                Codigo = "EQ001",
                Tipo = TipoEquipamento.Caminhao,
                Modelo = "Modelo X",
                Horimetro = 100,
                StatusOperacional = StatusOperacional.Operacional,
                DataAquisicao = DateTime.UtcNow,
                LocalizacaoAtual = "Mina A"
            });

            await context.SaveChangesAsync();

            var controller = new EquipamentosController(context);

            var dto = new CreateEquipamentoDto
            {
                Codigo = "EQ001",
                Tipo = "Caminhao",
                Modelo = "Outro Modelo",
                Horimetro = 50,
                StatusOperacional = "Operacional"
            };

            var result = await controller.Create(dto);

            Assert.IsType<ConflictObjectResult>(result);
        }

        [Fact]
        public async Task GetById_DeveRetornarOk_QuandoExiste()
        {
            var context = GetDbContext();

            var equipamento = new Equipamento
            {
                Codigo = "EQ002",
                Tipo = TipoEquipamento.Trator,
                Modelo = "Modelo T",
                Horimetro = 200,
                StatusOperacional = StatusOperacional.Operacional,
                DataAquisicao = DateTime.UtcNow,
                LocalizacaoAtual = "Mina B"
            };

            context.Equipamentos.Add(equipamento);
            await context.SaveChangesAsync();

            var controller = new EquipamentosController(context);

            var result = await controller.GetById(equipamento.Id);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_DeveRetornarNotFound_QuandoNaoExiste()
        {
            var context = GetDbContext();
            var controller = new EquipamentosController(context);

            var result = await controller.GetById(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Delete_DeveRemoverEquipamento_QuandoExiste()
        {
            var context = GetDbContext();

            var equipamento = new Equipamento
            {
                Codigo = "EQ003",
                Tipo = TipoEquipamento.Escavadeira,
                Modelo = "Modelo Z",
                Horimetro = 300,
                StatusOperacional = StatusOperacional.Operacional,
                DataAquisicao = DateTime.UtcNow,
                LocalizacaoAtual = "Mina C"
            };

            context.Equipamentos.Add(equipamento);
            await context.SaveChangesAsync();

            var controller = new EquipamentosController(context);

            var result = await controller.Delete(equipamento.Id);

            Assert.IsType<NoContentResult>(result);
            Assert.Equal(0, await context.Equipamentos.CountAsync());
        }
    }
}