namespace ServiPuntosUy.Enums
{
    /// <summary>
    /// Tipos de usuario en el sistema
    /// </summary>
    public enum UserType
    {
        /// <summary>
        /// Administrador central de la plataforma
        /// </summary>
        Central = 1,
        
        /// <summary>
        /// Administrador de tenant (cadena de estaciones)
        /// </summary>
        Tenant = 2,
        
        /// <summary>
        /// Administrador de estación
        /// </summary>
        Branch = 3,
        
        /// <summary>
        /// Usuario final (cliente)
        /// </summary>
        EndUser = 4
    }
}
