using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiMina.Dtos;
using DesafioFinal.Models;

namespace ApiMina.Controllers
{
    [ApiController]
    [Route("api/equipamentos")]
    public class EquipamentosController : ControllerBase
    {
        private readonly DesafioFinal.Data.AppDbContext _db;

        public EquipamentosController(DesafioFinal.Data.AppDbContext db) => _db = db;



        private static readonly string[] TiposValidos = { "Caminhao", "Escavadeira", "Perfuratriz", "Carregadeira", "Trator" };
        private static readonly string[] StatusValidos = { "Operacional", "EmManutencao", "Parado" };

        private static bool TryParseTipo(string valor, out DesafioFinal.Models.TipoEquipamento tipo)
        {
            tipo = default;
            return valor switch
            {
                "Caminhao" => (tipo = DesafioFinal.Models.TipoEquipamento.Caminhao) == tipo,
                "Escavadeira" => (tipo = DesafioFinal.Models.TipoEquipamento.Escavadeira) == tipo,
                "Perfuratriz" => (tipo = DesafioFinal.Models.TipoEquipamento.Perfuratriz) == tipo,
                "Carregadeira" => (tipo = DesafioFinal.Models.TipoEquipamento.Carregadeira) == tipo,
                "Trator" => (tipo = DesafioFinal.Models.TipoEquipamento.Trator) == tipo,
                _ => false
            };
        }

        private static bool TryParseStatus(string valor, out DesafioFinal.Models.StatusOperacional status)
        {
            status = default;
            return valor switch
            {
                "Operacional" => (status = DesafioFinal.Models.StatusOperacional.Operacional) == status,
                "EmManutencao" => (status = DesafioFinal.Models.StatusOperacional.EmManutencao) == status,
                "Parado" => (status = DesafioFinal.Models.StatusOperacional.Parado) == status,
                _ => false
            };
        }

        private static string TipoToString(DesafioFinal.Models.TipoEquipamento tipo) => tipo switch
        {
            TipoEquipamento.Caminhao => "Caminhao",
            TipoEquipamento.Escavadeira => "Escavadeira",
            TipoEquipamento.Perfuratriz => "Perfuratriz",
            TipoEquipamento.Carregadeira => "Carregadeira",
            TipoEquipamento.Trator => "Trator",
            _ => tipo.ToString()
        };

        private static string StatusToString(StatusOperacional status) => status switch
        {
            StatusOperacional.Operacional => "Operacional",
            StatusOperacional.EmManutencao => "EmManutencao",
            StatusOperacional.Parado => "Parado",
            _ => status.ToString()
        };

