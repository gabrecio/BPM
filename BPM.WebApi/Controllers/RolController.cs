using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using BPM.Maps.Interfaces;
using BPM.ViewModels;
using BPM.ViewModels.Responses;
using Newtonsoft.Json;


namespace WebApi.Controllers
{
    [RoutePrefix("api/Rol")]
    public class RolController : BaseController
    {
     
         private IRoleMap roleMap = null;

         public RolController(IRoleMap roleMap)
        {
            this.roleMap = roleMap;
        }

     
        [Authorize]       
        [HttpGet]
        // GET /api/rol
        public IEnumerable<RoleViewModel> Get()
        {
            log.Info("GET /api/rol");           
            var roles=  roleMap.GetAllActiveRoles().ToList();
            return roles;                
        }

        // GET api/rol/5
        [Authorize]   
        [HttpGet]
        [ResponseType(typeof(RoleViewModel))]
          public IHttpActionResult Get(int id)
          {
              log.Info("GET api/rol/" + id);
            
              var rol = roleMap.GetRoleById(id);
              if (rol == null)
                {
                    log.Error("ERROR GET api/rol:  Status 404");
                    //return Status 404
                    return NotFound();
                }
              return Ok(rol);
          }


        [Authorize]
        [HttpGet]
        [Route("Find/{pagingInfo}")]
        public HttpResponseMessage Find(string pagingInfo)
        {
            PageInfo dPageInfo = JsonConvert.DeserializeObject<PageInfo>(pagingInfo);
            log.Info("GET /api/rol/Find/{pagingInfo}");
            var rolResp = new RolResponse();

            List<RoleViewModel> rolList;
            // filtering
            if ((dPageInfo.search != null && dPageInfo.search.Trim() != String.Empty))
            {
                rolList = roleMap.GetAllActiveRoles().Where(au => au.Nombre.ToLower().Contains(dPageInfo.search)).ToList();
               
            }
            else
                rolList = roleMap.GetAllActiveRoles().OrderBy(x => x.Nombre).ToList();

            if (!dPageInfo.reverse)
                rolList = rolList.OrderBy(x => x.Nombre).ToList();
            else
                rolList = rolList.OrderByDescending(x => x.Nombre).ToList();

            rolResp.TotalRoles = rolList.Count();
            rolResp.rols = rolList.Skip((dPageInfo.page - 1) * dPageInfo.itemsPerPage).Take(dPageInfo.itemsPerPage).ToList();

            // Write the list to the response body.
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, rolResp);
            return response;
        }

        // GET api/rol/nameValidation/5
        [Authorize]
        [HttpGet]
        [Route("nameValidation/{name}")]
        public IHttpActionResult NameValidation(string name)
        {
            log.Info("GET api/rol/nameValidation/" + name);
            var rol = roleMap.GetRoleByName(name);
            if (rol == null)
            {
                return Ok(new GenericResponse { Status = 200, Message = "El rol no existe en la base de datos." });
            }
            return Ok(new GenericResponse { Status = 203, Message = "Ya existe un rol con el mismo nombre." });
        }

        // POST api/rol
          [Authorize]
          [HttpPost]
          [ResponseType(typeof(RoleViewModel))]
          public IHttpActionResult Post(RoleViewModel rol)
          {
             
              if (!ModelState.IsValid)
              {
                  log.Error("ERROR POST api/rol: Model Invalid");
                  return BadRequest(ModelState);
              }
              log.Info("POST api/rol * rolId: " + rol.Id);
             // var newRol = Role.For(rol);

             // _repository.Insert(newRol);
              //_repository.save();
              var newRolId = roleMap.RoleInsert(rol);
              return CreatedAtRoute("DefaultApi", new { id = newRolId }, rol);           
           
          }

          // PUT api/rol/5
          [Authorize]
          [HttpPut]
          public IHttpActionResult Put(int id, RoleViewModel rol)
          {
             
              if (!ModelState.IsValid)
              {
                  return BadRequest(ModelState);
              }
              log.Info("PUT api/rol - rolId: " + rol.Id); 
              if (id != rol.Id)
              {
                  log.Error("ERROR PUT api/rol: BadRequest");
                  return BadRequest();
              }
              try
              {


                  roleMap.RoleUpdate(rol);
              }
              catch (DbUpdateConcurrencyException e)
              {
                  if (!roleMap.RoleExists(id))
                  {
                      log.Error("ERROR PUT api/rol: " + e.Message);
                      return NotFound();
                  }
                  log.Error("ERROR PUT api/rol: " + e.Message);
                  throw;
              }
            
              return Ok(new GenericResponse { Status = 200, Message = "El rol se actualizo correctamente." });
          }

          // DELETE api/rol/5
          [Authorize]
          [HttpDelete]
          public IHttpActionResult Delete(int id)
          {
              log.Info("DELETE api/rol * rolId: " + id);

              if (!roleMap.RoleExists(id))
              {
                  log.Error("ERROR DELETE api/rol: ");
                  return NotFound();
              }             
              //crear metodo rolhasusers
             // if (!roleMap.RolHasUsers(id)) {
                  roleMap.RoleDelete(id);
                  return Ok(new GenericResponse { Status = 200, Message = "El rol se elimino correctamente" });
            //  }
            //  return Ok(new GenericResponse { Status = 203, Message = "No se puede eliminar el rol. Tiene usuarios asociados." });
          }

          [Authorize]
          [HttpGet]
          [AllowAnonymous]
          [Route("RolPermission/{rolId}")]
          public List<Permissions> RolPermission(string rolId)
          {
              return roleMap.GetRolePermission(Convert.ToInt32(rolId));
            
          }
    }
}
