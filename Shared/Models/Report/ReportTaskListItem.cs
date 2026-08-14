namespace GestionProyectos.Shared.Models.Report

{

    public class ReportTaskListItem

    {

        public long IdTarea { get; set; }

        public long IdRequerimiento { get; set; }

        public string Requerimiento { get; set; } = string.Empty;

        public string DescripcionTarea { get; set; } = string.Empty;

        public decimal HorasEstimadas { get; set; }

        public string Desarrollador { get; set; } = string.Empty;

        public decimal HorasEnPeriodo { get; set; }
        public string AlcanceOriginal { get; set; } = string.Empty;
        public string MotivoCambioAlcance { get; set; } = string.Empty;
        public string FaseDesarrollo { get; set; } = string.Empty;

    }

}