        private static EquipamentoResponseDto ToResponseDto(Equipamento e) => new(
            e.Id,
            e.Codigo,
            TipoToString(e.Tipo),
            e.Modelo,
            e.Horimetro,
            StatusToString(e.StatusOperacional),
            e.DataAquisicao,
            e.LocalizacaoAtual
        );


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEquipamentoDto input)
        {
  
            if (string.IsNullOrWhiteSpace(input.Codigo))
                return BadRequest(new { message = "Codigo e obrigatorio." });

            input.Codigo = input.Codigo.Trim();

            if (string.IsNullOrWhiteSpace(input.Modelo))
                return BadRequest(new { message = "Modelo e obrigatorio." });

            if (string.IsNullOrWhiteSpace(input.Tipo))
                return BadRequest(new { message = "Tipo e obrigatorio." });

            if (!TryParseTipo(input.Tipo, out var tipo))
                return BadRequest(new { message = $"Tipo invalido. Valores aceitos: {string.Join(", ", TiposValidos)}" });

            if (string.IsNullOrWhiteSpace(input.StatusOperacional))
                return BadRequest(new { message = "StatusOperacional e obrigatorio." });

            if (!TryParseStatus(input.StatusOperacional, out var status))
                return BadRequest(new { message = $"StatusOperacional invalido. Valores aceitos: {string.Join(", ", StatusValidos)}" });

            if (input.Horimetro < 0)
                return BadRequest(new { message = "Horimetro nao pode ser negativo." });

        
            var exists = await _db.Equipamentos.AnyAsync(x => x.Codigo == input.Codigo);
            if (exists)
                return Conflict(new { message = $"Ja existe um equipamento com o Codigo '{input.Codigo}'." });

            var equipamento = new Equipamento
            {
                Codigo = input.Codigo,
                Tipo = tipo,
                Modelo = input.Modelo.Trim(),
                Horimetro = input.Horimetro,
                StatusOperacional = status,
                DataAquisicao = input.DataAquisicao ?? DateTime.UtcNow,
                LocalizacaoAtual = input.LocalizacaoAtual?.Trim() ?? string.Empty
            };

            _db.Equipamentos.Add(equipamento);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = equipamento.Id }, ToResponseDto(equipamento));
        }

 
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? tipo = null,
            [FromQuery] string? status = null,
            [FromQuery] string? codigo = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 50) pageSize = 50;

            var query = _db.Equipamentos.AsQueryable();


            if (!string.IsNullOrWhiteSpace(tipo))
            {
                if (TryParseTipo(tipo, out var tipoEnum))
                    query = query.Where(x => x.Tipo == tipoEnum);
                else
                    return BadRequest(new { message = $"Filtro 'tipo' invalido. Valores aceitos: {string.Join(", ", TiposValidos)}" });
            }


            if (!string.IsNullOrWhiteSpace(status))
            {
                if (TryParseStatus(status, out var statusEnum))
                    query = query.Where(x => x.StatusOperacional == statusEnum);
                else
                    return BadRequest(new { message = $"Filtro 'status' invalido. Valores aceitos: {string.Join(", ", StatusValidos)}" });
            }

         
            if (!string.IsNullOrWhiteSpace(codigo))
                query = query.Where(x => x.Codigo.ToLower().Contains(codigo.ToLower()));

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await query
                .OrderBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PageResultDto<EquipamentoResponseDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Items = items.Select(ToResponseDto).ToList()
            };

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var equipamento = await _db.Equipamentos.FindAsync(id);

            if (equipamento == null)
                return NotFound(new { message = "Equipamento nao encontrado." });

            return Ok(ToResponseDto(equipamento));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEquipamentoDto input)
        {
           
            if (string.IsNullOrWhiteSpace(input.Codigo))
                return BadRequest(new { message = "Codigo e obrigatorio." });

            input.Codigo = input.Codigo.Trim();

            if (string.IsNullOrWhiteSpace(input.Modelo))
                return BadRequest(new { message = "Modelo e obrigatorio." });

            if (!TryParseTipo(input.Tipo, out var tipo))
                return BadRequest(new { message = $"Tipo invalido. Valores aceitos: {string.Join(", ", TiposValidos)}" });

            if (!TryParseStatus(input.StatusOperacional, out var status))
                return BadRequest(new { message = $"StatusOperacional invalido. Valores aceitos: {string.Join(", ", StatusValidos)}" });

            if (input.Horimetro < 0)
                return BadRequest(new { message = "Horimetro nao pode ser negativo." });

         
            var equipamento = await _db.Equipamentos.FindAsync(id);
            if (equipamento == null)
                return NotFound(new { message = "Equipamento nao encontrado para atualizacao." });

     
            if (equipamento.Codigo != input.Codigo)
            {
                var codigoJaExiste = await _db.Equipamentos.AnyAsync(x => x.Codigo == input.Codigo);
                if (codigoJaExiste)
                    return Conflict(new { message = $"Ja existe outro equipamento com o codigo '{input.Codigo}'." });
            }


            equipamento.Codigo = input.Codigo;
            equipamento.Tipo = tipo;
            equipamento.Modelo = input.Modelo.Trim();
            equipamento.Horimetro = input.Horimetro;
            equipamento.StatusOperacional = status;
            equipamento.LocalizacaoAtual = input.LocalizacaoAtual?.Trim() ?? string.Empty;

            if (input.DataAquisicao.HasValue)
                equipamento.DataAquisicao = input.DataAquisicao.Value;

            await _db.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
    
            var equipamento = await _db.Equipamentos.FindAsync(id);
            if (equipamento == null)
                return NotFound(new { message = "Equipamento nao encontrado para exclusao." });


            _db.Equipamentos.Remove(equipamento);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
