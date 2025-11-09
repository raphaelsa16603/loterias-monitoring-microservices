namespace Loterias.CollectorDailyService.Config
{
    public class CollectorSettings
    {
        /// <summary>
        /// Quantas vezes por dia o collector deve rodar (para Hangfire futuramente)
        /// </summary>
        public int ExecucoesDiarias { get; set; } = 3;

        /// <summary>
        /// Todos os tipos de jogos monitorados.
        /// </summary>
        public List<string> TiposDeJogo { get; set; } = new()
        {
            "megasena",
            "lotofacil",
            "quina",
            "lotomania",
            "timemania",
            "diadesorte",
            "duplasena",
            "supersete",
            "maismilionaria",
            "federal",
            "loteca"
        };

        /// <summary>
        /// URLs das APIs auxiliares
        /// </summary>
        public ApiEndpoints Endpoints { get; set; } = new();

        public class ApiEndpoints
        {
            public string CaixaApi { get; set; } = "http://loterias-caixa-api:8080/";
            public string QueryApi { get; set; } = "http://loterias-query-api:8080/";
        }
    }
}
