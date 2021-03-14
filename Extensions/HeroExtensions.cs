namespace Divine.SDK.Extensions
{
    public static class HeroExtensions
    {
        public static string GetDisplayName(this Hero hero)
        {
            var networkName = hero.NetworkName;
            return networkName switch
            {
                "CDOTA_Unit_Hero_DoomBringer" => "Doom",
                "CDOTA_Unit_Hero_Furion" => "Nature's Prophet",
                "CDOTA_Unit_Hero_Magnataur" => "Magnus",
                "CDOTA_Unit_Hero_Necrolyte" => "Necrophos",
                "CDOTA_Unit_Hero_Nevermore" => "ShadowFiend",
                "CDOTA_Unit_Hero_Obsidian_Destroyer" => "OutworldDevourer",
                "CDOTA_Unit_Hero_Rattletrap" => "Clockwerk",
                "CDOTA_Unit_Hero_Shredder" => "Timbersaw",
                "CDOTA_Unit_Hero_SkeletonKing" => "WraithKing",
                "CDOTA_Unit_Hero_Wisp" => "Io",
                "CDOTA_Unit_Hero_Zuus" => "Zeus",
                _ => networkName.Substring("CDOTA_Unit_Hero_".Length).Replace("_", string.Empty),
            };
        }
    }
}