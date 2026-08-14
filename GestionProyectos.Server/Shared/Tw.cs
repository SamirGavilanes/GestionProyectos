namespace GestionProyectos.Server.Shared;

/// <summary>Clases Tailwind: tipografía uniforme en text-xs; jerarquía por peso, no por tamaño.</summary>
public static class Tw
{
    // Tipografía (uniforme text-xs)
    public const string TextBody = "text-xs text-gray-800";
    public const string TextMuted = "text-xs text-gray-500";
    public const string TextLabel = "text-xs font-medium text-gray-700";
    public const string TextSection = "text-xs font-semibold uppercase tracking-wide text-gray-600";
    public const string TextCaption = "text-xs text-gray-500";
    public const string TextTitle = "text-xs font-bold leading-tight tracking-tight text-gray-900";
    public const string TextSubtitle = "text-xs leading-tight text-gray-500";
    public const string TextCardTitle = "text-xs font-semibold text-gray-900";
    public const string TextEmpty = "text-xs text-gray-500";
    public const string TextError = "text-xs text-red-700";
    public const string TextMetric = "text-xs font-bold tabular-nums text-gray-900";

    // Formularios compactos (creación / edición)
    public const string FormCard = "max-w-2xl space-y-1.5 rounded border border-gray-200 bg-white p-2.5";
    public const string FormLabel = "mb-0.5 block text-xs font-medium text-gray-700";
    public const string FormInput =
        "w-full rounded border border-gray-300 bg-white px-2 py-1 text-xs text-gray-800 " +
        "focus:border-primary-500 focus:outline-none focus:ring-1 focus:ring-primary-500/30";
    public const string FormInputError = FormInput + " border-red-400 focus:border-red-500 focus:ring-red-500/30";
    public const string FormSelect = FormInput;
    public const string FormDate = FormInput + " date-input";
    public const string FormTextarea = FormInput + " min-h-[60px] resize-y";
    public const string FormError = "mt-0.5 text-xs text-red-600";

    // Formularios (pantallas de edición — alias legado)
    public const string Input = FormInput;

    public const string DateInput = FormDate;
    public const string Select = FormSelect;
    public const string Textarea = FormTextarea;
    public const string Label = FormLabel;

    // Filtros en cabecera (compactos, inline)
    public const string FilterLabel = "text-xs font-medium text-gray-500 whitespace-nowrap";
    public const string FilterSelect =
        "h-7 min-w-[7rem] rounded-md border border-gray-300 bg-white px-1.5 text-xs text-gray-800 " +
        "focus:border-primary-500 focus:outline-none focus:ring-1 focus:ring-primary-500/30";
    public const string FilterDate = FilterSelect + " date-input !w-auto";
    public const string FacetSelect =
        "h-5 w-full min-w-0 rounded-md border border-gray-300 bg-white px-1.5 text-xs leading-none text-gray-800 " +
        "focus:border-primary-500 focus:outline-none focus:ring-1 focus:ring-primary-500/30";
    public const string FacetDate = FacetSelect + " date-input";
    public const string FilterInput =
        "h-7 w-36 rounded-md border border-gray-300 bg-white px-1.5 text-xs text-gray-800 " +
        "focus:border-primary-500 focus:outline-none focus:ring-1 focus:ring-primary-500/30";

    public const string PageTitle = TextTitle;
    public const string PageSubtitle = TextSubtitle;

    // Botones — paleta unificada (primario azul, secundario outline)
    public const string BtnPrimary =
        "inline-flex items-center justify-center gap-1 rounded-md bg-primary-600 px-2 py-1 text-xs font-medium text-white " +
        "transition hover:bg-primary-700 disabled:cursor-not-allowed disabled:opacity-50";

    public const string BtnOutline =
        "inline-flex items-center justify-center gap-1 rounded-md border border-gray-300 bg-white px-2 py-1 text-xs font-medium text-gray-700 " +
        "transition hover:bg-gray-50 disabled:cursor-not-allowed disabled:opacity-50";

    public const string BtnSecondary =
        "inline-flex items-center justify-center gap-1 rounded-md bg-secondary-600 px-2 py-1 text-xs font-medium text-white " +
        "transition hover:bg-secondary-700 disabled:cursor-not-allowed disabled:opacity-50";

    /// <summary>Alias de BtnPrimary — evita botones verdes sueltos.</summary>
    public const string BtnSuccess = BtnPrimary;

