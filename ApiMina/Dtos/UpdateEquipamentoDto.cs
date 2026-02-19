namespace ApiMina.Dtos
{
    public class UpdateEquipamentoDto
    {
        public string Codigo { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public decimal Horimetro { get; set; }
        public string StatusOperacional { get; set; } = string.Empty;
        public DateTime? DataAquisicao { get; set; }
        public string LocalizacaoAtual { get; set; } = string.Empty;
    }
}
