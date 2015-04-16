using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BPM.Maps.Interfaces;
using BPM.Maps.Implementations;
using BPM.ViewModels;
using BPM.ViewModels.Responses;
using System.Web.Http.Description;
using System.Data.Entity.Infrastructure;
using Newtonsoft.Json;
using System.Web.Http.Cors;


namespace WebApi.Controllers
{
    [EnableCors(origins: "http://local.bpm.com", headers: "*", methods: "*")]
    [RoutePrefix("api/Usuario")]
    public class UsuarioController : BaseController
    {
        private IUserMap userMap = null;  
      
        public UsuarioController(IUserMap userMap)
        {
            this.userMap = userMap;
        }
      
    
        /// <summary>
        /// Get all active users from db
        /// </summary>
        /// <returns></returns>
        [Authorize]       
        [HttpGet]
        public IEnumerable<UserViewModel> Get()
        {
            log.Info("GET /api/usuario");
            var users = userMap.GetAllActiveUsers();
            return users;
        }

      

        // GET api/usuario/5
      
        [Authorize]
        [HttpGet]
        [ResponseType(typeof(UserViewModel))]
        /// <summary>
        /// GET api/usuario/5 by ID.
        /// </summary>
        /// <param name="id">The ID of the data.</param>
        public IHttpActionResult Get(int id)
        {
            log.Info("GET api/usuario/" + id);

            var user = userMap.GetUserById(id);
            if (user == null)
            {
                log.Error("ERROR GET api/usuario:  Status 404");
                //return Status 404
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("UserPermission/{userId}")]
        public IEnumerable<string> UserPermission(string userId)
        {
           
            var userPerm = userMap.GetUserPermission(Convert.ToInt32(userId));
            return userPerm;
        }

        [Authorize]
        [HttpGet]
        [Route("Find")]
        public HttpResponseMessage find(int itemsPerPage, int page, bool reverse, string search, string sortBy, int totalItems)
        {            
          
            PageInfo dPageInfo = new PageInfo()
            {
                page = page,
                itemsPerPage = itemsPerPage,
                sortBy = sortBy,
                reverse = reverse,
                search = search,
                totalItems = totalItems
            };

            log.Info("GET /api/usuario/Find");
            var userResp = new UsersResponse();
            //esto que continua llevarlo a la capa de services
            List<UserViewModel> userList;
            // filtering
            if ((dPageInfo.search != null && dPageInfo.search.Trim() != String.Empty))
            {                
                userList = userMap.GetAllActiveUsers().Where(au => au.Mail.ToLower().Contains(dPageInfo.search) || au.Nombre.ToLower().Contains(dPageInfo.search)).ToList();                
            }
            else
                userList = userMap.GetAllActiveUsers().ToList();

            userResp.TotalUsers = userList.Count();
            userResp.users = userList.Skip((dPageInfo.page - 1) * dPageInfo.itemsPerPage).Take(dPageInfo.itemsPerPage).ToList();

            // Write the list to the response body.
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, userResp);
            return response;
        }     

        // GET api/usuario/usernameValidation/5
        //[Authorize]
        [HttpGet]        
        [Route("usernameValidation/{username}")]
        public IHttpActionResult usernameValidation(string username)
        {
            log.Info("GET api/usuario/usernameValidation/" + username);
            var user = userMap.GetUserByName(username);          
            if (user == null)
            {
                return Ok(new GenericResponse { Status = 200, Message = "El usuario no existe en la base de datos." });
            }
            else
            {
                return Ok(new GenericResponse { Status = 203, Message = "Ya existe un usuario con el mismo nombre de usuario." });
            }
           
        }


          // POST api/usuario
          //[Authorize]
          [HttpPost]
          [ResponseType(typeof(UserViewModel))]
          public IHttpActionResult Post(UserViewModel user)
          {             
              if (!ModelState.IsValid)
              {
                  log.Error("ERROR POST api/usuario: Model Invalid");
                  return BadRequest(ModelState);
              }
              log.Info("POST api/usuario * usuarioId: " + user.Id); 

              user.Id = userMap.UserInsert(user, user.roles.Select(x => x.Id).FirstOrDefault());
              //return 201 Created
              return CreatedAtRoute("DefaultApi", new { id = user.Id }, user);           
           
          }

          // PUT api/usuario/5
        // [Authorize]
          [HttpPut]
         public IHttpActionResult Put(int id, UserViewModel user)
          {
             
              if (!ModelState.IsValid)
              {
                  return BadRequest(ModelState);
              }
              log.Info("PUT api/usuario - usuarioId: " + id.ToString()); 
              if (id != user.Id)
              {
                  log.Error("ERROR PUT api/usuario: BadRequest");
                  return BadRequest("Error al intentar actualizar el usuario");
              }
             
           
              try
              {                  
                  userMap.UserUpdate(user);
              }
              catch (DbUpdateConcurrencyException e)
              {
                  if (!userMap.UserExists(id))
                  {
                      log.Error("ERROR PUT api/usuario: " + e.Message);
                      return NotFound();
                  }
                  else
                  {
                      log.Error("ERROR PUT api/usuario: " + e.Message);
                      throw;
                  }
              }

              //return StatusCode(HttpStatusCode.NoContent);

              return Ok(new GenericResponse { Status = 200, Message = "The user was updated successfully." });
          }
        
          // DELETE api/usuario/5
        // [Authorize]
          [HttpDelete]
          public IHttpActionResult Delete(int id)
          {
              log.Info("DELETE api/usuario * usuarioId: " + id.ToString()); 

              if (!userMap.UserExists(id))
              {
                  log.Error("ERROR DELETE api/usuario: ");
                  return NotFound();
              } 
              userMap.UserDelete(id);
              //repository.Save();
              return Ok(new GenericResponse { Status = 200, Message = "The user was deleted successfully." });
          }
    }
}