    public const string BtnDanger =
        "inline-flex items-center justify-center gap-1 rounded-md border border-gray-300 bg-white px-2 py-1 text-xs font-medium text-gray-600 " +
        "transition hover:border-red-300 hover:bg-red-50 hover:text-red-700";

    public const string BtnGhost =
        "inline-flex items-center justify-center gap-1 rounded-md px-2 py-1 text-xs font-medium text-gray-600 hover:bg-gray-100 hover:text-primary-700";

    // Pestañas / selector de vista
    public const string TabGroup =
        "inline-flex items-center gap-0.5 rounded-md border border-gray-200 bg-gray-100 p-0.5";
    public const string TabActive =
        "inline-flex items-center gap-1 rounded-md bg-white px-2 py-1 text-xs font-medium text-primary-700 shadow-sm";
    public const string TabInactive =
        "inline-flex items-center gap-1 rounded-md px-2 py-1 text-xs font-medium text-gray-600 hover:text-gray-900";
    public const string TabGroupFull =
        "grid w-full grid-cols-4 gap-0.5 rounded-lg border border-gray-200 bg-gray-100 p-0.5";
    public const string TabActiveFull =
        "inline-flex w-full items-center justify-center gap-1 rounded-md bg-white px-2 py-2 text-xs font-medium text-primary-700 shadow-sm";
    public const string TabInactiveFull =
        "inline-flex w-full items-center justify-center gap-1 rounded-md px-2 py-2 text-xs font-medium text-gray-600 hover:bg-white/60 hover:text-gray-900";
    public const string TabIconActive =
        "inline-flex items-center justify-center rounded p-1 text-primary-700 bg-white shadow-sm";
    public const string TabIconInactive =
        "inline-flex items-center justify-center rounded p-1 text-gray-500 hover:bg-white/70 hover:text-gray-800";

    // Tablas compactas
    public const string Table = "table-striped w-full table-fixed border-collapse text-xs leading-snug";
    public const string TableFluid = "table-striped w-full min-w-max border-collapse text-xs leading-snug";
    public const string Th =
        "border border-gray-200 bg-gray-50 px-1.5 py-0.5 text-left text-xs font-semibold uppercase tracking-wide text-gray-600";
    public const string ThSortable =
        "border border-gray-200 bg-gray-50 px-1.5 py-0.5 text-left text-xs font-semibold uppercase tracking-wide text-gray-600 " +
        "cursor-pointer select-none hover:bg-gray-100";
    public const string ThActions =
        "border border-gray-200 bg-gray-50 px-1.5 py-0.5 text-right text-xs font-semibold uppercase tracking-wide text-gray-600 whitespace-nowrap";
    public const string ThActionsWide =
        "border border-gray-200 bg-gray-50 px-1.5 py-0.5 w-80 text-right text-xs font-semibold uppercase tracking-wide text-gray-600 whitespace-nowrap";
    public const string Td = "border border-gray-200 px-1.5 py-0.5 text-xs leading-snug text-gray-800 align-middle";
    public const string TdEdit = Td;
    public const string TdText = Td + " max-w-0 truncate";
    public const string TdActions = Td + " text-right whitespace-nowrap";
    /// <summary>Columna de acciones: ancho fijo para menú ⋮ o botones Guardar/Cancelar apilados.</summary>
    public const string ThActionsAuto = ThActions + " w-28";
    public const string TdActionsAuto = TdActions + " w-28 align-top";
    public const string TdActionsWide = Td + " w-80 text-right whitespace-nowrap";
    public const string TableInput =
        "w-full rounded border border-gray-300 px-1 py-0 text-xs focus:border-primary-500 focus:outline-none focus:ring-1 focus:ring-primary-500/30";
    public const string TableSelect = TableInput;
    public const string TableDate = TableInput + " date-input";
    public const string TableBtn =
        "inline-flex items-center rounded border border-gray-300 bg-white px-1.5 py-0 text-xs font-medium text-gray-700 whitespace-nowrap " +
        "hover:border-primary-300 hover:bg-gray-50 hover:text-primary-700";
    public const string TableActions = "flex flex-col items-stretch gap-1 [&_button]:w-full [&_button]:justify-center";
    public const string TableActionsTrigger =
        "inline-flex h-5 w-5 items-center justify-center rounded text-gray-500 hover:bg-gray-100 hover:text-gray-800";
    public const string TableActionsDropdown =
        "fixed z-[60] min-w-[9rem] overflow-hidden rounded-md border border-gray-200 bg-white py-0.5 shadow-lg";
    public const string TableActionsDropdownItem =
        "flex w-full px-2 py-1 text-left text-xs text-gray-700 hover:bg-gray-50 disabled:cursor-not-allowed disabled:opacity-50";
    public const string TableActionsDropdownItemDanger =
        "flex w-full px-2 py-1 text-left text-xs text-red-600 hover:bg-red-50 disabled:cursor-not-allowed disabled:opacity-50";

