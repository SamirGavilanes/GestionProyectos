namespace GestionProyectos.Server.Extensions;



public static class StatusBadgeHelper

{

    /// <summary>Base para badges fuera de tablas (cabeceras, formularios).</summary>

    public const string BadgeBase =

        "inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium leading-tight ";



    /// <summary>Base ultra-compacta para badges en celdas de tabla.</summary>

    public const string TableBadgeBase =

        "inline-flex max-w-full items-center truncate rounded px-1 py-0 text-xs font-medium leading-none ";



    public static string GetClasses(string? colorKey) =>

        BadgeBase + GetColorClasses(colorKey);



    public static string GetTableClasses(string? colorKey) =>

        TableBadgeBase + GetColorClasses(colorKey);



    public static string GetKanbanColumnClasses(string? colorKey) =>

        GetKanbanPalette(colorKey).Column;



    public static string GetKanbanHeaderClasses(string? colorKey) =>

        GetKanbanPalette(colorKey).Header;



    public static string GetKanbanCountClasses(string? colorKey) =>

        GetKanbanPalette(colorKey).Count;



    private static string GetColorClasses(string? colorKey) =>

        colorKey?.Trim().ToLowerInvariant() switch

        {

            "green" => "bg-green-100 text-green-800",

            "blue" => "bg-blue-100 text-blue-800",

            "amber" => "bg-amber-100 text-amber-800",

            "red" => "bg-red-100 text-red-800",

            "violet" => "bg-violet-100 text-violet-800",

            "purple" => "bg-purple-100 text-purple-800",

            "orange" => "bg-orange-100 text-orange-800",

            "emerald" => "bg-emerald-100 text-emerald-800",

            "gray" => "bg-gray-100 text-gray-700",

            _ => "bg-primary-50 text-primary-800"

        };



    private static (string Column, string Header, string Count) GetKanbanPalette(string? colorKey) =>

        colorKey?.Trim().ToLowerInvariant() switch

        {

            "green" => ("border-green-200 bg-green-50", "border-green-200 bg-green-100 text-green-800", "bg-white text-green-700"),

            "blue" => ("border-blue-200 bg-blue-50", "border-blue-200 bg-blue-100 text-blue-800", "bg-white text-blue-700"),

            "amber" => ("border-amber-200 bg-amber-50", "border-amber-200 bg-amber-100 text-amber-800", "bg-white text-amber-700"),

            "red" => ("border-red-200 bg-red-50", "border-red-200 bg-red-100 text-red-800", "bg-white text-red-700"),

            "violet" => ("border-violet-200 bg-violet-50", "border-violet-200 bg-violet-100 text-violet-800", "bg-white text-violet-700"),

            "purple" => ("border-purple-200 bg-purple-50", "border-purple-200 bg-purple-100 text-purple-800", "bg-white text-purple-700"),

            "orange" => ("border-orange-200 bg-orange-50", "border-orange-200 bg-orange-100 text-orange-800", "bg-white text-orange-700"),

            "emerald" => ("border-emerald-200 bg-emerald-50", "border-emerald-200 bg-emerald-100 text-emerald-800", "bg-white text-emerald-700"),

            "gray" => ("border-gray-200 bg-gray-50", "border-gray-200 bg-gray-100 text-gray-700", "bg-white text-gray-600"),

            _ => ("border-primary-200 bg-primary-50", "border-primary-200 bg-primary-100 text-primary-800", "bg-white text-primary-700")

        };



    public static readonly (string Key, string Label)[] ColorOptions =

    {

        ("gray", "Gris"),

        ("blue", "Azul"),

        ("green", "Verde"),

        ("amber", "Ámbar"),

        ("red", "Rojo"),

        ("orange", "Naranja"),

        ("violet", "Violeta"),

        ("purple", "Púrpura"),

        ("emerald", "Esmeralda")

    };

}


