namespace ApiMina.Dtos
{
    public record EquipamentoResponseDto(
        int Id,
        string Codigo,
        string Tipo,
        string Modelo,
        decimal Horimetro,
        string StatusOperacional,
        DateTime DataAquisicao,
        string LocalizacaoAtual
    );
}