    // Navegación
    public const string NavLink =
        "flex items-center gap-1.5 rounded px-2 py-1 text-xs text-gray-600 hover:bg-gray-100";
    public const string NavLinkActive =
        "flex items-center gap-1.5 rounded bg-primary-50 px-2 py-1 text-xs font-medium text-primary-700";
    public const string NavGroupBtn =
        "flex w-full items-center justify-between rounded px-2 py-1 text-left text-xs font-semibold text-gray-700 transition hover:bg-gray-100";

    // Layout
    public const string SidebarWidth = "w-56";
    public const string SidebarWidthCollapsed = "w-14";
    public const string AppHeader =
        "flex h-8 shrink-0 items-stretch border-b border-primary-800 bg-primary-700 shadow-sm";
    public const string AppHeaderBrand =
        "flex min-w-0 shrink-0 items-center gap-1.5 border-r border-white/10 px-3";
    public const string AppHeaderMain = "flex min-w-0 flex-1 items-center px-2";
    public const string AppShell = "app-shell flex h-screen flex-col overflow-hidden";
    public const string AppPage = "app-page min-h-0 min-w-0 flex-1 overflow-y-auto p-3 text-xs text-gray-800";
    public const string TopBar =
        "flex h-8 shrink-0 items-center border-b border-primary-800 bg-primary-700 px-2 shadow-sm";
    public const string TopBarUserButton =
        "inline-flex h-6 max-w-[18rem] items-center gap-1.5 rounded-md px-1.5 text-xs text-white transition hover:bg-white/10";
    public const string TopBarUserAvatar =
        "flex h-5 w-5 shrink-0 items-center justify-center rounded-full bg-white/15 text-white";
    public const string PageHeader =
        "mb-3 rounded-lg border border-gray-200 bg-white shadow-sm";
    public const string PageHeaderTop = "page-header__top";
    public const string PageHeaderTitleBlock = "min-w-0";
    public const string PageHeaderTitle = PageTitle + " page-header__title";
    public const string PageHeaderSubtitle = PageSubtitle + " page-header__subtitle";
    public const string PageHeaderActions = "page-header__actions";
    public const string PageHeaderFilters =
        "flex flex-wrap items-start gap-2 border-t border-gray-100 px-3 py-1.5";
    public const string PageHeaderFiltersMain =
        "flex min-w-0 flex-1 flex-wrap items-center gap-2";
    public const string PageHeaderFilterToolbar =
        "flex shrink-0 items-center gap-2 self-center sm:self-start";
    public const string PageHeaderFiltersStacked =
        "flex flex-col gap-1.5 border-t border-gray-100 px-3 py-1.5";
    public const string PageHeaderFilterRow =
        "flex flex-wrap items-center gap-2";
    public const string PageHeaderFilterGroupLabel =
        "w-[4.5rem] shrink-0 text-xs font-semibold uppercase tracking-wide text-gray-400";
    public const string SummaryBadge =
        "ml-auto inline-flex items-center rounded-full bg-primary-50 px-3 py-1 text-xs font-medium text-primary-800";
    public const string ToolbarGroup = "flex items-center gap-2";
    public const string ToolbarPrimaryGroup = "flex shrink-0 flex-nowrap items-center gap-2";
    public const string ToolbarDivider = "hidden h-7 w-px shrink-0 bg-gray-200 sm:block";
    public const string FilterBarLabel =
        "mr-1 shrink-0 text-xs font-semibold uppercase tracking-wide text-gray-400";
    public const string Card = "rounded-lg border border-gray-200 bg-white p-4 shadow-sm";
    public const string CardTitle = TextCardTitle;
    public const string MetricCard = "relative overflow-hidden rounded-xl border border-gray-200 bg-white p-4 shadow-sm";
    public const string MetricCardIcon = "flex h-9 w-9 shrink-0 items-center justify-center rounded-lg";
    public const string MetricValue = TextMetric;
    public const string MetricLabel = TextSection + " text-gray-500";
    public const string TableWrap = "overflow-x-auto rounded-lg border border-gray-200 bg-white shadow-sm";
    public const string TableWrapContained = "rounded-lg border border-gray-200 bg-white shadow-sm overflow-hidden";

