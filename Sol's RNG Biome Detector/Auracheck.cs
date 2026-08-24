using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using static System.Windows.Forms.AxHost;

namespace Sol_s_RNG_Biome_Detector
{
    class Auracheck
    {
        public static bool countstats = false;

        // Fuckass aura check, it will be done more efficient and just better in the future, but I just can't be bothered rn since idk if i'll keep this.

        public async Task<(bool ping, Int64 stat)> CheckAura(string aura, string rarity)
        {
            rarity = rarity.Replace(",", "");

            Int64 minstat = Int64.Parse(rarity);
            countstats = false;

            switch (aura.ToUpper())
            {
                case "CHROMATIC_GENESIS":
                    {
                        Int64 stat = 99_999_999;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "STARSCOURGE_RADIANT":
                    {
                        Int64 stat = 100_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "SPECTRAFLOW":
                    {
                        Int64 stat = 100_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "LILY":
                    {
                        Int64 stat = 112_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "SHARKYN_HAMMERHEAD":
                    {
                        Int64 stat = 120_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "OVERTURE":
                    {
                        Int64 stat = 150_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "BOUNDED_AICHMALOTOS":
                    {
                        Int64 stat = 170_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "SYMPHONY":
                    {
                        Int64 stat = 175_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "TWILIGHT_WITHERING GRACE":
                    {
                        Int64 stat = 180_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "FELLED":
                    {
                        Int64 stat = 180_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "IMPEACHED":
                    {
                        Int64 stat = 200_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "LUMENPOOL":
                    {
                        Int64 stat = 220_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "HYPER-VOLT_EVER-STORM":
                    {
                        Int64 stat = 225_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "VIRTUAL MEMORY":
                    {
                        Int64 stat = 232_232_232;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "ASTRAL_ZODIAC":
                    {
                        Int64 stat = 267_200_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "PROPHECY":
                    {
                        Int64 stat = 275_649_430;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "EXOTIC_VOID":
                    {
                        Int64 stat = 299_999_999;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "BLOODLUST":
                    {
                        Int64 stat = 300_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "OVERTURE_HISTORY":
                    {
                        Int64 stat = 300_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "MAELSTROM":
                    {
                        Int64 stat = 309_999_999;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "PERPETUAL":
                    {
                        Int64 stat = 315_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "DREAMER":
                    {
                        Int64 stat = 315_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "LOTUSFALL":
                    {
                        Int64 stat = 320_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "CYTOKINESIS":
                    {
                        Int64 stat = 330_400_472;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "JAZZ_ORCHESTRA":
                    {
                        Int64 stat = 336_870_912;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "ARCHANGEL":
                    {
                        Int64 stat = 350_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "ATLAS":
                    {
                        Int64 stat = 360_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "FLORA_EVERGREEN":
                    {
                        Int64 stat = 370_073_730;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "CHILLSEAR":
                    {
                        Int64 stat = 375_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "CELESTIAL_ECLIPSE":
                    {
                        Int64 stat = 384_400_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "ABYSSAL HUNTER":
                    {
                        Int64 stat = 400_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "GARGANTUA":
                    {
                        Int64 stat = 430_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "APOSTOLOS":
                    {
                        Int64 stat = 444_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "KYAWTHUITE_REMEMBRANCE":
                    {
                        Int64 stat = 450_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "RUINS":
                    {
                        Int64 stat = 500_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "MATRIX_OVERDRIVE":
                    {
                        Int64 stat = 503_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "SAILOR_ADMIRAL":
                    {
                        Int64 stat = 540_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "SOPHYRA":
                    {
                        Int64 stat = 570_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "MATRIX_REALITY":
                    {
                        Int64 stat = 601_020_102;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "SLOTH":
                    {
                        Int64 stat = 650_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "PYTHIOS":
                    {
                        Int64 stat = 666_666_666;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "SOVEREIGN":
                    {
                        Int64 stat = 750_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "RUINS_WITHERED":
                    {
                        Int64 stat = 800_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "AEGIS":
                    {
                        Int64 stat = 825_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "EISVEIL":
                    {
                        Int64 stat = 830_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "POSEIDON_ATLANTIS":
                    {
                        Int64 stat = 850_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "ASCENDANT":
                    {
                        Int64 stat = 935_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }

                case "PIXELATION":
                    {
                        Int64 stat = 1_073_741_824;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "LUMINOSITY":
                    {
                        Int64 stat = 1_200_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "BREAKTHROUGH":
                    {
                        Int64 stat = 1_999_999_999;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "EQUINOX":
                    {
                        Int64 stat = 2_500_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }

                case "RAVEN_PLAGUE":
                    {
                        Int64 stat = 200_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "UNKNOWN":
                    {
                        Int64 stat = 444_444_444;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "ELUDE":
                    {
                        Int64 stat = 555_555_555;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "PROLOGUE":
                    {
                        Int64 stat = 666_616_111;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "DREAMSCAPE":
                    {
                        Int64 stat = 850_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "NYCTOPHOBIA":
                    {
                        Int64 stat = 1_011_111_010;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }

                case "PROJECTION":
                    {
                        Int64 stat = 197_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "OPPRESSION":
                    {
                        Int64 stat = 220_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "DREAMMETRIC":
                    {
                        Int64 stat = 320_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "POINT_ZERO":
                    {
                        Int64 stat = 521_121_900;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "LEVIATHAN":
                    {
                        Int64 stat = 1_730_400_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "ASTRAIOS":
                    {
                        Int64 stat = 1_750_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                case "MONARCH":
                    {
                        Int64 stat = 3_000_000_000;

                        countstats = true;

                        if (stat < minstat)
                        {
                            return (false, stat);
                        }
                        return (true, stat);
                    }
                default:
                    countstats = false;
                    return (false, -1);
            }
        }
    }
}
