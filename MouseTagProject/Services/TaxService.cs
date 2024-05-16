using MouseTagProject.DTOs;

namespace MouseTagProject.Services

{
    public class TaxService
    {
        public async Task<string> Get(TaxInputDto TaxInputDto)
        {

            var url = "https://www.tax.lt/skaiciuokles/skaiciuoti_atlyginima?random_token=0ade7c2cf97f75d009975f4d720d1fa6c19f4897&mokestiniai_metai=2022&koks_atl=1&atlyginimas=2000&paskaiciuoti_npd=1&vaikai=0&kaip=1&taikomas_npd=0&papildomas_pensijos_kaupimas=0&papildomas_pensijos_kaupimas_procentai=0.027";

            url = url.Replace("atlyginimas=2000", "atlyginimas=" + TaxInputDto.salary);
            url = url.Replace("mokestiniai_metai=2022", "mokestiniai_metai=" + TaxInputDto.year);
            url = url.Replace("koks_atl=1", "koks_atl=" + TaxInputDto.salary_type);
            url = url.Replace("paskaiciuoti_npd=1", "paskaiciuoti_npd=" + TaxInputDto.npd);
            url = url.Replace("taikomas_npd=0", "taikomas_npd=" + TaxInputDto.npd_pats);
            url = url.Replace("papildomas_pensijos_kaupimas=0", "papildomas_pensijos_kaupimas=" + TaxInputDto.pensija_papil);
            url = url.Replace("papildomas_pensijos_kaupimas_procentai=0.027", "papildomas_pensijos_kaupimas_procentai=" + TaxInputDto.pensija_kiekis);

            using var client = new HttpClient();

            var response = await client.PostAsync(url, null);

            string result = response.Content.ReadAsStringAsync().Result;

            return result;
        }
    }
}
