namespace Kallimakhos.Application.Ports
{
    public class SettingsInput
    {
        #region Project
        /// <summary>
        /// The name of the project.
        /// </summary>
        public string? ProjectName { get; set; }

        /// <summary>
        /// The path where the project will be created.
        /// </summary>
        public string? ProjectPath { get; set; }
        #endregion

        #region External Services
        /// <summary>
        /// Indicates if the project has external services.
        /// </summary>
        public bool HasExternalServices { get; set; }

        /// <summary>
        /// Name of services.
        /// </summary>
        /// <example>Identity</example>
        /// <example>Authorization</example>
        /// <example>Payment</example>
        /// <example>Shipping</example>
        /// <example>...</example>
        public string[]? ServiceNames { get; set; }
        #endregion

        #region User Interface
        /// <summary>
        /// The type of UI.
        /// </summary>
        public string? TypeUI { get; set; }

        /// <summary>
        /// Name of UI project.
        /// </summary>
        public string? NameUI { get; set; }
        #endregion

        #region Database
        /// <summary>
        /// Has database?
        /// </summary>  
        public bool HasDatabase { get; set; }

        /// <summary>
        /// Database type.
        /// </summary>
        /// <example>SQL Server</example>
        /// <example>PostgreSQL</example>
        /// <example>MySQL</example>
        /// <example>Oracle</example>
        /// <example>...</example>
        public string? DatabaseType { get; set; }

        /// <summary>
        /// Has Repository?
        /// </summary>
        /// <example>true</example>
        /// <example>false</example>
        public bool HasRepositories { get; set; }
        #endregion

        #region Entities
        /// <summary>
        /// Has entities?
        /// </summary> 
        public bool HasEntities { get; set; }

        /// <summary>
        /// Name of entities.
        /// </summary>
        public string[]? EntityNames { get; set; }
        #endregion

        #region CRUD
        /// <summary>
        /// Has CRUD of any entity?
        /// </summary>
        /// <example>true</example>
        /// <example>false</example>
        public bool HasCRUDs { get; set; }

        /// <summary>
        /// CRUD of entities.
        /// </summary>
        /// <example>User, Category</example> 
        public string[]? CRUDEntities { get; set; }
        #endregion
    }
}