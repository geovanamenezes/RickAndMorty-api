
using RickAndMortyApi.DTOs;
namespace RequestRickAndMorty.Ports;

public static class RequestRickAndMortyApi
{
    private static readonly HttpClient httpClient = new();

    private const string ApiUrl = "https://rickandmortyapi.com/api";
    private const string CharacterPath = "character";
    private const string EpisodePath = "episode";
    private const string LocationPath = "location";

    public static async Task<CharacterTo?> BuscarPersonagem(int personagemId)
    {
        return await httpClient.GetFromJsonAsync<CharacterTo>($"{ApiUrl}/{CharacterPath}/{personagemId}");
    }

    public static async Task<EpisodeTo?> BuscarEpisodio(int episodioId)
    {
        return await httpClient.GetFromJsonAsync<EpisodeTo>($"{ApiUrl}/{EpisodePath}/{episodioId}");
    }

    public static async Task<LocationTo?> BuscarLocalizacao(int localizacaoId)
    {
        return await httpClient.GetFromJsonAsync<LocationTo>($"{ApiUrl}/{LocationPath}/{localizacaoId}");
    }

    public static async Task<LocationTo?> BuscarLocalizacaoPorUrl(string url)
    {
        return await httpClient.GetFromJsonAsync<LocationTo>(url);
    }
}