    // Layout grids (BEM — site.css)
    public const string DetailLayout = "detail-layout";
    public const string DetailLayoutPadded = "detail-layout detail-layout--padded";
    public const string DetailLayoutAside =
        "detail-layout__aside detail-layout__aside--sticky-flush";
    public const string DetailLayoutAsideSpan4 =
        "detail-layout__aside detail-layout__aside--span-4 detail-layout__aside--sticky";
    public const string DetailLayoutMain = "detail-layout__main";
    public const string DetailLayoutMainSpan8 = "detail-layout__main detail-layout__main--span-8";
    public const string FormGrid = "form-grid";
    public const string FormGridSm = "form-grid form-grid--gap-sm";
    public const string FormGridTextXs = "form-grid form-grid--text-xs";
    public const string KpiGrid = "kpi-grid";
    public const string KpiGridCols3 = "kpi-grid kpi-grid--cols-3";
    public const string KpiGridCols4 = "kpi-grid kpi-grid--cols-4";

    // Facets (panel lateral — BEM en site.css)
    public const string FilteredPageLayout = "filtered-layout";
    public const string FilteredPageMain = "filtered-layout__main";
    public const string FacetBackdrop =
        "fixed inset-0 z-40 bg-black/30 backdrop-blur-[1px] lg:hidden";
    public const string FacetPanel = "facet-panel";
    public const string FacetPanelHeader = "facet-panel__header";
    public const string FacetPanelHeaderBrand = "facet-panel__header-brand";
    public const string FacetPanelBadge = "facet-panel__badge";
    public const string FacetPanelClose = "facet-panel__close";
    public const string FacetPanelBody = "facet-panel__body";
    public const string FacetPanelFooter = "facet-panel__footer";
    public const string FacetPanelClearBtn = "facet-panel__clear-btn";
    public const string FacetField = "facet-panel__field";
    public const string FacetLabel = "facet-panel__field-label";
    public const string FacetControl = "facet-panel__field-control";
    public const string FacetAccordion = "facet-panel__section";
    public const string FacetAccordionTrigger = "facet-panel__section-trigger";
    public const string FacetAccordionBody = "facet-panel__section-body";
    public const string FacetSection = FacetAccordion;
    public const string FacetSectionTitle = FacetAccordionTrigger;
    public const string FacetOptionList = "facet-panel__option-list";
    public const string FacetOptionItem = "facet-panel__option";
    public const string FacetOptionCheckbox = "facet-panel__option-checkbox";
    public const string FacetOptionLabel = "facet-panel__option-label";
    public const string FacetOptionCount = "facet-panel__option-count";
    public const string FacetOptionEmpty = "facet-panel__option-empty";
    public const string FacetCheckbox = FacetOptionItem;
    public const string FacetHint = "facet-panel__hint";

    // Kanban
    public const string KanbanBoard =
        "flex gap-3 overflow-x-auto pb-2 min-h-[420px]";
    public const string KanbanColumn =
        "flex w-64 shrink-0 flex-col rounded-lg border";
    public const string KanbanColumnHeader =
        "flex items-center justify-between border-b px-3 py-2 text-xs font-semibold uppercase tracking-wide";
    public const string KanbanCount =
        "rounded-full px-2 py-0.5 text-xs font-semibold";
    public const string KanbanColumnBody =
        "flex flex-1 flex-col gap-2 overflow-y-auto p-2 min-h-[360px]";
    public const string KanbanCard =
        "rounded-md border border-gray-200 bg-white p-2 text-xs shadow-sm transition hover:border-primary-300 cursor-grab active:cursor-grabbing";
    public const string KanbanCardDragging = "opacity-40 ring-2 ring-primary-400";
    public const string KanbanDragHandle =
        "shrink-0 cursor-grab rounded p-0.5 text-gray-300 hover:bg-gray-100 hover:text-primary-600 active:cursor-grabbing";
    public const string KanbanCardTitle = "font-medium text-gray-900";
    public const string KanbanCardMeta = "mt-1 text-xs text-gray-500";
    public const string KanbanColumnDragOver = "ring-2 ring-primary-400 ring-offset-1";

    // Jerarquía menús seguridad
    public const string MenuParentRow = "table-row--parent bg-primary-50/60 font-semibold text-primary-900";
    public const string MenuChildRow = "text-gray-800";
}
