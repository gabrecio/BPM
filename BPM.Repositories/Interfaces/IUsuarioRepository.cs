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
using System.Collections.Generic;
using BPM.Repositories.DataContext;
using BPM.Repositories.Common;

namespace BPM.Repositories.Interfaces
{
    public interface IUsuarioRepository :  IRepository<SisUsuario, int> 
    {
        SisUsuario GetUserByName(string username);
        bool UserExists(int id);
        int UserInsert(SisUsuario newUser);
        int UserUpdate(SisUsuario user);
        IEnumerable<string> GetUserPermission(int userId);
        SisUsuario FindUser(string userName, string password);
    }
}
