using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Sol_s_RNG_Biome_Detector
{
    class Auracheck
    {
        // Fuckass aura check

        public async Task<bool> CheckAura(string aura, string rarity)
        {
            rarity = rarity.Replace(",", "");

            Int64 minstat = Int64.Parse(rarity);

            switch (aura.ToUpper())
            {
                case "CHROMATIC : GENESIS":
                    {
                        Int64 stat = 99_999_999;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "STARSCOURGE : RADIANT":
                    {
                        Int64 stat = 100_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "SPECTRAFLOW":
                    {
                        Int64 stat = 100_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "LILY":
                    {
                        Int64 stat = 112_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "SHARKYN : HAMMERHEAD":
                    {
                        Int64 stat = 120_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "OVERTURE":
                    {
                        Int64 stat = 150_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "BOUNDED : AICHMALOTOS":
                    {
                        Int64 stat = 170_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "SYMPHONY":
                    {
                        Int64 stat = 175_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "TWLILIGHT : WITHERING GRACE":
                    {
                        Int64 stat = 180_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "FELLED":
                    {
                        Int64 stat = 180_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "IMPEACHED":
                    {
                        Int64 stat = 200_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "LUMENPOOL":
                    {
                        Int64 stat = 220_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "HYPER-VOLT : EVER-STORM":
                    {
                        Int64 stat = 225_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "VIRTUAL MEMORY":
                    {
                        Int64 stat = 232_232_232;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "ASTRAL : ZODIAC":
                    {
                        Int64 stat = 267_200_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "PROPHECY":
                    {
                        Int64 stat = 275_649_430;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "EXOTIC : VOID":
                    {
                        Int64 stat = 299_999_999;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "BLOODLUST":
                    {
                        Int64 stat = 300_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "OVERTURE : HISTORY":
                    {
                        Int64 stat = 300_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "MAELSTROM":
                    {
                        Int64 stat = 309_999_999;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "PERPETUAL":
                    {
                        Int64 stat = 315_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "DREAMER":
                    {
                        Int64 stat = 315_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "LOTUSFALL":
                    {
                        Int64 stat = 320_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "CYTOKINESIS":
                    {
                        Int64 stat = 330_400_472;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "JAZZ : ORCHESTRA":
                    {
                        Int64 stat = 336_870_912;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "ARCHANGEL":
                    {
                        Int64 stat = 350_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "ATLAS":
                    {
                        Int64 stat = 360_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "FLORA : EVERGREEN":
                    {
                        Int64 stat = 370_073_730;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "CHILLSEAR":
                    {
                        Int64 stat = 375_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "CELESTIAL : ECLIPSE":
                    {
                        Int64 stat = 384_400_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "ABYSSAL HUNTER":
                    {
                        Int64 stat = 400_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "GARGANTUA":
                    {
                        Int64 stat = 430_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "APOSTOLOS":
                    {
                        Int64 stat = 444_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "KYAWTHUITE : REMEMBRANCE":
                    {
                        Int64 stat = 450_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "RUINS":
                    {
                        Int64 stat = 500_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "MATRIX : OVERDRIVE":
                    {
                        Int64 stat = 503_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "SAILOR : ADMIRAL":
                    {
                        Int64 stat = 540_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "SOPHYRA":
                    {
                        Int64 stat = 570_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "MATRIX : REALITY":
                    {
                        Int64 stat = 601_020_102;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "SLOTH":
                    {
                        Int64 stat = 650_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "PYTHIOS":
                    {
                        Int64 stat = 666_666_666;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "SOVEREIGN":
                    {
                        Int64 stat = 750_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "RUINS : WITHERED":
                    {
                        Int64 stat = 800_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "AEGIS":
                    {
                        Int64 stat = 825_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "EISVEIL":
                    {
                        Int64 stat = 830_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "POSEIDON : ATLANTIS":
                    {
                        Int64 stat = 850_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "ASCENDANT":
                    {
                        Int64 stat = 935_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }

                case "PIXELATION":
                    {
                        Int64 stat = 1_073_741_824;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "LUMINOSITY":
                    {
                        Int64 stat = 1_200_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "BREAKTHROUGH":
                    {
                        Int64 stat = 1_999_999_999;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "EQUINOX":
                    {
                        Int64 stat = 2_500_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }

                case "RAVEN : PLAGUE":
                    {
                        Int64 stat = 200_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "UNKNOWN":
                    {
                        Int64 stat = 444_444_444;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "ELUDE":
                    {
                        Int64 stat = 555_555_555;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "PROLOGUE":
                    {
                        Int64 stat = 666_616_111;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "DREAMSCAPE":
                    {
                        Int64 stat = 850_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "NYCTOPHOBIA":
                    {
                        Int64 stat = 1_011_111_010;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }

                case "PROJECTION":
                    {
                        Int64 stat = 197_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "OPPRESSION":
                    {
                        Int64 stat = 220_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "DREAMMETRIC":
                    {
                        Int64 stat = 320_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "POINT : ZERO":
                    {
                        Int64 stat = 521_121_900;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "LEVIATHAN":
                    {
                        Int64 stat = 1_730_400_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "ASTRAIOS":
                    {
                        Int64 stat = 1_750_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                case "MONARCH":
                    {
                        Int64 stat = 3_000_000_000;

                        if (stat < minstat)
                        {
                            return false;
                        }
                        return true;
                    }
                default:
                    return false;
            }
        }
    }
}
