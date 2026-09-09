using System.Text.Json;

namespace Sol_s_RNG_Biome_Detector
{
    internal class Aura
    {
        public string Name { get; set; } = "";
        public long Rarity { get; set; } = 0;
        public string NativeBiome { get; set; } = "";
        public long NativeRarity { get; set; } = 0;
    }

    internal class AuraLoader
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<List<Aura>> Load()
        {
            string url = "https://raw.githubusercontent.com/vexk1x/Vortex/main/auralist.json";

            string json = await client.GetStringAsync(url);

            List<Aura>? auras = JsonSerializer.Deserialize<List<Aura>>(json);

            if (auras is null)
                return new List<Aura>();

            return auras;
        }
    }

    public class MatchAura
    {
        private List<Aura> auras = new List<Aura>();

        private volatile bool isEmpty = false;

        public async Task initAuras()
        {
            auras = await AuraLoader.Load();

            if (auras.Count == 0)
                isEmpty = true;
        }

        private Aura? Match(string equippedaura)
        {
            foreach (Aura aura in auras)
            {
                if (aura.Name.ToUpper() == equippedaura.ToUpper())
                    return aura;
            }

            return null;
        }

        private bool IsNative(string currentbiome, string NativeBiome)
        {
            if (currentbiome.ToUpper() == NativeBiome.ToUpper())
                return true;

            return false;
        }

        private bool IsGlobal(long AuraRarity)
        {
            if (AuraRarity > 99_999_999)
                return true;

            return false;
        }

        public async Task<(string Name, long Rarity, bool Global, bool bIsNative)> GetAura(string equippedaura, string currentbiome, long minrarity)
        {
            bool Global = false;
            bool bIsNative = false;

            if (isEmpty)
                return ("", 0, Global, bIsNative);

            Aura? matchedaura = Match(equippedaura);

            if (matchedaura is null)
                return ("", 0, Global, bIsNative);

            if (IsGlobal(matchedaura.Rarity))
                Global = true;

            if (IsNative(currentbiome, matchedaura.NativeBiome))
                bIsNative = true;

            if (matchedaura.Rarity < minrarity)
                return ("", 0, Global, bIsNative);

            if (bIsNative)
                return (matchedaura.Name, matchedaura.NativeRarity, Global, bIsNative);


            return (matchedaura.Name, matchedaura.Rarity, Global, bIsNative);
        }
    }
}