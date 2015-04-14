/*************************************************************************************************
 * Descripción: Clase de Interfaz Repositorio de Usuario
 * Observaciones: 
 * Creado por: Gabriel Recio
 * Historial de Revisiones:
 * -----------------------------------------------------------------------------------------------
 * Fecha        Evento / Método                     Autor       Descripción
 * -----------------------------------------------------------------------------------------------
 * 03/06/2014                                       GR          Implementación Incial
 * ----------------------------------------------------------------------------------------------- 
 */
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BPM.Repositories.DataContext;
using BPM.Repositories.Common;
using BPM.ViewModels;

namespace BPM.Repositories.Interfaces
{
    public interface IUsuarioRepository : IGenericRepository<SisUsuario>
    {
        SisUsuario GetUserByName(string username);
        bool UserExists(int id);
        int UserInsert(SisUsuario newUser, int rolId);
        int UserUpdate(SisUsuario user);
        IEnumerable<string> GetUserPermission(int userId);
        SisUsuario FindUser(string userName, string password);
    }
}
