using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DesafioFinal.Models
{
    // 1. Força o EF a usar o nome da tabela em minúsculo, como no seu SQL
    [Table("equipamentos", Schema = "public")] 
    public class Equipamento
    {
        [Key]
        [Column("id")] // Mapeia para a coluna 'id' do SQL
        public int Id { get; set; }

        [Column("codigo")]
        public string Codigo { get; set; } = string.Empty;

        [Column("tipo")]
        public TipoEquipamento Tipo { get; set; }

        [Column("modelo")]
        public string Modelo { get; set; } = string.Empty;

        [Column("horimetro")]
        public decimal Horimetro { get; set; }

        // 2. Importante: O nome no SQL é status_operacional (snake_case)
        [Column("status_operacional")] 
        public StatusOperacional StatusOperacional { get; set; }

        [Column("data_aquisicao")]
        public DateTime DataAquisicao { get; set; }

        [Column("localizacao_atual")]
        public string LocalizacaoAtual { get; set; } = string.Empty;
    }
}